using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Common.Config;
using Newtera.Server.Util;

namespace Newtera.MasterServerClient
{
    /// <summary>
    /// The master server client
    /// </summary>
    public class MasterServerClient : IMasterServerClient
    {
        protected T GetAPICall<T>(string apiUrl)
        {
            T apiResult = default(T);
            string baseUri = MasterServerResolver.Instance.MasterServerUrl;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(apiUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string json = responseContent.ReadAsStringAsync().Result;

                    apiResult = JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    string msg = response.Content.ReadAsStringAsync().Result;

                    throw new Exception(msg);
                }
            }

            return apiResult;
        }

        private void PostAPICall<T>(string apiUrl, T value)
        {
            JObject apiResult = null;

            string baseUri = MasterServerResolver.Instance.MasterServerUrl;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                client.MaxResponseContentBufferSize = 556000;

                StringContent postContent = null;

                if (value != null)
                {
                    var jsonString = JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented);
                    //ErrorLog.Instance.WriteLine(jsonString);
                    postContent = new StringContent(jsonString, Encoding.UTF8);

                    postContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                var response = client.PostAsync(apiUrl, postContent).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
            }
        }
  
        private string DeleteAPICall(string apiUrl)
        {
            string apiResult = null;

            string baseUri = MasterServerResolver.Instance.MasterServerUrl;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                client.MaxResponseContentBufferSize = 556000;

                var response = client.DeleteAsync(apiUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    apiResult = responseContent.ReadAsStringAsync().Result;
                }
                else
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
            }

            return apiResult;
        }

        public void Add<T>(string cacheName, string key, T value)
        {
            PostAPICall("api/mastercache/" + cacheName + "?key=" + key, value);
        }

        public bool Contains(string cacheName, string key)
        {
            return GetAPICall<bool>("api/mastercache/" + cacheName + "/contains?key=" + key);
        }

        public T Get<T>(string cacheName, string key)
        {
            return GetAPICall<T>("api/mastercache/" + cacheName + "?key=" + key);
        }

        public void Remove(string cacheName, string key)
        {
            DeleteAPICall("api/mastercache/" + cacheName + "?key=" + key);
        }

        public IList<string> GetKeys(string cacheName)
        {
            return GetAPICall<IList<string>>("api/mastercache/" + cacheName + "/keys");
        }

        public IList<T> GetValues<T>(string cacheName)
        {
            return GetAPICall<IList<T>>("api/mastercache/" + cacheName + "/values");
        }

        public void Clear(string cacheName)
        {
            DeleteAPICall("api/mastercache/" + cacheName + "/clear");
        }
    }
}

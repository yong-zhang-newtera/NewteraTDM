using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace Newtera.MLStudio.WebApi
{
    public class WebApiServiceBase
    {
        private const string ServerBaseUri = "http://localhost:8080";
        public string BaseUri;

        public WebApiServiceBase()
        {
            BaseUri = null;
        }

        protected string GetAPICall(string apiUrl)
        {
            string apiResult = null;

            using (var client = new HttpClient())
            {
                string baseUri = BaseUri;
                if (string.IsNullOrEmpty(baseUri))
                {
                    baseUri = ServerBaseUri;
                }

                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = client.GetAsync(apiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // by calling .Result you are performing a synchronous call
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        apiResult = responseContent.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        string msg = response.Content.ReadAsStringAsync().Result;

                        throw new Exception(msg);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("failed in get call for " + apiUrl + " for the reason of " + ex.Message);
                }
            }

            return apiResult;
        }

        protected string PostAPICall(string apiUrl, string data, string mineType)
        {
            string apiResult = null;

            using (var client = new HttpClient())
            {
                string baseUri = ServerBaseUri;
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                client.MaxResponseContentBufferSize = 556000;
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringContent postContent = null;

                if (!string.IsNullOrEmpty(data))
                {
                    postContent = new StringContent(data, Encoding.UTF8);
                    postContent.Headers.ContentType = new MediaTypeHeaderValue(mineType);
                }

                var response = client.PostAsync(apiUrl, postContent).Result;

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

        protected string DeleteAPICall(string apiUrl)
        {
            string apiResult = null;

            using (var client = new HttpClient())
            {
                string baseUri = BaseUri;
                if (string.IsNullOrEmpty(baseUri))
                {
                    baseUri = ServerBaseUri;
                }

                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);

                try
                {
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
                        string msg = response.Content.ReadAsStringAsync().Result;

                        throw new Exception(msg);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("failed in delete call for " + apiUrl + " for the reason of " + ex.Message);
                }
            }

            return apiResult;
        }

        protected void PostFile(string apiUrl, string filePath)
        {
            string baseUri = BaseUri;
            if (string.IsNullOrEmpty(baseUri))
            {
                baseUri = ServerBaseUri;
            }

            string uri = baseUri + "/" + apiUrl;
            using (WebClient client = new WebClient())
            {
                client.UploadFile(uri, filePath);
            }
        }
    }
}

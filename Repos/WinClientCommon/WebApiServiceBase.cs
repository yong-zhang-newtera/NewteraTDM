using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

using Newtera.Common.Core;

namespace Newtera.WinClientCommon
{
    public class WebApiServiceBase
    {
        public string BaseUri;

        public WebApiServiceBase()
        {
            BaseUri = null;
        }

        protected string GetAPICall(string apiUrl)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                string apiResult = null;

                using (var client = new HttpClient())
                {
                    string baseUri = BaseUri;
                    if (string.IsNullOrEmpty(baseUri))
                    {
                        baseUri = NewteraNameSpace.ServerBaseUri();
                    }

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
                        apiResult = responseContent.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        string msg = response.Content.ReadAsStringAsync().Result;

                        throw new Exception(msg);
                    }
                }

                return apiResult;
            }
            finally
            {
                // Restore the cursor
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;
            }
        }

        protected string PostAPICall(string apiUrl, string data, string mineType)
        {
            return PostAPICall(apiUrl, data, mineType, CancellationToken.None);
        }

        protected string PostAPICall(string apiUrl, string data, string mineType, CancellationToken cancellationToken)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                string apiResult = null;

                using (var client = new HttpClient())
                {
                    string baseUri = NewteraNameSpace.ServerBaseUri();
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

                    var response = client.PostAsync(apiUrl, postContent, cancellationToken).Result;

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
            finally
            {
                // Restore the cursor
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;
            }
        }

        protected string PostAPICall(string apiUrl, FormUrlEncodedContent content)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                string apiResult = null;

                using (var client = new HttpClient())
                {
                    string baseUri = NewteraNameSpace.ServerBaseUri();
                    client.BaseAddress = new Uri(baseUri);
                    client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                    client.MaxResponseContentBufferSize = 556000;
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.PostAsync(apiUrl, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // by calling .Result you are performing a synchronous call
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        apiResult = responseContent.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        //throw new Exception(baseUri + @"/" + apiUrl + " got an error: " + response.StatusCode);

                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                }

                return apiResult;
            }
            finally
            {
                // Restore the cursor
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Newtera.MLStudio.WebApi
{
    public class UserInfoServiceStub : WebApiServiceBase
    {
        public UserInfoServiceStub()
        {
        }

        public bool Authenticate(string name, string password)
        {
            bool status = false;

            string result = GetAPICall("api/userInfoService/Authenticate/" + name + "/" + password);

            try
            {
                status = bool.Parse(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return status;
        }

        public string[] GetUserData(string name)
        {
            string result = GetAPICall("api/userInfoService/GetUserData/" + name);

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }
    }
}

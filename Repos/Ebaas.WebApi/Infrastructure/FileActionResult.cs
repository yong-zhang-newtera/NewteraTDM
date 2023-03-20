using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace Ebaas.WebApi.Infrastructure
{
    public class FileActionResult
    {
        public bool Successful { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
        public HttpResponseMessage Response {get;set;}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newtera.WebApi.Models
{
    public class BucketModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string ServiceUrl { get; set; }
        public string Path { get; set; }
        public string ForDBSchema { get; set; }
        public string ForDBClass { get; set; }
    }
}
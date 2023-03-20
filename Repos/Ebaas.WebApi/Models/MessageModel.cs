using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ebaas.WebApi.Models
{
    public class MessageModel
    {
        public string id { get; set; }
        public string objId { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public string postTime { get; set; }
        public string poster { get; set; }
        public string posterId { get; set; }
        public string receiver { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string urlparams {get; set;}

        public string dbschema { get; set; }

        public string dbclass { get; set; }

        public string oid { get; set; }
    }
}
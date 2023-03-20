using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ebaas.WebApi.Models
{
    public class SearchCountModel
    {
        public string schemaName { get; set; }
        public string className{ get; set; }
        public string classDisplayName { get; set; }
        public int count{ get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newtera.WebApi.Models
{
    public class FileViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Suffix { get; set; }
        public string InstanceId { get; set; }
        public string ClassName { get; set; }
        public string Creator { get; set; }

    }

    public class UploadFileResultModel
    {
        public string message;
        public IEnumerable<FileViewModel> files;
    }
}
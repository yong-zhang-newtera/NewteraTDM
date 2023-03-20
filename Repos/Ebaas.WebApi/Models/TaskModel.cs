using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ebaas.WebApi.Models
{
    public class TaskModel
    {
        public string TaskId { get; set; }
        public string CreateTime { get; set; }
        public string FinishTime { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string BindingSchemaId { get; set; }
        public string BindingClassName { get; set; }
        public string BindingInstanceId { get; set; }

        public string Owners { get; set; }

        public string FormUrl { get; set; }

        public string FormParams { get; set; }
        public IEnumerable<ActionModel> CustomActions { get; set; }
    }

    public class ActionModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Api { get; set; }

        public string FormAction { get; set; }
    }
}
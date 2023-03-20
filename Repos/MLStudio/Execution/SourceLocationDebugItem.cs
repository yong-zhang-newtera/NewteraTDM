
namespace Newtera.MLStudio.Execution
{
    using System;

    public class SourceLocationDebugItem
    {
        public int StepCount { get; set; }

        public string ActivityName { get; set; }

        public string Id { get; set; }

        public string State { get; set; }

        public Guid InstanceId { get; set; }
    }
}

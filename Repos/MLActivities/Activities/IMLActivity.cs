using System.Activities;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Newtera.MLActivities.MLConfig;

namespace Newtera.MLActivities
{
    public interface IMLActivity
    {
        IMLComponnet ParentComponent { get; set; }

    }
}

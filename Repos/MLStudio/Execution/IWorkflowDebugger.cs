
namespace Newtera.MLStudio.Execution
{
    using System.Windows;

    public interface IWorkflowDebugger : IWorkflowRunner
    {
        UIElement GetDebugView();
    }
}

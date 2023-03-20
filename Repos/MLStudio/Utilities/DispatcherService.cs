
namespace Newtera.MLStudio.Utilities
{
    using System;
    using System.Windows.Threading;

    public static class DispatcherService
    {
        public static Dispatcher DispatchObject { get; set; }

        public static void Dispatch(Action action)
        {
            if (DispatchObject == null || DispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                DispatchObject.Invoke(action);
            }
        }
    }
}

namespace Newtera.MLStudio.ViewModel
{
    using Newtera.MLStudio.Properties;

    public class StatusViewModel : ViewModelBase
    {
        private static StatusViewModel statusViewModelSingleton = new StatusViewModel();

        private string statusText = Resources.ReadyStatus;

        public static StatusViewModel GetInstance
        {
            get
            {
                return statusViewModelSingleton;
            }
        }

        public string StatusText
        {
            get
            {
                return this.statusText;            
            }
        }

        public static void SetStatusText(string text, string workflowName)
        {
            statusViewModelSingleton.statusText = string.Format("{0} : {1}", workflowName, text);
            statusViewModelSingleton.OnPropertyChanged("StatusText");
        }
    }
}

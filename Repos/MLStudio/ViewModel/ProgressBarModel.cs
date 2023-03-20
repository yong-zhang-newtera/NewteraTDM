
namespace Newtera.MLStudio.ViewModel
{
    using Newtera.MLStudio.Properties;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class ProgressBarModel : ViewModelBase
    {
        private static ProgressBarModel progressBarModelSingleton = new ProgressBarModel();

        private ProgressBar progressBar;

        public static ProgressBarModel TheInstance
        {
            get
            {
                return progressBarModelSingleton;
            }
        }

        public ProgressBar ProgressBar
        {
            get
            {
                return this.progressBar;            
            }
            set
            {
                this.progressBar = value;
            }
        }

        public void SetIsIndeterminateStatus(bool status)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                this.progressBar.IsIndeterminate = status;
            }));
        }
    }
}

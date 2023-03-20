
namespace Newtera.MLStudio.ViewModel
{
    using Newtera.MLStudio.Properties;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class TensorBoard : ViewModelBase
    {
        private static TensorBoard tensorBoardSingleton = new TensorBoard();

        private bool isAvailable = true;
        private string message;

        public static TensorBoard TheInstance
        {
            get
            {
                return tensorBoardSingleton;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return this.isAvailable;            
            }
            set
            {
                this.isAvailable = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        public void LaunchBrowser()
        {
            System.Diagnostics.Process.Start("http://localhost:6006");
        }
    }
}

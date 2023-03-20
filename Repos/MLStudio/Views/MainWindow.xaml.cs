
namespace Newtera.MLStudio.Views
{
    using System.ComponentModel;
    using System.Windows;

    using Newtera.MLStudio.Utilities;
    using Newtera.MLStudio.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            this.viewModel = new MainWindowViewModel(this.DockManager, this.HorizontalResizingPanel, this.VerticalResizingPanel, this.TabsPane, this.progressBar);
            this.DataContext = this.viewModel;
           
            Status.DataContext = StatusViewModel.GetInstance;
            DispatcherService.DispatchObject = this.Dispatcher;
            this.Closing += new CancelEventHandler(this.viewModel.Closing);
        }
    }
}
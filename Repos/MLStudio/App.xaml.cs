
namespace Newtera.MLStudio
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using Newtera.MLStudio.Utilities;
    using Newtera.MLStudio.Views;
    using Resx = Newtera.MLStudio.Properties;

    public partial class App : Application
    {
        private TraceSource errorSource;

        public App() : base()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
              typeof(FrameworkElement),
              new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            this.Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(this.Dispatcher_UnhandledException);

            this.errorSource = new TraceSource("ErrorTraceSource", SourceLevels.Error);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format(Resx.Resources.UnhandledExceptionMessage, e.Exception.Message);
            MessageBox.Show(errorMessage, Resx.Resources.DialogCaptionError, MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;

            this.errorSource.TraceData(TraceEventType.Error, 0, string.Format(Resx.Resources.UnhandledExceptionTrace, DateTime.UtcNow, ExceptionHelper.FormatStackTrace(e.Exception)));
        }
    }
}
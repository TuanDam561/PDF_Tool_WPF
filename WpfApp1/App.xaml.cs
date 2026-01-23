using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using WpfApp1.Functions;
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnDispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            LogService.LogException(e.Exception, "UI Thread");
            e.Handled = true; // ❗ không crash app
        }

        private void OnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                LogService.LogException(ex, "AppDomain");
        }

        private void OnUnobservedTaskException(
            object? sender,
            UnobservedTaskExceptionEventArgs e)
        {
            LogService.LogException(e.Exception, "Task");
            e.SetObserved();
        }
    }
}

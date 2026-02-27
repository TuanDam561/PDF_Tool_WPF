using DotNetEnv;
using PdfSharp.Fonts;
using Syncfusion.Licensing;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using WpfApp1.Functions;
using WpfApp1.Utils;

namespace WpfApp1
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Đăng ký global exception TRƯỚC
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            // 1️⃣ Load .env (ưu tiên ProgramData → fallback project/exe)
            var prodEnv = @"C:\ProgramData\PDF_TOOL\.env";

            if (File.Exists(prodEnv))
                Env.Load(prodEnv);
            else
                Env.Load(); // dev: WpfApp1/.env hoặc cạnh exe

            // 2️⃣ Đọc license
            var key = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE");

            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show(
                    "Không tìm thấy SYNCFUSION_LICENSE trong .env",
                    "Thiếu license",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                Shutdown(); // ❗ thoát app gọn gàng
                return;
            }

            // 3️⃣ Register Syncfusion license (PHẢI TRƯỚC base.OnStartup)
            SyncfusionLicenseProvider.RegisterLicense(key);

            // 4️⃣ Gọi base SAU KHI license OK
            base.OnStartup(e);
            GlobalFontSettings.FontResolver = new CustomFontResolver();
        }

        private void OnDispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            LogService.LogException(e.Exception, "UI Thread");
            e.Handled = true; // không crash app
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

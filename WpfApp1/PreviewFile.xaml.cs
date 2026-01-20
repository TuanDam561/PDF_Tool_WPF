using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows;
using WpfApp1.Functions;

namespace WpfApp1
{
    public partial class PreviewFile : Window
    {
        private readonly string _pdfPath;

        // Constructor nhận path PDF
        public PreviewFile (string pdfPath)
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
            {
                MessageBox.Show("Không tìm thấy file PDF để xem trước.",
                                "Lỗi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Close();
                return;
            }

            _pdfPath = pdfPath;
            FileNameText.Text = Path.GetFileName(pdfPath);

            Loaded += PreviewFile_Loaded;
        }

        private async void PreviewFile_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await PdfWebView.EnsureCoreWebView2Async();

                // Load trực tiếp PDF
                PdfWebView.Source = new Uri(_pdfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể preview PDF.\n" + ex.Message,
                                "Lỗi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

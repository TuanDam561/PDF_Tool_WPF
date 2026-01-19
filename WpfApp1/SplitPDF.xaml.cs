
using Microsoft.Win32;
using Microsoft.Web.WebView2.Wpf;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using WpfApp1.Functions;

namespace WpfApp1
{
    public partial class SplitPDF : Window
    {
        private string _pdfPath = "";
        private int _pageCount = 0;

        public SplitPDF()
        {
            InitializeComponent();
        }

        // =====================
        // Chọn PDF
        // =====================
        private async void SelectPdf_Click(object sender, RoutedEventArgs e)
        {
            var files = FilePicker.Pick(false); // chỉ chọn 1 file

            if (files.Count == 0)
                return;

            _pdfPath = files[0];

            // đọc số trang
            using (PdfDocument doc = PdfReader.Open(_pdfPath, PdfDocumentOpenMode.Import))
            {
                _pageCount = doc.PageCount;
            }

            PdfFileNameText.Text = $"Tên File: {Path.GetFileName(_pdfPath)}";
            PdfPageCountText.Text = $"Tổng số trang: {_pageCount}";

            // load preview
            await PdfViewer.EnsureCoreWebView2Async();
            PdfViewer.Source = new Uri(_pdfPath);
        }


        // =====================
        // Cắt PDF
        // =====================
        private void Split_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_pdfPath))
            {
                MessageBox.Show("Chưa chọn file PDF");
                return;
            }

            List<int> pages = ParsePages(PageInput.Text);

            if (pages.Count == 0)
            {
                MessageBox.Show("Danh sách trang không hợp lệ");
                return;
            }

            SaveFileDialog saveDlg = new SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "split.pdf"
            };

            if (saveDlg.ShowDialog() != true)
                return;

            SplitButton.IsEnabled = false;
            SplitProgress.Visibility = Visibility.Visible;
            StatusText.Visibility = Visibility.Visible;

            Thread t = new Thread(() =>
            {
                try
                {
                    using PdfDocument input = PdfReader.Open(_pdfPath, PdfDocumentOpenMode.Import);
                    using PdfDocument output = new PdfDocument();

                    int total = pages.Count;
                    int done = 0;

                    foreach (int p in pages)
                    {
                        if (p < 1 || p > input.PageCount)
                            continue;

                        output.AddPage(input.Pages[p - 1]);
                        done++;

                        Dispatcher.Invoke(() =>
                        {
                            SplitProgress.Value = done * 100 / total;
                            StatusText.Text = $"Đang cắt trang {p}...";
                        });
                    }

                    output.Save(saveDlg.FileName);

                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Cắt PDF thành công!");
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message, "Lỗi");
                    });
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        SplitButton.IsEnabled = true;
                        SplitProgress.Visibility = Visibility.Collapsed;
                        StatusText.Visibility = Visibility.Collapsed;
                    });
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        // =====================
        // Parse: 1-3,5,7-9
        // =====================
        private List<int> ParsePages(string input)
        {
            List<int> result = new();

            if (string.IsNullOrWhiteSpace(input))
                return result;

            foreach (var part in input.Split(','))
            {
                if (part.Contains('-'))
                {
                    var range = part.Split('-');
                    if (int.TryParse(range[0], out int start) &&
                        int.TryParse(range[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                            result.Add(i);
                    }
                }
                else if (int.TryParse(part, out int page))
                {
                    result.Add(page);
                }
            }

            return result.Distinct().OrderBy(x => x).ToList();
        }
    }
}


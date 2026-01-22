using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using WpfApp1.Functions;

namespace WpfApp1
{
    public partial class SplitPDF : Window
    {
        private string _pdfPath = "";
        private int _pageCount = 0;

        private string _previewPdfPath = "";

        private DispatcherTimer _previewTimer;

        public SplitPDF()
        {
            InitializeComponent();

            // Timer debounce preview
            _previewTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // delay 0.5s
            };
            _previewTimer.Tick += PreviewTimer_Tick;
        }

        // =====================
        // CHỌN PDF
        // =====================
        private async void SelectPdf_Click(object sender, RoutedEventArgs e)
        {
            // 1. Chọn file PDF
            var files = FilePicker.Pick(multiSelect: false);
            if (!files.Any())
                return;

            string selectedPdf = files.First();

            // 2. Thử mở PDF (tự xử lý trường hợp bị khóa)
            bool opened = HandleLockedPdf.TryOpenPdf(
                selectedPdf,
                this,
                out string usablePdfPath,
                out int pageCount);

            if (!opened)
                return;

            // 3. Lưu trạng thái
            _pdfPath = usablePdfPath;
            _pageCount = pageCount;

            // 4. Update UI
            PdfFileNameText.Text = $"Tên file: {Path.GetFileName(_pdfPath)}";
            PdfPageCountText.Text = $"Tổng số trang: {_pageCount}";

            // 5. Preview PDF
            await PdfViewer.EnsureCoreWebView2Async();
            PdfViewer.Source = new Uri(_pdfPath);

            if (files.Count >= 1)
            {
                PageInput.IsEnabled = true;
            }
        }



        // =====================
        // TEXT CHANGED → RESET TIMER
        // =====================
        private void PageInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_pdfPath))
                return;

            _previewTimer.Stop();
            _previewTimer.Start();
        }

        // =====================
        // TIMER TICK → PREVIEW THẬT
        // =====================
        private void PreviewTimer_Tick(object? sender, EventArgs e)
        {
            _previewTimer.Stop();

            var pages = ParsePages(PageInput.Text);
            if (pages.Count == 0)
                return;

            try
            {
                // xóa preview cũ
                if (!string.IsNullOrEmpty(_previewPdfPath) && File.Exists(_previewPdfPath))
                    File.Delete(_previewPdfPath);

                _previewPdfPath = CreatePreviewPdf(pages);
                LoadPreviewPdf(_previewPdfPath);
            }
            catch
            {
                // input đang gõ dở → bỏ qua
            }
        }

        // =====================
        // TẠO PDF PREVIEW (ĐÃ CẮT)
        // =====================
        private string CreatePreviewPdf(List<int> pages)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "WpfSplitPreview");
            Directory.CreateDirectory(tempDir);

            string previewPath = Path.Combine(
                tempDir,
                "preview_" + Guid.NewGuid() + ".pdf"
            );

            using PdfDocument input = PdfReader.Open(_pdfPath, PdfDocumentOpenMode.Import);
            using PdfDocument output = new PdfDocument();

            foreach (int p in pages)
            {
                if (p >= 1 && p <= input.PageCount)
                    output.AddPage(input.Pages[p - 1]);
            }

            output.Save(previewPath);
            return previewPath;
        }

        // =====================
        // LOAD PREVIEW PDF
        // =====================
        private async void LoadPreviewPdf(string path)
        {
            await PdfViewer.EnsureCoreWebView2Async();
            PdfViewer.Source = new Uri(path);
        }

        // =====================
        // CẮT PDF THẬT
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
        // PARSE: 1-3,5,7-9
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

            return result
                .Where(p => p >= 1 && p <= _pageCount)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}

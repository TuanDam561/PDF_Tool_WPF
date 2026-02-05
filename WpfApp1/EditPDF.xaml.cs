using Microsoft.Win32;
using PdfiumViewer;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// 👉 alias để KHÔNG bị ambiguous
using PdfiumDoc = PdfiumViewer.PdfDocument;
using ITextDoc = iText.Kernel.Pdf.PdfDocument;
using iText.Kernel.Pdf;

using WpfApp1.Functions;
using WpfApp1.Model;

namespace WpfApp1
{
    public partial class EditPDF : Window
    {
        private string? _currentPdfPath;

        // 👉 Pdfium dùng render
        private PdfiumDoc? _pdfDoc;

        public ObservableCollection<PdfPageViewModel> Pages { get; } = new();

        public EditPDF()
        {
            InitializeComponent();
            PagesControl.ItemsSource = Pages;
        }

        // ================= OPEN =================
        private void OpenPdf_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new()
            {
                Filter = "PDF (*.pdf)|*.pdf"
            };

            if (dlg.ShowDialog() != true)
                return;

            bool opened = HandleLockedPdf.TryOpenPdf(
                dlg.FileName,
                this,
                out string usablePdfPath,
                out _);

            if (!opened)
                return;

            LoadPdf(usablePdfPath);
        }

        private void LoadPdf(string path)
        {
            ClearPdf();

            _currentPdfPath = path;
            _pdfDoc = PdfiumDoc.Load(path);

            for (int i = 0; i < _pdfDoc.PageCount; i++)
            {
                var img = RenderPage(i, 180);

                Pages.Add(new PdfPageViewModel
                {
                    OriginalPageNumber = i + 1,
                    PageImage = img,
                    OverlayHeight = img.PixelHeight,
                    Rotation = 0
                });
            }

            StatusText.Text = $"Đã mở {_pdfDoc.PageCount} trang";
        }

        // ================= RENDER =================
        private BitmapSource RenderPage(int pageIndex, int width)
        {
            using var bmp = _pdfDoc!.Render(
                pageIndex,
                width,
                (int)(width * 1.4),
                96,
                96,
                PdfRenderFlags.Annotations);

            return ConvertBitmap((Bitmap)bmp);
        }

        private BitmapSource ConvertBitmap(Bitmap bmp)
        {
            using MemoryStream ms = new();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            BitmapImage img = new();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();
            img.Freeze();

            return img;
        }

        // ================= SELECT =================
        private void Page_Click(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not PdfPageViewModel vm)
                return;

            foreach (var p in Pages)
                p.IsSelected = false;

            vm.IsSelected = true;
            StatusText.Text = $"Đã chọn trang {vm.PageNumber}";
        }

        // ================= ROTATE =================
        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            var page = Pages.FirstOrDefault(p => p.IsSelected);
            if (page == null)
            {
                StatusText.Text = "Chưa chọn trang";
                return;
            }

            page.Rotation = (page.Rotation + 90) % 360;
            page.PageImage = RotateBitmap(page.PageImage!, page.Rotation);

            StatusText.Text = $"Đã xoay trang {page.PageNumber}";
        }

        private BitmapSource RotateBitmap(BitmapSource source, int angle)
        {
            var tb = new TransformedBitmap(
                source,
                new RotateTransform(angle));

            tb.Freeze();
            return tb;
        }

        // ================= MOVE =================
        private void MoveSelectedPage(int offset)
        {
            var page = Pages.FirstOrDefault(p => p.IsSelected);
            if (page == null) return;

            int oldIndex = Pages.IndexOf(page);
            int newIndex = oldIndex + offset;

            if (newIndex < 0 || newIndex >= Pages.Count)
                return;

            Pages.Move(oldIndex, newIndex);
        }

        private void MoveUp_Click(object s, RoutedEventArgs e) => MoveSelectedPage(-1);
        private void MoveDown_Click(object s, RoutedEventArgs e) => MoveSelectedPage(1);
        private void MoveLeft_Click(object s, RoutedEventArgs e) => MoveSelectedPage(-1);
        private void MoveRight_Click(object s, RoutedEventArgs e) => MoveSelectedPage(1);

        // ================= DELETE =================
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var page = Pages.FirstOrDefault(p => p.IsSelected);
            if (page == null) return;

            Pages.Remove(page);
        }

        // ================= SAVE =================
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPdfPath == null || Pages.Count == 0)
                return;

            SaveFileDialog dlg = new()
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = "edited.pdf"
            };

            if (dlg.ShowDialog() != true)
                return;

            using var reader = new PdfReader(_currentPdfPath);
            using var writer = new PdfWriter(dlg.FileName);

            using var src = new iText.Kernel.Pdf.PdfDocument(reader);
            using var dest = new iText.Kernel.Pdf.PdfDocument(writer);

            var merger = new iText.Kernel.Utils.PdfMerger(dest);

            foreach (var p in Pages)
            {
                // merge đúng thứ tự
                merger.Merge(src, p.OriginalPageNumber, p.OriginalPageNumber);

                // xoay trang vừa merge
                var page = dest.GetLastPage();
                if (p.Rotation != 0)
                    page.SetRotation(p.Rotation);
            }

            dest.Close(); // ⚠️ bắt buộc
            src.Close();

            StatusText.Text = "Đã lưu PDF thành công";
        }


        // ================= CLEAN =================
        private void ClearPdf()
        {
            _pdfDoc?.Dispose();
            _pdfDoc = null;
            Pages.Clear();
        }
    }
}

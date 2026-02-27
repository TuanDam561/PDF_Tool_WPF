using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfApp1.Functions;

namespace WpfApp1
{
    public partial class ComparePdfWindow : Window
    {
        private string _pdfA = "";
        private string _pdfB = "";

        public ComparePdfWindow()
        {
            InitializeComponent();
        }

        private void SelectPdfA_Click(object sender, RoutedEventArgs e)
        {
            var files = FilePicker.Pick(false);

            if (!files.Any())
            {
                return;
            }
            string selectedPdf = files.First();
            bool opened = HandleLockedPdf.TryOpenPdf(
               selectedPdf,
               this,
               out string usablePdfPath,
               out int pageCount);

            if (!opened)
                return;
            // Set PDF A (có thể là file đã unlock)
            _pdfA = usablePdfPath;

            PdfAText.Text = Path.GetFileName(_pdfA);
        }

        private void SelectPdfB_Click(object sender, RoutedEventArgs e)
        {
            var files = FilePicker.Pick(multiSelect: false);
            if (!files.Any())
                return;

            string selectedPdf = files.First();

            bool opened = HandleLockedPdf.TryOpenPdf(
                selectedPdf,
                this,
                out string usablePdfPath,
                out int pageCount);

            if (!opened)
                return;

            // Set PDF B
            _pdfB = usablePdfPath;

            PdfBText.Text = Path.GetFileName(_pdfB);
        }


        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_pdfA) || string.IsNullOrEmpty(_pdfB))
            {
                MessageBox.Show("Vui lòng chọn đủ 2 file PDF");
                return;
            }

            //var result = PdfCompareService.CompareDetail(_pdfA, _pdfB);

            //string message = result switch
            //{
            //    PdfCompareResult.Identical =>
            //        "✅ Hai file PDF giống nhau hoàn toàn",
            //    PdfCompareResult.SameContent =>
            //        "⚠️ Nội dung giống nhau nhưng file khác",
            //    _ =>
            //        "❌ Hai file PDF khác nhau"
            //};

            //MessageBox.Show(message, "Kết quả so sánh");
            var detail = PdfCompareService.CompareDetail(_pdfA, _pdfB);

            if (detail.IsCompletelySame)
            {
                ResultText.Text = "✅ Hai file PDF giống nhau";
                ResultText.Foreground = Brushes.Green;
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine(detail.IsHashSame
                ? "• Mã PDF : giống"
                : "• Mã PDF : khác");

            sb.AppendLine(detail.IsPageCountSame
                ? $"• Số trang: giống ({detail.PageCountA})"
                : $"• Số trang: khác ({detail.PageCountA} / {detail.PageCountB})");

            if (detail.DifferentPages.Any())
            {
                sb.AppendLine("• Trang khác nội dung:");
                sb.AppendLine("  - " + string.Join(", ", detail.DifferentPages));
            }

            ResultText.Text = sb.ToString();
            ResultText.Foreground = Brushes.DarkRed;

        }
    }
}

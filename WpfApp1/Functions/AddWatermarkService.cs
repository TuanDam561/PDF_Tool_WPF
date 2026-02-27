using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Windows;
using WpfApp1.Model;

namespace WpfApp1.Functions
{
    public class AddWatermarkService
    {
        //public void AddWatermark(
        //    string inputPdfPath,
        //    string outputPdfPath,
        //    WatermarkOptions options)
        //{
        //    //Biết đang ở tab nào để lấy thông tin tương ứng
        //    int selectionTab = options.SelectedTabIndex;

        //    if (!File.Exists(inputPdfPath))
        //        throw new FileNotFoundException("PDF not found");

        //    PdfDocument document = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Modify);

        //    for (int i = 0; i < document.Pages.Count; i++)
        //    {
        //        int pageIndex = i + 1;

        //        if (!options.ApplyToAllPages &&
        //            options.PageNumber.HasValue &&
        //            options.PageNumber.Value != pageIndex)
        //            continue;

        //        PdfPage page = document.Pages[i];

        //        var mode = options.DrawOnTop
        //            ? XGraphicsPdfPageOptions.Append
        //            : XGraphicsPdfPageOptions.Prepend;

        //        using (XGraphics gfx = XGraphics.FromPdfPage(page, mode))
        //        {
        //            double centerX = page.Width / 2;
        //            double centerY = page.Height / 2;

        //            gfx.TranslateTransform(centerX, centerY);
        //            gfx.RotateTransform(options.TextRotation);
        //            // ===== TEXT WATERMARK =====
        //            if (options.Enabled && string.IsNullOrWhiteSpace(options.Text) &&selectionTab==0)
        //            {
        //                throw new InvalidOperationException("Chưa nhập nội dung watermark");
        //            }
        //            if (options.Enabled && !string.IsNullOrEmpty(options.Text) && !string.IsNullOrWhiteSpace(options.Text))
        //            {
        //                DrawTextWatermark(gfx, options);
        //            }

        //            // ===== IMAGE WATERMARK =====
        //            if (options.EnableImage && File.Exists(options.ImagePath))
        //            {
        //                DrawImageWatermark(gfx, options);
        //            }

        //            gfx.Save();
        //            // transform
        //            gfx.Restore();
        //        }
        //    }

        //    document.Save(outputPdfPath);
        //}

        public void AddWatermark(
        string inputPdfPath,
        string outputPdfPath,
        WatermarkOptions options)
        {
            if (!File.Exists(inputPdfPath))
                throw new FileNotFoundException("PDF not found");

            PdfDocument document = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Modify);

            //for (int i = 0; i < document.Pages.Count; i++)
            //{
            //    int pageIndex = i + 1;

            //    // ===== CHECK TRANG =====
            //    //if (!options.ApplyToAllPages &&
            //    //    options.PageNumber.HasValue &&
            //    //    options.PageNumber.Value != pageIndex)
            //    //    continue;

            //    PdfPage page = document.Pages[i];

            //    var mode = options.DrawOnTop
            //        ? XGraphicsPdfPageOptions.Append
            //        : XGraphicsPdfPageOptions.Prepend;

            //    using (XGraphics gfx = XGraphics.FromPdfPage(page, mode))
            //    {
            //        gfx.Save(); // ✅ PHẢI SAVE TRƯỚC

            //        double centerX = page.Width / 2;
            //        double centerY = page.Height / 2;

            //        gfx.TranslateTransform(centerX, centerY);

            //        // ================= TAB TEXT =================
            //        if (options.SelectedTabIndex == 0)
            //        {
            //            if (options.Enabled)
            //            {
            //                if (string.IsNullOrWhiteSpace(options.Text))
            //                    throw new InvalidOperationException("Chưa nhập nội dung watermark");

            //                gfx.RotateTransform(options.TextRotation);
            //                DrawTextWatermark(gfx, options);
            //            }
            //        }
            //        // ================= TAB IMAGE =================
            //        else if (options.SelectedTabIndex == 1)
            //        {
            //            if (!string.IsNullOrWhiteSpace(options.ImagePath) &&
            //                File.Exists(options.ImagePath))
            //            {
            //                gfx.RotateTransform(options.ImageRotation);
            //                DrawImageWatermark(gfx, options);
            //            }
            //            else
            //            {
            //                throw new InvalidOperationException("Chưa chọn ảnh watermark");
            //            }
            //        }

            //        gfx.Restore(); // ✅ RESTORE SAU KHI VẼ
            //    }
            //}

            for (int i = 0; i < document.Pages.Count; i++)
            {
                int pageIndex = i + 1; // trang thực tế (bắt đầu từ 1)

                // ===== CHECK TRANG =====
                if (!options.ApplyToAllPages)
                {
                    if (options.PageFrom.HasValue && pageIndex < options.PageFrom.Value)
                        continue;

                    if (options.PageTo.HasValue && pageIndex > options.PageTo.Value)
                        continue;
                }

                PdfPage page = document.Pages[i];

                var mode = options.DrawOnTop
                    ? XGraphicsPdfPageOptions.Append
                    : XGraphicsPdfPageOptions.Prepend;

                using (XGraphics gfx = XGraphics.FromPdfPage(page, mode))
                {
                    gfx.Save();

                    double centerX = page.Width / 2;
                    double centerY = page.Height / 2;

                    gfx.TranslateTransform(centerX, centerY);

                    if (options.SelectedTabIndex == 0)
                    {
                        gfx.RotateTransform(options.TextRotation);
                        DrawTextWatermark(gfx, options);
                    }
                    else if (options.SelectedTabIndex == 1)
                    {
                        gfx.RotateTransform(options.ImageRotation);
                        DrawImageWatermark(gfx, options);
                    }

                    gfx.Restore();
                }
            }

            document.Save(outputPdfPath);
        }

        // ================= TEXT =================
        private void DrawTextWatermark(XGraphics gfx, WatermarkOptions options)
        {
            XFont font = new XFont(
                options.FontName,
                options.FontSize,
                XFontStyleEx.Bold
            );

            XColor color = XColor.FromArgb(
                options.TextOpacity,
                options.TextColor.R,
                options.TextColor.G,
                options.TextColor.B
            );

            XBrush brush = new XSolidBrush(color);

            XSize textSize = gfx.MeasureString(options.Text, font);

            gfx.DrawString(
                options.Text,
                font,
                brush,
                -textSize.Width / 2,
                textSize.Height / 2
            );
        }

        // ================= IMAGE =================
        private void DrawImageWatermark(XGraphics gfx, WatermarkOptions options)
        {
            using (XImage image = XImage.FromFile(options.ImagePath))
            {
                gfx.Save(); // ⭐ BẮT BUỘC

                double width = image.PixelWidth * options.ImageScale;
                double height = image.PixelHeight * options.ImageScale;

                // Xoay quanh tâm
                gfx.RotateTransform(options.ImageRotation);

                gfx.DrawImage(
                    image,
                    -width / 2,
                    -height / 2,
                    width,
                    height
                );

                gfx.Restore(); // ⭐ BẮT BUỘC
            }
        }
    }
}
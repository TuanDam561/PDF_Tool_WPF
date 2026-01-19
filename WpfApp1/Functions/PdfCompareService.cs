//using PdfSharp.Pdf;
//using PdfSharp.Pdf.IO;
//using System.IO;
//using System.Security.Cryptography;
//using System.Text;
//using WpfApp1.Model;

//namespace WpfApp1.Functions
//{
//    internal static class PdfCompareService
//    {
//        public static PdfCompareResult Compare(string pdfA, string pdfB)
//        {
//            // 1. Hash giống → identical
//            if (ComputeHash(pdfA) == ComputeHash(pdfB))
//                return PdfCompareResult.Identical;

//            using var docA = PdfReader.Open(pdfA, PdfDocumentOpenMode.ReadOnly);
//            using var docB = PdfReader.Open(pdfB, PdfDocumentOpenMode.ReadOnly);

//            // 2. Khác số trang → khác
//            if (docA.PageCount != docB.PageCount)
//                return PdfCompareResult.Different;

//            // 3. So text từng trang
//            for (int i = 0; i < docA.PageCount; i++)
//            {
//                var textA = ExtractText(docA.Pages[i]);
//                var textB = ExtractText(docB.Pages[i]);

//                if (Normalize(textA) != Normalize(textB))
//                    return PdfCompareResult.Different;
//            }

//            return PdfCompareResult.SameContent;
//        }

//        // =====================
//        // Helpers
//        // =====================
//        private static string ComputeHash(string filePath)
//        {
//            using var sha = SHA256.Create();
//            using var stream = File.OpenRead(filePath);
//            return Convert.ToHexString(sha.ComputeHash(stream));
//        }

//        private static string ExtractText(PdfPage page)
//        {
//            // PdfSharp không có text extractor mạnh
//            // Dùng Content stream cơ bản (đủ cho so sánh text đơn giản)
//            return page.Contents.ToString();
//        }

//        private static string Normalize(string text)
//        {
//            return text
//                .Replace("\r", "")
//                .Replace("\n", "")
//                .Replace(" ", "")
//                .ToLower();
//        }
//    }
//}


using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Security.Cryptography;
using WpfApp1.Model;

namespace WpfApp1.Functions
{
    internal static class PdfCompareService
    {
        public static PdfCompareDetail CompareDetail(string pdfA, string pdfB)
        {
            var result = new PdfCompareDetail();

            // 1. Hash
            result.IsHashSame = ComputeHash(pdfA) == ComputeHash(pdfB);

            using var docA = PdfReader.Open(pdfA, PdfDocumentOpenMode.ReadOnly);
            using var docB = PdfReader.Open(pdfB, PdfDocumentOpenMode.ReadOnly);

            result.PageCountA = docA.PageCount;
            result.PageCountB = docB.PageCount;
            result.IsPageCountSame = docA.PageCount == docB.PageCount;

            if (!result.IsPageCountSame)
                return result;

            // 2. So text từng trang
            for (int i = 0; i < docA.PageCount; i++)
            {
                var textA = Normalize(docA.Pages[i].Contents.ToString());
                var textB = Normalize(docB.Pages[i].Contents.ToString());

                if (textA == textB)
                    result.SamePages.Add(i + 1);
                else
                    result.DifferentPages.Add(i + 1);
            }

            return result;
        }

        private static string ComputeHash(string filePath)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            return Convert.ToHexString(sha.ComputeHash(stream));
        }

        private static string Normalize(string text)
        {
            return text
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(" ", "")
                .ToLower();
        }
    }
}

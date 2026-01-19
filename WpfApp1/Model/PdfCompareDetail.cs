using System.Collections.Generic;

namespace WpfApp1.Model
{
    internal class PdfCompareDetail
    {
        public bool IsHashSame { get; set; }
        public bool IsPageCountSame { get; set; }

        public int PageCountA { get; set; }
        public int PageCountB { get; set; }

        public List<int> DifferentPages { get; set; } = new();
        public List<int> SamePages { get; set; } = new();

        public bool IsCompletelySame =>
            IsHashSame || (IsPageCountSame && DifferentPages.Count == 0);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    internal enum PdfCompareResult
    {
        Identical,      // giống tuyệt đối (hash)
        SameContent,    // nội dung giống, file khác
        Different       // khác nội dung
    }
}

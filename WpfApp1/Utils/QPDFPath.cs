using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utils
{
    internal class QPDFPath
    {
        public static string Get()
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Tool",
                "qpdf",
                "qpdf.exe"
                );
        }
        /// <summary>
        /// Kiểm tra qpdf.exe có tồn tại hay không
        /// </summary>
        public static bool Exists()
        {
            return File.Exists(Get());
        }

    }
}

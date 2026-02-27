using PdfSharp.Fonts;
using System.IO;
using System.Reflection;


namespace WpfApp1.Utils
{
    public class CustomFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            if (faceName == "Arial#Regular")
            {
                return File.ReadAllBytes("Font/ARIAL.TTF");
            }
            return null;
        }

        public FontResolverInfo ResolveTypeface(
            string familyName,
            bool isBold,
            bool isItalic)
        {
            if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                return new FontResolverInfo("Arial#Regular");
            }
            return null;
        }
    }
}

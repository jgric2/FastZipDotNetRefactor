using System.Text;

namespace FastZipDotNet.Zip.Helpers
{
    public static class EncodingHelper
    {
        public static Encoding DefaultEncoding = Encoding.GetEncoding(437);
        public static void Register437Encoding()
        {
            CodePagesEncodingProvider.Instance.GetEncoding(437);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}

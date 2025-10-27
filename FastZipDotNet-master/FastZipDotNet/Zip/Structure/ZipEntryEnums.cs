using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastZipDotNet.Zip.Structure
{
    public static class ZipEntryEnums
    {
        public enum Compression : ushort
        {
            Store = 0,
            Deflate = 8,
            Zstd = 93
        }


    }
}

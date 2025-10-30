using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brutal_Zip.Classes.Helpers
{
    public static class BinaryReaderExtensions
    {
        public static ulong ReadUInt64AsUInt32Safe(this BinaryReader br)
        {
            // Reads a UInt64 but returns only lower 32 bits, advancing the stream
            ulong v = br.ReadUInt64();
            return v;
        }
    }
}

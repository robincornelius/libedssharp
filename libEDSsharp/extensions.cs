using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libEDSsharp
{
    public static class extensions
    {
        public static string ToHexString(this byte val)
        {
            return String.Format("0x{0:x}", val);
        }

        public static string ToHexString(this UInt16 val)
        {
            return String.Format("0x{0:x}",val);
        }

        public static string ToHexString(this UInt32 val)
        {
            return String.Format("0x{0:x}", val);
        }

    }
}

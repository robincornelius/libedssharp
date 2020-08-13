using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libEDSsharp
{
    public static class Warnings
    {
        public enum warning_class
        {
            WARNING_GENERIC = 0x01,
            WARNING_RENAME = 0x02,
            WARNING_BUILD = 0x04,
            WARNING_STRING = 0x08,
            WARNING_STRUCT = 0x10,
        }

        public static List<string> warning_list = new List<string>();

        public static UInt32 warning_mask = 0xffff;

        public static void AddWarning(string warning,warning_class c = warning_class.WARNING_GENERIC)
        {
            if (((UInt32)c & warning_mask) != 0)
            {
                warning_list.Add(warning);
            }
        }
    }

   

}

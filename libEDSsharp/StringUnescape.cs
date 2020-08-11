using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libEDSsharp
{
    public static class StringUnescape
    {
        public static string Escape(char c)
        {
            switch (c)
            {
                case '\0':
                    return @"\0";
                case '\n':
                    return @"\n";
                case '\r':
                    return @"\r";
                case '\t':
                    return @"\t";
                case '\a':
                    return @"\a";
                case '\b':
                    return @"\b";
                case '\f':
                    return @"\f";
                case '\v':
                    return @"\v";

                default:
                    return c.ToString();
            }
        }

        public static string Unescape(this string txt)
        {
            if (string.IsNullOrEmpty(txt)) { return txt; }
            StringBuilder retval = new StringBuilder(txt.Length);
            for (int ix = 0; ix < txt.Length;)
            {
                int jx = txt.IndexOf('\\', ix);
                if (jx < 0 || jx == txt.Length - 1) jx = txt.Length;
                retval.Append(txt, ix, jx - ix);
                if (jx >= txt.Length) break;
                switch (txt[jx + 1])
                {
                    case 'n': retval.Append('\n'); break;  // Line feed
                    case 'r': retval.Append('\r'); break;  // Carriage return
                    case 't': retval.Append('\t'); break;  // Tab
                    case '0': retval.Append('\0'); break;  // Null
                    case 'a': retval.Append('\a'); break;  // Bell
                    case 'b': retval.Append('\b'); break;  // Backspace
                    case 'f': retval.Append('\f'); break;  // Form feed
                    case 'v': retval.Append('\v'); break;  // Vertical tab
                    case '\\': retval.Append('\\'); break; // Don't escape
                    default:                                 // Unrecognized, copy as-is
                        retval.Append('\\').Append(txt[jx + 1]); break;
                }
                ix = jx + 2;
            }
            return retval.ToString();
        }
    }
}

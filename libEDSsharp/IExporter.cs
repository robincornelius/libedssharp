using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libEDSsharp
{
    public interface IExporter
    {
        void export(string folderpath, string filename, string gitVersion, EDSsharp eds , string odname="xyz");
    }
}

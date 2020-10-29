using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libEDSsharp
{
    public static class ExporterFactory
    {
        public enum Exporter
        {
            CANOPENNODE_LEGACY = 0,
            CANOPENNODE_V4 = 1,
        }

        public static IExporter getExporter(Exporter ex = Exporter.CANOPENNODE_LEGACY)
        {
            IExporter exporter;

            switch (ex)
            {
                default:
                case Exporter.CANOPENNODE_LEGACY:
                    exporter = new CanOpenNodeExporter();
                    break;

                case Exporter.CANOPENNODE_V4:
                    exporter = new CanOpenNodeExporter_V4();
                    break;
            }
            

            return exporter;
        }
    }
}

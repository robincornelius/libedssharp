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
            CANOPENNODE_V2 = 1,
            CANOPENNODE_EXPERIMENTAL = 2,
        }

        public static IExporter getExporter(Exporter ex = Exporter.CANOPENNODE_LEGACY)
        {
            IExporter exporter;

            switch (ex)
            {
                default:
                case Exporter.CANOPENNODE_LEGACY:
                    exporter = new LegacyCanOpenNodeExporter();
                    break;

                case Exporter.CANOPENNODE_V2:
                     exporter = new CanOpenNodeExporter();
                    break;

                case Exporter.CANOPENNODE_EXPERIMENTAL:
                    exporter = new ExperimentalCanOpenNodeExporter();
                    break;
            }
            

            return exporter;
        }
    }
}


namespace libEDSsharp
{
    public static class ExporterFactory
    {
        public enum Exporter
        {
            CANOPENNODE_V4 = 0,
            CANOPENNODE_LEGACY = 1
        }

        public static IExporter getExporter(Exporter ex = Exporter.CANOPENNODE_LEGACY)
        {
            IExporter exporter;

            switch (ex)
            {
                default:
                case Exporter.CANOPENNODE_V4:
                    exporter = new CanOpenNodeExporter_V4();
                    break;

                case Exporter.CANOPENNODE_LEGACY:
                    exporter = new CanOpenNodeExporter();
                    break;
            }


            return exporter;
        }
    }
}

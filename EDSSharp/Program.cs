using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libEDSsharp;

namespace EDSSharp
{
    class Program
    {

        static libEDSsharp.EDSsharp eds = new EDSsharp();
        static string gitversion = "";

        static void Main(string[] args)
        {
            try
            {

                Dictionary<string, string> argskvp = new Dictionary<string, string>();

                int argv = 0;

                for (argv = 0; argv < (args.Length - 1); argv++)
                {
                    if (args[argv] == "--infile")
                    {
                        argskvp.Add("--infile", args[argv + 1]);
                    }

                    if (args[argv] == "--outfile")
                    {
                        argskvp.Add("--outfile", args[argv + 1]);
                    }

                    if (args[argv] == "--type")
                    {
                        argskvp.Add("--type", args[argv + 1]);
                    }

                    argv++;
                }


                if (argskvp.ContainsKey("--type") && argskvp.ContainsKey("--infile") && argskvp.ContainsKey("--outfile"))
                {
                    string infile = argskvp["--infile"];
                    string outfile = argskvp["--outfile"];

                    ExporterFactory.Exporter type = ExporterFactory.Exporter.CANOPENNODE_LEGACY; //sensible default

                    if (argskvp["--type"].IndexOf("4") > 0)
                        type = ExporterFactory.Exporter.CANOPENNODE_V4;

                    switch (Path.GetExtension(infile).ToLower())
                    {
                        case ".xdd":
                            openXDDfile(infile, outfile,type);
                            break;

                        case ".xml":
                            openXMLfile(infile,outfile,type);
                            break;

                        case ".eds":
                            openEDSfile(infile, outfile,InfoSection.Filetype.File_EDS,type);
                            break;


                        default:
                            return;

                    }
                }
                else
                {
                    Console.WriteLine("Usage EDSEditor --type [CanOpenNode|CanOpenNodeV4] --infile file.[xdd|eds|xml] --outfile [CO_OD.c|OD]");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void openEDSfile(string infile, string outfile, InfoSection.Filetype ft, ExporterFactory.Exporter exporttype)
        {
          
            eds.Loadfile(infile);

            exportCOOD(outfile,exporttype);

        }

        private static void exportCOOD(string outpath,ExporterFactory.Exporter type)
        {
            string odname = outpath;

            outpath = Path.GetFullPath(outpath);

            string savePath = Path.GetDirectoryName(outpath);

            eds.fi.exportFolder = savePath;

            Warnings.warning_list.Clear();

            IExporter exporter = ExporterFactory.getExporter(type);

            exporter.export(savePath, Path.GetFileNameWithoutExtension(outpath), gitversion, eds, odname);

            foreach(string warning in Warnings.warning_list)
            {
                Console.WriteLine("WARNING :" + warning);
            }

        }

        private static void openXMLfile(string path,string outpath,ExporterFactory.Exporter exportertype)
        {

            CanOpenXML coxml = new CanOpenXML();
            coxml.readXML(path);

            Bridge b = new Bridge();

            eds = b.convert(coxml.dev);

            eds.projectFilename = path;
            exportCOOD(outpath,exportertype);

        }

        private static void openXDDfile(string path, string outpath,ExporterFactory.Exporter exportertype)
        {
            CanOpenXDD_1_1 coxml_1_1 = new CanOpenXDD_1_1();
            eds = coxml_1_1.ReadXML(path);

            if (eds == null)
            {
                CanOpenXDD coxml = new CanOpenXDD();
                eds = coxml.readXML(path);

                if (eds == null)
                    return;
            }

            eds.projectFilename = path;
            exportCOOD(outpath,exportertype);
        }
    }
}

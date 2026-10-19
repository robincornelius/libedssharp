using System;
using Xunit;
using libEDSsharp;

namespace Tests
{
    public class XddImportExportTest : CanOpenNodeExporter
    {
        [Theory]
        [InlineData(false, ObjectType.RECORD)]
        [InlineData(true, ObjectType.RECORD)]
        [InlineData(false, ObjectType.ARRAY)]
        [InlineData(true, ObjectType.ARRAY)]
        public void TestSubindexEncoding(bool singleByte, ObjectType objectType)
        {
            EDSsharp source = new EDSsharp();
            source.ods.Clear();
            ODentry parent = new ODentry
            {
                Index = 0x2000,
                objecttype = objectType,
                datatype = DataType.UNSIGNED32,
                parameter_name = "Subindex encoding",
                accesstype = EDSsharp.AccessType.rw
            };
            foreach (ushort subindex in new ushort[] { 0, 1, 0x7F, 0xFF })
            {
                parent.addsubobject((byte)subindex, new ODentry("Entry " + subindex, parent.Index,
                    subindex == 0 ? DataType.UNSIGNED8 : DataType.UNSIGNED32,
                    subindex.ToString(), EDSsharp.AccessType.rw, PDOMappingType.no, parent));
            }
            source.ods.Add(parent.Index, parent);

            string filename = System.IO.Path.GetTempFileName();
            try
            {
                new CanOpenXDD().writeXML(filename, source);
                if (singleByte)
                {
                    // Also accept tools which encode the 8-bit subindex without a leading zero byte.
                    string xml = System.IO.File.ReadAllText(filename);
                    xml = System.Text.RegularExpressions.Regex.Replace(xml,
                        "subIndex=\"00([0-9A-Fa-f]{2})\"", "subIndex=\"$1\"");
                    System.IO.File.WriteAllText(filename, xml);
                }

                EDSsharp imported = new CanOpenXDD().readXML(filename);
                Assert.NotNull(imported);
                ODentry actual = imported.ods[parent.Index];
                Assert.Equal(parent.subobjects.Keys, actual.subobjects.Keys);
                foreach (ushort subindex in parent.subobjects.Keys)
                {
                    Assert.Equal(parent.subobjects[subindex].defaultvalue, actual.subobjects[subindex].defaultvalue);
                    Assert.Equal(parent.subobjects[subindex].parameter_name, actual.subobjects[subindex].parameter_name);
                    Assert.Same(actual, actual.subobjects[subindex].parent);
                }
            }
            finally
            {
                System.IO.File.Delete(filename);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("000001")]
        [InlineData("0100")]
        public void TestRejectInvalidSubindexEncoding(string encoding)
        {
            EDSsharp source = new EDSsharp();
            source.ods.Clear();
            ODentry parent = new ODentry
            {
                Index = 0x2000,
                objecttype = ObjectType.RECORD,
                parameter_name = "Invalid subindex encoding"
            };
            parent.addsubobject(0, new ODentry("Highest subindex", parent.Index,
                DataType.UNSIGNED8, "0", EDSsharp.AccessType.ro, PDOMappingType.no, parent));
            source.ods.Add(parent.Index, parent);

            string filename = System.IO.Path.GetTempFileName();
            try
            {
                new CanOpenXDD().writeXML(filename, source);
                string xml = System.IO.File.ReadAllText(filename);
                xml = System.Text.RegularExpressions.Regex.Replace(xml,
                    "subIndex=\"[^\"]*\"", "subIndex=\"" + encoding + "\"");
                System.IO.File.WriteAllText(filename, xml);

                // Never truncate an invalid encoding into a different, apparently valid OD entry.
                Assert.Throws<FormatException>(() => new CanOpenXDD().readXML(filename));
            }
            finally
            {
                System.IO.File.Delete(filename);
            }
        }

        [Fact]
        public void TestImportExportVar()
        {

            eds = new EDSsharp
            {
                ods = new System.Collections.Generic.SortedDictionary<ushort, ODentry>()
            };

            ODentry od = new ODentry
            {
                objecttype = ObjectType.VAR,
                datatype = DataType.UNSIGNED8,
                parameter_name = "Test VAR",
                accesstype = EDSsharp.AccessType.ro,
                PDOtype = PDOMappingType.optional,
                Index = 0x2000
            };
            od.prop.CO_flagsPDO = true;

            eds.ods.Add(0x2000, od);

            string tempfile = System.IO.Path.GetTempFileName();
            CanOpenXDD coxdd = new CanOpenXDD();
            coxdd.writeXML(tempfile, eds);
            CanOpenXDD coxml = new CanOpenXDD();
            eds = coxml.readXML(tempfile);

            od = eds.ods[0x2000];

            if (od.prop.CO_flagsPDO == false)
                throw new Exception("TPDODetect not set in EDS for VAR");

            if (od.PDOtype != PDOMappingType.optional)
                throw new Exception("TPDOMappingType.optional not set in EDS for VAR");

        }

        /// </summary>
        [Fact]
        public void TestImportExportRecord()
        {

            eds = new EDSsharp
            {
                ods = new System.Collections.Generic.SortedDictionary<ushort, ODentry>()
            };

            ODentry od = new ODentry
            {
                objecttype = ObjectType.RECORD,
                parameter_name = "Test REC",
                Index = 0x2000
            };

            ODentry sub = new ODentry();
            sub.parameter_name = "max sub-index";
            sub.datatype = DataType.UNSIGNED8;
            sub.parent = od;
            sub.accesstype = EDSsharp.AccessType.ro;
            sub.defaultvalue = "1";
            sub.PDOtype = PDOMappingType.no;
            sub.objecttype = ObjectType.VAR;
           
            od.subobjects.Add(0x00, sub);

            sub = new ODentry();
            sub.parameter_name = "entry 1";
            sub.datatype = DataType.UNSIGNED8;
            sub.parent = od;
            sub.accesstype = EDSsharp.AccessType.rw;
            sub.defaultvalue = "0";
            sub.PDOtype = PDOMappingType.optional;
            sub.objecttype = ObjectType.VAR;
            sub.prop.CO_flagsPDO = true;

            od.subobjects.Add(0x01, sub);

            eds.ods.Add(0x2000, od);

            string tempfile = System.IO.Path.GetTempFileName();
            CanOpenXDD coxdd = new CanOpenXDD();
            coxdd.writeXML(tempfile, eds);
            CanOpenXDD coxml = new CanOpenXDD();
            eds = coxml.readXML(tempfile);

            od = eds.ods[0x2000];

            if (od.subobjects[1].prop.CO_flagsPDO == false)
                throw new Exception("TPDODetect not set in EDS for REC");

            if (od.subobjects[1].PDOtype != PDOMappingType.optional)
                throw new Exception("TPDOMappingType.optional not set in EDS for REC");

        }

        [Fact]
        public void TestImportExportArray()
        {

            // NOTE although can opennode does not support per array entry flags, they are supported in EDS
            // so the  PDOtype and TPDODetectCos flags are set per array entry (every VAR sub object) but 
            // they all must be the same
            // and they should not exist on the parent object.

            eds = new EDSsharp
            {
                ods = new System.Collections.Generic.SortedDictionary<ushort, ODentry>()
            };

            ODentry od = new ODentry
            {
                objecttype = ObjectType.ARRAY,
                datatype = DataType.UNSIGNED32,
                parameter_name = "Test Array",
                accesstype = EDSsharp.AccessType.rw,
                Index = 0x2000
            };

            ODentry sub = new ODentry();
            sub.parameter_name = "max sub-index";
            sub.datatype = DataType.UNSIGNED8;
            sub.parent = od;
            sub.accesstype = EDSsharp.AccessType.ro;
            sub.PDOtype = PDOMappingType.no;
       

            sub.defaultvalue = "2";
            sub.objecttype = ObjectType.VAR;

            od.subobjects.Add(0x00, sub);

            sub = new ODentry();
            sub.parameter_name = "entry 1";
            sub.datatype = DataType.UNSIGNED32;
            sub.parent = od;
            sub.accesstype = EDSsharp.AccessType.rw;
            sub.defaultvalue = "0";
            sub.objecttype = ObjectType.VAR;
            sub.PDOtype = PDOMappingType.optional;
            sub.prop.CO_flagsPDO = true;
   
            od.subobjects.Add(0x01, sub);
 
            sub = new ODentry();
            sub.parameter_name = "entry 2";
            sub.datatype = DataType.UNSIGNED32;
            sub.parent = od;
            sub.accesstype = EDSsharp.AccessType.rw;
            sub.defaultvalue = "0";
            sub.objecttype = ObjectType.VAR;
            sub.PDOtype = PDOMappingType.optional;
            sub.prop.CO_flagsPDO = true;
   
            od.subobjects.Add(0x02, sub);

            eds.ods.Add(0x2000, od);

            string tempfile = System.IO.Path.GetTempFileName();
            CanOpenXDD coxdd = new CanOpenXDD();
            coxdd.writeXML(tempfile, eds);
            CanOpenXDD coxml = new CanOpenXDD();
            eds = coxml.readXML(tempfile);

            od = eds.ods[0x2000];

            if (od.subobjects[1].prop.CO_flagsPDO == false)
                throw new Exception("TPDODetect not set in EDS for ARRAY");

            if (od.subobjects[1].PDOtype != PDOMappingType.optional)
                throw new Exception("TPDOMappingType.optional not set in EDS for ARRAY");



        }


    }
}

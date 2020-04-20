using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using libEDSsharp;

namespace Tests
{
    [TestClass]
    public class XddImportExportTest : CanOpenNodeExporter
    {
        /// </summary>
        [TestMethod]
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
                TPDODetectCos = true,
                PDOtype = PDOMappingType.optional,
                Index = 0x2000
            };

            eds.ods.Add(0x2000, od);

            string tempfile = System.IO.Path.GetTempFileName();
            CanOpenXDD coxdd = new CanOpenXDD();
            coxdd.writeXML(tempfile, eds);
            CanOpenXDD coxml = new CanOpenXDD();
            eds = coxml.readXML(tempfile);

            od = eds.ods[0x2000];

            if (od.TPDODetectCos == false)
                throw new Exception("TPDODetect not set in EDS for VAR");

            if (od.PDOtype != PDOMappingType.optional)
                throw new Exception("TPDOMappingType.optional not set in EDS for VAR");

        }

        /// </summary>
        [TestMethod]
        public void TestImportExportRecord()
        {

            eds = new EDSsharp
            {
                ods = new System.Collections.Generic.SortedDictionary<ushort, ODentry>()
            };

            ODentry od = new ODentry
            {
                objecttype = ObjectType.REC,
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
            sub.TPDODetectCos = true;

            od.subobjects.Add(0x01, sub);

            eds.ods.Add(0x2000, od);

            string tempfile = System.IO.Path.GetTempFileName();
            CanOpenXDD coxdd = new CanOpenXDD();
            coxdd.writeXML(tempfile, eds);
            CanOpenXDD coxml = new CanOpenXDD();
            eds = coxml.readXML(tempfile);

            od = eds.ods[0x2000];

            if (od.subobjects[1].TPDODetectCos == false)
                throw new Exception("TPDODetect not set in EDS for REC");

            if (od.subobjects[1].PDOtype != PDOMappingType.optional)
                throw new Exception("TPDOMappingType.optional not set in EDS for REC");

        }

        [TestMethod]
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
            sub.TPDODetectCos = true;
   
            od.subobjects.Add(0x01, sub);
 
            sub = new ODentry();
            sub.parameter_name = "entry 2";
            sub.datatype = DataType.UNSIGNED32;
            sub.parent = od;
            sub.accesstype = EDSsharp.AccessType.rw;
            sub.defaultvalue = "0";
            sub.objecttype = ObjectType.VAR;
            sub.PDOtype = PDOMappingType.optional;
            sub.TPDODetectCos = true;
   
            od.subobjects.Add(0x02, sub);

            eds.ods.Add(0x2000, od);

            string tempfile = System.IO.Path.GetTempFileName();
            CanOpenXDD coxdd = new CanOpenXDD();
            coxdd.writeXML(tempfile, eds);
            CanOpenXDD coxml = new CanOpenXDD();
            eds = coxml.readXML(tempfile);

            od = eds.ods[0x2000];

            if (od.subobjects[1].TPDODetectCos == false)
                throw new Exception("TPDODetect not set in EDS for ARRAY");

            if (od.subobjects[1].PDOtype != PDOMappingType.optional)
                throw new Exception("TPDOMappingType.optional not set in EDS for ARRAY");



        }


    }
}

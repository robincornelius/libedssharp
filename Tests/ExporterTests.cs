using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using libEDSsharp;

namespace Tests
{
    [TestClass]
    public class ExporterTests : CanOpenNodeExporter
    {
        [TestMethod]
        public void Test_cname_conversion()
        {

            if (make_cname("axle 0 wheel right controlword") != "axle0WheelRightControlword")
                throw (new Exception("make_cname Conversion error"));

            if (make_cname("mapped object 4") != "mappedObject4")
                throw (new Exception("make_cname Conversion error"));

            if (make_cname("COB ID used by RPDO") != "COB_IDUsedByRPDO")
                throw (new Exception("make_cname Conversion error"));
        }

        [TestMethod]
        public void Test_record_objects()
        {

            ODentry od = new ODentry
            {
                objecttype = ObjectType.REC,
                parameter_name = "Test Record",
                accesstype = EDSsharp.AccessType.ro,
                Index = 0x2000
            };

            ODentry subod = new ODentry("Test String 1", 0x2000, DataType.VISIBLE_STRING, "abcdefg", EDSsharp.AccessType.rw, PDOMappingType.optional, od);

            string test = export_one_record_type(subod, "");

            if (test != "           {(void*)&CO_OD_RAM.testRecord.testString1, 0x3E, 0x7 }," + Environment.NewLine)
                throw (new Exception("export_one_record_type() error test 1"));

            subod = new ODentry("Test String 2", 0x01, DataType.VISIBLE_STRING, new string('*', 255), EDSsharp.AccessType.ro, PDOMappingType.optional, od);
            test = export_one_record_type(subod, "");

            if (test != "           {(void*)&CO_OD_RAM.testRecord.testString2, 0x26, 0xFF }," + Environment.NewLine)
                throw (new Exception("export_one_record_type() error test 2"));

        }

        [TestMethod]
        public void TestArrays()
        {

            ODentry od = new ODentry
            {
                objecttype = ObjectType.ARRAY,
                datatype = DataType.VISIBLE_STRING,
                parameter_name = "Test Array",
                accesstype = EDSsharp.AccessType.ro,
                Index = 0x2000
            };

            eds = new EDSsharp
            {
                ods = new System.Collections.Generic.SortedDictionary<ushort, ODentry>()
            };

            eds.ods.Add(0x2000, od);

            prewalkArrays();
            od.subobjects.Add(0x00, new ODentry("No Entries", 0x00, DataType.UNSIGNED8, "4", EDSsharp.AccessType.ro, PDOMappingType.no));
            od.subobjects.Add(0x01, new ODentry("LINE1", 0x01, DataType.VISIBLE_STRING, new string('*', 1), EDSsharp.AccessType.ro, PDOMappingType.optional));
            od.subobjects.Add(0x02, new ODentry("LINE1", 0x02, DataType.VISIBLE_STRING, new string('*', 10), EDSsharp.AccessType.ro, PDOMappingType.optional));
            od.subobjects.Add(0x03, new ODentry("LINE1", 0x03, DataType.VISIBLE_STRING, new string('*', 16), EDSsharp.AccessType.ro, PDOMappingType.optional));
            od.subobjects.Add(0x04, new ODentry("LINE1", 0x04, DataType.VISIBLE_STRING, new string('*', 32), EDSsharp.AccessType.ro, PDOMappingType.optional));

            string test = print_h_entry(od);

            if (test != "/*2000      */ VISIBLE_STRING  testArray[32][4];" + Environment.NewLine)
                throw (new Exception("TestArrays() test 1 failed"));


        }


        /// <summary>
        /// Check we can override maxsub index correctly
        /// </summary>
        [TestMethod]
        public void TestArrayNoEntries()
        {
            eds = new EDSsharp
            {
                ods = new System.Collections.Generic.SortedDictionary<ushort, ODentry>()
            };

            ODentry od = new ODentry
            {
                objecttype = ObjectType.ARRAY,
                datatype = DataType.VISIBLE_STRING,
                parameter_name = "Test Array",
                accesstype = EDSsharp.AccessType.ro,
                Index = 0x1011
            };

            eds.ods.Add(0x1011, od);

            od.subobjects.Add(0x00, new ODentry("No Entries", 0x00, DataType.UNSIGNED8, "0x7f", EDSsharp.AccessType.ro, PDOMappingType.no));
            od.subobjects.Add(0x01, new ODentry("LINE1", 0x01, DataType.UNSIGNED32, "0x01", EDSsharp.AccessType.ro, PDOMappingType.optional));
          
            string test = write_od_line(od);
            if(test != "{0x1011, 0x7F, 0x06,  0, (void*)&CO_OD_RAM.testArray[0]}," + Environment.NewLine)
                throw (new Exception("TestArrayNoEntries() failed"));


            od = new ODentry
            {
                objecttype = ObjectType.ARRAY,
                datatype = DataType.VISIBLE_STRING,
                parameter_name = "Test Array",
                accesstype = EDSsharp.AccessType.ro,
                Index = 0x2000
            };

            eds.ods.Add(0x2000, od);

            od.subobjects.Add(0x00, new ODentry("No Entries", 0x00, DataType.UNSIGNED8, "0x7f", EDSsharp.AccessType.ro, PDOMappingType.no));
            od.subobjects.Add(0x01, new ODentry("LINE1", 0x01, DataType.UNSIGNED32, "0x01", EDSsharp.AccessType.ro, PDOMappingType.optional));

            test = write_od_line(od);
            if (test != "{0x2000, 0x7F, 0x06,  0, (void*)&CO_OD_RAM.testArray[0]}," + Environment.NewLine)
                throw (new Exception("TestArrayNoEntries() failed"));


            od = new ODentry
            {
                objecttype = ObjectType.ARRAY,
                datatype = DataType.VISIBLE_STRING,
                parameter_name = "Test Array",
                accesstype = EDSsharp.AccessType.ro,
                Index = 0x1003
            };

            eds.ods.Add(0x1003, od);

            od.subobjects.Add(0x00, new ODentry("No Entries", 0x00, DataType.UNSIGNED8, "0x00", EDSsharp.AccessType.ro, PDOMappingType.no));
            od.subobjects.Add(0x01, new ODentry("LINE1", 0x01, DataType.UNSIGNED32, "0x01", EDSsharp.AccessType.ro, PDOMappingType.optional));
            od.subobjects.Add(0x02, new ODentry("LINE1", 0x02, DataType.UNSIGNED32, "0x01", EDSsharp.AccessType.ro, PDOMappingType.optional));
            od.subobjects.Add(0x03, new ODentry("LINE1", 0x03, DataType.UNSIGNED32, "0x01", EDSsharp.AccessType.ro, PDOMappingType.optional));

            test = write_od_line(od);
            if (test != "{0x1003, 0x03, 0x06,  0, (void*)&CO_OD_RAM.testArray[0]}," + Environment.NewLine)
                throw (new Exception("TestArrayNoEntries() failed"));



        }
    }
}

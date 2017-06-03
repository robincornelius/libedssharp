using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using libEDSsharp;

namespace Tests
{
    [TestClass]
    public class EDSParserTests : EDSsharp
    {

        [TestMethod]
        public void Test_parser()
        {

            string[] teststrings =
                {   "ParameterName=Error register",
                    "ParameterName =Error register",
                    "ParameterName = Error register",
                    "ParameterName =Error register",
                    "ParameterName = Error register ",
            };


            foreach (string teststring in teststrings)
            {
                eds.Clear();
                sectionname = "Tests";
                parseline(teststring);

                if (!eds["Tests"].ContainsKey("ParameterName"))
                    throw (new Exception("Parser key detection error on string \"" + teststring + "\""));

                if (eds["Tests"]["ParameterName"] != "Error register")
                    throw (new Exception("Parser value detection error on string \"" + teststring + "\""));
            }

        }

        public void injectobject(string testobject)
        {
            eds.Clear();


            string[] lines = testobject.Split('\n');

            foreach (string line in lines)
                parseline(line);

            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in eds)
            {
                parseEDSentry(kvp);
            }

        }

        [TestMethod]
        public void Test_CompactSubObj()
        {


            string testobject = @"[1003]
ParameterName=Pre-defined error field
ObjectType=0x8
DataType=0x0007
AccessType=rw
CompactSubObj=9
";

            injectobject(testobject);

            if (!ods.ContainsKey(0x1003))
                throw (new Exception("parseEDSentry() failed no object"));

            ODentry od = ods[0x1003];

            if (od.subobjects.Count != 10)
                throw (new Exception("parseEDSentry() CompactSubObj faield to generate children"));

            ODentry sub = od.getsubobject(0);

            if (sub.parameter_name != "NrOfObjects")
                throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

            if (sub.datatype != DataType.UNSIGNED8)
                throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

            if (sub.accesstype != AccessType.ro)
                throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

            UInt16 defaultvalue = EDSsharp.ConvertToByte(sub.defaultvalue);

            if (defaultvalue != 9)
                throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

            if (sub.PDOtype != PDOMappingType.no)
                throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

            for (int x = 1; x < 10; x++)
            {
                if (!od.containssubindex(x))
                    throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

                sub = od.getsubobject(x);

                string name = string.Format("{0}{1:00}", od.parameter_name, x);
                if (sub.parameter_name != name)
                    throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

                if (sub.datatype != DataType.UNSIGNED32)
                    throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

                if (sub.accesstype != AccessType.rw)
                    throw (new Exception("parseEDSentry() CompactSubObj incorrect generation"));

            }


        }

        public void test_object_coverage(List<string> MandatorySet, List<string> OptionalSet)
        {
            int MandatoryBitMask = (int)Math.Pow(2, MandatorySet.Count);

            int x;
            for (x = 1; x < MandatoryBitMask; x++)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[2000]");

                for (int y = 0; y < MandatorySet.Count; y++)
                {
                    if (((1 << y) & x) != 0)
                    {
                        sb.AppendLine(MandatorySet[y]);
                    }
                }

                string teststring = sb.ToString();

                bool ok = false;

                try
                {
                    injectobject(teststring);
                    if (x == (MandatoryBitMask - 1))
                    {
                        ok = true;
                    }
                }
                catch (ParameterException p)
                {
                    ok = true;

                    if (x == (MandatoryBitMask - 1))
                    {
                        throw new Exception("Test_array_object_loader() failed this did fire exception when it should not:\n\r" + sb.ToString());
                    }
                }

                if (ok == false)
                    throw new Exception("Test_array_object_loader() failed this did not fire exception when it should :\n\r" + sb.ToString());
            }

        }

        [TestMethod]
        public void Test_var_object_loader()
        {

            List<string> MandatorySet = new List<string>();
            List<string> OptionalSet = new List<string>();

            MandatorySet.Add("ParameterName=My Test Object");
            MandatorySet.Add("DataType=0x0007");
            MandatorySet.Add("AccessType=rw");

            test_object_coverage(MandatorySet, OptionalSet);


            return;

            /*


            //minimum mandatory var data set
            string testobject = @"[2000]
ParameterName=My Test Object
DataType=0x0007
AccessType=rw
";

            injectobject(testobject);

            ODentry od = ods[0x2000];

            if (od.objecttype != ObjectType.VAR)
                throw (new Exception("Test_object_loader() 1 failed"));

            if (od.defaultvalue != "")
                throw (new Exception("Test_object_loader() 2 failed"));

            if (od.PDOMapping != false)
                throw (new Exception("Test_object_loader() 3 failed"));

            if (od.LowLimit != "")
                throw (new Exception("Test_object_loader() 4 failed"));

            if (od.HighLimit != "")
                throw (new Exception("Test_object_loader() 5 failed"));

            if (od.ObjFlags != 0)
                throw (new Exception("Test_object_loader() 6 failed"));


            //Fail tests

            testobject = @"[2000]
ParameterName=My Test Object
DataType=0x0007
";

            bool ok = false;
            try
            {
                injectobject(testobject);
            }
            catch(ParameterException pe)
            {
                ok = true;
            }

            if (ok = false)
                throw new Exception("Test_object_loader() 7 failed");

            // Missing parameter

            ok = false;
            testobject = @"[2000]
ParameterName=My Test Object
AccessType=rw
";

            try
            {
                injectobject(testobject);
            }
            catch (ParameterException pe)
            {
                ok = true;
            }

            if (ok = false)
                throw new Exception("Test_object_loader() 8 failed");


            // Missing paramater 

            ok = false;
            testobject = @"[2000]
DataType=0x0007
AccessType=rw
";

            try
            {
                injectobject(testobject);
            }
            catch (ParameterException pe)
            {
                ok = true;
            }

            if (ok = false)
                throw new Exception("Test_object_loader() 9 failed");
                */

        }

        [TestMethod]
        public void Test_array_object_loader()
        {

            List<string> MandatorySet = new List<string>();
            List<string> OptionalSet = new List<string>();

            MandatorySet.Add("ParameterName=My Test Object");
            MandatorySet.Add("ObjectType=0x0008");
            MandatorySet.Add("SubNumber=5");

            test_object_coverage(MandatorySet, OptionalSet);

         
        }

        [TestMethod]
        public void Test_compact_array_object_loader()
        {

            List<string> MandatorySet = new List<string>();
            List<string> OptionalSet = new List<string>();

            MandatorySet.Add("ParameterName=My Test Object");
            MandatorySet.Add("ObjectType=0x0008");
            MandatorySet.Add("CompactSubObj=5");
            MandatorySet.Add("DataType=0x0007");
            MandatorySet.Add("AccessType=rw");

            test_object_coverage(MandatorySet, OptionalSet);


        }
    }

}

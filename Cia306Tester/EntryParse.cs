using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using libEDSsharp;
using System.Collections.Generic;

namespace Cia306Tester
{
    [TestClass]
    public class EntryParse
    {
        [TestMethod]
        public void KeyEntryValueInterpretationHex()
        {
            EDSsharp eds = new EDSsharp();

            Dictionary<string, Dictionary<string, string>> od = new Dictionary<string,Dictionary<string,string>>();
            
            od.Add("1000",new Dictionary<string,string>());
            od["1000"].Add("ParameterName", "Test");
            od["1000"].Add("DataType", "5");
            od["1000"].Add("AccessType", "rw");


            foreach(KeyValuePair<string, Dictionary<string, string>> kvp in od)
            {
               eds.parseEDSentry(kvp);


                   if(!eds.ods.ContainsKey("1000"))
                       throw new Exception("Failed to parse key HEX");
               
                
            }

        }

        [TestMethod]
        public void UnknownAccessType()
        {
            EDSsharp eds;
            Dictionary<string, Dictionary<string, string>> od;

            eds = new EDSsharp();      
            od = new Dictionary<string, Dictionary<string, string>>();


            od.Add("1000", new Dictionary<string, string>());
            od["1000"].Add("ParameterName", "Test");
            od["1000"].Add("DataType", "5");
            od["1000"].Add("AccessType", "foobar");

            try
            {
                eds.parseEDSentry(new KeyValuePair<string, Dictionary<string, string>>("1000",od["1000"]));
            }
            catch(ParameterException e)
            {
                return;
            }

            throw new Exception("Accesstype validation failed");

        }

        [TestMethod]
        public void CheckParamaterNamerequired()
        {
            EDSsharp eds = new EDSsharp();

            Dictionary<string, Dictionary<string, string>> od = new Dictionary<string, Dictionary<string, string>>();
            od.Add("1000", new Dictionary<string, string>());
            od["1000"].Add("ObjectType", "0x7");
            //od["1000"].Add("ParameterName", "Test");
            od["1000"].Add("DataType", "Test");

            try
            {
                eds.parseEDSentry(new KeyValuePair<string, Dictionary<string, string>>("1000", od["1000"]));
            }
            catch (ParameterException e)
            {
                return;
            }

            throw new Exception("Missing paramater ParameterName check failed");

        }

        [TestMethod]
        public void CheckParamaterDataTyperequired()
        {
            EDSsharp eds = new EDSsharp();

            Dictionary<string, Dictionary<string, string>> od = new Dictionary<string, Dictionary<string, string>>();
            od.Add("1000", new Dictionary<string, string>());
            od["1000"].Add("ObjectType", "0x7");
            od["1000"].Add("ParameterName", "Test");
            //od["1000"].Add("DataType", "Test");

            try
            {
                eds.parseEDSentry(new KeyValuePair<string, Dictionary<string, string>>("1000", od["1000"]));
            }
            catch (ParameterException e)
            {
                return;
            }

            throw new Exception("Compulasory paramater DataType check failed");

        }

    }
}

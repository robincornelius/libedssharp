using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using libEDSsharp;
using System.Collections.Generic;

namespace Cia306Tester
{
    [TestClass]
    public class ValueEntryValueInterpretation
    {
        [TestMethod]
        public void EntryValueInterpretationHex()
        {
            EDSsharp eds = new EDSsharp();

            Dictionary<string, Dictionary<string, string>> od = new Dictionary<string, Dictionary<string, string>>();
            od.Add("1000", new Dictionary<string, string>());
            od["1000"].Add("ObjectType", "0x1234");
            od["1000"].Add("ParameterName", "Test");

            eds.parseEDSentry(new KeyValuePair<string, Dictionary<string, string>>("1000", od["1000"]));
            
            if((UInt16)eds.ods["1000"].objecttype!=0x1234)
                throw new Exception("Failed to parse value HEX");
           
        }

        [TestMethod]
        public void EntryValueInterpretationDecimal()
        {
            EDSsharp eds = new EDSsharp();

            Dictionary<string, Dictionary<string, string>> od = new Dictionary<string, Dictionary<string, string>>();
            od.Add("1000", new Dictionary<string, string>());
            od["1000"].Add("ObjectType", "1234");
            od["1000"].Add("ParameterName", "Test");

            eds.parseEDSentry(new KeyValuePair<string, Dictionary<string, string>>("1000", od["1000"]));

            if ((UInt16)eds.ods["1000"].objecttype != 1234)
                throw new Exception("Failed to parse value Decimal");

        }

        [TestMethod]
        public void EntryValueInterpretationOctal()
        {
            EDSsharp eds = new EDSsharp();

            Dictionary<string, Dictionary<string, string>> od = new Dictionary<string, Dictionary<string, string>>();
            od.Add("1000", new Dictionary<string, string>());
            od["1000"].Add("ObjectType", "012");
            od["1000"].Add("ParameterName", "Test");

            eds.parseEDSentry(new KeyValuePair<string, Dictionary<string, string>>("1000", od["1000"]));

            if ((UInt16)eds.ods["1000"].objecttype != 10)
                throw new Exception("Failed to parse value Octall");

        }

    }
}

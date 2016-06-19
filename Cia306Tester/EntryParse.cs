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

            foreach(KeyValuePair<string, Dictionary<string, string>> kvp in od)
            {
               eds.parseEDSentry(kvp);

               foreach(ODentry o in eds.ods)
               {
                   if(o.index!=0x1000)
                       throw new Exception("Failed to parse key HEX");
               }
                
            }

        }

    }
}

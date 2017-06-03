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
                    throw (new Exception("Parser key detection error on string \""+teststring+"\""));

                if (eds["Tests"]["ParameterName"] != "Error register")
                    throw (new Exception("Parser value detection error on string \"" + teststring + "\""));
            }

        }
    }
}

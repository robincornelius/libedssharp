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
    }
}

using System;
using Xunit;
using libEDSsharp;

namespace Tests
{
    public class ExporterTestsV4 : CanOpenNodeExporter_V4
    {
        [Fact]
        public void Test_cname_conversion()
        {

            if (Make_cname("axle 0 wheel right controlword") != "axle0WheelRightControlword")
                throw (new Exception("Make_cname Conversion error"));

            if (Make_cname("mapped object 4") != "mappedObject4")
                throw (new Exception("Make_cname Conversion error"));

            if (Make_cname("COB ID used by RPDO") != "COB_IDUsedByRPDO")
                throw (new Exception("Make_cname Conversion error"));

        }
    }
}

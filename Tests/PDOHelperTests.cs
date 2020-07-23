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
    public class PDOHelperTests : libEDSsharp.EDSsharp
    {

        [TestMethod]
        public void Test_TPDO()
        {

            PDOHelper pdo = new PDOHelper(this);

            //configure a new slot
            PDOSlot slot = new PDOSlot();
            pdo.pdoslots.Add(slot);

            slot.COB = 0x180;
            slot.ConfigurationIndex = 0x1800;
            slot.transmissiontype = 254;
            slot.inhibit = 10;
            slot.eventtimer = 20;
            slot.syncstart = 30;


            //fill it with some dummy entries
            ODentry od;
            tryGetODEntry(0x0002, out od);
            slot.Mapping.Add(od);

            tryGetODEntry(0x0003, out od);
            slot.Mapping.Add(od);

            tryGetODEntry(0x0004, out od);
            slot.Mapping.Add(od);

            pdo.buildmappingsfromlists();

            //check configuration object exists
            ODentry comparamOD;

            if (!tryGetODEntry(0x1800, out comparamOD))
            {
                throw new Exception("Communication paramaters not generated");
            }

            ODentry mappingOD;

            if (!tryGetODEntry(0x1a00, out mappingOD))
            {
                throw new Exception("Mapping paramaters not generated");
            }

            if (comparamOD.subobjects.Count != 7)
                throw new Exception("Wrong number of sub objects generated");

            if(comparamOD.Nosubindexes!=7)
                throw new Exception("Wrong number of sub objects generated");

            if (comparamOD.subobjects[1].datatype != DataType.UNSIGNED32)
                throw new Exception("Wrong data type for COB");
            if (comparamOD.subobjects[2].datatype != DataType.UNSIGNED8)
                throw new Exception("Wrong data type for Transmission type");
            if (comparamOD.subobjects[3].datatype != DataType.UNSIGNED16)
                throw new Exception("Wrong data type for Inhibit time");
            if (comparamOD.subobjects[4].datatype != DataType.UNSIGNED8)
                throw new Exception("Wrong data type for Compatibility Entry");
            if (comparamOD.subobjects[5].datatype != DataType.UNSIGNED16)
                throw new Exception("Wrong data type for Event timer");
            if (comparamOD.subobjects[6].datatype != DataType.UNSIGNED8)
                throw new Exception("Wrong data type for Sync Start");

            if (comparamOD.subobjects[1].defaultvalue != "384") //180 hex
                throw new Exception("TPDO COB wrong");
            if (comparamOD.subobjects[2].defaultvalue != "254")
                throw new Exception("TPDO transmission type wrong");
            if (comparamOD.subobjects[3].defaultvalue != "10")
                throw new Exception("TPDO inhibit wrong");
            if (comparamOD.subobjects[5].defaultvalue != "20")
                throw new Exception("TPDO event timer wrong");
            if (comparamOD.subobjects[6].defaultvalue != "30")
                throw new Exception("TPDO sync start wrong");

        }

        [TestMethod]
        public void Test_RPDO()
        {

            PDOHelper pdo = new PDOHelper(this);

            //configure a new slot
            PDOSlot slot = new PDOSlot();
            pdo.pdoslots.Add(slot);

            slot.COB = 0x401;
            slot.ConfigurationIndex = 0x1400;
            
            slot.transmissiontype = 254;
            slot.inhibit = 10;
            slot.eventtimer = 20;
            slot.syncstart = 30;


            //fill it with some dummy entries
            ODentry od;
            tryGetODEntry(0x0002, out od);
            slot.Mapping.Add(od);

            tryGetODEntry(0x0003, out od);
            slot.Mapping.Add(od);

            tryGetODEntry(0x0004, out od);
            slot.Mapping.Add(od);

            pdo.buildmappingsfromlists();

            //check configuration object exists
            ODentry comparamOD;

            if (!tryGetODEntry(0x1400, out comparamOD))
            {
                throw new Exception("Communication paramaters not generated");
            }

            ODentry mappingOD;

            if (!tryGetODEntry(0x1600, out mappingOD))
            {
                throw new Exception("Mapping paramaters not generated");
            }

            if (comparamOD.subobjects.Count != 3)
                throw new Exception("Wrong number of sub objects generated");

            if (comparamOD.Nosubindexes != 3)
                throw new Exception("Wrong number of sub objects generated");

            if (comparamOD.subobjects[1].datatype != DataType.UNSIGNED32)
                throw new Exception("Wrong data type for COB");
            if (comparamOD.subobjects[2].datatype != DataType.UNSIGNED8)
                throw new Exception("Wrong data type for Transmission type");
     
            if (comparamOD.subobjects[1].defaultvalue != "1025") //481 hex
                throw new Exception("TPDO COB wrong");
            if (comparamOD.subobjects[2].defaultvalue != "254")
                throw new Exception("TPDO transmission type wrong");
          
        }


        [TestMethod]
        public void Test_Bits()
        {

            PDOHelper pdo = new PDOHelper(this);

            //configure a new slot
            PDOSlot slot = new PDOSlot();
            pdo.pdoslots.Add(slot);

            ODentry od = new ODentry();

            od.Index = 0x3000;
            od.parameter_name = "Bit";
            od.datatype = DataType.BOOLEAN;
            od.PDOtype = PDOMappingType.optional;

            slot.insertMapping(0, od);
            slot.insertMapping(0, od);
            slot.insertMapping(0, od);
            slot.insertMapping(0, od);
            slot.insertMapping(0, od);
            slot.insertMapping(0, od);
            slot.insertMapping(0, od);
            slot.insertMapping(0, od);

        }

    }
}

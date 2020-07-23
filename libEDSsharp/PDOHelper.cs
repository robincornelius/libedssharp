using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libEDSsharp
{

    public class PDOSlot
    {

        private UInt16 _MappingIndex;
        private UInt16 _ConfigurationIndex;

        public ushort ConfigurationIndex
        {
            get { return _ConfigurationIndex; }
            set {

                if(value==0)
                {
                    _ConfigurationIndex = 0;
                    _MappingIndex = 0;
                    return;
                }

                if ( ((value >= 0x1400) && (value < 0x1600)) || ((value >=0x1800) && (value <0x1a00)) )
                {
                    _ConfigurationIndex = value; _MappingIndex = (UInt16)(_ConfigurationIndex + (UInt16)0x200);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Configuration Index", "Must be between 0x1400 and 0x17FF");
                }
                   
                   
            }
        }

        public ushort MappingIndex
        {
            get { return _MappingIndex; }
        }
        
        public EDSsharp.AccessType mappingAccessType;
        public EDSsharp.AccessType configAccessType;
        public string mappingloc;
        public string configloc;

        public uint COB;

        public bool isTXPDO()
        {
            return ConfigurationIndex >= 0x1800;
        }

        public bool isRXPDO()
        {
            return ConfigurationIndex < 0x1800;
        }

        public bool invalid
        {
            get
            {
                return (COB & 0x80000000) != 0;
            }
            set
            {
                if (value == true)
                    COB = COB | 0x80000000;
                else
                    COB = COB & 0x0FFFFFFF;
            }
        }
 
        public List<ODentry> Mapping = new List<ODentry>();

        public UInt16 inhibit;
        public UInt16 eventtimer;
        public byte syncstart;
        public byte transmissiontype;

        public PDOSlot()
        {
            Mapping = new List<ODentry>();
        }

        public string getTargetName(ODentry od)
        {
            string target = "";

            if (od.Index >= 0x0002 && od.Index <= 0x007)
            {
                //the dummy objects
                switch (od.Index)
                {
                    case 0x002:
                        target = "0x0002/00/Dummy Int8";
                        break;
                    case 0x003:
                        target = "0x0003/00/Dummy Int16";
                        break;
                    case 0x004:
                        target = "0x0004/00/Dummy Int32";
                        break;
                    case 0x005:
                        target = "0x0005/00/Dummy UInt8";
                        break;
                    case 0x006:
                        target = "0x0006/00/Dummy UInt16";
                        break;
                    case 0x007:
                        target = "0x0007/00/Dummy UInt32";
                        break;
                }

            }
            else
            {
                target = String.Format("0x{0:x4}/{1:x2}/", od.Index, od.Subindex) + od.parameter_name;
            }

            return target;

        }

        public void insertMapping(int ordinal, ODentry entry)
        {
            int size = 0;
            foreach(ODentry e in Mapping)
            {
                size += e.Sizeofdatatype();
            }

            if (size + entry.Sizeofdatatype() > 64)
                return;

            Mapping.Insert(ordinal,entry);
        }

    }


    public class PDOHelper
    {

        EDSsharp eds;

        public PDOHelper(EDSsharp eds)
        {
            this.eds = eds;
        }

        public List<PDOSlot> pdoslots = new List<PDOSlot>();

        public void build_PDOlists()
        {
            //List<ODentry> odl = new List<ODentry>();
            build_PDOlist(0x1800,pdoslots);
            build_PDOlist(0x1400,pdoslots);

        }

        public void build_PDOlist(UInt16 startcob, List<PDOSlot> slots)
        {
            for (UInt16 idx = startcob; idx < startcob + 0x01ff; idx++)
            {
                if (eds.ods.ContainsKey(idx))
                {
                    ODentry od = eds.ods[idx];
                    if (od.Disabled == true)
                        continue;

                    //protect against not completed new CommunicationParamater sections
                    //we probably could do better and do more checking but as long as
                    //we protect against the subobjects[1] read in a few lines all else is
                    //good
                    if (od.subobjects.Count <= 1)
                        continue;

                    PDOSlot slot = new PDOSlot();

                    bool nodeidpresent;

                    slot.COB = eds.GetNodeID(od.subobjects[1].defaultvalue, out nodeidpresent);

                    if (od.Containssubindex(2))
                        slot.transmissiontype = EDSsharp.ConvertToByte(od.Getsubobject(2).defaultvalue);
                    
                    if (od.Containssubindex(3))
                        slot.inhibit = EDSsharp.ConvertToUInt16(od.Getsubobject(3).defaultvalue);

                    if (od.Containssubindex(5))
                        slot.eventtimer = EDSsharp.ConvertToUInt16(od.Getsubobject(5).defaultvalue);

                    if (od.Containssubindex(6))                  
                        slot.syncstart = EDSsharp.ConvertToByte(od.Getsubobject(6).defaultvalue);

                    slot.ConfigurationIndex = idx;

                    slot.configAccessType = od.accesstype;
                    slot.configloc = od.StorageLocation;


                    Console.WriteLine(String.Format("Found PDO Entry {0:x4} {1:x3}", idx, slot.COB));

                    //Look at mappings

                    ODentry mapping = eds.Getobject((ushort)(idx + 0x200));
                    if(mapping==null)
                    {
                        Console.WriteLine(string.Format("No mapping for index 0x{0:x4} should be at 0x{1:x4}", idx, idx + 0x200));
                        continue;
                    }

                    uint totalsize = 0;

                    slot.mappingAccessType = od.accesstype;
                    slot.mappingloc = od.StorageLocation;

                    for (ushort subindex= 1; subindex<= mapping.Getmaxsubindex();subindex++)
                    {
                        ODentry sub = mapping.Getsubobject(subindex);
                        if (sub == null)
                            continue;

                        //Decode the mapping

                        UInt32 data = 0;

                        if (sub.defaultvalue.Length < 10)
                            continue;

                        if (sub.defaultvalue != "")
                            data = Convert.ToUInt32(sub.defaultvalue, EDSsharp.Getbase(sub.defaultvalue));

                        if (data == 0)
                            continue;

                        byte datasize = (byte)(data & 0x000000FF);
                        UInt16 pdoindex = (UInt16)((data >> 16) & 0x0000FFFF);
                        byte pdosub = (byte)((data >> 8) & 0x000000FF);

                        totalsize += datasize;

                        Console.WriteLine(string.Format("Mapping 0x{0:x4}/{1:x2} size {2}", pdoindex, pdosub, datasize));

                        //validate this against what is in the actual object mapped

                        ODentry maptarget;
                        if (pdosub == 0)
                        {
                            if(eds.tryGetODEntry(pdoindex, out maptarget)==false)
                            {
                                Console.WriteLine("MAPPING FAILED");
                                //Critical PDO error
                                return;
                            }
                        }
                        else
                            maptarget = eds.ods[pdoindex].Getsubobject(pdosub);

                        if (maptarget.Disabled == false && datasize == (maptarget.Sizeofdatatype()))
                        {
                            //mappingfail = false;
                        }
                        else
                        {
                            Console.WriteLine(String.Format("MAPPING FAILED {0} != {1}",datasize, maptarget.Sizeofdatatype()));
                        }

                        slot.Mapping.Add(maptarget);

                    }

                    Console.WriteLine(String.Format("Total PDO Size {0}\n", totalsize));

                    slots.Add(slot);
                }
            }

        }

        /// <summary>
        /// Rebuild the communication and mapping paramaters from the
        /// lists the PDOhelper currently has. These live in the list pdoslots
        /// </summary>
        public void buildmappingsfromlists()
        {
            for(ushort x=0x1400;x<0x1c00;x++)
            {
                if (eds.ods.ContainsKey(x))
                    eds.ods.Remove(x);
            }

            foreach(PDOSlot slot in pdoslots)
            {

                ODentry config = new ODentry();
                config.Index = slot.ConfigurationIndex;
                config.datatype = DataType.PDO_COMMUNICATION_PARAMETER;
                config.objecttype = ObjectType.REC;

                ODentry sub = new ODentry("max sub-index", (ushort)slot.ConfigurationIndex, 0);
                sub.defaultvalue = "6";
                sub.datatype = DataType.UNSIGNED8;
                config.addsubobject(0x00,sub);

                config.accesstype = slot.configAccessType;
                config.StorageLocation = slot.configloc;


                if (slot.isTXPDO())
                {

                    config.parameter_name = "TPDO communication parameter";

                    sub = new ODentry("COB-ID used by TPDO", (ushort)slot.ConfigurationIndex, 1);
                    sub.datatype = DataType.UNSIGNED32;
                    sub.defaultvalue = slot.COB.ToHexString();
                    config.addsubobject(0x01, sub);

                    sub = new ODentry("transmission type", (ushort)slot.ConfigurationIndex, 2);
                    sub.datatype = DataType.UNSIGNED8;
                    sub.defaultvalue = slot.transmissiontype.ToString();
                    config.addsubobject(0x02, sub);

                    sub = new ODentry("inhibit time", (ushort)slot.ConfigurationIndex, 3);
                    sub.datatype = DataType.UNSIGNED16;
                    sub.defaultvalue = slot.inhibit.ToString();
                    config.addsubobject(0x03, sub);

                    sub = new ODentry("compatibility entry", (ushort)slot.ConfigurationIndex, 4);
                    sub.datatype = DataType.UNSIGNED8;
                    sub.defaultvalue = "0";
                    config.addsubobject(0x04, sub);

                    sub = new ODentry("event timer", (ushort)slot.ConfigurationIndex, 5);
                    sub.datatype = DataType.UNSIGNED16;
                    sub.defaultvalue = slot.eventtimer.ToString();
                    config.addsubobject(0x05, sub);

                    sub = new ODentry("SYNC start value", (ushort)slot.ConfigurationIndex, 6);
                    sub.datatype = DataType.UNSIGNED8;
                    sub.defaultvalue = slot.syncstart.ToString(); ;
                    config.addsubobject(0x06, sub);

                }
                else
                {
                    config.parameter_name = "RPDO communication parameter";

                    sub = new ODentry("COB-ID used by RPDO", (ushort)slot.ConfigurationIndex, 1);
                    sub.datatype = DataType.UNSIGNED32;
                    sub.defaultvalue = slot.COB.ToHexString();
                    config.addsubobject(0x01, sub);

                    sub = new ODentry("transmission type", (ushort)slot.ConfigurationIndex, 2);
                    sub.datatype = DataType.UNSIGNED8;
                    sub.defaultvalue = slot.transmissiontype.ToString();
                    config.addsubobject(0x02, sub);
                }

                eds.ods.Add(slot.ConfigurationIndex,config);

                ODentry mapping = new ODentry();
                mapping.Index = slot.MappingIndex;
                mapping.datatype = DataType.PDO_MAPPING;
                mapping.objecttype = ObjectType.REC;

                if(slot.isTXPDO())
                    mapping.parameter_name = "TPDO mapping parameter";
                else
                    mapping.parameter_name = "RPDO mapping parameter";

                mapping.StorageLocation = slot.mappingloc;
                mapping.accesstype = slot.mappingAccessType;

                sub = new ODentry("Number of mapped objects", (ushort)slot.MappingIndex, 0);
                sub.datatype = DataType.UNSIGNED8;
                sub.defaultvalue = slot.Mapping.Count().ToString();
                mapping.addsubobject(0x00, sub);

                byte mappingcount = 1;
                foreach (ODentry mapslot in slot.Mapping)
                {
                    sub = new ODentry(String.Format("Mapped object {0:x}",mappingcount), (ushort)slot.MappingIndex, mappingcount);
                    sub.datatype = DataType.UNSIGNED32;
                    sub.defaultvalue = string.Format("0x{0:x4}{1:x2}{2:x2}", mapslot.Index, mapslot.Subindex, mapslot.Sizeofdatatype());
                    mapping.addsubobject(mappingcount, sub);

                    mappingcount++;

                }
                eds.ods.Add(slot.MappingIndex,mapping);
               
            }
        }

        /// <summary>
        /// Add a PDO slot as set by index
        /// </summary>
        /// <param name="configindex"></param>
        public void addPDOslot(UInt16 configindex)
        {
            //quick range check, it must be a config index for an RXPDO or a TXPDO
            if( (configindex<0x1400) || (configindex >= 0x1a00)  || ((configindex>=1600) && (configindex<0x1800)))
                return;

            foreach(PDOSlot slot in pdoslots)
            {
                if (slot.ConfigurationIndex == configindex)
                    return;
            }

            bool isTXPDO = configindex >= 0x1800;

            PDOSlot newslot = new PDOSlot();
            newslot.ConfigurationIndex = configindex;

            newslot.COB = 0x180; //Fixme need a better default???

            pdoslots.Add(newslot);

        }

        /// <summary>
        /// This finds a gap in the PDO slots
        /// </summary>
        public UInt16 findPDOslotgap(bool isTXPDO)
        {
            //firstly find the first gap and place it there

            UInt16 startindex = 0x1400;

            if (isTXPDO)
                startindex = 0x1800;

            for(UInt16 index = startindex; index<(startindex+0x200);index++)
            {
                bool found = false;
                foreach(PDOSlot slot in pdoslots)
                {
                    if (slot.ConfigurationIndex == index)
                    {
                        found = true;
                        break;
                    }
                }

                if(found==false)
                {
                    return index;
                }
            }

            //no gaps
            return 0x0000;
        }
    }
}

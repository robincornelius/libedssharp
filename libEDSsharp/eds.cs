/*
    This file is part of libEDSsharp.

    libEDSsharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    libEDSsharp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with libEDSsharp.  If not, see <http://www.gnu.org/licenses/>.
 
    Copyright(c) 2016 Robin Cornelius <robin.cornelius@gmail.com>
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;

 
namespace libEDSsharp
{

   
    public enum DataType
    {
        UNKNOWN = 0,
        BOOLEAN = 1,
        INTEGER8 = 2,
        INTEGER16 = 3,
        INTEGER32 = 4,
        UNSIGNED8 = 5,
        UNSIGNED16 = 6,
        UNSIGNED32 = 7,
        REAL32 = 8,
        VISIBLE_STRING = 9,
        OCTET_STRING = 0x0A,
        UNICODE_STRING = 0x0B,
        TIME_OF_DAY = 0x0C,
        TIME_DIFFERENCE = 0x0D,
        DOMAIN = 0x0F,
        INTEGER24 = 0x10,
        REAL64 = 0x11,
        INTEGER40 = 0x12,
        INTEGER48 = 0x13,
        INTEGER56 = 0x14,
        INTEGER64 = 0x15,
        UNSIGNED24 = 0x16,
        UNSIGNED40 = 0x18,
        UNSIGNED48 = 0x19,
        UNSIGNED56 = 0x1A,
        UNSIGNED64 = 0x1B,

        PDO_COMMUNICATION_PARAMETER = 0x20,  //PDO_CommPar
        PDO_MAPPING  = 0x21, //PDO_Mapping
        SDO_PARAMETER = 0x22,
        IDENTITY = 0x23,

    }

    public enum ObjectType
    {
        UNKNOWN = -1,
        NULL = 0,
        DOMAIN =2,
        DEFTYPE=5,
        DEFSTRUCT=6,
        VAR = 7,
        ARRAY = 8,
        REC = 9,
    }

    //Additional Info for CANOpenNode c and h generation
    public enum StorageLocation
    {
        ROM=1,
        RAM=2,
        EEPROM=3,
    }

    public enum PDOMappingType
    {
        no=0,
        optional=1,
        RPDO=2,
        TPDO=3,
    }

  
    public class EdsExport : Attribute
    {
    }

    public class InfoSection
    {
        protected Dictionary<string, string> section;

        protected string infoheader;
        protected string edssection;

        public virtual void parse(Dictionary<string, string> section)
        {

            this.section = section;

            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo f in fields)
            {
                if(Attribute.IsDefined(f, typeof(EdsExport)))
                    getField(f.Name, f.Name);
            }

        }

        public bool getField(string name, string varname)
        {
            FieldInfo f = null;

            try 
            {
                if (section.ContainsKey(name))
                {

                    Type tx = this.GetType();

                    f = tx.GetField(varname);
                    object var = null;

                    switch (f.FieldType.Name)
                    {
                        case "String":
                            var = section[name];
                            break;

                        case "UInt32":
                            var = Convert.ToUInt32(section[name], EDSsharp.getbase(section[name]));
                            break;

                        case "Int16":
                            var = Convert.ToInt16(section[name], EDSsharp.getbase(section[name]));
                            break;

                        case "UInt16":
                            var = Convert.ToUInt16(section[name], EDSsharp.getbase(section[name]));
                            break;

                        case "Byte":
                            var = Convert.ToByte(section[name], EDSsharp.getbase(section[name]));
                            break;

                        case "Boolean":
                            var = section[name] == "1"; //beacuse Convert is Awesome
                            break;

                        default:
                            Console.WriteLine(String.Format("Unhanded variable {0} for {1}", f.FieldType.Name, varname));
                            break;
                    }

                    if (var != null)
                    {
                        tx.GetField(varname).SetValue(this, var);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is OverflowException)
                {
                    Warnings.warning_list.Add(string.Format("Warning parsing {0} tried to fit {1} into {2}", name, section[name], f.FieldType.Name));
                }
            }

            return false;
        }

        public override string ToString()
        {
            string msg;

            msg = "*****************************************************\n";
            msg += String.Format("*{0,-51}*\n", String.Format("{0," + ((51 + infoheader.Length) / 2).ToString() + "}", infoheader));
            msg += "*****************************************************\n";

            Type tx = this.GetType();
            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo f in fields)
            {
                msg += string.Format("{0,-28}: {1}\n", f.Name, f.GetValue(this).ToString());
            }

            return msg;
        }

        public void write(StreamWriter writer)
        {
            writer.WriteLine("[" + edssection + "]");
            Type tx = this.GetType();
            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo f in fields)
            {
                if (f.Name == "EDSVersionMajor")
                    continue;                 
                if (f.Name == "EDSVersionMinor")
                    continue;
                if (f.Name == "CreationDateTime")
                    continue;
                if (f.Name == "ModificationDateTime")
                    continue;

                if (f.FieldType.Name == "Boolean")
                {
                    writer.WriteLine(string.Format("{0}={1}", f.Name, ((bool)f.GetValue(this)) == true ? 1 : 0));
                }
                else
                {
                    writer.WriteLine(string.Format("{0}={1}", f.Name, f.GetValue(this).ToString()));
                }
            }

            writer.WriteLine("");

        }
       
    }

 
    public class MandatoryObjects : SupportedObjects
    {
        public MandatoryObjects()
            : base()
         {
              infoheader = "Mandatory Objects";
              edssection = "MandatoryObjects";
         }

         public MandatoryObjects(Dictionary<string, string> section)
             : base()
         {
             infoheader = "Mandatory Objects";
             edssection = "MandatoryObjects";
             parse(section);
         }
    }

    public class OptionalObjects : SupportedObjects
    {
        public OptionalObjects()
            : base()
        {
            infoheader = "Optional Objects";
            edssection = "OptionalObjects";
        }

        public OptionalObjects(Dictionary<string, string> section)
            : base()
        {
            infoheader = "Optional Objects";
            edssection = "OptionalObjects";
            parse(section);
        }
    }

    public class ManufacturerObjects : SupportedObjects
    {
        public ManufacturerObjects() : base()
        {
            infoheader = "Manufacturer Objects";
            edssection = "ManufacturerObjects";
        }

        public ManufacturerObjects(Dictionary<string, string> section)
            : base()
        {
            infoheader = "Manufacturer Objects";
            edssection = "ManufacturerObjects";
            parse(section);
        }
    }

    public class SupportedObjects
    {

        public Dictionary<int, int> objectlist;
        public string infoheader;
        public string edssection;

        public SupportedObjects()
        {
            objectlist = new Dictionary<int, int>();
        }

        public virtual void parse(Dictionary<string, string> section)
        {
            objectlist = new Dictionary<int, int>();
            foreach(KeyValuePair<string,string> kvp in section)
            {
                if(kvp.Key=="SupportedObjects")
                    continue;

                int count = Convert.ToInt16(kvp.Key, EDSsharp.getbase(kvp.Key));
                int target = Convert.ToInt16(kvp.Value, EDSsharp.getbase(kvp.Value));
                objectlist.Add(count, target);
            }
        }

        public override string ToString()
        {
            string msg;

            msg = "*****************************************************\n";
            msg += String.Format("*{0,-51}*\n", String.Format("{0," + ((51 + infoheader.Length) / 2).ToString() +  "}", infoheader));
            msg += "*****************************************************\n";
            msg += string.Format("\nSupported Objects = {0}\n", objectlist.Count);
            foreach(KeyValuePair<int,int> kvp in objectlist)
            {
                msg += string.Format("{0,-5}: {1:x4}\n", kvp.Key, kvp.Value);
            }

            return msg;

        }

        public void write(StreamWriter writer)
        {
            writer.WriteLine("[" + edssection + "]");
            writer.WriteLine(string.Format("SupportedObjects={0}", objectlist.Count));
            foreach (KeyValuePair<int, int> kvp in objectlist)
            {
                writer.WriteLine(string.Format("{0}=0x{1:X4}", kvp.Key, kvp.Value));
            }

            writer.WriteLine("");
        }

    }

    public class Comments
    {

        public List<string> comments;
        public string infoheader = "Comments";
        public string edssection = "Comments";

        public Comments()
        {
           
        }

        public Comments(Dictionary<string, string> section)
        {
            parse(section);
        }

        public virtual void parse(Dictionary<string, string> section)
        {
            comments = new List<string>();
            foreach (KeyValuePair<string, string> kvp in section)
            {
                if (kvp.Key == "Lines")
                    continue;

                comments.Add(kvp.Value);

            }
        }

        public override string ToString()
        {
            string msg;

            msg = "*****************************************************\n";
            msg += String.Format("*{0,-51}*\n", String.Format("{0," + ((51 + infoheader.Length) / 2).ToString() + "}", infoheader));
            msg += "*****************************************************\n";
            msg += string.Format("\nLines = {0}\n", comments.Count);
            foreach (string s in comments)
            {
                msg += string.Format("{0}\n",s);
            }

            return msg;

        }

        public void write(StreamWriter writer)
        {
            //Comments block is optional so if there are no comments do not include it
            if(comments==null || comments.Count==0)
                return;

            writer.WriteLine("[" + edssection + "]");

            writer.WriteLine(string.Format("Lines={0}", comments.Count));

            int count = 1;
            foreach (string s in comments)
            {
                writer.WriteLine(string.Format("Line{0}={1}", count, s));
                count++;
            }

            writer.WriteLine("");
        }   
    }


    public class Dummyusage : InfoSection
    {
        [EdsExport]
        public bool Dummy0001;
        [EdsExport]
        public bool Dummy0002;
        [EdsExport]
        public bool Dummy0003;
        [EdsExport]
        public bool Dummy0004;
        [EdsExport]
        public bool Dummy0005;
        [EdsExport]
        public bool Dummy0006;
        [EdsExport]
        public bool Dummy0007;

 
        public Dummyusage()
        {
             infoheader = "CAN OPEN Dummy Usage";
             edssection = "DummyUsage";
        }

        public Dummyusage(Dictionary<string, string> section)
        {
            infoheader = "CAN OPEN Dummy Usage";
            edssection = "DummyUsage";
            parse(section);
        }
    }

    public class FileInfo : InfoSection
    {
        [EdsExport]
        public string FileName="";//=example_objdict.eds
        [EdsExport]
        public byte FileVersion;//=1
        [EdsExport]
        public byte FileRevision;//=1

        [EdsExport]
        public byte EDSVersionMajor;//=4.0
        [EdsExport]
        public byte EDSVersionMinor;//=4.0
        [EdsExport]
        public string EDSVersion="";

        [EdsExport]
        public string Description="";//= //max 243 characters

        public DateTime CreationDateTime;//
        [EdsExport]
        public string CreationTime="";
        [EdsExport]
        public string CreationDate="";

        [EdsExport]
        public string CreatedBy = "";//=CANFestival //max245

        public DateTime ModificationDateTime;//
        [EdsExport]
        public string ModificationTime="";
        [EdsExport]
        public string ModificationDate="";
        [EdsExport]
        public string ModifiedBy="";//=CANFestival //max244

        public string exportFolder = "";

        public FileInfo(Dictionary<string, string> section)
        {
            infoheader = "CAN OPEN FileInfo";
            edssection = "FileInfo";
            parse(section);
        }

        public FileInfo()
        {
            infoheader = "CAN OPEN FileInfo";
            edssection = "FileInfo";
        }


        override public void parse(Dictionary<string, string> section)
        {

            base.parse(section);

            string dtcombined = "";
            try
            {
                if (section.ContainsKey("CreationTime") && section.ContainsKey("CreationDate"))
                {
                    dtcombined = section["CreationTime"] + " " + section["CreationDate"];
                    CreationDateTime = DateTime.ParseExact(dtcombined, "h:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch(Exception e)
            {
                if (e is System.FormatException)
                {
                    Warnings.warning_list.Add(String.Format("Unable to parse DateTime {0} for CreationTime, not in DS306 format", dtcombined));
                }
            }

            try
            {
                if (section.ContainsKey("ModificationTime") && section.ContainsKey("ModificationTime"))
                {
                    dtcombined = section["ModificationTime"] + " " + section["ModificationDate"];
                    ModificationDateTime = DateTime.ParseExact(dtcombined, "h:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                if (e is System.FormatException)
                {
                    Warnings.warning_list.Add(String.Format("Unable to parse DateTime {0} for ModificationTime, not in DS306 format", dtcombined));
                }
            }
            

            if (section.ContainsKey("EDSVersion"))
            {
                string[] bits = section["EDSVersion"].Split('.');
                if(bits.Length >=1 )
                    EDSVersionMajor = Convert.ToByte(bits[0]);
                if (bits.Length >= 2)
                    EDSVersionMinor = Convert.ToByte(bits[1]);
                //EDSVersion = String.Format("{0}.{1}", EDSVersionMajor, EDSVersionMinor);
            }


        }
    }

    public class DeviceInfo : InfoSection
    {

        [EdsExport]
        public string VendorName="";
        [EdsExport]
        public UInt32 VendorNumber;

        [EdsExport]
        public string ProductName="";
        [EdsExport]
        public UInt32 ProductNumber;
        [EdsExport]
        public UInt32 RevisionNumber;

        [EdsExport]
        public bool BaudRate_10;
        [EdsExport]
        public bool BaudRate_20;
        [EdsExport]
        public bool BaudRate_50;
        [EdsExport]
        public bool BaudRate_125;
        [EdsExport]
        public bool BaudRate_250;
        [EdsExport]
        public bool BaudRate_500;
        [EdsExport]
        public bool BaudRate_800;
        [EdsExport]
        public bool BaudRate_1000;

        [EdsExport]
        public bool SimpleBootUpMaster;
        [EdsExport]
        public bool SimpleBootUpSlave;

        [EdsExport]
        public byte Granularity;
        [EdsExport]
        public bool DynamicChannelsSupported;

        [EdsExport]
        public bool CompactPDO;

        [EdsExport]
        public bool GroupMessaging;

        [EdsExport]
        public UInt16 NrOfRXPDO;

        [EdsExport]
        public UInt16 NrOfTXPDO;

        [EdsExport]
        public bool LSS_Supported;

        public DeviceInfo(Dictionary<string, string> section)
        {
            infoheader = "CAN OPEN DeviceInfo";
            edssection = "DeviceInfo";
            parse(section);
        }

        public DeviceInfo()
        {
            infoheader = "CAN OPEN DeviceInfo";
            edssection = "DeviceInfo";
        }

        public int concreteNodeId = -1;

    }


    public class ODentry
    {
        [EdsExport]
        public UInt16 index;
        [EdsExport]
        public UInt16 subindex;
        [EdsExport]
        public int nosubindexes
        {
            get
            {
                return subobjects.Count;
            }
        }

        [EdsExport]
        public string parameter_name;
        [EdsExport]
        public ObjectType objecttype;
        [EdsExport]
        public DataType datatype;
        [EdsExport]
        public EDSsharp.AccessType accesstype;
        [EdsExport]
        public string defaultvalue;
        [EdsExport]
        public bool PDOMapping
        {
            get
            {
                return PDOtype != PDOMappingType.no;
            }
        }

        public PDOMappingType PDOtype;

        //CanOpenNode specific extra storage
        public string Label = "";
        public string Description = "";

        public StorageLocation location = StorageLocation.RAM;
        public SortedDictionary<UInt16, ODentry> subobjects = new SortedDictionary<UInt16, ODentry>();
        public ODentry parent = null;

        public string AccessFunctionName = "";
        public string AccessFunctionPreCode ="";

        public bool Disabled = false;

        public bool TPDODetectCos = false;

        public ODentry()
        {

        }

        //Constructor for a simple VAR type
        public ODentry(string parameter_name,UInt16 index, DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, PDOMappingType PDOMapping)
        {
            this.parameter_name = parameter_name;
            this.index = index;
            this.objecttype = ObjectType.VAR;
            this.datatype = datatype;
            this.defaultvalue = defaultvalue;


            if (accesstype >= EDSsharp.AccessType_Min && accesstype <= EDSsharp.AccessType_Max)
                this.accesstype = accesstype;
            else
                throw new ParameterException("AccessType invalid");

            this.PDOtype = PDOMapping;

        }

        //SubIndex type
        public ODentry(string parameter_name, UInt16 index, byte subindex, DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, PDOMappingType PDOMapping)
        {
            this.parameter_name = parameter_name;
            this.index = index;
            this.subindex = subindex;
            this.objecttype = ObjectType.VAR;
            this.datatype = datatype;
            this.defaultvalue = defaultvalue;

            if (accesstype >= EDSsharp.AccessType_Min && accesstype <= EDSsharp.AccessType_Max)
                this.accesstype = accesstype;
            else
                throw new ParameterException("AccessType invalid");

            this.PDOtype = PDOMapping;
        }

        //Array subindex type
        public ODentry(string parameter_name,UInt16 index, byte nosubindex)
        {
            this.parameter_name = parameter_name;
            this.objecttype = ObjectType.ARRAY;
            this.index = index;
            //this.nosubindexes = nosubindex;
            this.objecttype = ObjectType.VAR;     
        }


        public override string ToString()
        {
            if (nosubindexes > 0)
            {
                return String.Format("{0:x4}[{1}] : {2} : {3}", index, nosubindexes, parameter_name, datatype);
 
            }
            else
            {
                return String.Format("{0:x4}/{1} : {2} : {3}", index, subindex, parameter_name, datatype);
            }
        }

        public void write(StreamWriter writer)
        {

            if (parent!=null)
            {
                writer.WriteLine(string.Format("[{0:X}sub{1}]", index,subindex));
            }
            else
            {
                writer.WriteLine(string.Format("[{0:X}]", index));
            }

            writer.WriteLine(string.Format("ParameterName={0}", parameter_name));
            writer.WriteLine(string.Format("ObjectType=0x{0:X}", (int)objecttype));
            writer.WriteLine(string.Format(";StorageLocation={0}",location.ToString()));

            if (objecttype == ObjectType.ARRAY)
            {
                writer.WriteLine(string.Format("SubNumber={0:X}", nosubindexes));
            }
            if (objecttype == ObjectType.REC)
            {
                writer.WriteLine(string.Format("SubNumber={0:X}", nosubindexes));
            }


            if (objecttype == ObjectType.VAR)
            {
                writer.WriteLine(string.Format("DataType=0x{0:X4}", (int)datatype));
                writer.WriteLine(string.Format("AccessType={0}", accesstype.ToString()));

                writer.WriteLine(string.Format("DefaultValue={0}", defaultvalue));
                writer.WriteLine(string.Format("PDOMapping={0}", PDOMapping==true?1:0));
            }

            writer.WriteLine("");
        }

        public string paramater_cname()
        {
            string cname = parameter_name.Replace("-", "_");

            cname =  Regex.Replace(cname, @"([A-Z]) ([A-Z])", "$1_$2");
            cname = cname.Replace(" ", "");

            return cname;
        }

        public int sizeofdatatype()
        {
            DataType dt = datatype;

            if (dt == DataType.UNKNOWN && this.parent != null)
                dt = parent.datatype;
 
                

            switch (dt)
            {
                case DataType.BOOLEAN:
                case DataType.UNSIGNED8:
                case DataType.INTEGER8:
                    return 1;

                case DataType.INTEGER16:
                case DataType.UNSIGNED16:
                    return 2;

                case DataType.UNSIGNED24:
                case DataType.INTEGER24:
                    return 3;

                case DataType.INTEGER32:
                case DataType.UNSIGNED32:
                case DataType.REAL32:
                    return 4;

                case DataType.INTEGER40:
                case DataType.UNSIGNED40:
                    return 5;

                case DataType.INTEGER48:
                case DataType.UNSIGNED48:
                case DataType.TIME_DIFFERENCE:
                case DataType.TIME_OF_DAY:
                    return 6;

                case DataType.INTEGER56:
                case DataType.UNSIGNED56:
                    return 7;

                case DataType.INTEGER64:
                case DataType.UNSIGNED64:
                case DataType.REAL64:
                    return 8;


                case DataType.VISIBLE_STRING:
                    {
                        if (defaultvalue == null)
                            return 0;
                        return defaultvalue.Length;
                    }

                case DataType.OCTET_STRING:
                    {
                        if (defaultvalue == null)
                            return 0;
                        return Regex.Replace(defaultvalue, @"\s", "").Length / 2;
                    }

                case DataType.UNICODE_STRING:
                    {
                        if (defaultvalue == null)
                            return 0;
                        return Regex.Replace(defaultvalue, @"\s", "").Length / 4;
                    }

                case DataType.DOMAIN:
                    return 0;

                default: //FIXME
                    return 0;

            }
        }
    }

    public class EDSsharp
    {

        public enum AccessType
        {
            rw = 0,
            ro = 1,
            wo = 2,
            rwr = 3,
            rww = 4,
            cons = 5,
            UNKNOWN
        }

        public const AccessType AccessType_Min = AccessType.rw;
        public const AccessType AccessType_Max = AccessType.cons;

        Dictionary<string, Dictionary<string, string>> eds;
        public SortedDictionary<UInt16, ODentry> ods;
        public FileInfo fi;
        public DeviceInfo di;
        public MandatoryObjects md;
        public OptionalObjects oo;
        public ManufacturerObjects mo;
        public Comments c;
        public Dummyusage du;

        public UInt16 NodeId = 0;



        public EDSsharp()
        {

            eds = new Dictionary<string, Dictionary<string, string>>();
            ods = new SortedDictionary<UInt16, ODentry>();

            fi = new FileInfo();
            di = new DeviceInfo();
            du = new Dummyusage();
            md = new MandatoryObjects();
            oo = new OptionalObjects();
            mo = new ManufacturerObjects();
            c = new Comments();


            //FIXME no way for the Major/Minor to make it to EDSVersion
            fi.EDSVersionMajor = 4;
            fi.EDSVersionMinor = 0;

            fi.FileVersion = 1;
            fi.FileRevision = 1;

            //FixMe too need a extra function to sort the data out;
            fi.CreationDateTime = DateTime.Now;
            fi.ModificationDateTime = DateTime.Now;

            du.Dummy0001 = false;
            du.Dummy0002 = true;
            du.Dummy0003 = true;
            du.Dummy0004 = true;
            du.Dummy0005 = true;
            du.Dummy0006 = true;
            du.Dummy0007 = true;

            ODentry od = new ODentry();






        }

        string sectionname = "";

        public void parseline(string linex)
        {
            string key = "";
            string value = "";

            //Special Handling of custom fields
            if (linex.IndexOf(';') == 0 && linex.IndexOf(";StorageLocation") != 0)
                return;

            string line = linex.TrimStart(';');

            //extract sections
            {
                string pat = @"\[([a-z0-9]+)\]";

                Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                Match m = r.Match(line);
                if (m.Success)
                {
                    Group g = m.Groups[1];
                    sectionname = g.ToString();
                }
            }

            //extract keyvalues
            {
                string pat = @"([a-z0-9_]+)=(.*)";

                Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                Match m = r.Match(line);
                if (m.Success)
                {

                    key = m.Groups[1].ToString();
                    value = m.Groups[2].ToString();

                    if (!eds.ContainsKey(sectionname))
                    {
                        eds.Add(sectionname, new Dictionary<string, string>());
                    }

                    eds[sectionname].Add(key, value);

                }
            }
        }

        public void parseEDSentry(KeyValuePair<string, Dictionary<string, string>> kvp)
        {
            string section = kvp.Key;

            string pat = @"^([a-fA-F0-9]+)(sub)?([0-9a-fA-F]*)$";

            Regex r = new Regex(pat);
            Match m = r.Match(section);
            if (m.Success)
            {

                ODentry od = new ODentry();

                if (!kvp.Value.ContainsKey("ParameterName"))
                    throw new ParameterException("Missing required field ParameterName on" + section);
                od.parameter_name = kvp.Value["ParameterName"];

                if (kvp.Value.ContainsKey("ObjectType"))
                {
                    int type = Convert.ToInt16(kvp.Value["ObjectType"], getbase(kvp.Value["ObjectType"]));
                    od.objecttype = (ObjectType)type;
                }
                else
                {
                    od.objecttype = ObjectType.VAR;
                }

                //Indexes in the EDS are always in hex format without the pre 0x
                od.index = Convert.ToUInt16(m.Groups[1].ToString(), 16);

                if (od.objecttype == ObjectType.VAR)
                {

                    if (m.Groups[3].Length != 0)
                    {
                        //FIXME are subindexes in hex always?
                        od.subindex = Convert.ToUInt16(m.Groups[3].ToString(),16);
                        od.parent = ods[od.index];
                        ods[od.index].subobjects.Add(od.subindex, od);
                    }


                    if (!kvp.Value.ContainsKey("DataType"))
                        throw new ParameterException("Missing required field DataType on" + section);

                    od.datatype = (DataType)Convert.ToInt16(kvp.Value["DataType"], getbase(kvp.Value["DataType"]));

                    if (!kvp.Value.ContainsKey("AccessType"))
                        throw new ParameterException("Missing required AccessType on" + section);

                    string accesstype = kvp.Value["AccessType"];

                    // fudging because of enum enumeration and the const keyword
                    accesstype = accesstype.Replace("const", "cons");
                    if (Enum.IsDefined(typeof(AccessType), accesstype))
                    {
                        od.accesstype = (AccessType)Enum.Parse(typeof(AccessType), accesstype);
                    }
                    else
                    {
                        throw new ParameterException("Unknown AccessType on" + section);
                    }

                    if (kvp.Value.ContainsKey("DefaultValue"))
                        od.defaultvalue = kvp.Value["DefaultValue"];

                    od.PDOtype = PDOMappingType.no;
                    if (kvp.Value.ContainsKey("PDOMapping"))
                    {
                        bool pdo = Convert.ToInt16(kvp.Value["PDOMapping"]) == 1;
                        if (pdo == true)
                            od.PDOtype = PDOMappingType.optional;
                    }
                       

                }

                //Only add top level to this list
                if (m.Groups[3].Length == 0)
                {
                    ods.Add(od.index, od);
                }
            }

        }

        public void loadfile(string filename)
        {
            //try
            {
                foreach (string linex in File.ReadLines(filename))
                {
                    parseline(linex);
                }

                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in eds)
                {
                    parseEDSentry(kvp);
                }

                fi = new FileInfo(eds["FileInfo"]);
                di = new DeviceInfo(eds["DeviceInfo"]);
                du = new Dummyusage(eds["DummyUsage"]);
                md = new MandatoryObjects(eds["MandatoryObjects"]);
                oo = new OptionalObjects(eds["OptionalObjects"]);
                mo = new ManufacturerObjects(eds["ManufacturerObjects"]);
                c = new Comments(eds["Comments"]);

                updatePDOcount();
            }
            // catch(Exception e)
            //{
            //  Console.WriteLine("** ALL GONE WRONG **" + e.ToString());
            // }
        }

        public void savefile(string filename)
        {
            updatePDOcount();

            StreamWriter writer = File.CreateText(filename);
            fi.write(writer);
            di.write(writer);
            du.write(writer);
            c.write(writer);
            md.write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (md.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry od2 = kvp2.Value;
                        od2.write(writer);
                    }                    
                }
            }

            oo.write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (oo.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry od2 = kvp2.Value;
                        od2.write(writer);
                    }                    
                }
            }

            mo.write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (mo.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry od2 = kvp2.Value;
                        od2.write(writer);
                    }                    
                }
            }


            writer.Close();

        }

        public DataType getdatatype(ODentry od)
        {

            if (od.objecttype == ObjectType.VAR)
            {
                return od.datatype;
            }

            if (od.objecttype == ObjectType.ARRAY)
            {
                ODentry sub2 = ods[od.index];

                //FIX ME !!! INCONSISTANT setup of the datatype for arrays when loading xml and eds!!

                DataType t = sub2.subobjects[1].datatype;
                if (t == DataType.UNKNOWN)
                    t = sub2.datatype;

                return t;
            }

            //Warning, REC types need to be handled else where as the specific
            //implementation of a REC type depends on the exporter being used

            return DataType.UNKNOWN;

        }


        static public int getbase(string defaultvalue)
        {

            if (defaultvalue == null)
                return 10;

            int nobase = 10;

            String pat = @"^0[xX][0-9]+";

            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            Match m = r.Match(defaultvalue);
            if (m.Success)
            {
                nobase = 16;
            }

            pat = @"^0[0-9]+";
            r = new Regex(pat, RegexOptions.IgnoreCase);
            m = r.Match(defaultvalue);
            if (m.Success)
            {
                nobase = 8;
            }

            return nobase;
        }

        public void updatePDOcount()
        {
            di.NrOfRXPDO = 0;
            di.NrOfTXPDO = 0;
            foreach(KeyValuePair<UInt16,ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if(od.Disabled==false && ((od.index&0xFF00) == 0x1400 ))
                    di.NrOfRXPDO++;

                if(od.Disabled==false && ((od.index&0xFF00) == 0x1800 ))
                    di.NrOfTXPDO++;

            }

        }

        //Split on + , replace $NODEID with concrete value and add together
        public UInt16 GetNodeID(string input)
        {
            try
            {
                if (di.concreteNodeId == -1)
                {
                    input = input.Replace("$NODEID", "");
                    input = input.Replace("+", "");
                    input = input.Replace(" ", "");
                    return Convert.ToUInt16(input, getbase(input));
                }

                input = input.Replace("$NODEID", String.Format("0x{0}", di.concreteNodeId));

                string[] bits = input.Split('+');

                if (bits.Length != 2)
                {
                    throw new FormatException("cannot parse " + input + "\nExpecting N+$NODEID or $NODEID+N");
                }

                UInt16 b1 = Convert.ToUInt16(bits[0], getbase(bits[0]));
                UInt16 b2 = Convert.ToUInt16(bits[1], getbase(bits[1]));

                return (UInt16)(b1 + b2);
            }
            catch(Exception e)
            {
                Warnings.warning_list.Add(String.Format("Error parsing node id {0} nodes, {1}", input,e.ToString()));
            }

            return 0;
        }
     

    }

        public class ParameterException : Exception
        {
            public ParameterException(String message)
                : base(message)
            {
        
            }
        }

        public static class Warnings
        {
            public static List<string> warning_list = new List<string>();

        }

 }

/*
    This file is part of libEDSsharp.

    libEDSsharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
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
            if (section.ContainsKey(name))
            {

                Type tx = this.GetType();

                FieldInfo f  = tx.GetField(varname);
                object var = null;

                switch(f.FieldType.Name)
                { 
                    case "String":
                        var = section[name];
                        break;

                    case "UInt32":
                        var = Convert.ToUInt32(section[name],16);
                        break;

                    case "Int16":
                        var = Convert.ToInt16(section[name]);
                        break;

                    case "UInt16":
                        var = Convert.ToUInt16(section[name]);
                        break;

                    case "Byte":
                        var = Convert.ToByte(section[name]);
                        break;

                    case "Boolean":
                        var = section[name] == "1"; //beacuse Convert is Awesome
                        break;

                    default:
                        Console.WriteLine(String.Format("Unhanded variable {0} for {1}",f.FieldType.Name,varname));
                        break;
                }

                if (var != null)
                {
                    tx.GetField(varname).SetValue(this, var);
                    return true;
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

                int count = Convert.ToInt16(kvp.Key);
                int target = Convert.ToInt16(kvp.Value, 16);
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
        public string FileName;//=example_objdict.eds
        [EdsExport]
        public byte FileVersion;//=1
        [EdsExport]
        public byte FileRevision;//=1

        [EdsExport]
        public byte EDSVersionMajor;//=4.0
        [EdsExport]
        public byte EDSVersionMinor;//=4.0
        [EdsExport]
        public string EDSVersion;

        [EdsExport]
        public string Description;//= //max 243 characters

        public DateTime CreationDateTime;//
        [EdsExport]
        public string CreationTime;
        [EdsExport]
        public string CreationDate;

        [EdsExport]
        public string CreatedBy;//=CANFestival //max245

        public DateTime ModificationDateTime;//
        [EdsExport]
        public string ModificationTime;
        [EdsExport]
        public string ModificationDate;
        [EdsExport]
        public string ModifiedBy;//=CANFestival //max244

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


            if (section.ContainsKey("CreationTime") && section.ContainsKey("CreationDate"))
            {
                string dtcombined = section["CreationTime"] + " " + section["CreationDate"];

                CreationDateTime = DateTime.ParseExact(dtcombined, "h:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
            }

            if (section.ContainsKey("ModificationTime") && section.ContainsKey("ModificationTime"))
            {
                string dtcombined = section["ModificationTime"] + " " + section["ModificationDate"];
                ModificationDateTime = DateTime.ParseExact(dtcombined, "h:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
            }

            if (section.ContainsKey("EDSVersion"))
            {
                string[] bits = section["EDSVersion"].Split('.');
                EDSVersionMajor = Convert.ToByte(bits[0]);
                EDSVersionMinor = Convert.ToByte(bits[1]);
                //EDSVersion = String.Format("{0}.{1}", EDSVersionMajor, EDSVersionMinor);
            }


        }
    }

    public class DeviceInfo : InfoSection
    {

        [EdsExport]
        public string VendorName;
        [EdsExport]
        public UInt32 VendorNumber;

        [EdsExport]
        public string ProductName;
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
        public bool PDOMapping;

        //CanOpenNode specific extra storage
        public string Label = "";
        public string Description = "";

        public StorageLocation location = StorageLocation.RAM;
        public Dictionary<UInt16, ODentry> subobjects = new Dictionary<UInt16, ODentry>();
        public ODentry parent = null;

        public string AccessFunctionName = "";
        public string AccessFunctionPreCode ="";

        public bool Disabled = false;

        public bool TPDODetectCos = false;

        public ODentry()
        {

        }

        //Constructor for a simple VAR type
        public ODentry(string parameter_name,UInt16 index, DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, bool PDOMapping)
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

            this.PDOMapping = PDOMapping;
        }

        //SubIndex type
        public ODentry(string parameter_name, UInt16 index, byte subindex, DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, bool PDOMapping)
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

            this.PDOMapping = PDOMapping;
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
            string cname = parameter_name.Replace(" ","");
            return cname;
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
        public Dictionary<UInt16, ODentry> ods;
        public FileInfo fi;
        public DeviceInfo di;
        public MandatoryObjects md;
        public OptionalObjects oo;
        public ManufacturerObjects mo;
        public Comments c;
        public Dummyusage du;

        public UInt16 NodeId=0;

        public Dictionary <int,defstruct> defstructs = new  Dictionary<int,defstruct>();

        public EDSsharp()
        {
            init_defstructs();

            eds = new Dictionary<string, Dictionary<string, string>>();
            ods = new Dictionary<UInt16, ODentry>();

            fi = new FileInfo();
            di = new DeviceInfo();
            du = new Dummyusage();
            md = new MandatoryObjects();
            oo = new OptionalObjects();
            mo = new ManufacturerObjects();
            c = new Comments();


            //FIXME no way for the Major/Minor to make it to EDSVersion
            fi.EDSVersionMajor=4;
            fi.EDSVersionMinor=0;

            fi.FileVersion=1;
            fi.FileRevision=1;

            //FixMe too need a extra function to sort the data out;
            fi.CreationDateTime=DateTime.Now;
            fi.ModificationDateTime=DateTime.Now;

            du.Dummy0001=false;
            du.Dummy0002=true;
            du.Dummy0003=true;
            du.Dummy0004=true;
            du.Dummy0005=true;
            du.Dummy0006=true;
            du.Dummy0007=true;

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

        public int determinebase(string input)
        {
            if (input.Length > 2)
                if (input[0] == '0' && (input[1] == 'x' || input[1] == 'X'))
                    return 16;

            if (input.Length > 1)
                if (input[0] == '0')
                    return 8;

            return 10;

        }

        public void parseEDSentry(KeyValuePair<string, Dictionary<string, string>> kvp)
        {
            string section = kvp.Key;

            string pat = @"^([a-fA-F0-9]+)(sub)?([0-9]*)$";

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
                    int type = Convert.ToInt16(kvp.Value["ObjectType"], determinebase(kvp.Value["ObjectType"]));
                    od.objecttype = (ObjectType)type;
                }
                else
                {
                    od.objecttype = ObjectType.VAR;
                }

                od.index = Convert.ToUInt16(m.Groups[1].ToString(), 16);

                //if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
                //{
                    //od.nosubindexes = Convert.ToInt16(kvp.Value["SubNumber"], determinebase(kvp.Value["SubNumber"]));
                //}

                if (od.objecttype == ObjectType.VAR)
                {

                    if (m.Groups[3].Length != 0)
                    {
                        Console.WriteLine(m.Groups[3].ToString());
                        od.subindex = Convert.ToUInt16(m.Groups[3].ToString());
                        od.parent = ods[od.index];
                        ods[od.index].subobjects.Add(od.subindex,od);
                    }


                    if(!kvp.Value.ContainsKey("DataType"))
                        throw new ParameterException("Missing required field DataType on" + section);

                    od.datatype = (DataType)Convert.ToInt16(kvp.Value["DataType"], determinebase(kvp.Value["DataType"]));
                    
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
                    
                    if (kvp.Value.ContainsKey("PDOMapping"))
                        od.PDOMapping = Convert.ToInt16(kvp.Value["PDOMapping"]) == 1;

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

                foreach(KeyValuePair<string, Dictionary<string, string>> kvp in eds)
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

            }
           // catch(Exception e)
            //{
              //  Console.WriteLine("** ALL GONE WRONG **" + e.ToString());
           // }
        }

        public void savefile(string filename)
        {
            StreamWriter writer = File.CreateText(filename);
            fi.write(writer);
            di.write(writer);
            du.write(writer);
            c.write(writer);
            md.write(writer);

            foreach(KeyValuePair<UInt16,ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (md.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                }
            }

            oo.write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (oo.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                }
            }

            mo.write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (mo.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                }
            }


            writer.Close();

        }

        public DataType getdatatype(ODentry od)
        {
            if (od.objecttype == ObjectType.VAR)
            {
                if (od.parent==null)
                    return od.datatype;
            }

            if (od.objecttype == ObjectType.ARRAY)
            {
                ODentry sub2 = ods[od.index];
                DataType t = sub2.datatype;
                return t;
            }

            if (od.objecttype == ObjectType.REC) //NOT SURE????
            {
                ODentry sub2 = ods[od.index];
                DataType t = sub2.datatype;
                return t;
            }

            return DataType.UNKNOWN;

        }

        void init_defstructs
        {
            
            {
                //0x1018 Identity Record Specification, DataType 0x23
                defstruct ds = new defstruct("Identity Record Specification",
                "OD_identity_t");

                ds.elements.Add(0,new subdefstruct("number of supported entries in the record","maxSubIndex",DataType.UNSIGNED8));
                ds.elements.Add(1,new subdefstruct("Vendor-ID","vendorID",DataType.UNSIGNED32));
                ds.elements.Add(2,new subdefstruct("Product code","productCode",DataType.UNSIGNED32));
                ds.elements.Add(3,new subdefstruct("Revision number","revisionNumber",DataType.UNSIGNED32));
                ds.elements.Add(4,new subdefstruct("Serial number","serialNumber",DataType.UNSIGNED32));
                
                defstructs.Add(0x1018,ds);
            }
 
            {
                //0x1200 Identity Record Specification, DataType 0x22
                defstruct ds = new defstruct("SDO Parameter Record Specification",
                "OD_identity_t");

                ds.elements.Add(0,new subdefstruct("number of supported entries in the record","maxSubIndex",DataType.UNSIGNED8));
                ds.elements.Add(1,new subdefstruct("COB-ID client -> server","COB_IDClientToServer",DataType.UNSIGNED32));
                ds.elements.Add(2,new subdefstruct("COB-ID server -> client","COB_IDServerToClient",DataType.UNSIGNED32));
                ds.elements.Add(3,new subdefstruct("node ID of SDO’s client resp. server","OD_SDOServerParameter_t",DataType.UNSIGNED32));
                
                defstructs.Add(0x1200,ds);
            }


            {
                //0x1800 PDO Communication Parameter Record
                defstruct ds = new defstruct("PDO Communication Parameter Record",
                "OD_TPDOCommunicationParameter_t");

                ds.elements.Add(0,new subdefstruct("number of supported entries in the record","maxSubIndex",DataType.UNSIGNED8));
                ds.elements.Add(1,new subdefstruct("COB-ID","COB_IDUsedByTPDO",DataType.UNSIGNED32));
                ds.elements.Add(2,new subdefstruct("transmission type","transmissionType",DataType.UNSIGNED8));
                ds.elements.Add(3,new subdefstruct("inhibit time","inhibitTime",DataType.UNSIGNED16));
                ds.elements.Add(4,new subdefstruct("reserved","compatibilityEntry",DataType.UNSIGNED8));
                ds.elements.Add(5,new subdefstruct("event timer","eventTimer",DataType.UNSIGNED16));
                ds.elements.Add(6,new subdefstruct("SYNCStartValue","SYNCStartValue",DataType.UNSIGNED8));

                defstructs.Add(0x1800,ds);
            }

            {
                //0x1A00 PDO TX Mapping Paramater DataType 0x21
                defstruct ds = new defstruct("PDO TX Mapping Parameter Record",
                "OD_TPDOMappingParameter_t");

                ds.elements.Add(0,new subdefstruct("number of mapped objects in PDO","numberOfMappedObjects",DataType.UNSIGNED8);

                ds.elements.Add(1,new subdefstruct("1st object to be mapped","mappedObject1",DataType.UNSIGNED32));
                ds.elements.Add(2,new subdefstruct("2nd object to be mapped","mappedObject2",DataType.UNSIGNED32));
                ds.elements.Add(3,new subdefstruct("3rd object to be mapped","mappedObject3",DataType.UNSIGNED32));
                ds.elements.Add(4,new subdefstruct("4th object to be mapped","mappedObject4",DataType.UNSIGNED32));
                ds.elements.Add(5,new subdefstruct("5th object to be mapped","mappedObject5",DataType.UNSIGNED32));
                ds.elements.Add(6,new subdefstruct("6th object to be mapped","mappedObject6",DataType.UNSIGNED32));
                ds.elements.Add(7,new subdefstruct("7th object to be mapped","mappedObject7",DataType.UNSIGNED32));
                ds.elements.Add(8,new subdefstruct("8th object to be mapped","mappedObject8",DataType.UNSIGNED32));
    
                defstructs.Add(0x1a00,ds);
            }

             {
                //0x1400 PDO Communication Parameter Record
                defstruct ds = new defstruct("PDO RX Communication Parameter Record",
                "OD_RPDOCommunicationParameter_t");

                ds.elements.Add(0,new subdefstruct("number of supported entries in the record","maxSubIndex",DataType.UNSIGNED8));
                ds.elements.Add(1,new subdefstruct("COB-ID","COB_IDUsedByTPDO",DataType.UNSIGNED32));
                ds.elements.Add(2,new subdefstruct("transmission type","transmissionType",DataType.UNSIGNED8));
          
                defstructs.Add(0x1400,ds);
            }

            {
                //0x1600 PDO TX Mapping Paramater DataType 0x21
                defstruct ds = new defstruct("PDO RX Mapping Parameter Record",
                "OD_RPDOMappingParameter_t");

                ds.elements.Add(0,new subdefstruct("number of mapped objects in PDO","numberOfMappedObjects",DataType.UNSIGNED8);

                ds.elements.Add(1,new subdefstruct("1st object to be mapped","mappedObject1",DataType.UNSIGNED32));
                ds.elements.Add(2,new subdefstruct("2nd object to be mapped","mappedObject2",DataType.UNSIGNED32));
                ds.elements.Add(3,new subdefstruct("3rd object to be mapped","mappedObject3",DataType.UNSIGNED32));
                ds.elements.Add(4,new subdefstruct("4th object to be mapped","mappedObject4",DataType.UNSIGNED32));
                ds.elements.Add(5,new subdefstruct("5th object to be mapped","mappedObject5",DataType.UNSIGNED32));
                ds.elements.Add(6,new subdefstruct("6th object to be mapped","mappedObject6",DataType.UNSIGNED32));
                ds.elements.Add(7,new subdefstruct("7th object to be mapped","mappedObject7",DataType.UNSIGNED32));
                ds.elements.Add(8,new subdefstruct("8th object to be mapped","mappedObject8",DataType.UNSIGNED32));
    
                defstructs.Add(0x1600,ds);
            }

        }

    }

    public class defstruct
    {
        public string name;
        public string c_declaration;
    
        public Dictinary<int,subdefstruct> elements;

        public defstruct(string name,string c_dec)
        {
            this.name = name;
            this.c_declaration = c_dec;
            elements = new Dictinary<int,subdefstruct>();
        }

    }
    public class subdefstruct
    {
    public string description;
    public string c_declaration;

    public DataType datatype;

    public subdefstruct(string description,string c_dec,DataType datatype)
    {
        this.description = description;
        this.c_declaration = c_dec;
        this.datatype = datatype;
    }

}

public class ParameterException : Exception
{
    public ParameterException(String message)
        : base(message)
    {
        
    }
}

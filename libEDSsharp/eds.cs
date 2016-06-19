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
        BOOLEAN = 1,
        INTEGER8 = 2,
        INTEGER16 = 3,
        INTEGER32 = 4,
        UNSIGNED8 = 5,
        UNSIGNED16 = 6,
        UNSIGNED32 = 7,
        REAL32 = 8,
        VISIBLE_STRING = 9,
        OCTET_STRING = 0xA,
        PDO_CommPar = 20,
        PDO_Mapping = 21,

    }

    public enum ObjectType
    {
        VAR = 7,
        REC = 8,
        ARRAY = 9,
    }


    //Additional Info for CANOpenNode c and h generation
    public enum StorageLocation
    {
        RAM=0,
        EEPROM=1,
        ROM=2,
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
         {
              infoheader = "Mandatory Objects";
              edssection = "MandatoryObjects";
         }

         public MandatoryObjects(Dictionary<string, string> section)
         {
             infoheader = "Mandatory Objects";
             edssection = "MandatoryObjects";
             parse(section);
         }
    }

    public class OptionalObjects : SupportedObjects
    {
        public OptionalObjects()
        {
            infoheader = "Optional Objects";
            edssection = "OptionalObjects";
        }

        public OptionalObjects(Dictionary<string, string> section)
        {
            infoheader = "Optional Objects";
            edssection = "OptionalObjects";
            parse(section);
        }
    }

    public class ManufacturerObjects : SupportedObjects
    {
        public ManufacturerObjects()
        {
            infoheader = "Manufacturer Objects";
            edssection = "ManufacturerObjects";
        }

        public ManufacturerObjects(Dictionary<string, string> section)
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
        public bool Dummy0001;
        public bool Dummy0002;
        public bool Dummy0003;
        public bool Dummy0004;
        public bool Dummy0005;
        public bool Dummy0006;
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
        public string FileName;//=example_objdict.eds
        public byte FileVersion;//=1
        public byte FileRevision;//=1

        public byte EDSVersionMajor;//=4.0
        public byte EDSVersionMinor;//=4.0
        public string EDSVersion;

        public string Description;//= //max 243 characters
        public DateTime CreationDateTime;//
        public string CreationTime;
        public string CreationDate;

        public string CreatedBy;//=CANFestival //max245
        public DateTime ModificationDateTime;//
        public string ModificationTime;
        public string ModificationDate;

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

                CreationDateTime = DateTime.ParseExact(dtcombined, "hh:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
            }

            if (section.ContainsKey("ModificationTime") && section.ContainsKey("ModificationTime"))
            {
                string dtcombined = section["ModificationTime"] + " " + section["ModificationDate"];
                ModificationDateTime = DateTime.ParseExact(dtcombined, "hh:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
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

        public string VendorName;
        public UInt32 VendorNumber;

        public string ProductName;
        public UInt32 ProductNumber;
        public UInt32 RevisionNumber;

        public bool BaudRate_10;
        public bool BaudRate_20;
        public bool BaudRate_50;
        public bool BaudRate_125;
        public bool BaudRate_250;
        public bool BaudRate_500;
        public bool BaudRate_800;
        public bool BaudRate_1000;

        public  bool SimpleBootUpMaster;
        public bool SimpleBootUpSlave;

        public byte Granularity;
        public bool DynamicChannelsSupported;

        public bool CompactPDO;

        public bool GroupMessaging;
        public UInt16 NrOfRXPDO;
        public UInt16 NrOfTXPDO;

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
        public int index;
        public int subindex=-1;
        public int nosubindexes;
        public string parameter_name;
        public ObjectType objecttype;
        public DataType datatype;
        public EDSsharp.AccessType accesstype;
        public string defaultvalue;
        public bool PDOMapping;
        public StorageLocation location;

        public ODentry()
        {

        }

        //Constructor for a simple VAR type
        public ODentry(string parameter_name,Int16 index, DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, bool PDOMapping)
        {
            this.parameter_name = parameter_name;
            this.index = index;
            this.objecttype = ObjectType.VAR;
            this.datatype = datatype;
            this.defaultvalue = defaultvalue;
            this.subindex = -1;

            if (accesstype >= EDSsharp.AccessType_Min && accesstype <= EDSsharp.AccessType_Max)
                this.accesstype = accesstype;
            else
                throw new ParameterException("AccessType invalid");

            this.PDOMapping = PDOMapping;
        }

        //SubIndex type
        public ODentry(string parameter_name, Int16 index, byte subindex, DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, bool PDOMapping)
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
        public ODentry(string parameter_name,Int16 index, byte nosubindex)
        {
            this.parameter_name = parameter_name;
            this.objecttype = ObjectType.ARRAY;
            this.index = index;
            this.subindex = -1;
            this.nosubindexes = nosubindex;
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

            if (subindex != -1)
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
        }

        public const AccessType AccessType_Min = AccessType.rw;
        public const AccessType AccessType_Max = AccessType.cons;

        Dictionary<string, Dictionary<string, string>> eds;
        public List<ODentry> ods;
        public FileInfo fi;
        public DeviceInfo di;
        public MandatoryObjects md;
        public OptionalObjects oo;
        public ManufacturerObjects mo;
        public Comments c;
        public Dummyusage du;

        public EDSsharp()
        {
            eds = new Dictionary<string, Dictionary<string, string>>();
            ods = new List<ODentry>();

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

        public ODentry getentryforindex(UInt16 index,Int16 sub)
        {
            foreach(ODentry od in ods)
            {
                if (od.index == index && od.subindex==sub)
                    return od;

            }

            return null;

        }

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

                od.index = Convert.ToInt16(m.Groups[1].ToString(), 16);

                if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
                {
                    od.nosubindexes = Convert.ToInt16(kvp.Value["SubNumber"], determinebase(kvp.Value["SubNumber"]));
                }

                if (od.objecttype == ObjectType.VAR)
                {

                    if (m.Groups[3].Length != 0)
                    {
                        Console.WriteLine(m.Groups[3].ToString());
                        od.subindex = Convert.ToInt16(m.Groups[3].ToString());
                    }
                    else
                        od.subindex = -1;

                    if(!kvp.Value.ContainsKey("DataType"))
                        throw new ParameterException("Missing required field DataType on" + section);

                    od.datatype = (DataType)Convert.ToInt16(kvp.Value["DataType"], determinebase(kvp.Value["DataType"]));
                    
                    if (!kvp.Value.ContainsKey("AccessType"))
                        throw new ParameterException("Missing required AccessType on" + section);
                    
                    string accesstype = kvp.Value["AccessType"];

                    // fudging because of enum enumeration and the const keyword
                    accesstype.Replace("const", "cons");
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

                ods.Add(od);
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

            foreach(ODentry od in ods)
            {
                if (md.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                }
            }

            oo.write(writer);

            foreach (ODentry od in ods)
            {
                if (oo.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                }
            }

            mo.write(writer);

            foreach (ODentry od in ods)
            {
                if (mo.objectlist.ContainsValue(od.index))
                {
                    od.write(writer);
                }
            }


            writer.Close();

        }

    }
}

public class ParameterException : Exception
{
    public ParameterException(String message)
        : base(message)
    {
        
    }
}

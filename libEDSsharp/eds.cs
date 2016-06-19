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
        ARRAY = 9,
    }

    public enum AccessType
    {
          rw=0,
          ro=1,
          wo=2,
    }

    public class InfoSection
    {
        protected Dictionary<string, string> section;

        protected string infoheader;



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
       
    }

 
    public class MandatoryObjects : SupportedObjects
    {
         public MandatoryObjects()
         {
              infoheader = "Mandatory Objects";
         }

         public MandatoryObjects(Dictionary<string, string> section)
         {
             infoheader = "Mandatory Objects";
             parse(section);
         }
    }

    public class OptionalObjects : SupportedObjects
    {
        public OptionalObjects()
        {
            infoheader = "Optional Objects";
        }

        public OptionalObjects(Dictionary<string, string> section)
        {
            infoheader = "Optional Objects";
            parse(section);
        }
    }

    public class SupportedObjects
    {

        public Dictionary<int, int> objectlist;
        public string infoheader;

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

    }

    public class Comments
    {

        public List<string> comments;
        public string infoheader = "Comments";

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
        }

        public Dummyusage(Dictionary<string, string> section)
        {
            infoheader = "CAN OPEN Dummy Usage";
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

        public string Description;//= //max 243 characters
        public DateTime CreationDateTime;//
        public string CreatedBy;//=CANFestival //max245
        public DateTime ModificationDateTime;//

        public string ModifiedBy;//=CANFestival //max244

        public FileInfo(Dictionary<string, string> section)
        {
            infoheader = "CAN OPEN FileInfo";
            parse(section);
        }

        public FileInfo()
        {
            infoheader = "CAN OPEN FileInfo";
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
            parse(section);
        }

        public DeviceInfo()
        {
            infoheader = "CAN OPEN DeviceInfo";
        }

    }


    public class ODentry
    {
        public int index;
        public int subindex;
        public int nosubindexes;
        public string paramater_name;
        public ObjectType objecttype;
        public DataType datatype;
        public AccessType accesstype;
        public string defaultvalue;
        public bool PDOMapping;

        public override string ToString()
        {
            if (nosubindexes > 0)
            {
                return String.Format("{0:x4}[{1}] : {2} : {3}", index, nosubindexes, paramater_name, datatype);
 
            }
            else
            {
                return String.Format("{0:x4}/{1} : {2} : {3}", index, subindex, paramater_name, datatype);
            }
        }

    }

    public class EDSsharp
    {

        Dictionary<string, Dictionary<string, string>> eds;
        public List<ODentry> ods;
        public FileInfo fi;
        public DeviceInfo di;
        public MandatoryObjects md;
        public OptionalObjects oo;
        public Comments c;
        public Dummyusage du;

        public void loadfile(string filename)
        {
            try
            {

                eds = new Dictionary<string, Dictionary<string, string>>();
                ods = new List<ODentry>();

                string sectionname = "";

                foreach (string line in File.ReadLines(filename))
                {
                  
                    string key = "";
                    string value = "";

                    if (line.IndexOf(';') == 0)
                        continue;

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

                            if(!eds.ContainsKey(sectionname))
                            {
                                eds.Add(sectionname, new Dictionary<string, string>());
                            }

                            eds[sectionname].Add(key,value);

                        }
                    }
                }

                foreach(KeyValuePair<string, Dictionary<string, string>> kvp in eds)
                {
                    string section = kvp.Key;


                    string pat = @"^\[([a-fA-F0-9]+)(sub)?([0-9]*)\]$";

                    Regex r = new Regex(pat);
                    Match m = r.Match(section);
                    if (m.Success)
                    {

                        ODentry od = new ODentry();

                        od.paramater_name = kvp.Value["ParameterName"];
                        int type = Convert.ToInt16(kvp.Value["ObjectType"], 16);
                        od.objecttype = (ObjectType)type;
                        od.index = Convert.ToInt16(m.Groups[1].ToString(),16);

                        if (od.objecttype == ObjectType.ARRAY)
                        {
                            od.nosubindexes = Convert.ToInt16(kvp.Value["SubNumber"],16);

                        }

                        if (od.objecttype == ObjectType.VAR)
                        {

                            if (m.Groups[3].ToString() != "")
                            {
                                Console.WriteLine(m.Groups[3].ToString());
                                od.subindex = Convert.ToInt16(m.Groups[3].ToString());
                            }
                            else
                                od.subindex = 0;

                            od.datatype = (DataType)Convert.ToInt16(kvp.Value["DataType"], 16);
                            od.accesstype = (AccessType)Enum.Parse(typeof(AccessType), kvp.Value["AccessType"]);
                            od.defaultvalue = kvp.Value["DefaultValue"];
                            od.PDOMapping = Convert.ToInt16(kvp.Value["PDOMapping"]) == 1;

                        }

                        ods.Add(od);
                    }

                }

                FileInfo fi = new FileInfo();
                fi.parse(eds["FileInfo"]);
                Console.WriteLine(fi.ToString());

                di = new DeviceInfo(eds["DeviceInfo"]);

                du = new Dummyusage(eds["DummyUsage"]);

                md = new MandatoryObjects(eds["MandatoryObjects"]);    

                oo = new OptionalObjects(eds["OptionalObjects"]);

                c = new Comments(eds["Comments"]);

            }
            catch(Exception e)
            {
                Console.WriteLine("** ALL GONE WRONG **" + e.ToString());
            }
        }

    }
}

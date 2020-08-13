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
 
    Copyright(c) 2016 - 2019 Robin Cornelius <robin.cornelius@gmail.com>
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
    public class StorageLocation : List<string>
    {
        public StorageLocation()
        {
            /* those are the values used in CANopenNode, starting at index "1".
             * Don't change the indexes, they are also used as binary flags */
            Add("Unused");
            Add("ROM");
            Add("RAM");
            Add("EEPROM");
        }

        new public void Add(string item)
        {
            /* we check if the storage location already exists */
            if ( ! Contains(item))
            {
                base.Add(item);
            }
        }
    }

    public enum PDOMappingType
    {
        no=0,
        optional=1,
        RPDO=2,
        TPDO=3,
        @default=4,
    }

 

    public class EdsExport : Attribute
    {
        public UInt16 maxlength;
        public bool commentonly=false;

        //mehmeh
        public EdsExport()
        {
        }

        public EdsExport(UInt16 maxlength)
        {
            this.maxlength = maxlength;
        }

        public bool IsReadOnly()
        {
            return commentonly;
        }

      
    }

    public class DcfExport : EdsExport
    {
    }

    public class InfoSection
    {
        protected Dictionary<string, string> section;

        protected string infoheader;
        protected string edssection;

        public enum Filetype
        {
            File_EDS,
            File_DCF
        }

        public virtual void Parse(Dictionary<string, string> section,string sectionname)
        {

            this.section = section;

            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo f in fields)
            {
                if(Attribute.IsDefined(f, typeof(EdsExport)))
                    GetField(f.Name, f.Name);

                if (Attribute.IsDefined(f, typeof(DcfExport)))
                    GetField(f.Name, f.Name);
            }

        }

        public bool GetField(string name, string varname)
        {
            FieldInfo f = null;

            try 
            {
                foreach (var element in section)
                {
                    if (String.Equals(element.Key, name, StringComparison.OrdinalIgnoreCase))
                    {

                        name = element.Key;
                        Type tx = this.GetType();

                        f = tx.GetField(varname);
                        object var = null;

                        switch (f.FieldType.Name)
                        {
                            case "String":
                                var = section[name];
                                break;

                            case "UInt32":
                                var = Convert.ToUInt32(section[name], EDSsharp.Getbase(section[name]));
                                break;

                            case "Int16":
                                var = Convert.ToInt16(section[name], EDSsharp.Getbase(section[name]));
                                break;

                            case "UInt16":
                                var = Convert.ToUInt16(section[name], EDSsharp.Getbase(section[name]));
                                break;

                            case "Byte":
                                var = Convert.ToByte(section[name], EDSsharp.Getbase(section[name]));
                                break;

                            case "Boolean":
                                var = section[name] == "1"; //because Convert is Awesome
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

            msg = $"*****************************************************{Environment.NewLine}";
            msg += $"*{String.Format("{0," + ((51 + infoheader.Length) / 2).ToString() + "}", infoheader),-51}*{Environment.NewLine}";
            msg += $"*****************************************************{Environment.NewLine}";

            Type tx = this.GetType();
            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo f in fields)
            {
                msg += $"{f.Name,-28}: {f.GetValue(this).ToString()}{Environment.NewLine}";
            }

            return msg;
        }

        public void Write(StreamWriter writer, Filetype ft)
        {
            writer.WriteLine("[" + edssection + "]");
            Type tx = this.GetType();
            FieldInfo[] fields = this.GetType().GetFields();

            foreach (FieldInfo f in fields)
            {

                if ((ft==Filetype.File_EDS) && (!Attribute.IsDefined(f, typeof(EdsExport))))
                    continue;

                if ((ft == Filetype.File_DCF) && (!(Attribute.IsDefined(f, typeof(DcfExport)) || Attribute.IsDefined(f, typeof(EdsExport)))))
                    continue;

                if (f.GetValue(this) == null)
                    continue;

                EdsExport ex = (EdsExport)f.GetCustomAttribute(typeof(EdsExport));

                bool comment = ex.IsReadOnly();

                if (f.FieldType.Name == "Boolean")
                {
                    writer.WriteLine(string.Format("{2}{0}={1}", f.Name, ((bool)f.GetValue(this)) == true ? 1 : 0,comment==true?";":""));
                }
                else
                {
                    writer.WriteLine(string.Format("{2}{0}={1}", f.Name, f.GetValue(this).ToString(), comment == true ? ";" : ""));
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
             : this()
         {
             Parse(section);
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
            : this()
        {
            Parse(section);
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
            : this()
        {
            Parse(section);
        }
    }

    public class TypeDefinitions : SupportedObjects
    {   
        public TypeDefinitions() : base()
        {
            infoheader = "Type Definitions";
            edssection = "TypeDefinitions";
        }

        public TypeDefinitions(Dictionary<string, string> section)
        {
            Parse(section);
        }

    }

    public class SupportedObjects
    {

        public Dictionary<int, int> objectlist;
        public string infoheader;
        public string edssection = "Supported Objects";
        public string countmsg = "SupportedObjects";

        public SupportedObjects()
        {
            objectlist = new Dictionary<int, int>();
        }

        public virtual void Parse(Dictionary<string, string> section)
        {
            objectlist = new Dictionary<int, int>();
            foreach(KeyValuePair<string,string> kvp in section)
            {
                if(kvp.Key.ToLower()=="supportedobjects")
                    continue;

                if (kvp.Key.ToLower() == "nrofentries")
                    continue;

                int count = Convert.ToInt16(kvp.Key, EDSsharp.Getbase(kvp.Key));
                int target = Convert.ToInt16(kvp.Value, EDSsharp.Getbase(kvp.Value));
                objectlist.Add(count, target);
            }
        }

        public override string ToString()
        {
            string msg;

            msg = $"*****************************************************{Environment.NewLine}";
            msg += $"*{String.Format("{0," + ((51 + infoheader.Length) / 2).ToString() + "}", infoheader),-51}*{Environment.NewLine}";
            msg += $"*****************************************************{Environment.NewLine}";
            msg += $"}}{Environment.NewLine}{countmsg} = {objectlist.Count}{Environment.NewLine}";
            foreach(KeyValuePair<int,int> kvp in objectlist)
            {
                msg += $"{kvp.Key,-5}: {kvp.Value:x4}{Environment.NewLine}";
            }

            return msg;

        }

        public void Write(StreamWriter writer)
        {
            writer.WriteLine("[" + edssection + "]");
            writer.WriteLine(string.Format("{0}={1}", countmsg,objectlist.Count));
            foreach (KeyValuePair<int, int> kvp in objectlist)
            {
                writer.WriteLine(string.Format("{0}=0x{1:X4}", kvp.Key, kvp.Value));
            }

            writer.WriteLine("");
        }

    }

    public class Comments
    {

        public List<string> comments = new List<string>();
        public string infoheader = "Comments";
        public string edssection = "Comments";

        public Comments()
        {
           
        }

        public Comments(Dictionary<string, string> section) 
        {
            Parse(section);
        }

        public virtual void Parse(Dictionary<string, string> section)
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

            msg = $"*****************************************************{Environment.NewLine}";
            msg += $"*{String.Format("{0," + ((51 + infoheader.Length) / 2).ToString() + "}", infoheader),-51}*{Environment.NewLine}";
            msg += $"*****************************************************{Environment.NewLine}";
            msg += $"{Environment.NewLine}Lines = {comments.Count}{Environment.NewLine}";
            foreach (string s in comments)
            {
                msg += $"{s}{Environment.NewLine}";
            }

            return msg;

        }

        public void Write(StreamWriter writer)
        {
            if(comments == null)
            {
                comments = new List<string>();
            }

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

        public Dummyusage(Dictionary<string, string> section) : this()
        {
            Parse(section,edssection);
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

        [DcfExport]
        public string LastEDS = "";

        public byte EDSVersionMajor;//=4.0
        
        public byte EDSVersionMinor;//=4.0
        [EdsExport]
        public string EDSVersion="";

        [EdsExport(maxlength=243)]
        public string Description="";//= //max 243 characters

        public DateTime CreationDateTime;//
        [EdsExport]
        public string CreationTime="";
        [EdsExport]
        public string CreationDate="";

        [EdsExport(maxlength = 245)]
        public string CreatedBy = "";//=CANFestival //max245
        
        public DateTime ModificationDateTime;//

        [EdsExport]
        public string ModificationTime="";
        [EdsExport]
        public string ModificationDate="";

        [EdsExport(maxlength = 244)]
        public string ModifiedBy="";//=CANFestival //max244

        //Folder CO_OD.c and CO_OD.h will be exported into
        public string exportFolder = "";


        public FileInfo(Dictionary<string, string> section) : this()
        {
            Parse(section,edssection);
        }

        public FileInfo()
        {
            infoheader = "CAN OPEN FileInfo";
            edssection = "FileInfo";
        }


        override public void Parse(Dictionary<string, string> section, string sectionname)
        {

            base.Parse(section,edssection);

            string dtcombined = "";
            try
            {
                if (section.ContainsKey("CreationTime") && section.ContainsKey("CreationDate"))
                {
                    dtcombined = section["CreationTime"].Replace(" ","") + " " + section["CreationDate"];
                    CreationDateTime = DateTime.ParseExact(dtcombined, "h:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch(Exception e)
            {
                if (e is System.FormatException)
                {
                    Warnings.warning_list.Add(String.Format("EDS Error: Section [{1}] Unable to parse DateTime {0} for CreationTime, not in DS306 format", dtcombined,sectionname));
                }
            }

            try
            {
                if (section.ContainsKey("ModificationTime") && section.ContainsKey("ModificationTime"))
                {
                    dtcombined = section["ModificationTime"].Replace(" ", "") + " " + section["ModificationDate"];
                    ModificationDateTime = DateTime.ParseExact(dtcombined, "h:mmtt MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                if (e is System.FormatException)
                {
                    Warnings.warning_list.Add(String.Format("EDS Error: Section [{1}] Unable to parse DateTime {0} for ModificationTime, not in DS306 format", dtcombined, sectionname));
                }
            }


            try
            {
                if (section.ContainsKey("EDSVersion"))
                {
                    string[] bits = section["EDSVersion"].Split('.');
                    if (bits.Length >= 1)
                        EDSVersionMajor = Convert.ToByte(bits[0]);
                    if (bits.Length >= 2)
                        EDSVersionMinor = Convert.ToByte(bits[1]);
                    //EDSVersion = String.Format("{0}.{1}", EDSVersionMajor, EDSVersionMinor);
                }
            }
            catch
            {
                Warnings.warning_list.Add(String.Format("Unable to parse EDS version {0}", section["EDSVersion"]));
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
        public byte CompactPDO;

        [EdsExport]
        public bool GroupMessaging;

        [EdsExport]
        public UInt16 NrOfRXPDO;

        [EdsExport]
        public UInt16 NrOfTXPDO;

        [EdsExport]
        public bool LSS_Supported;

        [EdsExport(commentonly=true)] //comment only, not supported by eds
        public string LSS_Type = "Server";

        public DeviceInfo()
        {
            infoheader = "CAN OPEN DeviceInfo";
            edssection = "DeviceInfo";
        }

        public DeviceInfo(Dictionary<string, string> section) : this()
        {
            Parse(section,edssection);
        }
    }


    public class DeviceCommissioning : InfoSection
    {

        public DeviceCommissioning()
        {
            infoheader = "CAN OPEN DeviceCommissioning";
            edssection = "DeviceCommissioning";
        }

        public DeviceCommissioning(Dictionary<string, string> section) : this()
        {
            Parse(section,edssection);
        }

        [DcfExport]
        public byte NodeId = 0;

        [DcfExport(maxlength = 246)]
        public string NodeName; //Max 246 characters

        [DcfExport]
        public UInt16 BaudRate;

        [DcfExport]
        public UInt32 NetNumber;

        [DcfExport(maxlength = 243)]
        public string NetworkName; //Max 243 characters

        [DcfExport]
        public bool CANopenManager;  //1 = CANopen manager, 0 or missing = not the manager

        [DcfExport]
        public UInt32 LSS_SerialNumber;

    }

    public class SupportedModules : InfoSection
    {
        [EdsExport]
        public UInt16 NrOfEntries;

        public SupportedModules()
        {
            infoheader = "CAN OPEN Supported Modules";
            edssection = "SupportedModules";
        }

        public SupportedModules(Dictionary<string, string> section) : this()
        {
            Parse(section,edssection);
        }

       
    }

    public class ConnectedModules : SupportedObjects
    {
        [EdsExport]
        public UInt16 NrOfEntries
        {
            get {  return (UInt16)connectedmodulelist.Count;  }
        }

        public Dictionary<int, int> connectedmodulelist;

        public ConnectedModules()
        {
            infoheader = "CAN OPEN Connected Modules";
            edssection = "ConnectedModules";
            countmsg = "NrOfEntries";
            connectedmodulelist = new Dictionary<int, int>();
        }

        public ConnectedModules(Dictionary<string, string> section) : this()
        {
            Parse(section);

            
            foreach(KeyValuePair<int,int> kvp in this.objectlist)
            {


                UInt16 K = (UInt16)kvp.Value;
                UInt16 V = (UInt16)kvp.Key;

                connectedmodulelist.Add(K, V);


            }

        }
     
    }

    public class MxFixedObjects : SupportedObjects
    {
        [EdsExport]
        public UInt16 NrOfEntries
        {
            get { return (UInt16)connectedmodulelist.Count; }
        }

        public Dictionary<int, int> connectedmodulelist;

        private int _moduleindex;

        public int Moduleindex
        {
            get { return _moduleindex; }
            set { _moduleindex = value; edssection = String.Format("M{0}FixedObjects",value); }
        }

        public MxFixedObjects(UInt16 modindex)
        {
            infoheader = "CAN OPEN Module Fixed Objects";
            this.Moduleindex = modindex;
            countmsg = "NrOfEntries";
            connectedmodulelist = new Dictionary<int, int>();
        }

        public MxFixedObjects(Dictionary<string, string> section, UInt16 modindex) : this(modindex)
        {
            Parse(section);

            foreach (KeyValuePair<int, int> kvp in this.objectlist)
            {
                connectedmodulelist.Add((UInt16)kvp.Value, (UInt16)kvp.Key);
            }
        }

    }



    public class ModuleInfo : InfoSection
    {
        [EdsExport(maxlength = 248)]
        public string ProductName;

        [EdsExport]
        public byte ProductVersion;

        [EdsExport]
        public byte ProductRevision;

        [EdsExport]
        public string OrderCode;

        UInt16 moduleindex = 0;

        public ModuleInfo(UInt16 moduleindex)
        {
            this.moduleindex = moduleindex;
            infoheader = "CAN OPEN Module Info " + moduleindex.ToString();
            edssection = string.Format("M{0}{1}", moduleindex, "ModuleInfo");
        }

        public ModuleInfo(Dictionary<string, string> section, UInt16 moduleindex) : this (moduleindex)
        {
            Parse(section,edssection);
        }
    }


    public class ModuleComments : Comments
    {

        UInt16 moduleindex;

        public ModuleComments(UInt16 moduleindex)
        {
            this.moduleindex = moduleindex;
            infoheader = "CAN OPEN Module Comments " + moduleindex.ToString();
            edssection = string.Format("M{0}{1}", moduleindex, "Comments");
        }

        public ModuleComments(Dictionary<string, string> section,UInt16 moduleindex) : this (moduleindex)
        {
            Parse(section);
        }


    }

    public class ModuleSubExtends : SupportedObjects
    {

        UInt16 moduleindex;

        public ModuleSubExtends(UInt16 moduleindex)
              : base()
        {
            this.moduleindex = moduleindex;
            infoheader = "CAN OPEN ModuleSubExtends "+moduleindex.ToString();
            edssection = string.Format("M{0}{1}", moduleindex, "SubExtends");
        }

        public ModuleSubExtends(Dictionary<string, string> section, UInt16 moduleindex)
              : this(moduleindex)
        {
            Parse(section);
        }

    }

    public class ODentry
    {

        private UInt16 _index;

        /// <summary>
        /// The index of the object in the Object Dictionary
        /// This cannot be set for child objects, if you read a child object you get the parents index
        /// </summary>
        [EdsExport]
        public UInt16 Index
        {
            get
            {
                if (parent != null)
                    return parent.Index;
                else
                    return _index;
            }
            set
            {
                if(value==0)
                {

                    //throw (new Exception("Object index must be set"));
                }

                if(parent == null)
                {
                    _index = value;
                }
                else
                {

                    //throw (new Exception("Typing to set index of a subobject"));
                }
               
            }
        }

        [EdsExport]
        public string parameter_name = "";

        [DcfExport]
        public string denotation = "";

        [EdsExport]
        public ObjectType objecttype;
        [EdsExport]
        public DataType datatype;
        [EdsExport]
        public EDSsharp.AccessType accesstype;

        [EdsExport]
        public string defaultvalue = "";

        [EdsExport]
        public string LowLimit = "";

        [EdsExport]
        public string HighLimit = "";

        [DcfExport]
        public string actualvalue = "";

        [EdsExport]
        public Byte ObjFlags = 0;

        [EdsExport]
        public byte CompactSubObj = 0;

        [EdsExport]
        public bool PDOMapping
        {
            get
            {
                return PDOtype != PDOMappingType.no;
            }
        }

        //FIXME Count "If several modules are gathered to form a new Sub-Index,
        //then the number is 0, followed by semicolon and the
        //number of bits that are created per module to build a new
        //Sub-Index"

        [EdsExport]
        public byte count = 0;

        [EdsExport]
        public byte ObjExtend = 0;

        public PDOMappingType PDOtype;

        //CANopenNode specific extra storage
        public string Label = "";
        public string Description = "";

        public string StorageLocation = "RAM";
        public SortedDictionary<UInt16, ODentry> subobjects = new SortedDictionary<UInt16, ODentry>();
        public ODentry parent = null;

        public string AccessFunctionName = "";
        public string AccessFunctionPreCode ="";

        public bool Disabled = false;

        public bool TPDODetectCos = false;

        //XDD Extensions//
        public string uniqueID;

        /// <summary>
        /// Used when writing out objects to know if we are writing the normal or the module parts out
        /// Two module parts subext and fixed are available.
        /// </summary>
        public enum Odtype
        {
            NORMAL,
            SUBEXT,
            FIXED,
        }

        /// <summary>
        /// Empty object constructor
        /// </summary>
        public ODentry()
        {

        }

        /// <summary>
        /// ODentry constructor for a simple VAR type
        /// </summary>
        /// <param name="parameter_name">Name of Object Dictionary Entry</param>
        /// <param name="index">Index of object in object dictionary</param>
        /// <param name="datatype">Type of this objects data</param>
        /// <param name="defaultvalue">Default value (always set as a string)</param>
        /// <param name="accesstype">Allowed CANopen access permissions</param>
        /// <param name="PDOMapping">Allowed PDO mapping options</param>
        public ODentry(string parameter_name,UInt16 index, DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, PDOMappingType PDOMapping)
        {
            this.parameter_name = parameter_name;
            this.Index = index;
            this.objecttype = ObjectType.VAR;
            this.datatype = datatype;
            this.defaultvalue = defaultvalue;


            if (accesstype >= EDSsharp.AccessType_Min && accesstype <= EDSsharp.AccessType_Max)
                this.accesstype = accesstype;
            else
                throw new ParameterException("AccessType invalid");

            this.PDOtype = PDOMapping;

        }

         /// <summary>
         /// ODConstructor useful for subobjects
         /// </summary>
         /// <param name="parameter_name"></param>
         /// <param name="index">NOT USED</param>
         /// <param name="datatype"></param>
         /// <param name="defaultvalue"></param>
         /// <param name="accesstype"></param>
         /// <param name="PDOMapping"></param>
         /// <param name="parent"></param>
        public ODentry(string parameter_name, UInt16 index,  DataType datatype, string defaultvalue, EDSsharp.AccessType accesstype, PDOMappingType PDOMapping, ODentry parent)
        {
            this.parent = parent;
            this.parameter_name = parameter_name;
            this.objecttype = ObjectType.VAR;
            this.datatype = datatype;
            this.defaultvalue = defaultvalue;
            this.Index = index;

            if (accesstype >= EDSsharp.AccessType_Min && accesstype <= EDSsharp.AccessType_Max)
                this.accesstype = accesstype;
            else
                throw new ParameterException("AccessType invalid");

            this.PDOtype = PDOMapping;
        }
        

        /// <summary>
        /// ODEntry constructor for array subobjects
        /// </summary>
        /// <param name="parameter_name"></param>
        /// <param name="index"></param>
        /// <param name="nosubindex"></param>
        public ODentry(string parameter_name,UInt16 index, byte nosubindex)
        {
            this.parameter_name = parameter_name;
            this.objecttype = ObjectType.ARRAY;
            this.Index = index;
            //this.nosubindexes = nosubindex;
            this.objecttype = ObjectType.VAR;     
        }


        /// <summary>
        /// Provide a simple string representation of the object, only parameters index, no subindexes/subindex parameter name and data type are included
        /// Useful for debug and also appears in debugger when you inspect this object
        /// </summary>
        /// <returns>string summary of object</returns>
        public override string ToString()
        {
            if (subobjects.Count > 0)
            {
                return String.Format("{0:x4}[{1}] : {2} : {3}", Index, subobjects.Count, parameter_name, datatype);
 
            }
            else
            {
                return String.Format("{0:x4}/{1} : {2} : {3}", Index, Subindex, parameter_name, datatype);
            }
        }

        /// <summary>
        /// If data type is an octet string we must remove all spaces when writing out to a EDS/DCF file
        /// </summary>
        /// <param name="value">Value to be processed</param>
        /// <returns>value if not octet string or value with spaces removed if octet string</returns>
        public string Formatoctetstring(string value)
        {
            DataType dt = datatype;
            if (dt == DataType.UNKNOWN && this.parent != null)
                dt = parent.datatype;

            string ret = value;

            if (dt == DataType.OCTET_STRING)
            {
                ret = value.Replace(" ", "");
            }

            return ret;
        }

        /// <summary>
        /// Write out this Object dictionary entry to an EDS/DCF file using correct formatting
        /// </summary>
        /// <param name="writer">Handle to the stream writer to write to</param>
        /// <param name="ft">File type being written</param>
        /// 
        public void Write(StreamWriter writer, InfoSection.Filetype ft, Odtype odt= Odtype.NORMAL, int module=0)
        {

            string fixedmodheader = "";

            if (odt == Odtype.FIXED)
            {
                fixedmodheader = string.Format("M{0}Fixed", module);
            }

            if(odt == Odtype.SUBEXT)
            {
                fixedmodheader = string.Format("M{0}SubExt", module);
            }

            if (parent != null)
            {
                writer.WriteLine(string.Format("[{0}{1:X}sub{2:X}]", fixedmodheader,Index, Subindex));
            }
            else
            {
                writer.WriteLine(string.Format("[{0}{1:X}]",fixedmodheader,Index));
            }

            writer.WriteLine(string.Format("ParameterName={0}", parameter_name));

            if(ft == InfoSection.Filetype.File_DCF)
            {
                writer.WriteLine(string.Format("Denotation={0}", denotation));
            }

            writer.WriteLine(string.Format("ObjectType=0x{0:X}", (int)objecttype));
            writer.WriteLine(string.Format(";StorageLocation={0}",StorageLocation));

            if (objecttype == ObjectType.ARRAY)
            {
                writer.WriteLine(string.Format("SubNumber=0x{0:X}", Nosubindexes));
            }

            if (objecttype == ObjectType.REC)
            {
                writer.WriteLine(string.Format("SubNumber=0x{0:X}", Nosubindexes));
            }

            if (objecttype == ObjectType.VAR)
            {
                DataType dt = datatype;
                if (dt == DataType.UNKNOWN && this.parent != null)
                    dt = parent.datatype;
                writer.WriteLine(string.Format("DataType=0x{0:X4}", (int)dt));
                writer.WriteLine(string.Format("AccessType={0}", accesstype.ToString()));


                if(HighLimit != null && HighLimit != "")
                {
                    writer.WriteLine(string.Format("HighLimit={0}", Formatoctetstring(HighLimit)));
                }

                if (LowLimit != null && LowLimit != "")
                {
                    writer.WriteLine(string.Format("LowLimit={0}", Formatoctetstring(LowLimit)));
                }
    
                writer.WriteLine(string.Format("DefaultValue={0}", Formatoctetstring(defaultvalue)));

                //TODO If the ObjectType is domain (0x2) the value of the object may be stored in a file,UploadFile and DownloadFile
                if (ft == InfoSection.Filetype.File_DCF)
                {
                    writer.WriteLine(string.Format("ParameterValue={0}", Formatoctetstring(actualvalue)));
                }

                writer.WriteLine(string.Format("PDOMapping={0}", PDOMapping==true?1:0));

                if (TPDODetectCos == true)
                {
                    writer.WriteLine(";TPDODetectCos=1");
                }


            }

            //Count is for modules in the [MxSubExtxxxx]
            //Should we export this on EDS only, or DCF or both?
            if (odt == Odtype.SUBEXT )
            {
                    writer.WriteLine(string.Format("Count={0}", count));
                    writer.WriteLine(string.Format("ObjExtend={0}", ObjExtend));
            }

            //ObjectFlags is always optional (Page 15, DSP306) and used for DCF writing to nodes
            //also recommended not to write if it is already 0
            if (ObjFlags != 0)
            {
                writer.WriteLine(string.Format("ObjFlags={0}", ObjFlags));
            }

            writer.WriteLine("");
        }

        /// <summary>
        /// Returns a c compatible string that represents the name of the object, - is replaced with _
        /// words separated by a space are replaced with _ for a separator eg ONE TWO becomes ONE_TWO
        /// </summary>
        /// <returns></returns>
        public string Paramater_cname()
        {
            string cname = parameter_name.Replace("-", "_");

            cname =  Regex.Replace(cname, @"([A-Z]) ([A-Z])", "$1_$2");
            cname = cname.Replace(" ", "");

            return cname;
        }

        /// <summary>
        /// Return the size in bytes for the given CANopen datatype of this object, eg the size of what ever the datatype field is set to 
        /// </summary>
        /// <returns>no of bytes</returns>
        public int Sizeofdatatype()
        {
            DataType dt = datatype;

            if (dt == DataType.UNKNOWN && this.parent != null)
                dt = parent.datatype;
 
            switch (dt)
            {
                case DataType.BOOLEAN:
                    return 1;

                case DataType.UNSIGNED8:
                case DataType.INTEGER8:
                    return 8;

                case DataType.VISIBLE_STRING:
                case DataType.OCTET_STRING:
                    return Lengthofstring*8;

                case DataType.INTEGER16:
                case DataType.UNSIGNED16:
                case DataType.UNICODE_STRING:
                    return 16; //FIXME is this corret for UNICODE_STRING seems dodgy?

                case DataType.UNSIGNED24:
                case DataType.INTEGER24:
                    return 24;

                case DataType.INTEGER32:
                case DataType.UNSIGNED32:
                case DataType.REAL32:
                    return 32;

                case DataType.INTEGER40:
                case DataType.UNSIGNED40:
                    return 40;

                case DataType.INTEGER48:
                case DataType.UNSIGNED48:
                case DataType.TIME_DIFFERENCE:
                case DataType.TIME_OF_DAY:
                    return 48;

                case DataType.INTEGER56:
                case DataType.UNSIGNED56:
                    return 56;

                case DataType.INTEGER64:
                case DataType.UNSIGNED64:
                case DataType.REAL64:
                    return 64;

                case DataType.DOMAIN:
                    return 0;

                default: //FIXME
                    return 0;

            }
        }

        
        /// <summary>
        /// This is the no of subindexes present in the object, it is NOT the maximum subobject index
        /// </summary>
        [EdsExport]
        public int Nosubindexes
        {
            get
            {
                return subobjects.Count;
            }
        }
        
        //warning eds files with gaps in subobject lists have been seen in the wild
        //this function tries to get the array index based on sub number not array number
        //it may return null
        //This needs expanding to be used globally through the application ;-(
        public ODentry Getsubobject(UInt16 no)
        {
            if (subobjects.ContainsKey(no))
                return subobjects[no];
            return null;
        }

        public string Getsubobjectdefaultvalue(UInt16 no)
        {
            if (subobjects.ContainsKey(no))
                return subobjects[no].defaultvalue;
            else
                return "";
        }

        public bool Containssubindex(UInt16 no)
        {
            if (subobjects.ContainsKey(no))
                return true;

            return false;
        }

        public byte Getmaxsubindex()
        {
            //Although subindex 0 should contain the max subindex value
            //we don't enforce that anywhere in this lib, we should have a setter function
            //that sets it to the highest subobject found.
            if (objecttype == ObjectType.ARRAY || objecttype == ObjectType.REC)
                if (Containssubindex(0))
                {
                    return EDSsharp.ConvertToByte(Getsubobjectdefaultvalue(0));
                }

            return 0;
        }

        public int Lengthofstring
        {
            get
            {
                string defaultvalue = this.defaultvalue;
                if (defaultvalue == null)
                    return 0;

                switch (this.datatype)
                {
                    case DataType.VISIBLE_STRING:
                        {
                            return defaultvalue.Unescape().Length;
                        }

                    case DataType.OCTET_STRING:
                        {
                            return Regex.Replace(defaultvalue, @"\s", "").Length / 2;
                        }

                    case DataType.UNICODE_STRING:
                        {
                            return Regex.Replace(defaultvalue, @"\s", "").Length / 4;
                        }
                    default:
                        {
                            return 0;
                        }
                }
            }
        }

        public UInt16 Subindex
        { 
            get
            {
                if(this.parent!=null)
                {
                    return parent.Findsubindex(this);
                }
                return 0;

            }
        }

        public UInt16 Findsubindex(ODentry od)
        {
            foreach(KeyValuePair<UInt16,ODentry>kvp in subobjects )
            {
                if (kvp.Value == od)
                    return kvp.Key;
            }

            return 0;

        }

        /// <summary>
        /// Add an existing entry as a subobject of this OD
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        public void addsubobject(byte index, ODentry sub)
        {
            sub.parent = this;
            this.subobjects.Add(index, sub);
        }

    }

    public class Module
    {

        public ModuleInfo mi;
        public ModuleComments mc;
        public ModuleSubExtends mse;
        public MxFixedObjects mxfo;
        public SortedDictionary<UInt16, ODentry> modulefixedobjects;
        public SortedDictionary<UInt16, ODentry> modulesubext;

        public UInt16 moduleindex;

        public Module(UInt16 moduleindex)
        {

            this.moduleindex = moduleindex;

            mi = new ModuleInfo(moduleindex);
            mc = new ModuleComments(moduleindex);
            mse = new ModuleSubExtends(moduleindex);
            mxfo = new MxFixedObjects(moduleindex);
            modulefixedobjects = new SortedDictionary<ushort, ODentry>();
            modulesubext = new SortedDictionary<ushort, ODentry>();
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
            @const = 5,
            UNKNOWN
        }

        public const AccessType AccessType_Min = AccessType.rw;
        public const AccessType AccessType_Max = AccessType.@const;


        //This is the last file name used for this eds/xml file and is not
        //the same as filename within the FileInfo structure.
        public string edsfilename = null;
        public string dcffilename = null;
        public string xmlfilename = null;
        public string xddfilename = null;

        //property to indicate unsaved data;
        private bool _dirty;
        public bool Dirty
        {
            get
            {
                return _dirty;
            }
            set
            {
                _dirty = value;
                OnDataDirty?.Invoke(_dirty, this);
            }
        }

        protected Dictionary<string, Dictionary<string, string>> eds;
        protected Dictionary<string, int> sectionlinenos;
        public SortedDictionary<UInt16, ODentry> ods;
        public SortedDictionary<UInt16, ODentry> dummy_ods;

        public StorageLocation storageLocation = new StorageLocation();

        public FileInfo fi;
        public DeviceInfo di;
        public MandatoryObjects md;
        public OptionalObjects oo;
        public ManufacturerObjects mo;
        public Comments c;
        public Dummyusage du;
        public DeviceCommissioning dc;

        public TypeDefinitions td;

        public SupportedModules sm;
        public ConnectedModules cm;

        // public Dictionary<UInt16, ModuleInfo> mi;
        // public Dictionary<UInt16, ModuleComments> mc;
        // public Dictionary<UInt16, ModuleSubExtends> mse;
        // public Dictionary<ushort, MxFixedObjects> mxfo;
        // public SortedDictionary<UInt16, SortedDictionary<UInt16, ODentry>> modulefixedobjects;
        // public SortedDictionary<UInt16, SortedDictionary<UInt16, ODentry>> modulesubext;

        public Dictionary<UInt16, Module> modules;

        public UInt16 NodeId = 0;

        public delegate void DataDirty(bool dirty, EDSsharp sender);
        public event DataDirty OnDataDirty;

        public EDSsharp()
        {


            eds = new Dictionary<string, Dictionary<string, string>>();
            ods = new SortedDictionary<UInt16, ODentry>();
            dummy_ods = new SortedDictionary<UInt16, ODentry>();

            fi = new FileInfo();
            di = new DeviceInfo();
            du = new Dummyusage();
            md = new MandatoryObjects();
            oo = new OptionalObjects();
            mo = new ManufacturerObjects();
            dc = new DeviceCommissioning();
            c = new Comments();
            sm = new SupportedModules();
            cm = new ConnectedModules();
            td = new TypeDefinitions();


            //mi = new Dictionary<ushort, ModuleInfo>();
            //mc = new Dictionary<ushort, ModuleComments>();
            //mse = new Dictionary<ushort, ModuleSubExtends>();
            //mxfo = new Dictionary <ushort, MxFixedObjects>();
            //modulefixedobjects = new SortedDictionary<ushort, SortedDictionary<ushort, ODentry>>();
            //modulesubext = new SortedDictionary<ushort, SortedDictionary<ushort, ODentry>>();

            modules = new Dictionary<UInt16, Module>();


            //FIXME no way for the Major/Minor to make it to EDSVersion
            fi.EDSVersionMajor = 4;
            fi.EDSVersionMinor = 0;

            fi.FileVersion = 1;
            fi.FileRevision = 1;

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

            dummy_ods.Add(0x002, new ODentry("Dummy Int8", 0x002,  DataType.INTEGER8, "0", AccessType.ro, PDOMappingType.optional, null));
            dummy_ods.Add(0x003, new ODentry("Dummy Int16", 0x003, DataType.INTEGER16, "0", AccessType.ro, PDOMappingType.optional, null));
            dummy_ods.Add(0x004, new ODentry("Dummy Int32", 0x004, DataType.INTEGER32, "0", AccessType.ro, PDOMappingType.optional, null));
            dummy_ods.Add(0x005, new ODentry("Dummy UInt8", 0x005, DataType.UNSIGNED8, "0", AccessType.ro, PDOMappingType.optional, null));
            dummy_ods.Add(0x006, new ODentry("Dummy UInt16", 0x006, DataType.UNSIGNED16, "0", AccessType.ro, PDOMappingType.optional, null));
            dummy_ods.Add(0x007, new ODentry("Dummy UInt32", 0x007, DataType.UNSIGNED32, "0", AccessType.ro, PDOMappingType.optional, null));

        }

        public void Setdirty()
        {

        }

        protected string sectionname = "";

        public void Parseline(string linex,int no)
        {

            string key = "";
            string value = "";

            string line = linex.TrimStart(';');
            bool custom_extension = false;

            if (linex == null || linex == "")
                return;

            if (linex[0] == ';')
                custom_extension = true;


            //extract sections
            {
                string pat = @"^\[([a-z0-9]+)\]";

                Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                Match m = r.Match(line);
                if (m.Success)
                {
                    Group g = m.Groups[1];
                    sectionname = g.ToString();

                    if (!eds.ContainsKey(sectionname))
                    {
                        eds.Add(sectionname, new Dictionary<string, string>());
                    }
                    else
                    {
                        Warnings.warning_list.Add(string.Format("EDS Error on Line {0} : Duplicate section [{1}] ", no,sectionname));
                    }
                }
            }

            //extract keyvalues
            {
                //Bug #70 Eat whitespace!
                string pat = @"^([a-z0-9_]+)[ ]*=[ ]*(.*)";

                Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                Match m = r.Match(line);
                if (m.Success)
                {
                    key = m.Groups[1].ToString();
                    value = m.Groups[2].ToString();
                    value = value.TrimEnd(' ','\t','\n','\r');

                    //not sure how we actually get here with out a section being in the dictionary already..
                    //suspect this is dead code.
                    if (!eds.ContainsKey(sectionname))
                    {
                        eds.Add(sectionname, new Dictionary<string, string>());
                    }
            
                    if (custom_extension == false)
                    {
                        try
                        {
                            eds[sectionname].Add(key, value);
                        }
                        catch(Exception e)
                        {
                            Warnings.warning_list.Add(string.Format("EDS Error on Line {3} : Duplicate key \"{0}\" value \"{1}\" in section [{2}]", key,value,sectionname, no));
                        }
                    }
                    else
                    //Only allow our own extensions to populate the key/value pair
                    {
                        if (key == "StorageLocation" || key == "LSS_Type" || key== "TPDODetectCos")
                        {
                            try
                            {
                                eds[sectionname].Add(key, value);
                            }
                            catch(Exception e)
                            {
                                Warnings.warning_list.Add(string.Format("EDS Error on Line {3} : Duplicate custom key \"{0}\" value \"{1}\" in section [{2}]", key, value, sectionname, no));
                            }
                        }
                    }
                }
            }
        }

        public void ParseEDSentry(KeyValuePair<string, Dictionary<string, string>> kvp)
        {
            string section = kvp.Key;

            string pat = @"^(M[0-9a-fA-F]+(Fixed|SubExt))?([a-fA-F0-9]+)(sub)?([0-9a-fA-F]*)$";

            Regex r = new Regex(pat);
            Match m = r.Match(section);
            if (m.Success)
            {

                SortedDictionary<UInt16, ODentry>  target = this.ods;

                //** MODULE DCF SUPPORT

                string pat2 = @"^M([0-9a-fA-F]+)(Fixed|SubExt)([0-9a-fA-F]+)";
                Regex r2 = new Regex(pat2, RegexOptions.IgnoreCase);
                Match m2 = r2.Match(m.Groups[0].ToString());

                if (m2.Success)
                {

                    UInt16 modindex = Convert.ToUInt16(m2.Groups[1].Value);
                    UInt16 odindex = Convert.ToUInt16(m2.Groups[3].Value);

                    if (!modules.ContainsKey(modindex))
                        modules.Add(modindex, new Module(modindex));

                    if (m2.Groups[2].ToString() == "SubExt")
                    {      
                        target = modules[modindex].modulesubext;
                          
                    }
                    else
                    {
                        target = modules[modindex].modulefixedobjects;
                    }
                }


                ODentry od = new ODentry
                {
                    //Indexes in the EDS are always in hex format without the pre 0x
                    Index = Convert.ToUInt16(m.Groups[3].ToString(), 16)
                };

                //Parameter name, mandatory always
                if (!kvp.Value.ContainsKey("ParameterName"))
                    throw new ParameterException("Missing required field ParameterName on" + section);
                od.parameter_name = kvp.Value["ParameterName"];

                //Object type, assumed to be VAR unless specified
                if (kvp.Value.ContainsKey("ObjectType"))
                {
                    int type = Convert.ToInt16(kvp.Value["ObjectType"], Getbase(kvp.Value["ObjectType"]));
                    od.objecttype = (ObjectType)type;
                }
                else
                {
                    od.objecttype = ObjectType.VAR;
                }

                if(kvp.Value.ContainsKey("CompactSubObj"))
                {
                    od.CompactSubObj = Convert.ToByte(kvp.Value["CompactSubObj"],Getbase(kvp.Value["CompactSubObj"]));
                }

                if(kvp.Value.ContainsKey("ObjFlags"))
                {
                    od.ObjFlags = Convert.ToByte(kvp.Value["ObjFlags"], Getbase(kvp.Value["ObjFlags"]));
                }
                else
                {
                    od.ObjFlags = 0;
                }

                //Access Type
                if(kvp.Value.ContainsKey("StorageLocation"))
                {
                    od.StorageLocation = kvp.Value["StorageLocation"];
                }

                if (kvp.Value.ContainsKey("TPDODetectCos"))
                {
                    string test = kvp.Value["TPDODetectCos"].ToLower();
                    if (test == "1" || test == "true")
                    {
                        od.TPDODetectCos = true;                     
                    }
                    else
                        od.TPDODetectCos = false; 
                }

                if (kvp.Value.ContainsKey("Count"))
                {
                    od.count = Convert.ToByte(kvp.Value["Count"]);
                }

                if (kvp.Value.ContainsKey("ObjExtend"))
                {
                    od.ObjExtend = Convert.ToByte(kvp.Value["ObjExtend"]);
                }


                if (od.objecttype == ObjectType.VAR)
                {

                    if (kvp.Value.ContainsKey("CompactSubObj"))
                        throw new ParameterException("CompactSubObj not valid for a VAR Object, section: " + section);

                    if (kvp.Value.ContainsKey("ParameterValue"))
                    {
                        od.actualvalue = kvp.Value["ParameterValue"];
                    }

                    if (kvp.Value.ContainsKey("HighLimit"))
                    {
                        od.HighLimit = kvp.Value["HighLimit"];
                    }

                    if (kvp.Value.ContainsKey("LowLimit"))
                    {
                        od.LowLimit = kvp.Value["LowLimit"];
                    }

                    if (kvp.Value.ContainsKey("Denotation"))
                    {
                        od.denotation = kvp.Value["Denotation"];
                    }

                    if (m.Groups[5].Length != 0)
                    {
                        //FIXME are subindexes in hex always?
                        UInt16 subindex = Convert.ToUInt16(m.Groups[5].ToString(),16);
                        od.parent = target[od.Index];
                        target[od.Index].subobjects.Add(subindex, od);
                    }

                    if (!kvp.Value.ContainsKey("DataType"))
                            throw new ParameterException("Missing required field DataType on" + section);
                        od.datatype = (DataType)Convert.ToInt16(kvp.Value["DataType"], Getbase(kvp.Value["DataType"]));
                    
                    if (!kvp.Value.ContainsKey("AccessType"))
                        throw new ParameterException("Missing required AccessType on" + section);

                    string accesstype = kvp.Value["AccessType"].ToLower();

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
                        
                        bool pdo = Convert.ToInt16(kvp.Value["PDOMapping"],Getbase(kvp.Value["PDOMapping"])) == 1;
                        if (pdo == true)
                            od.PDOtype = PDOMappingType.optional;
                    }

                }

              
                if(od.objecttype == ObjectType.REC|| od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.DEFSTRUCT)
                {

                    if (od.CompactSubObj != 0)
                    {
                        if (!kvp.Value.ContainsKey("DataType"))
                            throw new ParameterException("Missing required field DataType on" + section);
                        od.datatype = (DataType)Convert.ToInt16(kvp.Value["DataType"], Getbase(kvp.Value["DataType"]));

                        if (!kvp.Value.ContainsKey("AccessType"))
                            throw new ParameterException("Missing required AccessType on" + section);
                        string accesstype = kvp.Value["AccessType"];
                        if (Enum.IsDefined(typeof(AccessType), accesstype))
                        {
                            od.accesstype = (AccessType)Enum.Parse(typeof(AccessType), accesstype);
                        }
                        else
                        {
                            throw new ParameterException("Unknown AccessType on" + section);
                        }

                        //now we generate CompactSubObj number of var objects below this parent

                        if(od.CompactSubObj>=0xfe)
                        {
                            od.CompactSubObj = 0xfe;
                        }

                        ODentry subi = new ODentry("NrOfObjects", od.Index, DataType.UNSIGNED8, String.Format("0x{0:x2}",od.CompactSubObj), AccessType.ro, PDOMappingType.no, od);      
                        od.subobjects.Add(0x00, subi);

                        for (int x=1; x<= od.CompactSubObj; x++)
                        {
                            string parameter_name = string.Format("{0}{1:x2}", od.parameter_name, x );
                            ODentry sub = new ODentry(parameter_name, od.Index, od.datatype, od.defaultvalue, od.accesstype, od.PDOtype, od);

                            if (kvp.Value.ContainsKey("HighLimit"))
                                sub.HighLimit = kvp.Value["HighLimit"];

                            if (kvp.Value.ContainsKey("LowLimit"))
                                sub.HighLimit = kvp.Value["LowLimit"];

                            od.subobjects.Add((ushort)(x ), sub);
                        }

                    }
                    else
                    {
                        if (!kvp.Value.ContainsKey("SubNumber"))
                            throw new ParameterException("Missing SubNumber on Array for" + section);



                    }
                }

                if(od.objecttype == ObjectType.DOMAIN)
                {
                    od.datatype = DataType.DOMAIN;
                    od.accesstype = AccessType.rw;

                    if (kvp.Value.ContainsKey("DefaultValue"))
                        od.defaultvalue = kvp.Value["DefaultValue"];

                }

                //Only add top level to this list
                if (m.Groups[5].Length == 0)
                {
                    target.Add(od.Index, od);
                }
            }

        }

        public void Loadfile(string filename)
        {

            
            if (Path.GetExtension(filename).ToLower() == ".eds")
            {
                edsfilename = filename;
            }

            if (Path.GetExtension(filename).ToLower() == ".dcf")
            {
                dcffilename = filename;
            }

            //try
            {
                int lineno = 1;
                foreach (string linex in File.ReadLines(filename))
                {
                    Parseline(linex,lineno);
                    lineno++;
                }

                di = new DeviceInfo(eds["DeviceInfo"]);

                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in eds)
                {
                    ParseEDSentry(kvp);
                }

                fi = new FileInfo(eds["FileInfo"]);
                if(eds.ContainsKey("DummyUsage"))
                    du = new Dummyusage(eds["DummyUsage"]);

                md = new MandatoryObjects(eds["MandatoryObjects"]);

                if(eds.ContainsKey("OptionalObjects"))
                    oo = new OptionalObjects(eds["OptionalObjects"]);

                if(eds.ContainsKey("ManufacturerObjects"))
                    mo = new ManufacturerObjects(eds["ManufacturerObjects"]);

                if (eds.ContainsKey("TypeDefinitions"))
                    td = new TypeDefinitions(eds["TypeDefinitions"]);

                //Only DCF not EDS files
                dc = new DeviceCommissioning();
                if(eds.ContainsKey("DeviceCommissioning"))
                {
                    dc.Parse(eds["DeviceCommissioning"],"DeviceCommissioning");
                    edsfilename = fi.LastEDS;
                }
                
                c = new Comments();

                if (eds.ContainsKey("Comments"))
                    c.Parse(eds["Comments"]);

                //Modules

                //FIXME
                //we don't parse or support [MxFixedObjects] with MxFixedxxxx and MxFixedxxxxsubx

                if (eds.ContainsKey("SupportedModules"))
                {
                    sm = new SupportedModules(eds["SupportedModules"]);

                    //find MxModuleInfo

                    foreach (string s in eds.Keys)
                    {
                        String pat = @"^M([0-9]+)ModuleInfo";
                        Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                        Match m = r.Match(s);

                        if (m.Success)
                        {
                            UInt16 modindex = Convert.ToUInt16(m.Groups[1].Value);
                            ModuleInfo mi = new ModuleInfo(eds[s], modindex);

                            if (!modules.ContainsKey(modindex))
                                modules.Add(modindex, new Module(modindex));

                            modules[modindex].mi = mi;

                        }


                        pat = @"^M([0-9]+)Comments";
                        r = new Regex(pat, RegexOptions.IgnoreCase);
                        m = r.Match(s);

                        if (m.Success)
                        {
                            UInt16 modindex = Convert.ToUInt16(m.Groups[1].Value);
                            ModuleComments mc = new ModuleComments(eds[s], modindex);

                            if (!modules.ContainsKey(modindex))
                                modules.Add(modindex, new Module(modindex));

                            modules[modindex].mc = mc;

                        }

                        pat = @"^M([0-9]+)SubExtends";
                        r = new Regex(pat, RegexOptions.IgnoreCase);
                        m = r.Match(s);

                        if (m.Success)
                        {
                            UInt16 modindex = Convert.ToUInt16(m.Groups[1].Value);
                            ModuleSubExtends mse = new ModuleSubExtends(eds[s], modindex);

                            if (!modules.ContainsKey(modindex))
                                modules.Add(modindex, new Module(modindex));

                            modules[modindex].mse = mse;
                        }


                        //DCF only
                        pat = @"^M([0-9]+)FixedObjects";
                        r = new Regex(pat, RegexOptions.IgnoreCase);
                        m = r.Match(s);

                        if (m.Success)
                        {
                            UInt16 modindex = Convert.ToUInt16(m.Groups[1].Value);
                            MxFixedObjects mxf = new MxFixedObjects(eds[s],modindex);

                            if (!modules.ContainsKey(modindex))
                                modules.Add(modindex, new Module(modindex));

                            modules[modindex].mxfo = mxf;

                        }
                    }
                }


                if (eds.ContainsKey("ConnectedModules"))
                {
                    cm = new ConnectedModules(eds["ConnectedModules"]);              
                }

                 //COMPACT PDO

                if (di.CompactPDO != 0)
                {

                    for (UInt16 index = 0x1400; index < 0x1600; index++)
                    {
                        ApplycompactPDO(index);
                    }

                    for (UInt16 index = 0x1800; index < 0x1A00;index ++)
                    {
                        ApplycompactPDO(index);
                    }
                }

                ApplyimplicitPDO();
            }
            // catch(Exception e)
            //{
            //  Console.WriteLine("** ALL GONE WRONG **" + e.ToString());
            // }
        }

        public void ApplycompactPDO(UInt16 index)
        {
            if (ods.ContainsKey(index))
            {
                if ((!ods[index].Containssubindex(1)) && ((this.di.CompactPDO & 0x01) == 0))
                {
                    //Fill in cob ID
                    //FIX ME i'm really sure this is not correct, what default values should be used???
                    string cob = string.Format("0x180+$NODEID");
                    ODentry subod = new ODentry("COB-ID", index, DataType.UNSIGNED32, cob, AccessType.rw, PDOMappingType.no, ods[index]);
                    ods[index].subobjects.Add(0x05, subod);

                }

                if ((!ods[index].Containssubindex(2)) && ((this.di.CompactPDO & 0x02) == 0))
                {
                    //Fill in type

                    ODentry subod = new ODentry("Type", index, DataType.UNSIGNED8, "0xff", AccessType.rw, PDOMappingType.no, ods[index]);
                    ods[index].subobjects.Add(0x02, subod);
                }

                if ((!ods[index].Containssubindex(3)) && ((this.di.CompactPDO & 0x04) == 0))
                {
                    //Fill in inhibit

                    ODentry subod = new ODentry("Inhibit time", index, DataType.UNSIGNED16, "0", AccessType.rw, PDOMappingType.no, ods[index]);
                    ods[index].subobjects.Add(0x03, subod);
                }

                //NOT FOR RX PDO
                if (index < 0x1800)
                    return;

                if ((!ods[index].Containssubindex(4)) && ((this.di.CompactPDO & 0x08) == 0))
                {
                    //Fill in compatibility entry

                    ODentry subod = new ODentry("Compatibility entry", index, DataType.UNSIGNED8, "0", AccessType.ro, PDOMappingType.no, ods[index]);
                    ods[index].subobjects.Add(0x04, subod);
                }

                if ((!ods[index].Containssubindex(5)) && ((this.di.CompactPDO & 0x10) == 0))
                {
                    //Fill in event timer

                    ODentry subod = new ODentry("Event Timer", index, DataType.UNSIGNED16, "0", AccessType.rw, PDOMappingType.no, ods[index]);
                    ods[index].subobjects.Add(0x05, subod);
                }
            }
        }

        /// <summary>
        /// This function scans the PDO list and compares it to NrOfRXPDO and NrOfTXPDO
        /// if these do not match in count then implicit PDOs are present and they are
        /// filled in with default values from the lowest possible index
        /// </summary>
        public void ApplyimplicitPDO()
        {
            UInt16 totalnorxpdos = di.NrOfRXPDO;
            UInt16 totalnotxpdos = di.NrOfTXPDO;

            UpdatePDOcount();

            UInt16 noexplicitrxpdos = di.NrOfRXPDO;
            UInt16 noexplicittxpdos = di.NrOfTXPDO;

            //this is how many PDOS need generating on the fly
            UInt16 noimplictrxpdos = (UInt16) (totalnorxpdos - noexplicitrxpdos);
            UInt16 noimplicttxpdos = (UInt16) (totalnotxpdos - noexplicittxpdos);

            for(UInt16 index = 0x1400; (index < 0x1600) && (noimplictrxpdos > 0) ;index++)
            {
                if(!ods.ContainsKey(index))
                {
                    CreateRXPDO(index);
                    noimplictrxpdos--;
                }
            }

            for (UInt16 index = 0x1800; (index < 0x1A00) && (noimplicttxpdos > 0); index++)
            {
                if (!ods.ContainsKey(index))
                {
                    CreateTXPDO(index);
                    noimplicttxpdos--;
                }
            }

            UpdatePDOcount();

        }

        public void Savefile(string filename, InfoSection.Filetype ft)
        {
            if(ft==InfoSection.Filetype.File_EDS)
                this.edsfilename = filename;

            if (ft == InfoSection.Filetype.File_DCF)
            {
                this.dcffilename = filename;
                fi.LastEDS = edsfilename;
            }

            UpdatePDOcount();

            //generate date times in DS306 format; h:mmtt MM-dd-yyyy

            fi.CreationDate = fi.CreationDateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
            fi.CreationTime = fi.CreationDateTime.ToString("h:mmtt", CultureInfo.InvariantCulture);

            fi.ModificationDate = fi.ModificationDateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
            fi.ModificationTime = fi.ModificationDateTime.ToString("h:mmtt", CultureInfo.InvariantCulture);

            fi.FileName = Path.GetFileName(filename);

            fi.EDSVersion = "4.0";
            fi.EDSVersionMajor = 4;
            fi.EDSVersionMinor = 0;

            StreamWriter writer = File.CreateText(filename);
            fi.Write(writer,ft);
            di.Write(writer,ft);
            du.Write(writer,ft);
            c.Write(writer);

            if(ft == InfoSection.Filetype.File_DCF)
            {
                dc.Write(writer,ft);
            }

            //regenerate the object lists
            md.objectlist.Clear();
            mo.objectlist.Clear();
            oo.objectlist.Clear();

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry entry = kvp.Value;

				if (entry.Disabled == true)
					continue;

                if (entry.Index == 0x1000 || entry.Index == 0x1001 || entry.Index == 0x1018)
                {
                    md.objectlist.Add(md.objectlist.Count + 1, entry.Index);
                }
                else
               if (entry.Index >= 0x2000 && entry.Index < 0x6000)
                {
                    mo.objectlist.Add(mo.objectlist.Count + 1, entry.Index);
                }
                else
                {
                    oo.objectlist.Add(oo.objectlist.Count + 1, entry.Index);
                }
            }

            md.Write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (md.objectlist.ContainsValue(od.Index))
                {
                    od.Write(writer,ft);
                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry od2 = kvp2.Value;
                        od2.Write(writer,ft);
                    }                    
                }
            }

            oo.Write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (oo.objectlist.ContainsValue(od.Index))
                {
                    od.Write(writer,ft);
                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry od2 = kvp2.Value;
                        od2.Write(writer,ft);
                    }                    
                }
            }

            mo.Write(writer);

            foreach (KeyValuePair<UInt16, ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if (mo.objectlist.ContainsValue(od.Index))
                {
                    od.Write(writer,ft);
                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry od2 = kvp2.Value;
                        od2.Write(writer,ft);
                    }                    
                }
            }

            //modules

            if (sm.NrOfEntries > 0)
            {
                sm.Write(writer, ft);

                for (UInt16 moduleid = 1; moduleid <= sm.NrOfEntries; moduleid++)
                {

                    modules[moduleid].mi.Write(writer, ft);

                    modules[moduleid].mc.Write(writer);

                    modules[moduleid].mse.Write(writer);


                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in modules[moduleid].modulesubext)
                    {
                        ODentry od = kvp2.Value;
                        od.Write(writer, ft, ODentry.Odtype.SUBEXT, moduleid);

                    }

                    modules[moduleid].mxfo.Write(writer);

                    foreach (KeyValuePair<UInt16, ODentry> kvp3 in modules[moduleid].modulefixedobjects)
                    {
                        ODentry od = kvp3.Value;
                        od.Write(writer, ft, ODentry.Odtype.SUBEXT, moduleid);

                        foreach (KeyValuePair<UInt16, ODentry> kvp4 in od.subobjects)
                        {
                            ODentry subod = kvp4.Value;
                            subod.Write(writer, ft, ODentry.Odtype.FIXED, moduleid);
                        }
                    }
                }
            }

            if (ft == InfoSection.Filetype.File_DCF)
            {
                if (cm.NrOfEntries > 0)
                {
                    cm.Write(writer);
                }
            }

            writer.Close();

        }

        public DataType Getdatatype(ODentry od)
        {

            if (od.objecttype == ObjectType.VAR)
            {
                return od.datatype;
            }

            if (od.objecttype == ObjectType.ARRAY)
            {
                ODentry sub2 = ods[od.Index];

                //FIX ME !!! INCONSISTANT setup of the datatype for arrays when loading xml and eds!!

                DataType t = sub2.datatype;

                if (sub2.Getsubobject(1) != null)
                {
                    t = sub2.Getsubobject(1).datatype;
                    if (t == DataType.UNKNOWN)
                        t = sub2.datatype;
                }

                return t;
            }

            //Warning, REC types need to be handled else where as the specific
            //implementation of a REC type depends on the exporter being used

            return DataType.UNKNOWN;

        }


        static public byte ConvertToByte(string defaultvalue)
        {
            if (defaultvalue == null || defaultvalue == "")
                return 0;

            return (Convert.ToByte(defaultvalue, Getbase(defaultvalue)));
        }

        static public UInt16 ConvertToUInt16(byte [] bytes)
        {

            UInt16 value = 0;

            value = (UInt16) ((bytes[0] << 8) | bytes[1]);

            return value;

        }

        static public UInt16 ConvertToUInt16(string defaultvalue)
        {
            if (defaultvalue == null || defaultvalue == "" )
                return 0;

            return (Convert.ToUInt16(defaultvalue, Getbase(defaultvalue)));
        }

        static public UInt32 ConvertToUInt32(string defaultvalue)
        {
            if (defaultvalue == null || defaultvalue == "" )
                return 0;

            return (Convert.ToUInt32(defaultvalue, Getbase(defaultvalue)));
        }

        static public int Getbase(string defaultvalue)
        {

            if (defaultvalue == null || defaultvalue == "")
                return 10;

            int nobase = 10;

            String pat = @"^0[xX][0-9a-fA-F]+";

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

        public void UpdatePDOcount()
        {
            di.NrOfRXPDO = 0;
            di.NrOfTXPDO = 0;
            foreach(KeyValuePair<UInt16,ODentry> kvp in ods)
            {
                ODentry od = kvp.Value;
                if(od.Disabled==false && od.Index >= 0x1400 && od.Index < 0x1600)
                    di.NrOfRXPDO++;

                if(od.Disabled==false && od.Index >= 0x1800 && od.Index < 0x1A00)
                    di.NrOfTXPDO++;

            }

        }

        //Split on + , replace $NODEID with concrete value and add together
        public UInt32 GetNodeID(string input, out bool nodeidpresent)
        {

            if (input == null || input == "")
            {
                nodeidpresent = false;
                return 0;
            }

    		input = input.ToUpper();

            if(input.Contains("$NODEID"))     
                nodeidpresent = true;
            else
                nodeidpresent = false;

            try
            {
                if (dc.NodeId == 0)
                {
                    input = input.Replace("$NODEID", "");
                    input = input.Replace("+", "");
                    input = input.Replace(" ", "");
                    return Convert.ToUInt32(input, Getbase(input));
                }

                input = input.Replace("$NODEID", String.Format("0x{0}", dc.NodeId));

                string[] bits = input.Split('+');

                if(bits.Length==1)
                {
                    //nothing to parse here just return the value
                    return Convert.ToUInt32(input, Getbase(input));
                }

                if (bits.Length != 2)
                {
                    throw new FormatException("cannot parse " + input + "\nExpecting N+$NODEID or $NODEID+N");
                }

                UInt32 b1 = Convert.ToUInt32(bits[0], Getbase(bits[0]));
                UInt32 b2 = Convert.ToUInt32(bits[1], Getbase(bits[1]));

                return (UInt32)(b1 + b2);
            }
            catch(Exception e)
            {
                Warnings.warning_list.Add(String.Format("Error parsing node id {0} nodes, {1}", input,e.ToString()));
            }

            return 0;
        }


        public bool tryGetODEntry(UInt16 index, out ODentry od)
        {
            od = null;
            if(ods.ContainsKey(index))
            {
                od = ods[index];
                return true;
            }

            if(dummy_ods.ContainsKey(index))
            {
                od = dummy_ods[index];
                return true;
            }

            return false;
        }

        //RX COM 0x1400
        //RX Map 0x1600
        //TX COM 0x1800
        //TX MAP 0x1a00

        //call this with the comm param index not the mapping
        public bool CreatePDO(bool rx,UInt16 index)
        {
            //check if we are creating an RX PDO it is a valid index
            if (rx && (index < 0x1400 || index > 0x15ff))
                return false;

            //check if we are creating an PDO TX it is a valid index
            if (!rx & (index < 0x1800 || index > 0x19ff))
                return false;

            //Check it does not already exist
            if (ods.ContainsKey(index))
                return false;

            //check the associated mapping index does not exist
            if (ods.ContainsKey((UInt16)(index+0x200)))
                return false;

            ODentry od_comparam;
            ODentry od_mapping;

            if (rx)
            {
                od_comparam = new ODentry("RPDO communication parameter", index, 0)
                {
                    AccessFunctionName = "CO_ODF_RPDOcom",
                    Description = @"0x1400 - 0x15FF RPDO communication parameter
max sub-index

COB - ID
 bit  0 - 10: COB - ID for PDO, to change it bit 31 must be set
 bit 11 - 29: set to 0 for 11 bit COB - ID
 bit 30:    0(1) - rtr are allowed(are NOT allowed) for PDO
 bit 31:    0(1) - node uses(does NOT use) PDO

Transmission type
 value = 0 - 240:   receiving is synchronous, process after next reception of SYNC object
 value = 241 - 253: not used
 value = 254:     manufacturer specific
 value = 255:     asynchronous"
                };

                od_mapping = new ODentry("RPDO mapping parameter", (UInt16)(index + 0x200), 0)
                {
                    AccessFunctionName = "CO_ODF_RPDOmap",
                    Description = @"0x1600 - 0x17FF RPDO mapping parameter (To change mapping, 'Number of mapped objects' must be set to 0)
Number of mapped objects

mapped object  (subindex 1...8)
 bit  0 - 7:  data length in bits
 bit 8 - 15:  subindex from OD
 bit 16 - 31: index from OD"
                };


            }
            else
            {
                od_comparam = new ODentry("TPDO communication parameter", index, 0)
                {
                    AccessFunctionName = "CO_ODF_TPDOcom",
                    Description = @"0x1800 - 0x19FF TPDO communication parameter
max sub-index

COB - ID
 bit  0 - 10: COB - ID for PDO, to change it bit 31 must be set
 bit 11 - 29: set to 0 for 11 bit COB - ID
 bit 30:    0(1) - rtr are allowed(are NOT allowed) for PDO
 bit 31:    0(1) - node uses(does NOT use) PDO

Transmission type
 value = 0:       transmitting is synchronous, specification in device profile
 value = 1 - 240:   transmitting is synchronous after every N - th SYNC object
 value = 241 - 251: not used
 value = 252 - 253: Transmitted only on reception of Remote Transmission Request
 value = 254:     manufacturer specific
 value = 255:     asynchronous, specification in device profile

inhibit time
 bit 0 - 15:  Minimum time between transmissions of the PDO in 100µs.Zero disables functionality.

event timer
 bit 0-15:  Time between periodic transmissions of the PDO in ms.Zero disables functionality.

SYNC start value
 value = 0:       Counter of the SYNC message shall not be processed.
 value = 1-240:   The SYNC message with the counter value equal to this value shall be regarded as the first received SYNC message."
                };


                od_mapping = new ODentry("TPDO mapping parameter", (UInt16)(index + 0x200), 0)
                {
                    AccessFunctionName = "CO_ODF_TPDOmap",
                    Description = @"0x1A00 - 0x1BFF TPDO mapping parameter. (To change mapping, 'Number of mapped objects' must be set to 0).
Number of mapped objects

mapped object  (subindex 1...8)
 bit   0 - 7: data length in bits
 bit  8 - 15: subindex from OD
 bit 16 - 31: index from OD"
                };
            }

            od_comparam.objecttype = ObjectType.REC;
            od_comparam.StorageLocation = "ROM";
            od_comparam.accesstype = AccessType.ro;
            od_comparam.PDOtype = PDOMappingType.no;

            ODentry sub;

          
            if(rx)
            {
                sub = new ODentry("max sub-index", index, DataType.UNSIGNED8, "2", AccessType.ro, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(0, sub);
                sub = new ODentry("COB-ID used by RPDO", index, DataType.UNSIGNED32, "$NODEID+0x200", AccessType.rw, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(1, sub);
                sub = new ODentry("transmission type", index,  DataType.UNSIGNED8, "254", AccessType.rw, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(2, sub);

            }
            else
            {
                sub = new ODentry("max sub-index", index, DataType.UNSIGNED8, "6", AccessType.ro, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(0, sub);
                sub = new ODentry("COB-ID used by TPDO", index, DataType.UNSIGNED32, "$NODEID+0x180", AccessType.rw, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(1, sub);
                sub = new ODentry("transmission type", index, DataType.UNSIGNED8, "254", AccessType.rw, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(2, sub);
                sub = new ODentry("inhibit time", index, DataType.UNSIGNED16, "0", AccessType.rw, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(3, sub);
                //sub = new ODentry("compatibility entry", index, DataType.UNSIGNED8, "0", AccessType.rw, PDOMappingType.no, od_comparam);
                //od_comparam.subobjects.Add(4, sub);
                sub = new ODentry("event timer", index, DataType.UNSIGNED16, "0", AccessType.rw, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(5, sub);
                sub = new ODentry("SYNC start value", index, DataType.UNSIGNED8, "0", AccessType.rw, PDOMappingType.no, od_comparam);
                od_comparam.subobjects.Add(6, sub);

            }

            od_mapping.objecttype = ObjectType.REC;
            od_mapping.StorageLocation = "ROM";
            od_mapping.accesstype = AccessType.rw; //Same as default but inconsistent with ROM above
            od_mapping.PDOtype = PDOMappingType.no;

            sub = new ODentry("Number of mapped objects", (UInt16)(index + 0x200),  DataType.UNSIGNED8, "0", AccessType.ro, PDOMappingType.no, od_mapping);
            od_mapping.subobjects.Add(0, sub);

            for (int p=1;p<=8;p++)
            {
                sub = new ODentry(string.Format("mapped object {0}",p), (UInt16)(index+0x200), DataType.UNSIGNED32, "0x00000000", AccessType.ro, PDOMappingType.no, od_mapping);
                od_mapping.subobjects.Add((byte)p, sub);
            }

            ods.Add(index, od_comparam);
            ods.Add((UInt16)(index + 0x200), od_mapping);

            return true;
        }

        public bool CreateTXPDO(UInt16 index)
        {
            return CreatePDO(false, index);
        }

        public bool CreateRXPDO(UInt16 index)
        {
            return CreatePDO(true, index);
        }

        public ODentry Getobject(UInt16 no)
        {

            if(no>=0x002 && no<=0x007)
            {
                return dummy_ods[no];
            }

            if (ods.ContainsKey(no))
            {
                return ods[no];
            }

            return null;

        }


        public ODentry Getobject(string uniqueID)
        {
            foreach(KeyValuePair<UInt16,ODentry> e in ods)
            {
                if (e.Value.uniqueID == uniqueID)
                    return e.Value;

                if(e.Value.subobjects!=null && e.Value.subobjects.Count>0)
                {
                    foreach(KeyValuePair<UInt16, ODentry> sube in e.Value.subobjects)
                    {
                        if (sube.Value.uniqueID == uniqueID)
                            return sube.Value;
                    }

                }
                

            }

            return null;
        }

        public int GetNoEnabledObjects(bool includesub=false)
        {
            int enabledcount = 0;
            foreach (ODentry od in ods.Values)
            {
                if (od.Disabled == false)
                {
                    enabledcount++;

                    if(includesub)
                    {
                        foreach(ODentry sub in od.subobjects.Values)
                        {
                            if (od.Disabled == false)
                            {
                                enabledcount++;
                            }
                        }

                    }
                }
            }

            return enabledcount;

        }



    }

        public class ParameterException : Exception
        {
            public ParameterException(String message)
                : base(message)
            {
        
            }
        }

      

 }

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
    based heavily on the files CO_OD.h and CO_OD.c from CANopenNode which are
    Copyright(c) 2010 - 2016 Janez Paternoster
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace libEDSsharp
{
    public class CanOpenNodeExporter : IExporter
    {

        private string folderpath;
        private string gitVersion;
        protected EDSsharp eds;

        private int enabledcount = 0;

        //    Dictionary<DataType, defstruct> defstructs = new Dictionary<DataType, defstruct>();

        //Used for array tracking
        Dictionary<string, int> au = new Dictionary<string, int>();
        List<UInt16> openings = new List<UInt16>();
        List<UInt16> closings = new List<UInt16>();


        public void export(string folderpath, string filename, string gitVersion, EDSsharp eds)
        {
            this.folderpath = folderpath;
            this.gitVersion = gitVersion;
            this.eds = eds;


            enabledcount = eds.GetNoEnabledObjects();

            countPDOS();


            fixcompatentry();

            prewalkArrays();

            export_h(filename);
            export_c(filename);

        }

        private void fixcompatentry()
        {
            // Handle the TPDO communication parameters in a special way, because of
            // sizeof(OD_TPDOCommunicationParameter_t) != sizeof(CO_TPDOCommPar_t) in CANopen.c
            // the existing CO_TPDOCommPar_t has a compatibility entry so we must export one regardless
            // of if its in the OD or not

            for (UInt16 idx = 0x1800; idx < 0x1900; idx++)
            {
                if (eds.ods.ContainsKey(idx))
                {
                    ODentry od = eds.ods[idx];

                    if (!od.Containssubindex(0x04))
                    {
                        ODentry compatibility = new ODentry("compatibility entry", idx, DataType.UNSIGNED8, "0", EDSsharp.AccessType.ro, PDOMappingType.no, od);
                        od.subobjects.Add(0x04, compatibility);
                    }
                }
            }

        }

        private void specialarraysearch(UInt16 start, UInt16 end)
        {
            UInt16 lowest = 0xffff;
            UInt16 highest = 0x0000;

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {

                if (kvp.Value.Disabled == true)
                    continue;

                if (kvp.Key >= start && kvp.Key <= end)
                {
                    if (kvp.Key > highest)
                        highest = kvp.Key;

                    if (kvp.Key < lowest)
                        lowest = kvp.Key;
                }
            }

            if(lowest!=0xffff && highest!=0x0000)
            {
                openings.Add(lowest);
                closings.Add(highest);

                Console.WriteLine(string.Format("New special array detected start 0x{0:X4} end 0x{1:X4}", lowest, highest));
            }
        }

        protected void prewalkArrays()
        {

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                if (od.Disabled == true)
                    continue;

                string name = make_cname(od.parameter_name);
                if (au.ContainsKey(name))
                {
                    au[name]++;
                }
                else
                {
                    au[name] = 1;
                }
            }


            //Handle special arrays

            specialarraysearch(0x1301, 0x1340);
            specialarraysearch(0x1381, 0x13C0);


            //SDO Client parameters
            specialarraysearch(0x1200, 0x127F);
            //SDO Server Parameters
            specialarraysearch(0x1280, 0x12FF);

            //PDO Mappings and configs
            specialarraysearch(0x1400, 0x15FF);
            specialarraysearch(0x1600, 0x17FF);
            specialarraysearch(0x1800, 0x19FF);
            specialarraysearch(0x1A00, 0x1BFF);

            //now find opening and closing points for these arrays
            foreach (KeyValuePair<string, int> kvp in au)
            {
                if ( kvp.Value > 1)
                {
                    string targetname = kvp.Key;
                    UInt16 lowest=0xffff;
                    UInt16 highest=0x0000;
                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in eds.ods)
                    {

                        string name = make_cname(kvp2.Value.parameter_name);
                        if(name==targetname)
                        {
                            if (kvp2.Key > highest)
                                highest = kvp2.Key;

                            if (kvp2.Key < lowest)
                                lowest = kvp2.Key;
                        }

                    }

                    if (!openings.Contains(lowest))
                    {
                        openings.Add(lowest);
                        closings.Add(highest);
                        Console.WriteLine(string.Format("New array detected start 0x{0:X4} end 0x{1:X4}", lowest, highest));
                    }

                }

            }
        }

        string lastname = "";

        private string print_h_bylocation(string location)
        {

            StringBuilder sb = new StringBuilder();

            lastname = "";

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                if ((od.StorageLocation != location))
                {
                    if (!(od.StorageLocation == "Unused" && location == "RAM"))
                        /* this entry doesn't belong in this section */
                        continue;
                }

                sb.Append(print_h_entry(od));

            }

            return sb.ToString();
        }


        protected string print_h_entry(ODentry od)
        {
            StringBuilder sb = new StringBuilder();

            if (od.Nosubindexes == 0)
            {
                string specialarraylength = "";
                if (od.datatype == DataType.VISIBLE_STRING || od.datatype == DataType.OCTET_STRING || od.datatype == DataType.UNICODE_STRING)
                {
                    specialarraylength = string.Format("[{0}]", od.Lengthofstring);
                }

                sb.AppendLine($"/*{od.Index:X4}      */ {od.datatype.ToString(),-14} {make_cname(od.parameter_name)}{specialarraylength};");
            }
            else
            {
                //fixme why is this not od.datatype?
                DataType t = eds.Getdatatype(od);

                //If it not a defined type, and it probably is not for a REC, we must generate a name, this is
                //related to the previous code that generated the actual structures.

                string objecttypewords = "";

                switch (od.objecttype)
                {

                    case ObjectType.REC:
                        objecttypewords = String.Format("{1}OD_{0}_t", make_cname(od.parameter_name),eds.dc.NodeName);
                        break;
                    case ObjectType.ARRAY:
                        objecttypewords = t.ToString(); //this case is handled by the logic in eds.getdatatype();
                        break;
                    default:
                        objecttypewords = t.ToString();
                        break;
                }

                string name = make_cname(od.parameter_name);
                if (au[name] > 1)
                {
                    if (lastname == name)
                        return "";

                    lastname = name;
                    sb.AppendLine($"/*{od.Index:X4}      */ {objecttypewords,-15} {make_cname(od.parameter_name)}[{au[name]}];");
                }
                else
                {
                    //Don't put sub indexes on record type in h file unless there are multiples of the same
                    //in which case its not handled here, we need a special case for the predefined special
                    //values that arrayspecial() checks for, to generate 1 element arrays if needed
                    if (od.objecttype == ObjectType.REC)
                    {
                        if (arrayspecial(od.Index, true))
                        {
                            sb.AppendLine($"/*{od.Index:X4}      */ {objecttypewords,-15} {make_cname(od.parameter_name)}[1];");
                        }
                        else
                        {
                            sb.AppendLine($"/*{od.Index:X4}      */ {objecttypewords,-15} {make_cname(od.parameter_name)};");
                        }
                    }
                    else
                    {
                        string specialarraylength = "";

                        if (od.datatype == DataType.VISIBLE_STRING || od.datatype == DataType.OCTET_STRING || od.datatype == DataType.UNICODE_STRING)
                        {
                            int maxlength = 0;
                            foreach (ODentry sub in od.subobjects.Values)
                            {
                                if (sub.Lengthofstring> maxlength)
                                    maxlength = sub.Lengthofstring;
                            }

                            specialarraylength = string.Format("[{0}]", maxlength);
                        }

                        sb.AppendLine($"/*{od.Index:X4}      */ {objecttypewords,-15} {make_cname(od.parameter_name)}{specialarraylength}[{od.Nosubindexes - 1}];");
                    }
                }
            }

            return sb.ToString();
        }

        private void addHeader(StreamWriter file)
        {
            file.WriteLine(@"/*******************************************************************************

   File - CO_OD.c/CO_OD.h
   CANopen Object Dictionary.

   This file was automatically generated with libedssharp Object");

            file.Write("   Dictionary Editor v" + this.gitVersion);

            file.WriteLine(@"   DON'T EDIT THIS FILE MANUALLY !!!!
*******************************************************************************/

");

        }

        private void export_h(string filename)
        {
            if (filename == "")
                filename = "CO_OD";

            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + filename + ".h");

            file.WriteLine("// clang-format off");
            addHeader(file);

            file.WriteLine("#ifndef CO_OD_"+filename+"_H_");
            file.WriteLine("#define CO_OD_"+filename+"_H_");
            file.WriteLine("");
            file.WriteLine("#include \"CO_consts.h\"");
            file.WriteLine("");
            file.WriteLine(@"/*******************************************************************************
   CANopen DATA TYPES
*******************************************************************************/
   typedef bool_t       BOOLEAN;
   typedef uint8_t      UNSIGNED8;
   typedef uint16_t     UNSIGNED16;
   typedef uint32_t     UNSIGNED32;
   typedef uint64_t     UNSIGNED64;
   typedef int8_t       INTEGER8;
   typedef int16_t      INTEGER16;
   typedef int32_t      INTEGER32;
   typedef int64_t      INTEGER64;
   typedef float32_t    REAL32;
   typedef float64_t    REAL64;
   typedef char_t       VISIBLE_STRING;
   typedef oChar_t      OCTET_STRING;

   #ifdef DOMAIN
   #undef DOMAIN
   #endif

   typedef domain_t     DOMAIN;
/*******************************************************************************
   Defines for controlling the destination of ram/rom/eprom SECTIONS.
*******************************************************************************/
#ifndef CO_PREFIX_ROM
	#define CO_PREFIX_ROM
#endif
#ifndef CO_PREFIX_RAM
	#define CO_PREFIX_RAM
#endif
#ifndef CO_PREFIX_EEPROM
	#define CO_PREFIX_EEPROM
#endif
");

            file.WriteLine(string.Format("extern const CO_consts_t {0}CO_Consts;", eds.dc.NodeName));
            file.WriteLine("/*******************************************************************************");
            file.WriteLine("   FILE INFO:");
            file.WriteLine(string.Format("      FileName:     {0}", eds.fi.FileName));
            file.WriteLine(string.Format("      FileVersion:  {0}", eds.fi.FileVersion));
            file.WriteLine(string.Format("      CreationTime: {0}", eds.fi.CreationTime));
            file.WriteLine(string.Format("      CreationDate: {0}", eds.fi.CreationDate));
            file.WriteLine(string.Format("      CreatedBy:    {0}", eds.fi.CreatedBy));
            file.WriteLine("*******************************************************************************/");
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine("/*******************************************************************************");
            file.WriteLine("   DEVICE INFO:");
            file.WriteLine(string.Format("      VendorName:     {0}", eds.di.VendorName));
            file.WriteLine(string.Format("      VendorNumber:   {0}", eds.di.VendorNumber));
            file.WriteLine(string.Format("      ProductName:    {0}", eds.di.ProductName));
            file.WriteLine(string.Format("      ProductNumber:  {0}", eds.di.ProductNumber));
            file.WriteLine("*******************************************************************************/");
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine(@"/*******************************************************************************
   FEATURES
*******************************************************************************/");

            file.WriteLine(string.Format("  #define {0}CO_NO_SYNC                     {1}   //Associated objects: 1005-1007", eds.dc.NodeName,noSYNC));

            file.WriteLine(string.Format("  #define {0}CO_NO_EMERGENCY                {1}   //Associated objects: 1014, 1015", eds.dc.NodeName,noEMCY));

            file.WriteLine(string.Format("  #define {0}CO_NO_TIME                     {1}   //Associated objects: 1012, 1013", eds.dc.NodeName,noTIME));

            file.WriteLine(string.Format("  #define {0}CO_NO_SDO_SERVER               {1}   //Associated objects: 1200-127F", eds.dc.NodeName,noSDOservers));
            file.WriteLine(string.Format("  #define {0}CO_NO_SDO_CLIENT               {1}   //Associated objects: 1280-12FF", eds.dc.NodeName,noSDOclients));

            int lssServer = 0;
            if (eds.di.LSS_Supported == true && eds.di.LSS_Type == "Server")
            {
                lssServer = 1;
            }
            file.WriteLine(string.Format("  #define {0}CO_NO_LSS_SERVER               {1}   //LSS Slave", eds.dc.NodeName,lssServer));
            int lssClient = 0;
            if (eds.di.LSS_Supported == true && eds.di.LSS_Type == "Client")
            {
                lssClient = 1;
            }
            file.WriteLine(string.Format("  #define {0}CO_NO_LSS_CLIENT               {1}   //LSS Master", eds.dc.NodeName,lssClient));

            file.WriteLine(string.Format("  #define {0}CO_NO_RPDO                     {1}   //Associated objects: 14xx, 16xx", eds.dc.NodeName,noRXpdos));
            file.WriteLine(string.Format("  #define {0}CO_NO_TPDO                     {1}   //Associated objects: 18xx, 1Axx", eds.dc.NodeName,noTXpdos));
            file.WriteLine(string.Format("  #define {0}CO_NO_SRDO                     {1}   //Associated objects: 1301 - 1340", eds.dc.NodeName,noSRDO));
            file.WriteLine(string.Format("  #define {0}CO_NO_GFC                      {1}   //Associated objects: 1300", eds.dc.NodeName,noGFC));

            bool ismaster = false;
            if(eds.ods.ContainsKey(0x1f80))
            {
                ODentry master = eds.ods[0x1f80];

                // we could do with a cut down function that returns a value rather than a string
                string meh = formatvaluewithdatatype(master.defaultvalue, master.datatype);
                meh = meh.Replace("L", "");

                UInt32 NMTStartup = Convert.ToUInt32(meh, 16);
                if ((NMTStartup & 0x01) == 0x01)
                    ismaster = true;
            }

            file.WriteLine(string.Format("  #define {1}CO_NO_NMT_MASTER               {0}", ismaster==true?1:0, eds.dc.NodeName));
            file.WriteLine(string.Format("  #define {0}CO_NO_TRACE                    0", eds.dc.NodeName));
            file.WriteLine("");
            file.WriteLine("");
            file.WriteLine(@"/*******************************************************************************
   OBJECT DICTIONARY
*******************************************************************************/");

            file.WriteLine(string.Format("   #define {0}CO_OD_NoOfElements             {1}", eds.dc.NodeName, enabledcount));
			file.WriteLine("extern const CO_OD_entry_t " + eds.dc.NodeName + "CO_OD["+eds.dc.NodeName+"CO_OD_NoOfElements];");
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine(@"/*******************************************************************************
   TYPE DEFINITIONS FOR RECORDS
*******************************************************************************/");

            //We need to identify all the record types used and generate a struct for each one
            //FIXME the original CANopenNode exporter said how many items used this struct in the comments

            List<string> structnamelist = new List<string>();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.objecttype != ObjectType.REC)
                    continue;

                string structname = String.Format("{1}OD_{0}_t", make_cname(od.parameter_name), eds.dc.NodeName);

                if (structnamelist.Contains(structname))
                    continue;

                structnamelist.Add(structname);

                file.WriteLine(string.Format("/*{0:X4}      */ typedef struct {{", kvp.Key));
                foreach (KeyValuePair<UInt16, ODentry> kvp2 in kvp.Value.subobjects)
                {
                    string paramaterarrlen = "";

                    ODentry subod = kvp2.Value;

                    if(subod.datatype==DataType.VISIBLE_STRING || subod.datatype==DataType.OCTET_STRING)
                    {
                        paramaterarrlen = String.Format("[{0}]", subod.Lengthofstring);
                    }

                    file.WriteLine(string.Format("               {0,-15}{1}{2};", subod.datatype.ToString(), make_cname(subod.parameter_name),paramaterarrlen));

                }

                file.WriteLine(string.Format("               }}              {0};", structname));

            }



            file.WriteLine(@"
/*******************************************************************************
   TYPE DEFINITIONS FOR OBJECT DICTIONARY INDEXES

   some of those are redundant with CO_SDO.h CO_ObjDicId_t <Common CiA301 object
   dictionary entries>
*******************************************************************************/");

            //FIXME how can we get rid of that redundancy?

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {

                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                DataType t = eds.Getdatatype(od);


                switch (od.objecttype)
                {
                default:
                    {
                        file.WriteLine(string.Format("/*{0:X4} */", od.Index));
                        file.WriteLine(string.Format("        #define {0,-51} 0x{1:X4}", string.Format("OD_{0:X4}_{1}", od.Index, make_cname(od.parameter_name)), od.Index, t.ToString()));

                        file.WriteLine("");
                    }
                    break;

                case ObjectType.ARRAY:
                case ObjectType.REC:
                    {
                        file.WriteLine(string.Format("/*{0:X4} */", od.Index));
                        file.WriteLine(string.Format("        #define {0,-51} 0x{1:X4}", string.Format("OD_{0:X4}_{1}", od.Index, make_cname(od.parameter_name)), od.Index, t.ToString()));

                        file.WriteLine("");

                        //sub indexes
                        file.WriteLine(string.Format("        #define {0,-51} 0", string.Format("OD_{0:X4}_0_{1}_maxSubIndex", od.Index, make_cname(od.parameter_name))));

                        List<string> ODSIs = new List<string>();

                        string ODSIout = "";

                        foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                        {
                            ODentry sub = kvp2.Value;

                            if (kvp2.Key == 0)
                                continue;

                            string ODSI = string.Format("{0}", string.Format("OD_{0:X4}_{1}_{2}_{3}", od.Index, kvp2.Key, make_cname(od.parameter_name), make_cname(sub.parameter_name)));

                            if (ODSIs.Contains(ODSI))
                            {
                                continue;
                            }

                            ODSIs.Add(ODSI);

                            ODSIout += ($"        #define {ODSI,-51} {kvp2.Key}{Environment.NewLine}");
                        }

                        file.Write(ODSIout);
                        file.WriteLine("");
                    }
                    break;
                }
            }

            file.WriteLine(@"/*******************************************************************************
   STRUCTURES FOR VARIABLES IN DIFFERENT MEMORY LOCATIONS
*******************************************************************************/
#define  CO_OD_FIRST_LAST_WORD     0x55 //Any value from 0x01 to 0xFE. If changed, EEPROM will be reinitialized.
");
            foreach (string location in eds.storageLocation)
            {
                if (location == "Unused")
                {
                    continue;
                }

                file.Write("/***** Structure for ");
                file.Write(location);
                file.WriteLine(" variables ********************************************/");
                file.Write("struct s{0}CO_OD_",eds.dc.NodeName);
                file.Write(location);
                file.Write(@"{
               UNSIGNED32     FirstWord;

");

                file.Write(print_h_bylocation(location));

                file.WriteLine(@"
               UNSIGNED32     LastWord;
};
");
            }

            file.WriteLine(@"/***** Declaration of Object Dictionary variables *****************************/");

            foreach (string location in eds.storageLocation)
            {
                if (location == "Unused")
                {
                    continue;
                }

                file.Write("extern CO_PREFIX_{1} struct s{0}CO_OD_",eds.dc.NodeName,location);
                file.Write(location);
                file.Write(" {0}CO_OD_",eds.dc.NodeName);
                file.Write(location);
                file.WriteLine(@";
");
            }

file.WriteLine(@"/*******************************************************************************
   ALIASES FOR OBJECT DICTIONARY VARIABLES
*******************************************************************************/");

            List<string> constructed_rec_types = new List<string>();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {


                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                string loc = eds.dc.NodeName + "CO_OD_" + od.StorageLocation;
                DataType t = eds.Getdatatype(od);


                switch (od.objecttype)
                {
                    default:
                        {
                            file.WriteLine(string.Format("/*{0:X4}, Data Type: {1} */", od.Index, t.ToString()));
                            file.WriteLine(string.Format("        #define {3}{0,-51} {1}.{2}", string.Format("OD_{0}", make_cname(od.parameter_name)), loc, make_cname(od.parameter_name),eds.dc.NodeName));

                            DataType dt = od.datatype;

                            if (dt == DataType.OCTET_STRING || dt == DataType.VISIBLE_STRING)
                            {
                                file.WriteLine(string.Format("        #define {2}{0,-51} {1}", string.Format("ODL_{0}_stringLength", make_cname(od.parameter_name)), od.Lengthofstring,eds.dc.NodeName,eds.dc.NodeName));
                            }
                            file.WriteLine("");
                        }
                        break;

                    case ObjectType.ARRAY:
                        {
                            DataType dt = od.datatype;

                            file.WriteLine(string.Format("/*{0:X4}, Data Type: {1}, Array[{2}] */", od.Index, t.ToString(), od.Nosubindexes - 1));
                            file.WriteLine(string.Format("        #define {3}OD_{0,-48} {1}.{2}", make_cname(od.parameter_name), loc, make_cname(od.parameter_name),eds.dc.NodeName));
                            file.WriteLine(string.Format("        #define {0,-51} {1}", string.Format("{1}ODL_{0}_arrayLength", make_cname(od.parameter_name),eds.dc.NodeName), od.Nosubindexes - 1,eds.dc.NodeName));

                            List<string> ODAs = new List<string>();

                            string ODAout = "";

                            foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                            {
                                ODentry sub = kvp2.Value;

                                if (kvp2.Key == 0)
                                    continue;

                                string ODA = string.Format("{1}{0}", string.Format("ODA_{0}_{1}", make_cname(od.parameter_name), make_cname(sub.parameter_name)),eds.dc.NodeName);

                                if (ODAs.Contains(ODA))
                                {
                                    continue;
                                }

                                ODAs.Add(ODA);

                                //Arrays do not have a size in the raw CO objects, Records do
                                //so offset by one
                                if (od.objecttype == ObjectType.ARRAY)
                                {
                                    ODAout += ($"        #define {string.Format("{2}ODA_{0}_{1}", make_cname(od.parameter_name), make_cname(sub.parameter_name),eds.dc.NodeName),-51} {kvp2.Key - 1}{Environment.NewLine}");
                                }
                                else
                                {
                                    ODAout += ($"        #define {string.Format("{2}ODA_{0}_{1}", make_cname(od.parameter_name), make_cname(sub.parameter_name),eds.dc.NodeName),-51} {kvp2.Key}{Environment.NewLine}");
                                }
                            }

                            file.Write(ODAout);
                            file.WriteLine("");
                        }
                        break;

                    case ObjectType.REC:
                        {
                            string rectype = make_cname(od.parameter_name);

                            if (!constructed_rec_types.Contains(rectype))
                            {
                                file.WriteLine(string.Format("/*{0:X4}, Data Type: {1}_t */", od.Index, rectype));
                                file.WriteLine(string.Format("        #define {3}{0,-51} {1}.{2}", string.Format("OD_{0}", rectype), loc, rectype,eds.dc.NodeName));
                                constructed_rec_types.Add(rectype);
                                file.WriteLine("");
                            }

                        }
                        break;
                }
            }
            file.WriteLine("#endif");
            file.WriteLine("// clang-format on");
            file.Close();

        }

        private void export_c(string filename)
        {
            if (filename == "")
                filename =  "CO_OD";
            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + filename + ".c");

            file.WriteLine("// clang-format off");
            addHeader(file);
            file.WriteLine(@"#include ""301/CO_driver.h""
#include ""301/CO_SDOserver.h""
#include """  +  filename + @".h""

/*******************************************************************************
   DEFINITION AND INITIALIZATION OF OBJECT DICTIONARY VARIABLES
*******************************************************************************/

");
			file.WriteLine("const CO_consts_t "+ eds.dc.NodeName + "CO_Consts = {");
            file.WriteLine(string.Format("  .NO_SYNC = {0}CO_NO_SYNC,              //Associated objects: 1005-1007", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_EMERGENCY = {0}CO_NO_EMERGENCY,         //Associated objects: 1014, 1015", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_TIME = {0}CO_NO_TIME,              //Associated objects: 1012, 1013", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_SDO_SERVER = {0}CO_NO_SDO_SERVER,        //Associated objects: 1200-127F", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_SDO_CLIENT = {0}CO_NO_SDO_CLIENT,        //Associated objects: 1280-12FF", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_LSS_SERVER = {0}CO_NO_LSS_SERVER,        //LSS Slave", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_LSS_CLIENT = {0}CO_NO_LSS_CLIENT,        //LSS Master", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_RPDO = {0}CO_NO_RPDO,              //Associated objects: 14xx, 16xx", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_TPDO = {0}CO_NO_TPDO,              //Associated objects: 18xx, 1Axx", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_SRDO = {0}CO_NO_SRDO,              //Associated objects: 1301 - 1340", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_TPDO = {0}CO_NO_GFC,               //Associated objects: 1300", eds.dc.NodeName));
            file.WriteLine(string.Format("  .NO_NMT_MASTER = {0}CO_NO_NMT_MASTER,              //Associated objects: 18xx, 1Axx", eds.dc.NodeName));
//            file.WriteLine(string.Format("  .NO_TRACE = {0}CO_NO_TRACE,              //Associated objects: 18xx, 1Axx", eds.dc.NodeName));
			if (eds.ods.ContainsKey(0x1016)) { //Errorregister
            	file.WriteLine(string.Format("  .consumerHeartbeatTime_arrayLength = {0}ODL_consumerHeartbeatTime_arrayLength,              //Associated objects: 18xx, 1Axx", eds.dc.NodeName));
			} else {
            	file.WriteLine(string.Format("  .consumerHeartbeatTime_arrayLength = 0,              //Associated objects: 18xx, 1Axx", eds.dc.NodeName));
			}
            file.WriteLine(string.Format("  .OD_NoOfElements = {0}CO_OD_NoOfElements,", eds.dc.NodeName));
			if (eds.ods.ContainsKey(0x1800))
            {
				file.WriteLine(string.Format("  .sizeof_OD_TPDOCommunicationParameter = sizeof({0}OD_TPDOCommunicationParameter_t),", eds.dc.NodeName));
            	file.WriteLine(string.Format("  .sizeof_OD_TPDOMappingParameter = sizeof({0}OD_TPDOMappingParameter_t),", eds.dc.NodeName));
			} else {
				file.WriteLine(string.Format("  .sizeof_OD_TPDOCommunicationParameter = 0,", eds.dc.NodeName));
            	file.WriteLine(string.Format("  .sizeof_OD_TPDOMappingParameter = 0,", eds.dc.NodeName));
			}
            file.WriteLine(string.Format("  .sizeof_OD_RPDOCommunicationParameter = sizeof({0}OD_RPDOCommunicationParameter_t),", eds.dc.NodeName));
            file.WriteLine(string.Format("  .sizeof_OD_RPDOMappingParameter = sizeof({0}OD_RPDOMappingParameter_t),", eds.dc.NodeName));
			if (eds.ods.ContainsKey(0x1001)) { //Errorregister
	            file.WriteLine(string.Format("  .errorRegister = &{0}OD_errorRegister,", eds.dc.NodeName));
			} else  {
	            file.WriteLine(string.Format("  .errorRegister = NULL,", eds.dc.NodeName));
			}
			if (eds.ods.ContainsKey(0x1003)) { //Errorregister
				file.WriteLine(string.Format("  .preDefinedErrorField = &{0}OD_preDefinedErrorField[0],", eds.dc.NodeName));
				file.WriteLine(string.Format("  .preDefinedErrorFieldSize = {0}ODL_preDefinedErrorField_arrayLength", eds.dc.NodeName));
			} else {
				file.WriteLine(string.Format("  .preDefinedErrorField = NULL,", eds.dc.NodeName));
				file.WriteLine(string.Format("  .preDefinedErrorFieldSize = 0", eds.dc.NodeName));
			}
			file.WriteLine("};");
			file.WriteLine("");

            foreach (string location in eds.storageLocation)
            {
                if (location == "Unused")
                {
                    continue;
                }

                file.Write("/***** Definition for ");
                file.Write(location);
                file.WriteLine(" variables *******************************************/");
                file.Write("CO_PREFIX_{1} struct s{0}CO_OD_",eds.dc.NodeName,location);
                file.Write(location);
                file.Write(" {0}CO_OD_",eds.dc.NodeName);
                file.Write(location);
                file.Write(@" = {
           CO_OD_FIRST_LAST_WORD,

");

                file.Write(export_OD_def_array(location));

                file.WriteLine(@"
           CO_OD_FIRST_LAST_WORD,
};

");
            }


            file.WriteLine(@"

/*******************************************************************************
   STRUCTURES FOR RECORD TYPE OBJECTS
*******************************************************************************/

");

            file.Write(export_record_types());

            file.WriteLine(@"/*******************************************************************************
   OBJECT DICTIONARY
*******************************************************************************/");
			file.WriteLine("const CO_OD_entry_t " + eds.dc.NodeName + "CO_OD[" + eds.dc.NodeName + "CO_OD_NoOfElements] = {");

            file.Write(write_od());

            file.WriteLine("};");
            file.WriteLine("// clang-format on");

            file.Close();
        }

        bool arrayspecialcase = false;
        int arrayspecialcasecount = 0;

        string write_od()
        {

            StringBuilder returndata = new StringBuilder();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {

                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                returndata.Append(write_od_line(od));


            }

            return returndata.ToString();
        }

        protected string write_od_line(ODentry od)
        {
            StringBuilder sb = new StringBuilder();

            string loc = eds.dc.NodeName+"CO_OD_" + od.StorageLocation;

            byte flags = getflags(od);

            DataType t = eds.Getdatatype(od);
            int datasize = (int)Math.Ceiling((double)od.Sizeofdatatype() / (double)8.0);

            string odf;

            if (od.AccessFunctionName != null)
            {
                odf = od.AccessFunctionName;
            }
            else
            {
                odf = "CO_ODF";
            }

            string array = "";

            //only needed for array objects
            if (od.objecttype == ObjectType.ARRAY && od.Nosubindexes > 0)
                array = string.Format("[0]");


            if (arrayspecial(od.Index, true))
            {
                arrayspecialcase = true;
                arrayspecialcasecount = 0;
            }

            if (arrayspecialcase)
            {
                array = string.Format("[{0}]", arrayspecialcasecount);
                arrayspecialcasecount++;
            }

            //Arrays and Recs have 1 less subindex than actually present in the od.subobjects
            int nosubindexs = od.Nosubindexes;
            if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
            {
                if (nosubindexs > 0)
                    nosubindexs--;
            }

            //Arrays really should obey the max subindex parameter not the physical number of elements
            if (od.objecttype == ObjectType.ARRAY)
            {
                if ((od.Getmaxsubindex() != nosubindexs))
                {
                    if (od.Index != 0x1003 && od.Index != 0x1011)//ignore 0x1003, it is a special case as per CANopen specs, and ignore 0x1011 CANopenNode uses special sub indexes for eeprom resets
                    {
                        Warnings.warning_list.Add(String.Format("Subindex discrepancy on object 0x{0:X4} arraysize: {1} vs max sub-index: {2}", od.Index, nosubindexs, od.Getmaxsubindex()));
                    }

                    //0x1003 is a special case for CANopenNode
                    //SubIndex 0 will probably be 0 for no errors
                    //so we cannot read that to determine max subindex size, which is required to set up CANopenNode so we leave it alone here
                    //as its already set to subod.count
                    if (od.Index != 0x1003)
                    {
                        nosubindexs = od.Getmaxsubindex();
                    }
                }
            }

            string pdata; //CO_OD_entry_t pData generator

            if (od.objecttype == ObjectType.REC)
            {

                pdata = string.Format("&{1}OD_record{0:X4}", od.Index,eds.dc.NodeName);
            }
            else
            {
                pdata = string.Format("&{0}.{1}{2}", loc, make_cname(od.parameter_name), array);
            }

            if ((od.objecttype == ObjectType.VAR || od.objecttype == ObjectType.ARRAY) && od.datatype == DataType.DOMAIN)
            {
                //NB domain MUST have a data pointer of 0, can open node requires this and makes checks
                //against null to determine this is a DOMAIN type.
                pdata = "0";
            }

            sb.AppendLine($"{{0x{od.Index:X4}, 0x{nosubindexs:X2}, 0x{flags:X2}, {datasize,2:#0}, (void*){pdata}}},");

            if (arrayspecial(od.Index, false))
            {
                arrayspecialcase = false;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get the CANopenNode specific flags, these flags are used internally in CANopenNode to determine details about the object variable
        /// </summary>
        /// <param name="od">An odentry to access</param>
        /// <returns>byte containing the flag value</returns>
        public byte getflags(ODentry od)
        {
            byte flags = 0;
            byte mapping = 0; //mapping flags, if pdo is enabled

            //aways return 0 for REC objects as CO_OD_getDataPointer() uses this to pickup the details
            if (od.objecttype == ObjectType.REC)
                return 0;

            switch(od.StorageLocation.ToUpper())
            {
                case "ROM":
                    flags = 0x01;
                    break;

                case "RAM":
                    flags = 0x02;
                    break;

                case "EEPROM":
                default:
                    flags = 0x03;
                    break;
            }

            /*
            flags = (byte)eds.storageLocation.IndexOf(od.StorageLocation);
            //1 = ROM, 2 = RAM, >= 3 some EEPROM region
            if (flags > 0x03)
            {
                flags = 0x03;
            }
            */

            /* some exceptions for rwr/rww. Those are entries that are always r/w via SDO transfer,
             * but can only be read -or- written via PDO */
            if (od.accesstype == EDSsharp.AccessType.ro
                || od.accesstype == EDSsharp.AccessType.rw
                || od.accesstype == EDSsharp.AccessType.rwr
                || od.accesstype == EDSsharp.AccessType.rww
                || od.accesstype == EDSsharp.AccessType.@const)
            {
                /* SDO server may read from the variable */
                flags |= 0x04;

                if (od.accesstype != EDSsharp.AccessType.rww)
                {
                    /* Variable is mappable for TPDO  */
                    mapping |= 0x20;
                }
            }
            if (od.accesstype == EDSsharp.AccessType.wo
                || od.accesstype == EDSsharp.AccessType.rw
                || od.accesstype == EDSsharp.AccessType.rwr
                || od.accesstype == EDSsharp.AccessType.rww)
            {
                /* SDO server may write to the variable */
                flags |= 0x08;

                if (od.accesstype != EDSsharp.AccessType.rwr)
                {
                    /* Variable is mappable for RPDO */
                    mapping |= 0x10;
                }
            }

            if (od.PDOMapping)
            {
                flags |= mapping;
            }

            if(od.TPDODetectCos)
            {
              /* If variable is mapped to any PDO, then  is automatically send, if variable its value */
              flags |=0x40;
            }

            int datasize = (int)Math.Ceiling((double)od.Sizeofdatatype() / (double)8.0);

            if (datasize > 1)
            {
                if (od.datatype == DataType.VISIBLE_STRING ||
                    od.datatype == DataType.OCTET_STRING)
                {
                    //#149 VISIBLE_STRING and OCTET_STRING are an arrays of 8 bit values, either VISIBLE_CHAR or UNSIGNED8
                    //and therefor are NOT multi-byte
                }
                else
                {
                    /* variable is a multi-byte value */
                    flags |= 0x80;
                }
            }

            return flags;
        }

        string formatvaluewithdatatype(string defaultvalue, DataType dt)
        {
            try
            {
            int nobase = 10;
            bool nodeidreplace = false;

            if (defaultvalue == null || defaultvalue == "")
            {
                //No default value, we better supply one for sensible data types
                if (dt == DataType.VISIBLE_STRING ||
                    dt == DataType.OCTET_STRING ||
                    dt == DataType.UNKNOWN ||
                    dt == DataType.UNICODE_STRING)
                {
                    return "";
                }

                Console.WriteLine("Warning assuming a 0 default");
                defaultvalue = "0";
            }

            if (defaultvalue.Contains("$NODEID"))
            {
                defaultvalue = defaultvalue.Replace("$NODEID", "");
                defaultvalue = defaultvalue.Replace("+", "");
                nodeidreplace = true;
            }

            String pat = @"^0[xX][0-9a-fA-FL]+";

            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            Match m = r.Match(defaultvalue);
            if (m.Success)
            {
                nobase = 16;
                defaultvalue = defaultvalue.Replace("L", "");
            }

            pat = @"^0[0-7]+";
            r = new Regex(pat, RegexOptions.IgnoreCase);
            m = r.Match(defaultvalue);
            if (m.Success)
            {
                nobase = 8;
            }

            if (nodeidreplace)
            {
                UInt32 data = Convert.ToUInt32(defaultvalue, nobase);
                data += eds.NodeId;
                defaultvalue = string.Format("0x{0:X}", data);
                nobase = 16;
            }


            switch (dt)
            {
                case DataType.UNSIGNED24:
                case DataType.UNSIGNED32:
                    return String.Format("0x{0:X4}L", Convert.ToUInt32(defaultvalue, nobase));

                case DataType.INTEGER24:
                case DataType.INTEGER32:
                    return String.Format("0x{0:X4}L", Convert.ToInt32(defaultvalue, nobase));

                case DataType.REAL32:
                case DataType.REAL64:
                    return (String.Format("{0}", defaultvalue));


                //fix me this looks wrong
                case DataType.UNICODE_STRING:
                    return (String.Format("'{0}'", defaultvalue));

                case DataType.VISIBLE_STRING:
                    {

                        ASCIIEncoding a = new ASCIIEncoding();
                        string unescape = StringUnescape.Unescape(defaultvalue);
                        char[] chars = unescape.ToCharArray();

                        string array = "{";

                        foreach (char c in chars)
                        {

                            array += "'" + StringUnescape.Escape(c) + "', ";
                        }

                        array = array.Substring(0, array.Length - 2);

                        array += "}";
                        return array;

                    }


                case DataType.OCTET_STRING:
                    {
                        string[] bits = defaultvalue.Split(' ');
                        string octet = "{";
                        foreach (string s in bits)
                        {
                            octet += formatvaluewithdatatype(s, DataType.UNSIGNED8);

                            if (!object.ReferenceEquals(s, bits.Last()))
                            {
                                octet += ", ";
                            }
                        }
                        octet += "}";
                        return octet;
                    }

                case DataType.INTEGER8:
                    return String.Format("0x{0:X1}", Convert.ToSByte(defaultvalue, nobase));

                case DataType.INTEGER16:
                    return String.Format("0x{0:X2}", Convert.ToInt16(defaultvalue, nobase));

                case DataType.UNSIGNED8:
                    return String.Format("0x{0:X1}L", Convert.ToByte(defaultvalue, nobase));

                case DataType.UNSIGNED16:
                    return String.Format("0x{0:X2}", Convert.ToUInt16(defaultvalue, nobase));

                case DataType.INTEGER64:
                    return String.Format("0x{0:X8}L", Convert.ToInt64(defaultvalue, nobase));

                case DataType.UNSIGNED64:
                    return String.Format("0x{0:X8}L", Convert.ToUInt64(defaultvalue, nobase));

                case DataType.TIME_DIFFERENCE:
                case DataType.TIME_OF_DAY:
                    return String.Format("{{{0}}}", Convert.ToUInt64(defaultvalue, nobase));

                default:
                    return (String.Format("{0:X}", defaultvalue));

            }
        }
            catch(Exception e)
            {
                Warnings.warning_list.Add(String.Format("Error converting value {0} to type {1}", defaultvalue, dt.ToString()));
                return "";
            }
        }

        public static string ParseString(string input)
        {
            var provider = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new System.CodeDom.Compiler.CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };

            var code = @"
        namespace Tmp
        {
            public class TmpClass
            {
                public static string GetValue()
                {
                    return """ + input + @""";
                }
            }
        }";

            var compileResult = provider.CompileAssemblyFromSource(parameters, code);

            if (compileResult.Errors.HasErrors)
            {
                throw new ArgumentException(compileResult.Errors.Cast<System.CodeDom.Compiler.CompilerError>().First(e => !e.IsWarning).ErrorText);
            }

            var asmb = compileResult.CompiledAssembly;
            var method = asmb.GetType("Tmp.TmpClass").GetMethod("GetValue");

            return method.Invoke(null, null) as string;
        }

       protected string make_cname(string name)
       {
            if (name == null)
                return null;

            if (name == "")
                return "";

           Regex splitter = new Regex(@"[\W]+");

           //string[] bits = Regex.Split(name,@"[\W]+");
           var bits = splitter.Split(name).Where(s => s != String.Empty);

           string output = "";

           char lastchar = ' ';
           foreach (string s in bits)
           {
               if(Char.IsUpper(lastchar) && Char.IsUpper(s.First()))
                    output+="_";

                if (s.Length > 1)
                {
                    output += char.ToUpper(s[0]) + s.Substring(1);
                }
                else
                {
                    output += s;
                }

                if(output.Length>0)
                    lastchar = output.Last();

           }

            if (output.Length > 1)
            {
                if (Char.IsLower(output[1]))
                    output = Char.ToLower(output[0]) + output.Substring(1);
            }
            else
                output = output.ToLower(); //single character

            return output;
        }

        /// <summary>
        /// Export the record type objects in the CO_OD.c file
        /// </summary>
        /// <returns>string</returns>
        protected string export_record_types()
        {
            StringBuilder returndata = new StringBuilder();

            bool arrayopen = false;
            int arrayindex = 0;

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.objecttype != ObjectType.REC)
                    continue;

                if (od.Disabled == true)
                    continue;

                int count = od.subobjects.Count; //don't include index

                if(od.Index>=0x1400 && od.Index<0x1600)
                {
                    count = 3; //CANopenNode Fudging. Its only 3 parameters for RX PDOS in the c code despite being a PDO_COMMUNICATION_PARAMETER
                }

                returndata.AppendLine($"/*0x{od.Index:X4}*/ const CO_OD_entryRecord_t {eds.dc.NodeName}OD_record{od.Index:X4}[{count}] = {{");

                string arrayaccess = "";

                if (arrayspecial(od.Index, true) || arrayopen)
                {
                    arrayaccess = string.Format("[{0}]",arrayindex);
                    arrayindex++;
                    arrayopen = true;
                }

                foreach (KeyValuePair<UInt16, ODentry> kvpsub in od.subobjects)
                {
                    returndata.Append(export_one_record_type(kvpsub.Value,arrayaccess));
                }

                if (arrayspecial(od.Index, false))
                {
                    arrayindex=0;
                    arrayopen = false;
                }

                returndata.AppendLine($"}};{Environment.NewLine}");
            }

            return returndata.ToString();
        }

        /// <summary>
        /// Exports a sub object line in a record object
        /// </summary>
        /// <param name="sub">sub ODentry object to export</param>
        /// <param name="arrayaccess">string forming current array level or empty string for none</param>
        /// <returns>string forming one line of CO_OD.c record objects</returns>
        protected string export_one_record_type(ODentry sub,string arrayaccess)
        {

            if (sub == null || sub.parent == null)
                return "";

            StringBuilder sb = new StringBuilder();

            string cname = make_cname(sub.parent.parameter_name);

            string subcname = make_cname(sub.parameter_name);
            int datasize = (int)Math.Ceiling((double)sub.Sizeofdatatype() / (double)8.0);

            if (sub.datatype != DataType.DOMAIN)
            {
                sb.AppendLine($"           {{(void*)&{eds.dc.NodeName+"CO_OD_" + sub.parent.StorageLocation}.{cname}{arrayaccess}.{subcname}, 0x{getflags(sub):X2}, 0x{datasize:X} }},");
            }
            else
            {
                //Domain type MUST have its data pointer set to 0 for CANopenNode
                sb.AppendLine($"           {{(void*)0, 0x{getflags(sub):X2}, 0x{datasize:X} }},");
            }

            return sb.ToString();
        }


        int noTXpdos = 0;
        int noRXpdos = 0;
        int noSDOclients = 0;
        int noSDOservers = 0;
        int distTXpdo = 0;
        int distRXpdo = 0;
        int noSYNC = 0;
        int noEMCY = 0;
        int noTIME = 0;
        int noGFC = 0;
        int noSRDO = 0;

        void countPDOS()
        {
            noRXpdos = 0;
            noTXpdos = 0;

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                UInt16 index = kvp.Key;

                if (kvp.Value.Disabled == true)
                    continue;

                if (index >= 0x1400 && index < 0x1600)
                {
                    noRXpdos++;
                    distRXpdo = index - 0x1400;
                }

                if (index >= 0x1800 && index < 0x1A00)
                {
                    noTXpdos++;
                    distTXpdo = index - 0x1800;
                }

                if((index & 0xFF80) == 0x1200)
                {
                    noSDOservers++;
                }

                if ((index & 0xFF80) == 0x1280)
                {
                    noSDOclients++;
                }

                if (index == 0x1005)
                    noSYNC = 1;

                if (index == 0x1014)
                    noEMCY = 1;

                if (index == 0x1012)
                    noTIME = 1;

                if (index == 0x1300)
                    noGFC = 1;
                if (index >= 0x1301 && index <= 0x1340)
                    noSRDO++;
            }
        }

        bool arrayspecial(UInt16 index, bool open)
        {

            if (open)
            {

                if (openings.Contains(index))
                    return true;
            }
            else
            {

                if (closings.Contains(index))
                    return true;
            }

            return false;
        }


        string export_OD_def_array(string location)
        {

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                if ((od.StorageLocation != location))
                {
                    if (!(od.StorageLocation == "Unused" && location == "RAM"))
                        /* this entry doesn't belong in this section */
                        continue;
                }

                if (od.Nosubindexes == 0)
                {
                    sb.AppendLine($"/*{od.Index:X4}*/ {formatvaluewithdatatype(od.defaultvalue, od.datatype)},");
                }
                else
                {
                    if (arrayspecial(od.Index, true))
                    {
                        sb.AppendFormat("/*{0:X4}*/ {{{{", od.Index);
                    }
                    else
                    {
                        sb.AppendFormat("/*{0:X4}*/ {{", od.Index);
                    }

                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry sub = kvp2.Value;

                        DataType dt = sub.datatype;

                        if ((od.objecttype==ObjectType.ARRAY) && kvp2.Key == 0)
                            continue;

                        sb.Append(formatvaluewithdatatype(sub.defaultvalue, dt));

                        if (od.subobjects.Keys.Last() != kvp2.Key)
                            sb.Append(", ");
                    }

                    if (arrayspecial(od.Index, false))
                    {
                        sb.AppendLine("}},");
                    }
                    else
                    {
                        sb.AppendLine("},");
                    }
                }
            }

            return sb.ToString();
        }
    }





    
}

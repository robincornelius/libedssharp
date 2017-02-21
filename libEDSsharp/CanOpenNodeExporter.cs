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
    based heavly on the files CO_OD.h and CO_OD.c from CanOpenNode which are
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
    public class CanOpenNodeExporter
    {

        private string folderpath;
        private EDSsharp eds;

        private int enabledcount = 0;

    //    Dictionary<DataType, defstruct> defstructs = new Dictionary<DataType, defstruct>();

        public void export(string folderpath, EDSsharp eds)
        {
            this.folderpath = folderpath;
            this.eds = eds;
     
            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                if (kvp.Value.Disabled == false)
                    enabledcount++;
            }

            countPDOS();

            export_h();
            export_c();

        }

        private void print_h_bylocation(StreamWriter file, StorageLocation location)
        {

            string lastname = "";
            //pre walk the list to find groups for arrays

            Dictionary<string, int> au = new Dictionary<string, int>();

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

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                if ((od.location != location))
                {
                    if (!(od.location == 0 && location == StorageLocation.RAM))
                        continue;
                }


                if (od.nosubindexes == 0)
                {
                    string specialarraylength = ""; 
                    if(od.datatype==DataType.VISIBLE_STRING || od.datatype == DataType.OCTET_STRING)
                    {
                        specialarraylength = string.Format("[{0}]", od.sizeofdatatype());
                    }

                    file.WriteLine(string.Format("/*{0:x4}      */ {1,-15} {2}{3};", od.index, od.datatype.ToString(), make_cname(od.parameter_name), specialarraylength));                 
                }
                else
                {
                    DataType t = eds.getdatatype(od);


                    //If it not a defined type, and it probably is not for a REC, we must generate a name, this is 
                    //related to the previous code that generated the actual structures.

                    string objecttypewords = "";

                    switch(od.objecttype)
                    {

                        case ObjectType.REC:  
                            objecttypewords = String.Format("OD_{0}_t", make_cname(od.parameter_name));
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
                            continue;
                        lastname = name;
                        file.WriteLine(string.Format("/*{0:x4}      */ {1,-15} {2}[{3}];", od.index, objecttypewords, make_cname(od.parameter_name), au[name]));
                    }
                    else
                    {
                        //Don't put sub indexes on record type in h file unless there are multiples of the same
                        //in which case its not handleded here, we need a special case for the predefined special
                        //values that arrayspecial() checks for, to generate 1 element arrays if needed
                        if (od.objecttype == ObjectType.REC )
                        {
                            if (arrayspecial(od.index, true))
                            {
                                file.WriteLine(string.Format("/*{0:x4}      */ {1,-15} {2}[1];", od.index, objecttypewords, make_cname(od.parameter_name)));
                            }
                            else
                            {
                                file.WriteLine(string.Format("/*{0:x4}      */ {1,-15} {2};", od.index, objecttypewords, make_cname(od.parameter_name)));
                            }
                        }
                        else
                        {
                            file.WriteLine(string.Format("/*{0:x4}      */ {1,-15} {2}[{3}];", od.index, objecttypewords, make_cname(od.parameter_name), od.nosubindexes - 1));
                        }
                    }
                }

            }
        }

        private void addGPLheader(StreamWriter file)
        {
            file.WriteLine(@"/*******************************************************************************

   File - CO_OD.c/CO_OD.h
   CANopen Object Dictionary.

   Copyright (C) 2004-2008 Janez Paternoster

   License: GNU Lesser General Public License (LGPL).

   <http://canopennode.sourceforge.net>

   (For more information see <CO_SDO.h>.)

   This file is part of CANopenNode, an opensource CANopen Stack.
   Project home page is <https://github.com/CANopenNode/CANopenNode>.
   For more information on CANopen see <http://www.can-cia.org/>.
 
   CANopenNode is free and open source software: you can redistribute
   it and/or modify it under the terms of the GNU General Public License
   as published by the Free Software Foundation, either version 2 of the
   License, or (at your option) any later version.
  
   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
   GNU General Public License for more details.
  
   You should have received a copy of the GNU General Public License
   along with this program. If not, see <http://www.gnu.org/licenses/>.
  
   Following clarification and special exception to the GNU General Public
   License is included to the distribution terms of CANopenNode:
  
   Linking this library statically or dynamically with other modules is
   making a combined work based on this library. Thus, the terms and
   conditions of the GNU General Public License cover the whole combination.
  
   As a special exception, the copyright holders of this library give
   you permission to link this library with independent modules to
   produce an executable, regardless of the license terms of these
   independent modules, and to copy and distribute the resulting
   executable under terms of your choice, provided that you also meet,
   for each linked independent module, the terms and conditions of the
   license of that module. An independent module is a module which is
   not derived from or based on this library. If you modify this
   library, you may extend this exception to your version of the
   library, but you are not obliged to do so. If you do not wish
   to do so, delete this exception statement from your version.
 
   This file was automatically generated with libedssharp Object
   Dictionary Editor. DON'T EDIT THIS FILE MANUALLY !!!!

*******************************************************************************/

");

        }

        private void export_h()
        {

            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + "CO_OD.h");


            addGPLheader(file);

            file.WriteLine("#pragma once");
            file.WriteLine("");

            file.WriteLine(@"/*******************************************************************************
   CANopen DATA DYPES
*******************************************************************************/
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
   typedef domain_t     DOMAIN;

");

            file.WriteLine("/*******************************************************************************");
            file.WriteLine("   FILE INFO:");
            file.WriteLine(string.Format("      FileName:     {0}", eds.fi.FileName));
            file.WriteLine(string.Format("      FileVersion:  {0}", eds.fi.FileVersion));
            file.WriteLine(string.Format("      CreationTime: {0}", eds.fi.CreationTime));
            file.WriteLine(string.Format("      CreationDate: {0}", eds.fi.CreationDate));
            file.WriteLine(string.Format("      CreatedBy:    {0}", eds.fi.CreatedBy));
            file.WriteLine("******************************************************************************/");
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine("/*******************************************************************************");
            file.WriteLine("   DEVICE INFO:");
            file.WriteLine(string.Format("      VendorName:     {0}", eds.di.VendorName));
            file.WriteLine(string.Format("      VendorNumber    {0}", eds.di.VendorNumber));
            file.WriteLine(string.Format("      ProductName:    {0}", eds.di.ProductName));
            file.WriteLine(string.Format("      ProductNumber:  {0}", eds.di.ProductNumber));
            file.WriteLine("******************************************************************************/");
            file.WriteLine("");
            file.WriteLine("");

         
            file.WriteLine(@"/*******************************************************************************
   FEATURES
*******************************************************************************/");

            file.WriteLine(string.Format("  #define CO_NO_SYNC                     {0}   //Associated objects: 1005-1007", noSYNC));

            file.WriteLine(string.Format("  #define CO_NO_EMERGENCY                {0}   //Associated objects: 1014, 1015", noEMCY));

            file.WriteLine(string.Format("  #define CO_NO_SDO_SERVER               {0}   //Associated objects: 1200-127F", noSDOservers));
            file.WriteLine(string.Format("  #define CO_NO_SDO_CLIENT               {0}   //Associated objects: 1280-12FF", noSDOclients));

            file.WriteLine(string.Format("  #define CO_NO_RPDO                     {0}   //Associated objects: 14xx, 16xx", noRXpdos));
            file.WriteLine(string.Format("  #define CO_NO_TPDO                     {0}   //Associated objects: 18xx, 1Axx", noTXpdos));


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

            file.WriteLine(string.Format("  #define CO_NO_NMT_MASTER               {0}", ismaster==true?1:0));
            file.WriteLine("");
            file.WriteLine("");
            file.WriteLine(@"/*******************************************************************************
   OBJECT DICTIONARY
*******************************************************************************/");

            file.WriteLine(string.Format("   #define CO_OD_NoOfElements             {0}", enabledcount));
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine(@"/*******************************************************************************
   TYPE DEFINITIONS FOR RECORDS
*******************************************************************************/");

            //We need to identify all the record types used and generate a struct for each one
            //FIXME the original CanOpenNode exporter said how many items used this struct in the comments

            List<string> structnamelist = new List<string>();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.objecttype != ObjectType.REC)
                    continue;

                string structname = String.Format("OD_{0}_t", make_cname(od.parameter_name));

                if (structnamelist.Contains(structname))
                    continue;

                structnamelist.Add(structname);

                file.WriteLine(string.Format("/*{0:x4}    */ typedef struct {{", kvp.Key));
                foreach (KeyValuePair<UInt16, ODentry> kvp2 in kvp.Value.subobjects)
                {
                    string paramaterarrlen = "";
                    
                    ODentry subod = kvp2.Value;

                    if(subod.datatype==DataType.VISIBLE_STRING || subod.datatype==DataType.OCTET_STRING)
                    {
                        paramaterarrlen = String.Format("[{0}]", subod.sizeofdatatype());
                    }

                    file.WriteLine(string.Format("               {0,-15}{1}{2};", subod.datatype.ToString(), make_cname(subod.parameter_name),paramaterarrlen));

                }

                file.WriteLine(string.Format("               }}              {0};", structname));

            }

    

            file.WriteLine(@"/*******************************************************************************
   STRUCTURES FOR VARIABLES IN DIFFERENT MEMORY LOCATIONS
*******************************************************************************/
#define  CO_OD_FIRST_LAST_WORD     0x55 //Any value from 0x01 to 0xFE. If changed, EEPROM will be reinitialized.

/***** Structure for RAM variables ********************************************/
struct sCO_OD_RAM{
               UNSIGNED32     FirstWord;
");

            print_h_bylocation(file, StorageLocation.RAM);

            file.WriteLine(@"
               UNSIGNED32     LastWord;
};");

            file.WriteLine(@"/***** Structure for EEPROM variables *****************************************/
struct sCO_OD_EEPROM{
               UNSIGNED32     FirstWord;


");
            print_h_bylocation(file, StorageLocation.EEPROM);

            file.WriteLine(@"
               UNSIGNED32     LastWord;
};");

            file.WriteLine(@"/***** Structure for ROM variables ********************************************/
struct sCO_OD_ROM{
               UNSIGNED32     FirstWord;


");
            print_h_bylocation(file, StorageLocation.ROM);

            file.WriteLine(@"
               UNSIGNED32     LastWord;
};");


            file.WriteLine(@"/***** Declaration of Object Dictionary variables *****************************/
extern struct sCO_OD_RAM CO_OD_RAM;

extern struct sCO_OD_EEPROM CO_OD_EEPROM;

extern struct sCO_OD_ROM CO_OD_ROM;


/*******************************************************************************
   ALIASES FOR OBJECT DICTIONARY VARIABLES
*******************************************************************************/");

            List<string> constructed_rec_types = new List<string>();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {


                ODentry od = kvp.Value;

                string loc = getlocation(od.location);

                DataType t = eds.getdatatype(od);


                switch (od.objecttype)
                {
                    default:
                        {
                            file.WriteLine(string.Format("/*{0:x4}, Data Type: {1} */", od.index, t.ToString()));
                            file.WriteLine(string.Format("        #define {0,-51} {1}.{2}", string.Format("OD_{0}", make_cname(od.parameter_name)), loc, make_cname(od.parameter_name)));

                            DataType dt = od.datatype;

                            if (dt == DataType.OCTET_STRING || dt == DataType.VISIBLE_STRING)
                            {
                                file.WriteLine(string.Format("        #define {0,-51} {1}", string.Format("ODL_{0}_stringLength", make_cname(od.parameter_name)), od.sizeofdatatype()));
                            }
                            file.WriteLine("");
                        }
                        break;

                    case ObjectType.ARRAY:
                        {
                            DataType dt = od.datatype;

                            file.WriteLine(string.Format("/*{0:x4}, Data Type: {1}, Array[{2}] */", od.index, t.ToString(), od.nosubindexes - 1));
                            file.WriteLine(string.Format("        #define OD_{0,-48} {1}.{2}", make_cname(od.parameter_name), loc, make_cname(od.parameter_name)));
                            file.WriteLine(string.Format("        #define {0,-51} {1}", string.Format("ODL_{0}_arrayLength", make_cname(od.parameter_name)), od.nosubindexes - 1));


                            List<string> ODAs = new List<string>();

                            string ODAout = "";

                            foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                            {
                                ODentry sub = kvp2.Value;

                                if (sub.subindex == 0)
                                    continue;

                                string ODA = string.Format("{0}", string.Format("ODA_{0}_{1}", make_cname(od.parameter_name), make_cname(sub.parameter_name)));

                                if (ODAs.Contains(ODA))
                                {
                                    continue;
                                }

                                ODAs.Add(ODA);

                                //Arrays do not have a size in the raw CO objects, Records do
                                //so offset by one
                                if (od.objecttype == ObjectType.ARRAY)
                                {
                                    ODAout += (string.Format("        #define {0,-51} {1}\r\n", string.Format("ODA_{0}_{1}", make_cname(od.parameter_name), make_cname(sub.parameter_name)), sub.subindex - 1));
                                }
                                else
                                {
                                    ODAout += (string.Format("        #define {0,-51} {1}\r\n", string.Format("ODA_{0}_{1}", make_cname(od.parameter_name), make_cname(sub.parameter_name)), sub.subindex));
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
                                file.WriteLine(string.Format("/*{0:x4}, Data Type: {1}_t */", od.index, rectype));
                                file.WriteLine(string.Format("        #define {0,-51} {1}.{2}", string.Format("OD_{0}", rectype), loc, rectype));
                                constructed_rec_types.Add(rectype);
                                file.WriteLine("");
                            }

                        }
                        break;
                }
            }

            file.Close();

        }

        private void export_c()
        {
            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + "CO_OD.c");

            addGPLheader(file);

            file.WriteLine(@"#include ""CO_driver.h""
#include ""CO_OD.h""
#include ""CO_SDO.h""


/*******************************************************************************
   DEFINITION AND INITIALIZATION OF OBJECT DICTIONARY VARIABLES
*******************************************************************************/

/***** Definition for RAM variables *******************************************/
struct sCO_OD_RAM CO_OD_RAM = {
           CO_OD_FIRST_LAST_WORD,
");

            export_OD_def_array(file, StorageLocation.RAM);

            file.WriteLine(@"
           CO_OD_FIRST_LAST_WORD,
};

/***** Definition for EEPROM variables ****************************************/
struct sCO_OD_EEPROM CO_OD_EEPROM = {
           CO_OD_FIRST_LAST_WORD,
");


            export_OD_def_array(file, StorageLocation.EEPROM);

            file.WriteLine(@"  CO_OD_FIRST_LAST_WORD,
};


/***** Definition for ROM variables *******************************************/
struct sCO_OD_ROM CO_OD_ROM = {    //constant variables, stored in flash
           CO_OD_FIRST_LAST_WORD,

");


            export_OD_def_array(file, StorageLocation.ROM);

            file.WriteLine(@"

           CO_OD_FIRST_LAST_WORD
};


/*******************************************************************************
   STRUCTURES FOR RECORD TYPE OBJECTS
*******************************************************************************/

");

            export_record_types(file);

            file.Write(@"/*******************************************************************************
   OBJECT DICTIONARY
*******************************************************************************/
const CO_OD_entry_t CO_OD[");
            

            file.Write(string.Format("{0}", enabledcount));

            file.WriteLine(@"] = {
");

            bool arrayspecialcase = false;
            int count = 0;

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {

                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                string loc = getlocation(od.location);

                byte flags = getflags(od);

                DataType t = eds.getdatatype(od);
                int datasize = od.sizeofdatatype();

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
                if (od.objecttype == ObjectType.ARRAY && od.nosubindexes > 0)
                    array = string.Format("[0]");

                
                if(arrayspecial(od.index,true))
                {
                    arrayspecialcase = true;
                    count = 0;
                }

                if(arrayspecialcase)
                {
                    array = string.Format("[{0}]", count);
                    count++;
                }

                //Arrays and Recs have 1 less subindex than actually present in the od.subobjects
                int nosubindexs = od.nosubindexes;
                if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
                {
                    if(nosubindexs>0)
                        nosubindexs--;
                }

                //Arrays really should obey the max subindex paramater not the physical number of elements
                if (od.objecttype == ObjectType.ARRAY)
                {
                    if ((od.getmaxsubindex() != nosubindexs) && (od.index != 0x1003))
                    {
                        Warnings.warning_list.Add(String.Format("Subindex descripency on object 0x{0:x4} arraysize: {1} vs max-subindex: {2}", od.index, nosubindexs, od.getmaxsubindex()));
                    }
                    nosubindexs = od.getmaxsubindex();
                }

                string pdata; //CO_OD_entry_t pData generator

                if(od.objecttype==ObjectType.REC)
                {
                   
                    pdata = string.Format("OD_record{0:x4}",od.index);
                }
                else
                {
                    pdata = string.Format("{0}.{1}{2}", loc, make_cname(od.parameter_name), array);
                }

                file.WriteLine(string.Format("{{0x{0:x4}, 0x{1:x2}, 0x{2:x2}, {3}, (void*)&{4}}},", od.index, nosubindexs, flags, datasize, pdata ));

                if (arrayspecial(od.index, false))
                {
                    arrayspecialcase = false;
                }
            }



            file.WriteLine("};");

            file.Close();
        }



        byte getflags(ODentry od)
        {
            byte flags = 0;

            //aways return 0 for REC objects as CO_OD_getDataPointer() uses this to pickup the details
            if (od.objecttype == ObjectType.REC)
                return 0;

            flags = (byte)od.location;

            //fixme rwr and rrw are not supported
            if (od.accesstype == EDSsharp.AccessType.ro
                || od.accesstype == EDSsharp.AccessType.rw
                || od.accesstype == EDSsharp.AccessType.@const)
                flags |= 0x04;

            if (od.accesstype == EDSsharp.AccessType.wo
                || od.accesstype == EDSsharp.AccessType.rw)
                flags |= 0x08;

            if (od.PDOMapping)
                flags |= 0x10;

            if (od.PDOMapping)
                flags |= 0x20;

            if (od.PDOMapping)
                flags |= 0x30; //fix me no control over rx and tx mapping, its both or none

            if(od.TPDODetectCos)
              flags |=0x40;
   
            int datasize = od.sizeofdatatype();

            if (datasize > 1)
                flags |= 0x80;

            return flags;

        }


        string getlocation(StorageLocation location)
        {
            string loc;
            switch (location)
            {
                case StorageLocation.ROM:
                    loc = "CO_OD_ROM";
                    break;
                default:
                case StorageLocation.RAM:
                    loc = "CO_OD_RAM";
                    break;
                case StorageLocation.EEPROM:
                    loc = "CO_OD_EEPROM";
                    break;

            }

            return loc;
        }

        string formatvaluewithdatatype(string defaultvalue, DataType dt)
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
                UInt16 data = Convert.ToUInt16(defaultvalue, nobase);
                data += eds.NodeId;
                defaultvalue = string.Format("0x{0:x}", data);
                nobase = 16;
            }


            switch (dt)
            {
                case DataType.UNSIGNED24:
                case DataType.UNSIGNED32:
                    return String.Format("0x{0:x4}L", Convert.ToUInt32(defaultvalue, nobase));

                case DataType.INTEGER24:
                case DataType.INTEGER32:
                    return String.Format("0x{0:x4}L", Convert.ToInt32(defaultvalue, nobase));

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
                    return String.Format("0x{0:x1}", Convert.ToSByte(defaultvalue, nobase));

                case DataType.INTEGER16:
                    return String.Format("0x{0:x2}", Convert.ToInt16(defaultvalue, nobase));

                case DataType.UNSIGNED8:
                    return String.Format("0x{0:x1}L", Convert.ToByte(defaultvalue, nobase));

                case DataType.UNSIGNED16:
                    return String.Format("0x{0:x2}", Convert.ToUInt16(defaultvalue, nobase));

                case DataType.INTEGER64:
                    return String.Format("0x{0:x8}L", Convert.ToInt64(defaultvalue, nobase));

                case DataType.UNSIGNED64:
                    return String.Format("0x{0:x8}L", Convert.ToUInt64(defaultvalue, nobase));

                default:
                    return (String.Format("{0:x}", defaultvalue));

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

        void export_record_types(StreamWriter file)
        {


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

                if(od.index>=0x1400 && od.index<0x1600)
                {
                    count = 3; //CanOpenNode Fudging. Its only 3 paramaters for RX PDOS in the c code despite being a PDO_COMMUNICATION_PARAMETER
                }

                string cname = make_cname(od.parameter_name);
                
                file.WriteLine(String.Format("/*0x{0:x4}*/ const CO_OD_entryRecord_t OD_record{0:x4}[{1}] = {{", od.index, count));

                string arrayaccess = "";

                if (arrayspecial(od.index, true) || arrayopen)
                {
                    arrayaccess = string.Format("[{0}]",arrayindex);
                    arrayindex++;
                    arrayopen = true;
                }


                foreach (KeyValuePair<UInt16, ODentry> kvpsub in od.subobjects)
                {
                    ODentry sub = kvpsub.Value;

                    string subcname = make_cname(sub.parameter_name);

                ;

                    file.WriteLine(string.Format("           {{(void*)&{5}.{0}{4}.{1}, 0x{2:x2}, 0x{3} }},", cname, subcname, getflags(sub), sub.sizeofdatatype(),arrayaccess, getlocation(od.location)));

                }


                if (arrayspecial(od.index, false))
                {

                    arrayindex=0;
                    arrayopen = false;

                }

                file.Write("};\r\n\r\n");
            }
        }

        int noTXpdos = 0;
        int noRXpdos = 0;
        int noSDOclients = 0;
        int noSDOservers = 0;
        int distTXpdo = 0;
        int distRXpdo = 0;
        int noSYNC = 0;
        int noEMCY = 0;

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
            }

        }

        bool arrayspecial(UInt16 index, bool open)
        {

            if (open)
            {
                if (index == 0x1200) //SDO Server
                    return true;

                if (index == 0x1280) //SDO Client
                    return true;

                if (index == 0x1400) //RX PDO Config
                    return true;

                if (index == 0x1600) //RX PDO Map
                    return true;

                if (index == 0x1800) //TX PDO Config
                    return true;

                if (index == 0x1a00) //TX PDO Map
                    return true;
            }
            else
            {
                if (index == 0x1200 + noSDOservers - 1)
                    return true;

                if (index == 0x1280 + noSDOclients - 1) 
                    return true;

                if (index == 0x1400 + distRXpdo )
                    return true;

                if (index == 0x1600 + distRXpdo )
                    return true;

                if (index == 0x1800 + distTXpdo )
                    return true;

                if (index == 0x1a00 + distTXpdo )
                    return true;
            }

            return false;

        }


        void export_OD_def_array(StreamWriter file, StorageLocation location)
        {

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                if ((od.location != location))
                {
                    if (!(od.location == 0 && location == StorageLocation.RAM))
                        continue;
                }

                if (od.nosubindexes == 0)
                {
                    file.WriteLine(string.Format("/*{0:x4}*/ {1},", od.index, formatvaluewithdatatype(od.defaultvalue, od.datatype)));
                }
                else
                {
                    if (arrayspecial(od.index, true))
                    {
                        file.Write(string.Format("/*{0:x4}*/ {{{{", od.index));
                    }
                    else
                    {
                        file.Write(string.Format("/*{0:x4}*/ {{", od.index));
                    }

                    foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                    {
                        ODentry sub = kvp2.Value;

                        DataType dt = sub.datatype;

                        if ((od.objecttype==ObjectType.ARRAY) && sub.subindex == 0)
                            continue;

                        file.Write(formatvaluewithdatatype(sub.defaultvalue, dt));

                        if (od.subobjects.Keys.Last() != kvp2.Key)
                            file.Write(", ");
                    }


                    if (arrayspecial(od.index, false))
                    {
                        file.WriteLine("}},");
                    }
                    else
                    {
                        file.WriteLine("},");
                    }

                }
            }

        }
    }




    public static class StringUnescape
    {
        public static string Escape(char c)
        {

            switch(c)
            {
                case '\0':
                    return @"\0";
                case '\n':
                    return @"\n";
                case '\r':
                    return @"\r";
                case '\t':
                    return @"\t";
                case '\a':
                    return @"\a";
                case '\b':
                    return @"\b";
                case '\f':
                    return @"\f";
                case '\v':
                    return @"\v";

                default:
                    return c.ToString();
              
            }

        }

        public static string Unescape(this string txt)
        {
            if (string.IsNullOrEmpty(txt)) { return txt; }
            StringBuilder retval = new StringBuilder(txt.Length);
            for (int ix = 0; ix < txt.Length;)
            {
                int jx = txt.IndexOf('\\', ix);
                if (jx < 0 || jx == txt.Length - 1) jx = txt.Length;
                retval.Append(txt, ix, jx - ix);
                if (jx >= txt.Length) break;
                switch (txt[jx + 1])
                {
                    case 'n': retval.Append('\n'); break;  // Line feed
                    case 'r': retval.Append('\r'); break;  // Carriage return
                    case 't': retval.Append('\t'); break;  // Tab
                    case '0': retval.Append('\0'); break;  // Null
                    case 'a': retval.Append('\a'); break;  // Bell
                    case 'b': retval.Append('\b'); break;  // Backspace
                    case 'f': retval.Append('\f'); break;  // Formfeed
                    case 'v': retval.Append('\v'); break;  // Verticle tab
                    case '\\': retval.Append('\\'); break; // Don't escape
                    default:                                 // Unrecognized, copy as-is
                        retval.Append('\\').Append(txt[jx + 1]); break;
                }
                ix = jx + 2;
            }
            return retval.ToString();
        }
    }

}

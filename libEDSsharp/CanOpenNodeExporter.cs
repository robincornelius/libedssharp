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

        Dictionary<DataType, defstruct> defstructs = new Dictionary<DataType, defstruct>();

        public void export(string folderpath, EDSsharp eds)
        {
            this.folderpath = folderpath;
            this.eds = eds;

            init_defstructs();

            countPDOS();

            export_h();
            export_c();

        }

        private void print_h_bylocation(StreamWriter file, StorageLocation location)
        {
            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;


                if ((od.location != location))
                {
                    if (!(od.location == 0 && location == StorageLocation.RAM))
                        continue;
                }


                if (od.nosubindexes == 0)
                {
                    //if (od.subindex == -1)
                    {

                        file.WriteLine(string.Format("/*{0:x4}      */ {1,-15} {2};", od.index, od.datatype.ToString(), od.paramater_cname()));
                    }
                }
                else
                {
                    DataType t = eds.getdatatype(od);
                    file.WriteLine(string.Format("/*{0:x4}      */ {1,-15} {2}[{3}];", od.index, t.ToString(), od.paramater_cname(), od.nosubindexes));
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
*/
/*
   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.


   Original Author: Janez Paternoster


   This file was automatically generated with libedssharp Object
   Dictionary Editor. DON'T EDIT THIS FILE MANUALLY !!!!

*******************************************************************************/

");

        }

        private void export_h()
        {

            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + "CO_OD.h");


            addGPLheader(file);

            file.WriteLine("#pramga once");
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
            file.WriteLine("/******************************************************************************/");
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine("/*******************************************************************************");
            file.WriteLine("   DEVICE INFO:");
            file.WriteLine(string.Format("      VendorName:     {0}", eds.di.VendorName));
            file.WriteLine(string.Format("      VendorNumber    {0}", eds.di.VendorNumber));
            file.WriteLine(string.Format("      ProductName:    {0}", eds.di.ProductName));
            file.WriteLine(string.Format("      ProductNumber:  {0}", eds.di.ProductNumber));
            file.WriteLine("/******************************************************************************/");
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

            //FIX ME NMT MASTER should auto generate
            file.WriteLine(@"  #define CO_NO_NMT_MASTER               0   

");

            file.WriteLine(@"/*******************************************************************************
   OBJECT DICTIONARY
*******************************************************************************/");
            file.WriteLine(string.Format("   #define CO_OD_NoOfElements             {0}", eds.ods.Count));
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine(@"/*******************************************************************************
   TYPE DEFINITIONS FOR RECORDS
*******************************************************************************/");

            foreach (KeyValuePair<DataType, defstruct> kvp in defstructs)
            {
                file.WriteLine(string.Format("/*{0}    */ typedef struct {{", kvp.Key));
                foreach (KeyValuePair<UInt16, subdefstruct> kvp2 in kvp.Value.elements)
                {
                    subdefstruct sub = kvp2.Value;
                    file.WriteLine(string.Format("               {0,-15}{1};", sub.datatype.ToString(), sub.c_declaration));
                }

                file.WriteLine(string.Format("               }}              {0};", kvp.Value.c_declaration));

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

extern CO_OD_ROM_IDENT struct sCO_OD_ROM CO_OD_ROM;


/*******************************************************************************
   ALIASES FOR OBJECT DICTIONARY VARIABLES
*******************************************************************************/");

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {


                ODentry od = kvp.Value;

                string loc = getlocation(od.location);

                DataType t = eds.getdatatype(od);

                if (od.nosubindexes == 0)
                {
                    file.WriteLine(string.Format("/*{0:x4}, Data Type: {1} */", od.index, t.ToString()));
                    file.WriteLine(string.Format("        #define OD_{0}             {1}.{2}", od.paramater_cname(), loc, od.paramater_cname()));
                }
                else
                {

                    //ARRAY TYPES SUBS ONLY FIXME

                    DataType dt = od.datatype;

                    file.WriteLine(string.Format("/*{0:x4}, Data Type: {1}, Array[{2}] */", od.index, t.ToString(), od.nosubindexes - 1));
                    file.WriteLine(string.Format("        #define OD_{0}             {1}.{2}", od.paramater_cname(), loc, od.paramater_cname()));
                    file.WriteLine(string.Format("        #define ODL_{0}_arrayLength             {1}", od.paramater_cname(), od.nosubindexes - 1));


                    if (od.objecttype != ObjectType.ARRAY)
                    {
                        List<string> ODAs = new List<string>();

                        string ODAout = "";

                        foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                        {
                            ODentry sub = kvp2.Value;

                            if (sub.subindex == 0)
                                continue;

                            string ODA = string.Format("ODA_{0}_{1}", od.paramater_cname(), sub.paramater_cname());

                            if (ODAs.Contains(ODA))
                            {
                                ODAout = "";
                                break;
                            }

                            ODAs.Add(ODA);


                            ODAout += (string.Format("        #define ODA_{0}_{1}       {2}\r\n", od.paramater_cname(), sub.paramater_cname(), sub.subindex));

                        }

                        file.Write(ODAout);
                    }
                }

                file.WriteLine("");

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
   CO_OD_ROM_IDENT struct sCO_OD_ROM CO_OD_ROM = {    //constant variables, stored in flash
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
const sCO_OD_object CO_OD[");
            
            file.Write(string.Format("{0}",eds.ods.Count));

            file.WriteLine(@"] = {
");

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {

                ODentry od = kvp.Value;

                string loc = getlocation(od.location);

                byte flags = getflags(od);

                DataType t = eds.getdatatype(od);
                int datasize = sizeofdatatype(t,od);

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
                if (od.nosubindexes > 0)
                    array = string.Format("[0]");

                file.WriteLine(string.Format("{{0x{0:x4}, 0x{1:x2}, 0x{2:x2}, {3}, (const void*)&{4}.{5}{6},      {7}}},", od.index, od.nosubindexes, flags, datasize, loc, od.paramater_cname(), array, odf));

            }



            file.WriteLine("};");

            file.Close();
        }



        byte getflags(ODentry od)
        {
            byte flags = 0;

            flags = (byte)od.location;

            //fixme rwr and rrw are not supported
            if (od.accesstype == EDSsharp.AccessType.ro
                || od.accesstype == EDSsharp.AccessType.rw
                || od.accesstype == EDSsharp.AccessType.cons)
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
   
            DataType t = eds.getdatatype(od);

            int datasize = sizeofdatatype(t,od);

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

            if (defaultvalue.Contains("$NODEID"))
            {
                defaultvalue = defaultvalue.Replace("$NODEID", "");
                defaultvalue = defaultvalue.Replace("+", "");
                nodeidreplace = true;
            }

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
                        string array = "{";
                        foreach (char s in defaultvalue)
                        {
                            array += "'" + s + "'";

                            if (!object.ReferenceEquals(s, defaultvalue.Last()))
                            {
                                array += ", ";
                            }
                        }

                        array += "}";
                        return array;
                    }


                case DataType.OCTET_STRING:
                    {
                        string[] bits = defaultvalue.Split(' ');
                        string octet = "";
                        foreach (string s in bits)
                        {
                            octet += formatvaluewithdatatype(s, DataType.UNSIGNED8);

                            if (!object.ReferenceEquals(s, bits.Last()))
                            {
                                octet += ", ";
                            }
                        }
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

                default:
                    return (String.Format("{0:x}", defaultvalue));

            }
        }

       int sizeofdatatype(DataType dt,ODentry od)
        {
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

                case DataType.VISIBLE_STRING:
                    return 0;
                    

                default:
                    
                    return 0;


            }

        }


        void export_record_types(StreamWriter file)
        {


            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {


                ODentry od = kvp.Value;

                if (od.objecttype != ObjectType.REC)
                    continue;

                if (od.datatype == DataType.UNKNOWN)
                    continue;

                defstruct def = defstructs[kvp.Value.datatype];

                int count = def.elements.Count;

                if(od.index>=0x1400 && od.index<0x1600)
                {
                    count = 3; //CanOpenNode Fudging. Its only 3 paramaters for RX PDOS in the c code despite being a PDO_COMMUNICATION_PARAMETER
                }

                file.WriteLine(String.Format("/*0x{0:x4}*/ const CO_OD_entryRecord_t OD_record{0:x4}[{1}] = {{", od.index, count));

                foreach (KeyValuePair<UInt16, ODentry> kvpsub in od.subobjects)
                {
                    ODentry sub = kvpsub.Value;

                    file.WriteLine(string.Format("           {{(void*)&CO_OD_ROM.{0}.{1}, 0x{2:x2}, 0x{3} }}", def.c_declaration, def.elements[kvpsub.Key].c_declaration, getflags(sub), sizeofdatatype(sub.datatype, sub)));

                }

                file.Write("};\r\n\r\n");
            }
        }

        int noTXpdos = 0;
        int noRXpdos = 0;
        int noSDOclients = 0;
        int noSDOservers = 0;
        int noSYNC = 0;
        int noEMCY = 0;

        void countPDOS()
        {
            noRXpdos = 0;
            noTXpdos = 0;

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                UInt16 index = kvp.Key;

                if ((index & 0xFF00) == 0x1400)
                {
                    noRXpdos++;
                }

                if ((index & 0xFF00) == 0x1800)
                {
                    noTXpdos++;
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
                if (index == 0x1200)
                    return true;

                if (index == 0x1400)
                    return true;

                if (index == 0x1600)
                    return true;

                if (index == 0x1800)
                    return true;

                if (index == 0x1a00)
                    return true;
            }
            else
            {
                if (index == 0x1200)
                    return true;

                if (index == 0x1400 + noRXpdos - 1)
                    return true;

                if (index == 0x1600 + noRXpdos - 1)
                    return true;

                if (index == 0x1800 + noTXpdos - 1)
                    return true;

                if (index == 0x1a00 + noTXpdos - 1)
                    return true;
            }

            return false;

        }


        void export_OD_def_array(StreamWriter file, StorageLocation location)
        {

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

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

                        if ((od.objecttype == ObjectType.REC ||od.objecttype==ObjectType.ARRAY) && sub.subindex == 0)
                            continue;

                        if (od.objecttype == ObjectType.REC)
                            dt = od.datatype;

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



        void init_defstructs()
        {

            {
                //0x1018 Identity Record Specification, DataType 0x23
                defstruct ds = new defstruct("Identity Record Specification",
                "OD_identity_t");

                ds.elements.Add(0, new subdefstruct("number of supported entries in the record", "maxSubIndex", DataType.UNSIGNED8, ds));
                ds.elements.Add(1, new subdefstruct("Vendor-ID", "vendorID", DataType.UNSIGNED32, ds));
                ds.elements.Add(2, new subdefstruct("Product code", "productCode", DataType.UNSIGNED32, ds));
                ds.elements.Add(3, new subdefstruct("Revision number", "revisionNumber", DataType.UNSIGNED32, ds));
                ds.elements.Add(4, new subdefstruct("Serial number", "serialNumber", DataType.UNSIGNED32, ds));

                defstructs.Add(DataType.IDENTITY, ds);
            }

            {
                //0x1200 Identity Record Specification, DataType 0x22
                defstruct ds = new defstruct("SDO Parameter Record Specification",
                "OD_identity_t");

                ds.elements.Add(0, new subdefstruct("number of supported entries in the record", "maxSubIndex", DataType.UNSIGNED8, ds));
                ds.elements.Add(1, new subdefstruct("COB-ID client -> server", "COB_IDClientToServer", DataType.UNSIGNED32, ds));
                ds.elements.Add(2, new subdefstruct("COB-ID server -> client", "COB_IDServerToClient", DataType.UNSIGNED32, ds));
                ds.elements.Add(3, new subdefstruct("node ID of SDO’s client resp. server", "OD_SDOServerParameter_t", DataType.UNSIGNED32, ds));

                defstructs.Add(DataType.SDO_PARAMETER, ds);
            }


            {
                //0x1800 PDO Communication Parameter Record
                defstruct ds = new defstruct("PDO Communication Parameter Record",
                "OD_TPDOCommunicationParameter_t");

                ds.elements.Add(0, new subdefstruct("number of supported entries in the record", "maxSubIndex", DataType.UNSIGNED8, ds));
                ds.elements.Add(1, new subdefstruct("COB-ID", "COB_IDUsedByTPDO", DataType.UNSIGNED32, ds));
                ds.elements.Add(2, new subdefstruct("transmission type", "transmissionType", DataType.UNSIGNED8, ds));
                ds.elements.Add(3, new subdefstruct("inhibit time", "inhibitTime", DataType.UNSIGNED16, ds));
                ds.elements.Add(4, new subdefstruct("reserved", "compatibilityEntry", DataType.UNSIGNED8, ds));
                ds.elements.Add(5, new subdefstruct("event timer", "eventTimer", DataType.UNSIGNED16, ds));
                ds.elements.Add(6, new subdefstruct("SYNCStartValue", "SYNCStartValue", DataType.UNSIGNED8, ds));

                defstructs.Add(DataType.PDO_COMMUNICATION_PARAMETER, ds);
            }

            {
                //0x1A00 PDO TX Mapping Paramater DataType 0x21
                defstruct ds = new defstruct("PDO Mapping Parameter Record",
                "OD_TPDOMappingParameter_t");

                ds.elements.Add(0, new subdefstruct("number of mapped objects in PDO", "numberOfMappedObjects", DataType.UNSIGNED8, ds));

                ds.elements.Add(1, new subdefstruct("1st object to be mapped", "mappedObject1", DataType.UNSIGNED32, ds));
                ds.elements.Add(2, new subdefstruct("2nd object to be mapped", "mappedObject2", DataType.UNSIGNED32, ds));
                ds.elements.Add(3, new subdefstruct("3rd object to be mapped", "mappedObject3", DataType.UNSIGNED32, ds));
                ds.elements.Add(4, new subdefstruct("4th object to be mapped", "mappedObject4", DataType.UNSIGNED32, ds));
                ds.elements.Add(5, new subdefstruct("5th object to be mapped", "mappedObject5", DataType.UNSIGNED32, ds));
                ds.elements.Add(6, new subdefstruct("6th object to be mapped", "mappedObject6", DataType.UNSIGNED32, ds));
                ds.elements.Add(7, new subdefstruct("7th object to be mapped", "mappedObject7", DataType.UNSIGNED32, ds));
                ds.elements.Add(8, new subdefstruct("8th object to be mapped", "mappedObject8", DataType.UNSIGNED32, ds));

                defstructs.Add(DataType.PDO_MAPPING, ds);
            }



        }

    }

    public class defstruct
    {
        public string name;
        public string c_declaration;

        public Dictionary<UInt16, subdefstruct> elements;

        public defstruct(string name, string c_dec)
        {
            this.name = name;
            this.c_declaration = c_dec;
            elements = new Dictionary<UInt16, subdefstruct>();
        }

    }

    public class subdefstruct
    {
        public string description;
        public string c_declaration;

        public DataType datatype;
        public defstruct parent;

        public subdefstruct(string description, string c_dec, DataType datatype, defstruct parent)
        {
            this.description = description;
            this.c_declaration = c_dec;
            this.datatype = datatype;
            this.parent = parent;
        }
    }
}

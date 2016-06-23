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

        public void export(string folderpath,EDSsharp eds)
        {
            this.folderpath = folderpath;
            this.eds = eds;

            export_h();
            export_c();

        }

        private void print_h_bylocation(StreamWriter file,StorageLocation location)
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
                        file.WriteLine(string.Format("/*{0:x4}      */ {1} {2};", od.index, od.datatype.ToString(), od.paramater_cname()));
                    }
                }
                else
                {
                    DataType t = eds.getdatatype(od);
                    file.WriteLine(string.Format("/*{0:x4}      */ {1} {2}[{3}];", od.index, t.ToString(), od.paramater_cname(), od.nosubindexes));
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
            file.WriteLine(string.Format("      FileName:     {0}",eds.fi.FileName));
            file.WriteLine(string.Format("      FileVersion:  {0}",eds.fi.FileVersion));
            file.WriteLine(string.Format("      CreationTime: {0}",eds.fi.CreationTime));
            file.WriteLine(string.Format("      CreationDate: {0}",eds.fi.CreationDate));
            file.WriteLine(string.Format("      CreatedBy:    {0}",eds.fi.CreatedBy));
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

            //FIX ME features should auto generate
            file.WriteLine(@"/*******************************************************************************
   FEATURES
*******************************************************************************/
   #define CO_NO_SYNC                     1   //Associated objects: 1005, 1006, 1007, 2103, 2104
   #define CO_NO_EMERGENCY                1   //Associated objects: 1014, 1015
   #define CO_NO_SDO_SERVER               1   //Associated objects: 1200
   #define CO_NO_SDO_CLIENT               0   
   #define CO_NO_RPDO                     4   //Associated objects: 1400, 1401, 1402, 1403, 1600, 1601, 1602, 1603
   #define CO_NO_TPDO                     4   //Associated objects: 1800, 1801, 1802, 1803, 1A00, 1A01, 1A02, 1A03
   #define CO_NO_NMT_MASTER               0   

");


            //FIX ME we need to count the elements here
            file.WriteLine(@"/*******************************************************************************
   OBJECT DICTIONARY
*******************************************************************************/");
            file.WriteLine(string.Format("   #define CO_OD_NoOfElements             {0}",eds.ods.Count));
            file.WriteLine("");
            file.WriteLine("");



            file.WriteLine(@"/*******************************************************************************
   TYPE DEFINITIONS FOR RECORDS
*******************************************************************************/
/*1018      */ typedef struct{
               UNSIGNED8      maxSubIndex;
               UNSIGNED32     vendorID;
               UNSIGNED32     productCode;
               UNSIGNED32     revisionNumber;
               UNSIGNED32     serialNumber;
               }              OD_identity_t;

/*1200[1]   */ typedef struct{
               UNSIGNED8      maxSubIndex;
               UNSIGNED32     COB_IDClientToServer;
               UNSIGNED32     COB_IDServerToClient;
               }              OD_SDOServerParameter_t;

/*1400[4]   */ typedef struct{
               UNSIGNED8      maxSubIndex;
               UNSIGNED32     COB_IDUsedByRPDO;
               UNSIGNED8      transmissionType;
               }              OD_RPDOCommunicationParameter_t;

/*1600[4]   */ typedef struct{
               UNSIGNED8      numberOfMappedObjects;
               UNSIGNED32     mappedObject1;
               UNSIGNED32     mappedObject2;
               UNSIGNED32     mappedObject3;
               UNSIGNED32     mappedObject4;
               UNSIGNED32     mappedObject5;
               UNSIGNED32     mappedObject6;
               UNSIGNED32     mappedObject7;
               UNSIGNED32     mappedObject8;
               }              OD_RPDOMappingParameter_t;

/*1800[4]   */ typedef struct{
               UNSIGNED8      maxSubIndex;
               UNSIGNED32     COB_IDUsedByTPDO;
               UNSIGNED8      transmissionType;
               UNSIGNED16     inhibitTime;
               UNSIGNED8      compatibilityEntry;
               UNSIGNED16     eventTimer;
               UNSIGNED8      SYNCStartValue;
               }              OD_TPDOCommunicationParameter_t;

/*1A00[4]   */ typedef struct{
               UNSIGNED8      numberOfMappedObjects;
               UNSIGNED32     mappedObject1;
               UNSIGNED32     mappedObject2;
               UNSIGNED32     mappedObject3;
               UNSIGNED32     mappedObject4;
               UNSIGNED32     mappedObject5;
               UNSIGNED32     mappedObject6;
               UNSIGNED32     mappedObject7;
               UNSIGNED32     mappedObject8;
               }              OD_TPDOMappingParameter_t;

/*2120      */ typedef struct{
               UNSIGNED8      maxSubIndex;
               INTEGER64      I64;
               UNSIGNED64     U64;
               REAL32         R32;
               REAL64         R64;
               }              OD_testVar_t;

");

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

                    if(od.index==0x2120)
                    {
                        int x = 0;
                        x++;
                    }

                    DataType dt = od.datatype;

                    if(dt==DataType.UNKNOWN)
                    {

                    }


                    file.WriteLine(string.Format("/*{0:x4}, Data Type: {1}, Array[{2}] */", od.index, t.ToString(),od.nosubindexes-1));
                    file.WriteLine(string.Format("        #define OD_{0}             {1}.{2}", od.paramater_cname(), loc, od.paramater_cname()));
                    file.WriteLine(string.Format("        #define ODL_{0}_arrayLength             {1}", od.paramater_cname(),od.nosubindexes-1));


                    if (od.objecttype != ObjectType.ARRAY)
                    {
                        List <string> ODAs = new List<string>();

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


                            ODAout+=(string.Format("        #define ODA_{0}_{1}       {2}\r\n", od.paramater_cname(), sub.paramater_cname(), sub.subindex));

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

            //TODO export Record types here


            file.WriteLine(@"/*******************************************************************************
   SDO SERVER ACCESS FUNCTIONS WITH USER CODE
*******************************************************************************/
#define WRITING (dir == 1)
#define READING (dir == 0)

");

            //TODO Export SDO Server access functions here

            //FIXME CO_OD_NoOfElelemts

            file.WriteLine(@"/*******************************************************************************
   OBJECT DICTIONARY
*******************************************************************************/
const sCO_OD_object CO_OD[CO_OD_NoOfElements] = {
");


            //TODO Export OD

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                string loc = getlocation(od.location);

                byte flags = 0; //fixme magic numbers below
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
                    flags |= 0x30; //fix me no control over rx and tx mapping, its both or none

                //fix me we need a detect cos flag from libeds
                //if(DETECT COS)
                  //  flags |=0x40;

              
                int datasize = 0;

                DataType t = eds.getdatatype(od);

                switch (t)
                {
                    case DataType.BOOLEAN:
                    case DataType.INTEGER8:
                    case DataType.UNSIGNED8:
                        datasize = 1;
                        break;
                    case DataType.UNSIGNED16:
                    case DataType.INTEGER16:
                        datasize = 2;
                        break;
                    case DataType.INTEGER32:
                    case DataType.REAL32:
                    case DataType.UNSIGNED32:
                        datasize = 4;
                        break;

                }

                //fix me detect multibyte is this the only case???
                if(datasize>1)
                   flags |= 0x80;

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
            
            if(defaultvalue.Contains("$NODEID"))
            {
                defaultvalue=defaultvalue.Replace("$NODEID", "");
                defaultvalue=defaultvalue.Replace("+", "");
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
            
            if(nodeidreplace)
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
                    return String.Format("0x{0:x4}L", Convert.ToInt32(defaultvalue,nobase));

                case DataType.REAL32:
                case DataType.REAL64:
                    return (String.Format("{0}", defaultvalue));
 

                    //fix me this looks wrong
                case DataType.UNICODE_STRING:
                    return (String.Format("'{0}'", defaultvalue));

                case DataType.VISIBLE_STRING:
                {
                    string array="{";
                    foreach (char s in defaultvalue)
                    {
                        array+="'"+s+"'";

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
                    foreach(string s in bits)
                    {
                        octet+=formatvaluewithdatatype(s, DataType.UNSIGNED8);

                        if (!object.ReferenceEquals(s,bits.Last())) 
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

        void export_OD_def_array(StreamWriter file,StorageLocation location)
        {

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if ((od.location != location))
                {
                    if(!(od.location==0 && location==StorageLocation.RAM))
                        continue;
                }

                if (od.nosubindexes == 0)
                {
                    file.WriteLine(string.Format("/*{0:x4}*/ {1},", od.index, formatvaluewithdatatype(od.defaultvalue, od.datatype)));
                }
                else
                {
                    file.Write(string.Format("/*{0:x4}*/ {{", od.index));

                    foreach(KeyValuePair<UInt16,ODentry> kvp2 in od.subobjects)
                    {
                        ODentry sub = kvp2.Value;

                        DataType dt = sub.datatype;

                        if (od.objecttype == ObjectType.REC && sub.subindex == 0)
                            continue;

                        if(od.objecttype == ObjectType.REC)
                            dt=od.datatype;

                        file.Write(formatvaluewithdatatype(sub.defaultvalue, dt));

                        if (od.subobjects.Keys.Last() != kvp2.Key)
                                file.Write(", ");
                        }
                
                    file.WriteLine("},");
                }
            }


        }
       

    }
}

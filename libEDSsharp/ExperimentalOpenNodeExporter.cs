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

    public class ExperimentalCanOpenNodeExporter : IExporter
    {

        private string folderpath;
        private string gitVersion;
        protected EDSsharp eds;

        Dictionary<UInt16, String> defstructnames = new Dictionary<ushort, string>();

        /// <summary>
        /// export the current data set in the Experimental CanOpen Node format
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="filename"></param>
        /// <param name="gitVersion"></param>
        /// <param name="eds"></param>
        public void export(string folderpath, string filename, string gitVersion, EDSsharp eds,string odname)
        {
            this.folderpath = folderpath;
            this.gitVersion = gitVersion;
            this.eds = eds;

            //New stub to handle new ODInterface export!

            export_h(filename,odname);
            export_c(filename,odname);


        }

        /// <summary>
        /// Export the header file
        /// </summary>
        /// <param name="filename"></param>
        public void export_h(string filename,string odname)
        {

            if (filename == "")
                filename = "CO_OD";

            filename = string.Format("{0}_{1}", filename,odname);

            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + filename + ".h");

            file.WriteLine(string.Format(@"
/*
*******************************************************************************
 CANopen Object Dictionary.

   This file was automatically generated with 
   libedssharp Object`Dictionary Editor v{0}

DON'T EDIT THIS FILE MANUALLY !!!!
*******************************************************************************
*/
",this.gitVersion));

            generate_defstructs_for_records(file);

            foreach (string location in eds.storageLocation)
            {
                if (location == "Unused")
                    continue;

                generate_main_OD_struct(file, location,odname);
            }

            file.WriteLine(@"
/*
*******************************************************************************
   Externs
*******************************************************************************
*/
");

            foreach (string location in eds.storageLocation)
            {
                if (location == "Unused")
                    continue;


                file.WriteLine("extern OD_{0}_{1}_t OD_{2}_{1};",location,odname,location);
            }

            file.WriteLine(@"
/*
*******************************************************************************
   Defines
*******************************************************************************
*/
");

            /*
#define ODxyz_1000_deviceType &ODxyz.list[0]
#define ODxyz_1001_errorRegister &ODxyz.list[1]
#define ODxyz_1018_identity &ODxyz.list[2]
*/

            


            file.Close();
        }

        public void generate_main_OD_struct(StreamWriter file, string location,string odname)
        {


            file.WriteLine(string.Format(@"
/*
*******************************************************************************
   Main object dictionary structure for storage location {0}
*******************************************************************************
*/
            ",location));

            //start of main OD struct
            file.WriteLine("typedef struct {");

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                if (od.StorageLocation != location)
                    continue;

                switch (od.objecttype)
                {
                    case ObjectType.VAR:
                        file.WriteLine(string.Format("    {0} x{1:x4}_{2};", get_c_data_type(od.datatype), od.Index, make_cname(od)));
                        break;

                    case ObjectType.ARRAY:
                        file.WriteLine(string.Format("    {0} x{1:x4}_{2}[{3}];", get_c_data_type(od.datatype), od.Index, make_cname(od), od.Nosubindexes));
                        break;

                    case ObjectType.REC:

                        if (defstructnames.ContainsKey(od.Index))
                        {
                            file.WriteLine(string.Format("    {0} x{1:x4}_{2};", defstructnames[od.Index], od.Index,make_cname(od)));
                        }
                        else
                        {
                            //I'm not quite sure why the struct is not already defined, this might be unused code.
                            file.WriteLine("    struct {");
                            foreach (ODentry subod in od.subobjects.Values)
                            {
                                file.WriteLine(string.Format("        {0} {1};", get_c_data_type(subod.datatype), make_cname(subod)));
                            }

                            file.WriteLine(string.Format("    }} {0}", make_cname(od)));
                        }
                        break;                      
                }

            }
            file.WriteLine(string.Format("}} OD_{0}_{1}_t;",location,odname)); //fixme static name
        }

        /// <summary>
        /// Generate the structs that are used
        /// </summary>
        public void generate_defstructs_for_records(StreamWriter file)
        {

            file.WriteLine(@"
/*
*******************************************************************************
   Structure definitions for record objects
*******************************************************************************
*/
");


            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if (od.Disabled == true)
                    continue;

                if (od.objecttype != ObjectType.REC)
                    continue;

                string structname = string.Format("{0}_t", make_cname(od));

                // fixme we need to do better that just looking at the same
                // the records may have different content in which case we have a
                // problem to deal with
                if (defstructnames.ContainsValue(structname))
                {
                    defstructnames.Add(od.Index, structname);
                    continue;
                }

                file.WriteLine("typedef struct {");
                foreach (ODentry subod in od.subobjects.Values)
                {
                    file.WriteLine(string.Format("    {0} {1};", get_c_data_type(subod.datatype), make_cname(subod)));
                }

                //Keep a record of generated struct names so we can reuse them in main OD struct
                defstructnames.Add(od.Index, structname);

                file.WriteLine(string.Format("    }} {0};\n",structname ));

            }

        }


            /// <summary>
            /// Export the c file
            /// </summary>
            /// <param name="filename"></param>
            public void export_c(string filename,string odname)
            {

            if (filename == "")
                filename = "CO_OD";

            filename = string.Format("{0}_{1}", filename, odname);

            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + filename + ".c");

            file.WriteLine(string.Format(@"
/*
*******************************************************************************
 CANopen Object Dictionary.

   This file was automatically generated with 
   libedssharp Object`Dictionary Editor v{0}

DON'T EDIT THIS FILE MANUALLY !!!!
*******************************************************************************
*/

#define OD_DEFINITION
#include ""301/CO_ODinterface.h""
#include ""{1}.h""

", this.gitVersion,filename));


            file.WriteLine(@"
/*
*******************************************************************************
   Default values
*******************************************************************************
*/
");


            foreach (string location in eds.storageLocation)
            {

                if (location == "Unused")
                    continue;

                file.WriteLine("OD_{0}_{1}_t OD_{2}_{1} = {{", location, odname, location);

                foreach (ODentry od in eds.ods.Values)
                {
                    if (od.Disabled == true)
                        continue;

                    if (od.StorageLocation != location)
                        continue;

                    switch(od.objecttype)
                    {
                        case ObjectType.VAR:
                            file.WriteLine("    .x{0:x4}_{1} = {2},", od.Index, make_cname(od), formatvaluewithdatatype(od.defaultvalue,od.datatype));
                            break;

                        case ObjectType.ARRAY:
                            for(int x=1;x<od.subobjects.Count;x++)
                            {
                                file.WriteLine("    .x{0:x4}_{1}[{2}] = {3},", od.Index, make_cname(od),x-1, formatvaluewithdatatype(od.defaultvalue, od.datatype));
                            }
                            break;

                        case ObjectType.REC:

                            file.WriteLine("    .x{0:x4}_.{1} = {{", od.Index, make_cname(od));

                            foreach (ODentry subod in od.subobjects.Values)
                            {
                                file.WriteLine("        {1} = {2},", subod.Index, make_cname(subod), formatvaluewithdatatype(subod.defaultvalue, subod.datatype));
                            }

                            file.WriteLine("},");

                            break;

                    }

                }

                file.WriteLine("};\n\n");
            }

            file.WriteLine(@"
/*
*******************************************************************************
   Dictionary Configuration
*******************************************************************************
*/
");





            file.Close();
        }


        /// <summary>
        /// Take a paramater name from the object dictionary and make it acceptable
        /// for use in c variables/structs etc
        /// </summary>
        /// <param name="entry">string, name to convert</param>
        /// <returns>string</returns>
        protected string make_cname(ODentry entry)
        {
            string name = entry.parameter_name;

            if (name == null)
                return null;

            if (name == "")
                return "";

            Regex splitter = new Regex(@"[\W]+");

            var bits = splitter.Split(name).Where(s => s != String.Empty);

            string output = "";

            char lastchar = ' ';
            foreach (string s in bits)
            {
                if (Char.IsUpper(lastchar) && Char.IsUpper(s.First()))
                    output += "_";

                if (s.Length > 1)
                {
                    output += char.ToUpper(s[0]) + s.Substring(1);
                }
                else
                {
                    output += s;
                }

                if (output.Length > 0)
                    lastchar = output.Last();

            }

            if (output.Length > 1)
            {
                if (Char.IsLower(output[1]))
                    output = Char.ToLower(output[0]) + output.Substring(1);
            }
            else
                output = output.ToLower(); //single character

            //Do we need to apply the canopennode acceptable names filter here to prevent bad OD setup breaking
            //compile?

            return output;
        }

        /// <summary>
        /// Choose the correct c data type based on CANOPEN type
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        string get_c_data_type(DataType dt)
        {
            //TODO finish this list
            switch(dt)
            {
                case DataType.INTEGER8:
                    return "int8_t";
                case DataType.INTEGER16:
                    return "int16_t";
                case DataType.INTEGER24:
                case DataType.INTEGER32:
                    return "int32_t";
                case DataType.INTEGER40:
                case DataType.INTEGER48:
                case DataType.INTEGER56:
                case DataType.INTEGER64:
                    return "int64_t";

                case DataType.UNSIGNED8:
                    return "uint8_t";
                case DataType.UNSIGNED16:
                    return "uint16_t";
                case DataType.UNSIGNED24:
                case DataType.UNSIGNED32:
                    return "uint32_t";
                case DataType.UNSIGNED40:
                case DataType.UNSIGNED48:
                case DataType.UNSIGNED56:
                case DataType.UNSIGNED64:
                    return "uint64_t";

                case DataType.BOOLEAN:
                    return "bool_t";

                case DataType.REAL32:
                    return "float";

                case DataType.REAL64:
                    return "double";


                case DataType.OCTET_STRING:
                    return "ochar_t";

                case DataType.VISIBLE_STRING:
                    return "char_t";

                case DataType.DOMAIN:
                    return "// DOMAIN";

                //TODO these datatypes
                case DataType.UNICODE_STRING:
                case DataType.TIME_DIFFERENCE:
                case DataType.TIME_OF_DAY:
               
                case DataType.IDENTITY:

                default:
                    return "BROKEN EXPORTER";

            }

        }


        string formatvaluewithdatatype(string defaultvalue, DataType dt, bool fixstring = false)
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

                        if (fixstring == true)
                            return "'X'";

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
            catch (Exception e)
            {
                Warnings.AddWarning(String.Format("Error converting value {0} to type {1}", defaultvalue, dt.ToString()), Warnings.warning_class.WARNING_BUILD);
                return "";
            }
        }


    }
}

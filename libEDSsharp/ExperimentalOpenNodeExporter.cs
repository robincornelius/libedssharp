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
        public void export(string folderpath, string filename, string gitVersion, EDSsharp eds)
        {
            this.folderpath = folderpath;
            this.gitVersion = gitVersion;
            this.eds = eds;

            //New stub to handle new ODInterface export!

            export_h(filename);
            export_c(filename);


        }

        /// <summary>
        /// Export the header file
        /// </summary>
        /// <param name="filename"></param>
        public void export_h(string filename)
        {
            /*
             * typedef struct {
                uint32_t x1000_deviceType;
                uint8_t x1001_errorRegister;
                struct {
                    uint8_t maxSubIndex;
                    uint32_t vendorID;
                    uint32_t productCode;
                    uint32_t revisionNumber;
                    uint32_t serialNumber;
                } x1018_identity;
            } ODxyz_0_t;
            */

            if (filename == "")
                filename = "CO_OD";

            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + filename + ".h");

            generate_defstructs_for_records(file);

            foreach (string location in eds.storageLocation)
            {
                generate_main_OD_struct(file, location);
            }

            file.Close();
        }

        public void generate_main_OD_struct(StreamWriter file, string location)
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
            file.WriteLine(string.Format("}} OD_{0}_0_t",location)); //fixme static name
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

                file.WriteLine("struct {");
                foreach (ODentry subod in od.subobjects.Values)
                {
                    file.WriteLine(string.Format("    {0} {1};", get_c_data_type(subod.datatype), make_cname(subod)));
                }

                //Keep a record of generated struct names so we can reuse them in main OD struct
                defstructnames.Add(od.Index, structname);

                file.WriteLine(string.Format("    }} {0}\n",structname ));

            }

        }


            /// <summary>
            /// Export the c file
            /// </summary>
            /// <param name="filename"></param>
            public void export_c(string filename)
        {
            if (filename == "")
                filename = "CO_OD";

            StreamWriter file = new StreamWriter(folderpath + Path.DirectorySeparatorChar + filename + ".c");



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


    }
}

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

namespace libEDSsharp
{
    public class DocumentationGen
    {
        StreamWriter file = null;

        public void genhtmldoc(string filepath, EDSsharp eds)
        {

            file = new StreamWriter(filepath, false);

           file.Write("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\" /></head><body>");

           file.Write(string.Format("<h1> {0} Documentation </h1>",eds.di.ProductName));

           file.Write("<h2>Device Information</h2>");

           file.Write("<table id=\"deviceinfo\">");
           write2linetableheader("Product name", eds.di.ProductName);
           write2linetableheader("Product number", eds.di.ProductNumber);
           write2linetableheader("Revision number", eds.di.RevisionNumber);
           write2linetableheader("Vendor name", eds.di.VendorName);
           file.Write("</table>");

           file.Write("<h2>Mandatory objects</h2>");
         
           foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
           {
                ODentry od = kvp.Value;
                if (od.Disabled == true)
                    continue;

                if (od.Index == 0x1000 || od.Index == 0x1001 || od.Index == 0x1018)
                {
                    writeODentryhtml(od);
                }
            }

            file.Write("<h2>Optional objects</h2>");

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                if (od.Disabled == true)
                    continue;

                if ((od.Index > 0x1001 && od.Index != 0x1018 && od.Index<0x2000) || od.Index>=0x6000)
                {
                    writeODentryhtml(od);
                }
            }

            file.Write("<h2>Manufacturer specific objects</h2>");

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                if (od.Disabled == true)
                    continue;

                if (od.Index >= 0x2000 && od.Index<0x6000)
                {
                    writeODentryhtml(od);
                }
            }


            file.Write("</body></html>");

           file.Close();


        }

        public void writeODentryhtml(ODentry od)
        {
            if (od.parent == null)
            {
                file.Write("<hr/>");
                file.Write(String.Format("<h3>0x{0:x4} - {1}</h3>", od.Index, od.parameter_name));
            }
            else
            {
                file.Write(String.Format("<h3>0x{0:x4} sub 0x{2:x2} - {1}</h3>", od.Index, od.parameter_name,od.Subindex));
            }

            file.Write("<table id=\"odentry\">");
            write2linetableheader("Parameter", "Value");

            ObjectType ot = od.objecttype;
            if (ot == ObjectType.UNKNOWN && od.parent != null)
                ot = od.parent.objecttype;

            write2linetablerow("Object Type", ot.ToString());

            DataType dt = od.datatype;
            if (dt == DataType.UNKNOWN && od.parent != null)
                dt = od.parent.datatype;

            write2linetablerow("Data Type", dt.ToString());
            write2linetablerow("Default Value", od.defaultvalue);

            write2linetablerow("Location", od.StorageLocation);
            write2linetablerow("Access type", od.accesstype.ToString());
            write2linetablerow("PDO mapping", od.PDOMapping);
            write2linetablerow("No Sub index", od.Nosubindexes);

            file.Write("</table>");

            string description = od.Description;
            file.Write(string.Format("<pre>{0}</pre>", description));
     
            foreach (KeyValuePair<UInt16,ODentry> sub in od.subobjects)
            {
                ODentry subod = sub.Value;
                writeODentryhtml(subod);
            }
           
        }

        public void write2linetablerow(string a,object b)
        {
            if (b == null)
                b = "";
            file.Write("<tr><td>{0}</td><td>{1}</td></tr>", a, b.ToString());
        }

        public void write2linetableheader(string a, object b)
        {
            file.Write("<tr><th>{0}</th><th>{1}</th></tr>",a,b.ToString());
        }

        public void genmddoc(string filepath, EDSsharp eds)
        {

            file = new StreamWriter(filepath, false);

            file.WriteLine();
            file.WriteLine("# Device Information");
            file.WriteLine();

            file.WriteLine("Product name");
            file.WriteLine($"  ~ {eds.di.ProductName}");
            file.WriteLine();
            file.WriteLine("Product number");
            file.WriteLine($"  ~ {eds.di.ProductNumber}");
            file.WriteLine();
            file.WriteLine("Revision number");
            file.WriteLine($"  ~ {eds.di.RevisionNumber}");
            file.WriteLine();
            file.WriteLine("Vendor name");
            file.WriteLine($"  ~ {eds.di.VendorName}");
            file.WriteLine();


            file.WriteLine($"Granularity: {eds.di.Granularity}");
            file.WriteLine();
            file.WriteLine("Supported Baud rates:");
            file.WriteLine();

            file.WriteLine($"- [{(eds.di.BaudRate_1000 ? "x" : " ")}] 1000 kBit/s");
            file.WriteLine($"- [{(eds.di.BaudRate_800 ? "x" : " ")}] 800 kBit/s");
            file.WriteLine($"- [{(eds.di.BaudRate_500 ? "x" : " ")}] 500 kBit/s");
            file.WriteLine($"- [{(eds.di.BaudRate_250 ? "x" : " ")}] 250 kBit/s");
            file.WriteLine($"- [{(eds.di.BaudRate_125 ? "x" : " ")}] 125 kBit/s");
            //file.WriteLine($"- [{(true ? "x" : " ")}] 100 kBit/s");
            file.WriteLine($"- [{(eds.di.BaudRate_50 ? "x" : " ")}] 50 kBit/s");
            file.WriteLine($"- [{(eds.di.BaudRate_20 ? "x" : " ")}] 20 kBit/s");
            file.WriteLine($"- [{(eds.di.BaudRate_10 ? "x" : " ")}] 10 kBit/s");
            file.WriteLine();

            file.WriteLine("# PDO Mapping");
            file.WriteLine();

            PrintPdo(0x1600, 0x1800, "RPDO", eds);
            PrintPdo(0x1a00, 0x1c00, "TPDO", eds);

            file.WriteLine("# Mandatory objects");
            file.WriteLine();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                if (od.Disabled == true)
                    continue;

                if (od.Index == 0x1000 || od.Index == 0x1001 || od.Index == 0x1018)
                {
                    writeODentrymd(od);
                }
            }

            file.WriteLine("# Optional objects");
            file.WriteLine();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                if (od.Disabled == true)
                    continue;

                if ((od.Index > 0x1001 && od.Index != 0x1018 && od.Index < 0x2000) || od.Index >= 0x6000)
                {
                    writeODentrymd(od);
                }
            }

            file.WriteLine("# Manufacturer specific objects");
            file.WriteLine();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                if (od.Disabled == true)
                    continue;

                if (od.Index >= 0x2000 && od.Index < 0x6000)
                {
                    writeODentrymd(od);
                }
            }

            file.Close();
        }

        public void writeODentrymd(ODentry od)
        {
            if (od.parent == null)
            {
                file.WriteLine(String.Format("## 0x{0:x4} - {1}", od.Index, od.parameter_name));
            }
            else
            {
                file.WriteLine(String.Format("### 0x{0:x4} sub 0x{2:x2} - {1}", od.Index, od.parameter_name, od.Subindex));
            }
            file.WriteLine();
           
            write2linetableheadermd("Parameter", "Value");

            ObjectType ot = od.objecttype;
            if (ot == ObjectType.UNKNOWN && od.parent != null)
                ot = od.parent.objecttype;

            write2linetablerowmd("Object Type", ot.ToString());

            DataType dt = od.datatype;
            if (dt == DataType.UNKNOWN && od.parent != null)
                dt = od.parent.datatype;

            write2linetablerowmd("Data Type", dt.ToString());
            write2linetablerowmd("Default Value", od.defaultvalue);
            if (!String.IsNullOrEmpty(od.HighLimit))
                write2linetablerowmd("High Value", od.HighLimit);
            if (!String.IsNullOrEmpty(od.LowLimit))
                write2linetablerowmd("Low Value", od.LowLimit);

            write2linetablerowmd("Location", od.StorageLocation);
            write2linetablerowmd("Access type", od.accesstype.ToString());
            write2linetablerowmd("PDO mapping", od.PDOMapping);
            if (od.parent == null)
                write2linetablerowmd("No Sub index", od.Nosubindexes);

            
            file.WriteLine();

            string description = od.Description;
            file.WriteLine($"{description}");

            file.WriteLine();

            foreach (KeyValuePair<UInt16, ODentry> sub in od.subobjects)
            {
                ODentry subod = sub.Value;
                writeODentrymd(subod);
            }

            if (od.parent == null)
            {
                file.WriteLine("---------------");
                file.WriteLine();
            }
        }

        private void PrintPdo(ushort start, ushort end, string caption, EDSsharp eds)
        {
            file.WriteLine("## {0}", caption);
            file.WriteLine();
            foreach (var kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                int index = kvp.Key;

                if (od.Disabled)
                    continue;

                if (od.Index >= start && od.Index < end)
                {
                    file.WriteLine(string.Format("### {0} {1:x4}", caption, od.Index));
                    file.WriteLine();
                    byte current_bit = 0;
                    foreach (var kvp2 in od.subobjects)
                    {
                        ODentry odsub = kvp2.Value;
                        ushort subindex = kvp2.Key;

                        if (subindex == 0)
                            continue;

                        var data = Convert.ToUInt32(odsub.defaultvalue, EDSsharp.Getbase(odsub.defaultvalue));

                        if (data != 0)
                        {
                            byte datasize = (byte)(data & 0x000000FF);
                            ushort pdoindex = (ushort)((data >> 16) & 0x0000FFFF);
                            byte pdosub = (byte)((data >> 8) & 0x000000FF);

                            file.Write($"* Byte{current_bit / 8}: ");

                            if (eds.ods.ContainsKey(pdoindex) && (pdosub == 0 || eds.ods[pdoindex].Containssubindex(pdosub)))
                            {
                                ODentry maptarget;
                                if (pdosub == 0)
                                    maptarget = eds.ods[pdoindex];
                                else
                                    maptarget = eds.ods[pdoindex].Getsubobject(pdosub);

                                if (!maptarget.Disabled && datasize == (8 * maptarget.Sizeofdatatype()))
                                {
                                    if (maptarget.parent == null)
                                        file.Write(string.Format("[0x{0:x4} - {1}]", maptarget.Index, maptarget.parameter_name));
                                    else
                                        file.Write(string.Format("[0x{0:x4} sub 0x{1:x2} - {2}]", maptarget.Index, maptarget.Subindex, maptarget.parameter_name));
                                }
                            }
                            file.WriteLine();
                            var by = (datasize / 8);
                            for (int i = 1; i < by; ++i)
                                file.WriteLine($"* Byte{current_bit / 8 + i}: -\"-");
                            current_bit += datasize;
                        }
                    }
                    for (int i = (current_bit / 8); i < 8; ++i)
                        file.WriteLine($"* Byte{i}: empty");
                    file.WriteLine();
                }
            }
        }

        public void write2linetablerowmd(string a, object b)
        {
            if (b == null)
                b = "";
            file.WriteLine($"| {a} | {b.ToString()} |");
        }

        public void write2linetableheadermd(string a, object b)
        {
            write2linetablerowmd(a, b);
            file.WriteLine("|:------|:-----|");
        }

    }
}

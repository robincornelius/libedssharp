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

           file.Write("<!DOCTYPE html><html><head></head><body>");

           file.Write(string.Format("<h1> {0} Documementation </h1>",eds.di.ProductName));

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

                if (od.index == 0x1000 || od.index == 0x1001 || od.index == 0x1018)
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

                if ((od.index > 0x1001 && od.index != 0x1018 && od.index<0x2000) || od.index>=0x6000)
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

                if (od.index >= 0x2000 && od.index<0x6000)
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
                file.Write(String.Format("<h3>0x{0:x4} - {1}</h3>", od.index, od.parameter_name));
            }
            else
            {
                file.Write(String.Format("<h3>0x{0:x4} sub 0x{2:x2} - {1}</h3>", od.index, od.parameter_name,od.subindex));
            }

            file.Write("<table id=\"odentry\">");
            write2linetableheader("Paramater", "Value");

            ObjectType ot = od.objecttype;
            if (ot == ObjectType.UNKNOWN && od.parent != null)
                ot = od.parent.objecttype;

            write2linetablerow("Object Type", ot.ToString());

            DataType dt = od.datatype;
            if (dt == DataType.UNKNOWN && od.parent != null)
                dt = od.parent.datatype;

            write2linetablerow("Data Type", dt.ToString());
            write2linetablerow("Default Value", od.defaultvalue);

            write2linetablerow("Location", od.location.ToString());
            write2linetablerow("Access type", od.accesstype.ToString());
            write2linetablerow("PDO mapping", od.PDOMapping);
            write2linetablerow("No Sub index", od.nosubindexes);

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



    }
}

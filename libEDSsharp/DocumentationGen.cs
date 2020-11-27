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
    Copyright(c) 2020 Janez Paternoster
*/

using System;
using System.Collections.Generic;
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
                if (od.prop.CO_disabled == true)
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
                if (od.prop.CO_disabled == true)
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
                if (od.prop.CO_disabled == true)
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

            write2linetablerow("Location", od.prop.CO_storageGroup);
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

        public void genmddoc(string filepath, EDSsharp eds, string gitVersion)
        {
            file = new StreamWriter(filepath, false);
            file.NewLine = "\n";

            file.WriteLine(string.Format(
@"CANopen documentation
=====================
**{0}**

{1}

|              |                                |
| ------------ | ------------------------------ |
| Project File | {2,-30} |
| File Version | {3,-30} |
| Created      | {4,-30} |
| Created By   | {5,-30} |
| Modified     | {6,-30} |
| Modified By  | {7,-30} |

This file was automatically generated with [libedssharp](https://github.com/robincornelius/libedssharp) Object Dictionary Editor v{8}

* [Device Information](#device-information)* [PDO Mapping](#pdo-mapping)* [Communication Specific Parameters](#communication-specific-parameters)* [Manufacturer Specific Parameters](#manufacturer-specific-parameters)* [Device Profile Specific Parameters](#device-profile-specific-parameters)",
            eds.di.ProductName, eds.fi.Description,
            Path.GetFileName(eds.projectFilename), eds.fi.FileVersion,
            eds.fi.CreationDateTime, eds.fi.CreatedBy, eds.fi.ModificationDateTime, eds.fi.ModifiedBy,
            gitVersion));

            file.WriteLine(string.Format(@"
Device Information {{#device-information}}
----------------------------------------
|              |                                |
| ------------ | ------------------------------ |
| Vendor Name  | {0,-30} |
| Vendor ID    | {1,-30} |
| Product Name | {2,-30} |
| Product ID   | {3,-30} |
| Granularity  | {4,-30} |
| RPDO count   | {5,-30} |
| TPDO count   | {6,-30} |
| LSS Slave    | {7,-30} |
| LSS Master   | {8,-30} |
",          eds.di.VendorName, eds.di.VendorNumber, eds.di.ProductName, eds.di.ProductNumber,
            eds.di.Granularity, eds.di.NrOfRXPDO.ToString(), eds.di.NrOfTXPDO.ToString(),
            eds.di.LSS_Supported, eds.di.LSS_Master));

            file.WriteLine($"#### Supported Baud rates");
            file.WriteLine($"* [{(eds.di.BaudRate_10 ? "x" : " ")}] 10 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_20 ? "x" : " ")}] 20 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_50 ? "x" : " ")}] 50 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_125 ? "x" : " ")}] 125 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_250 ? "x" : " ")}] 250 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_500 ? "x" : " ")}] 500 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_800 ? "x" : " ")}] 800 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_1000 ? "x" : " ")}] 1000 kBit/s");
            file.WriteLine($"* [{(eds.di.BaudRate_auto ? "x" : " ")}] auto");
            file.WriteLine();
            file.WriteLine();

            file.WriteLine("PDO Mapping {#pdo-mapping}");
            file.WriteLine("--------------------------");
            PrintPdoMd(eds);

            int chapter = 0;
            foreach (ODentry od in eds.ods.Values)
            {
                if (chapter == 0)
                {
                    file.WriteLine();
                    file.WriteLine("Communication Specific Parameters {#communication-specific-parameters}");
                    file.WriteLine("----------------------------------------------------------------------");
                    chapter++;
                }
                if (chapter == 1 && od.Index >= 0x2000)
                {
                    file.WriteLine();
                    file.WriteLine("Manufacturer Specific Parameters {#manufacturer-specific-parameters}");
                    file.WriteLine("--------------------------------------------------------------------");
                    chapter++;
                }
                if (chapter == 2 && od.Index >= 0x6000)
                {
                    file.WriteLine();
                    file.WriteLine("Device Profile Specific Parameters {#device-profile-specific-parameters}");
                    file.WriteLine("------------------------------------------------------------------------");
                    chapter++;
                }

                if (!od.prop.CO_disabled)
                    PrintODentryMd(od);
            }

            file.Close();
        }

        private void PrintPdoMd(EDSsharp eds, bool skipDisabled = false)
        {
            var parameters = new SortedDictionary<UInt16, ODentry>();
            var mappings = new SortedDictionary<UInt16, ODentry>();
            foreach (ODentry od in eds.ods.Values)
            {
                if (od.Index < 0x1400 || od.prop.CO_disabled || od.objecttype != ObjectType.REC || od.subobjects.Count < 3)
                    continue;
                else if (od.Index < 0x1600)
                    parameters.Add(od.Index, od);
                else if (od.Index < 0x1800)
                    mappings.Add(od.Index, od);
                else if (od.Index < 0x1A00)
                    parameters.Add(od.Index, od);
                else if (od.Index < 0x1C00)
                    mappings.Add(od.Index, od);
                else
                    break;
            }

            foreach (ODentry par in parameters.Values)
            {
                UInt16 mappingIndex = (UInt16)(par.Index + 0x200);

                if (!mappings.ContainsKey(mappingIndex))
                    continue;

                ODentry map = mappings[mappingIndex];
                uint mapCount;

                // skip, if PDO (is disabled and) has no mapped entries
                try
                {
                    if (skipDisabled)
                    {
                        string cobId = par.subobjects[1].defaultvalue.Replace("$NODEID", "").Replace("+", "");
                        uint cobIdNum = (uint)new System.ComponentModel.UInt32Converter().ConvertFromString(cobId);
                        if ((cobIdNum & 0x80000000) != 0)
                            continue;
                    }
                    mapCount = (uint)new System.ComponentModel.UInt32Converter().ConvertFromString(map.subobjects[0].defaultvalue);
                    if (mapCount == 0)
                        continue;
                }
                catch (Exception)
                {
                    continue;
                }

                string transmissionType = $"type={par.subobjects[2].defaultvalue}";
                string PDO;

                if (par.Index < 0x1600)
                {
                    PDO = "RPDO";
                    if (par.subobjects.Count >= 6)
                        transmissionType += $"; event-timer={par.subobjects[5].defaultvalue}";
                }
                else
                {
                    PDO = "TPDO";
                    if (par.subobjects.Count >= 6)
                        transmissionType += $"; inhibit-time={par.subobjects[3].defaultvalue}; event-timer={par.subobjects[5].defaultvalue}";
                    if (par.subobjects.Count >= 7)
                        transmissionType += $"; SYNC-start-value={par.subobjects[6].defaultvalue}";
                }

                file.WriteLine(string.Format(@"
### {0} 0x{1:X4}
|              |                                                               |
| ------------ | ------------------------------------------------------------- |
| COB_ID       | {2,-62}|
| Transmission | {3,-62}|",
                    PDO, par.Index, par.subobjects[1].defaultvalue, transmissionType));

                for (byte subIdxPdo = 1; subIdxPdo <= mapCount; subIdxPdo++)
                {
                    UInt32 mapVal;
                    try { mapVal = (UInt32)new System.ComponentModel.UInt32Converter().ConvertFromString(map.subobjects[subIdxPdo].defaultvalue); }
                    catch (Exception) { break; }

                    UInt16 mapIdx = (UInt16)(mapVal >> 16);
                    UInt16 mapSub = (UInt16)((mapVal >> 8) & 0xFF);

                    string nameIdx = "(MISSING)";
                    string nameSub = " (MISSING)";
                    if (eds.ods.ContainsKey(mapIdx))
                    {
                        ODentry odMapped = eds.ods[mapIdx];
                        nameIdx = odMapped.parameter_name;

                        if (odMapped.objecttype == ObjectType.VAR)
                            nameSub = "";
                        else if (odMapped.subobjects.ContainsKey(mapSub))
                            nameSub = " (" + odMapped.subobjects[mapSub].parameter_name + ")";
                    }
                    else if (mapIdx < 0x20)
                    {
                        nameIdx = ((DataType)mapIdx).ToString();
                        nameSub = "";
                    }

                    file.WriteLine(string.Format(@"|   0x{0:X8} | {1,-62}|", mapVal, nameIdx + nameSub));
                }
                file.WriteLine();
            }
        }

        private void PrintODentryMd(ODentry od)
        {
            var descriptions = new List<string>();

            file.WriteLine(string.Format(@"
### 0x{0:X4} - {1}
| Object Type | Count Label    | Storage Group  | IO extension  | PDO flags    |
| ----------- | -------------- | -------------- | ------------- | ------------ |
| {2,-12}| {3,-15}| {4,-15}| {5,-14}| {6,-13}|",
                 od.Index, od.parameter_name,
                 od.ObjectTypeString(), od.prop.CO_countLabel, od.prop.CO_storageGroup, od.prop.CO_extensionIO, od.prop.CO_flagsPDO));

            if (od.Description != null && od.Description != "")
                descriptions.Add(od.Description);

            if (od.objecttype == ObjectType.VAR)
            {
                file.WriteLine(string.Format(@"
| Data Type               | SDO | PDO | SRDO | Default Value                   |
| ----------------------- | --- | --- | ---- | ------------------------------- |
| {0,-24}| {1,-4}| {2,-4}| {3,-5}| {4,-32}|",
                    PrintDataType(od), od.AccessSDO().ToString(), od.AccessPDO().ToString(),
                    od.prop.CO_accessSRDO.ToString(), od.defaultvalue));
            }
            else
            {
                file.WriteLine(string.Format(@"
| Sub  | Name                  | Data Type  | SDO | PDO | SRDO | Default Value |
| ---- | --------------------- | ---------- | --- | --- | ---- | ------------- |"));

                foreach (ODentry subod in od.subobjects.Values)
                {
                    file.WriteLine(string.Format(@"| 0x{0:X2} | {1,-22}| {2,-11}| {3,-4}| {4,-4}| {5,-5}| {6,-14}|",
                        subod.Subindex, subod.parameter_name, PrintDataType(subod),
                        subod.AccessSDO().ToString(), subod.AccessPDO().ToString(),
                        subod.prop.CO_accessSRDO.ToString(), subod.defaultvalue));

                    if (subod.Description != null && subod.Description != "")
                        descriptions.Add(subod.Description);
                }
            }

            if (descriptions.Count > 0)
            {
                file.WriteLine();
                file.WriteLine(string.Join("\n", descriptions));
            }
        }

        private string PrintDataType(ODentry od)
        {
            string dt = od.datatype.ToString();
            if ((od.datatype == DataType.VISIBLE_STRING || od.datatype == DataType.UNICODE_STRING)
                && od.prop.CO_stringLengthMin > od.defaultvalue.Length)
            {
                dt += $" (len={od.prop.CO_stringLengthMin})";
            }

            return dt;
        }
    }
}

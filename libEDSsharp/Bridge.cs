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
using Xml2CSharp;

/* I know i'm going to regret this
 * 
 * I quite like my eds class as it trys to validate the EDS using typing and enums etc
 * but i also want the XML wrappers for the CanOpenXML
 * so i'm going to make a converter outside of both classes hence this bridge
 * which is more code to manage ;-(
 * */

namespace libEDSsharp
{
    public class Bridge
    {

        public Device convert(EDSsharp eds)
        {
            eds.updatePDOcount();

            Device dev = new Device();
            dev.CANopenObjectList = new CANopenObjectList();
            dev.CANopenObjectList.CANopenObject = new List<CANopenObject>();

            /* OBJECT DICTIONARY */

            foreach(KeyValuePair<UInt16,ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

               // if(od.subindex==-1)
                {
                    CANopenObject coo = new CANopenObject();
                    coo.Index =string.Format("{0:x4}",od.index);
                    coo.Name = od.parameter_name;
                    coo.ObjectType = od.objecttype.ToString();
                    coo.Disabled = od.Disabled.ToString().ToLower();
                    coo.MemoryType = od.location.ToString();
                    coo.AccessType = od.accesstype.ToString();
                    coo.DataType = string.Format("0x{0:x2}",(int)od.datatype);
                    coo.DefaultValue = od.defaultvalue;
                    coo.PDOmapping = od.PDOtype.ToString();
                    coo.TPDOdetectCOS = od.TPDODetectCos.ToString().ToLower();
                    coo.AccessFunctionPreCode = od.AccessFunctionPreCode;
                    coo.AccessFunctionName = od.AccessFunctionName;

                    coo.Description = new Description();
                    coo.Description.Text = od.Description;
                        
                    //if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
                    {
                        coo.SubNumber = od.nosubindexes.ToString(); //-1?? //check me 
                        coo.CANopenSubObject = new List<CANopenSubObject>();

                        foreach(KeyValuePair<UInt16,ODentry> kvp2 in od.subobjects)
                        {
                            ODentry subod = kvp2.Value;
                   
                            CANopenSubObject sub = new CANopenSubObject();

                            sub.Name = subod.parameter_name;
                            sub.ObjectType = subod.objecttype.ToString();
                            sub.AccessType = subod.accesstype.ToString();
                            sub.DataType = string.Format("0x{0:x2}", (int)subod.datatype);
                            sub.DefaultValue = subod.defaultvalue;
                            sub.PDOmapping = subod.PDOtype.ToString();
                            sub.SubIndex = String.Format("{0:x2}",subod.subindex);
                            sub.TPDOdetectCOS = subod.TPDODetectCos.ToString().ToLower();
                            coo.CANopenSubObject.Add(sub);
                                                 
                        }
                    }

                    dev.CANopenObjectList.CANopenObject.Add(coo);
                }

            }
  

            /* DUMMY USAGE */
            
            dev.Other = new Other();
            dev.Other.DummyUsage = new DummyUsage();
            dev.Other.DummyUsage.Dummy = new List<Dummy>();

            Dummy d; 

            d = new Dummy();
            d.Entry = eds.du.Dummy0001.ToString();
            dev.Other.DummyUsage.Dummy.Add(d);
            d = new Dummy();
            d.Entry = eds.du.Dummy0002.ToString();
            dev.Other.DummyUsage.Dummy.Add(d);
            d = new Dummy();
            d.Entry = eds.du.Dummy0003.ToString();
            dev.Other.DummyUsage.Dummy.Add(d);
            d = new Dummy();
            d.Entry = eds.du.Dummy0004.ToString();
            dev.Other.DummyUsage.Dummy.Add(d);
            d = new Dummy();
            d.Entry = eds.du.Dummy0005.ToString();
            dev.Other.DummyUsage.Dummy.Add(d);
            d = new Dummy();
            d.Entry = eds.du.Dummy0006.ToString();
            dev.Other.DummyUsage.Dummy.Add(d);
            d = new Dummy();
            d.Entry = eds.du.Dummy0007.ToString();
            dev.Other.DummyUsage.Dummy.Add(d);


            SupportedBaudRate baud = new SupportedBaudRate();
            dev.Other.BaudRate = new BaudRate();
            dev.Other.BaudRate.SupportedBaudRate = new List<SupportedBaudRate>();

            baud.Value="10 Kbps";
            if (eds.di.BaudRate_10 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            baud = new SupportedBaudRate();
            baud.Value = "20 Kbps";
            if (eds.di.BaudRate_20 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            baud = new SupportedBaudRate();
            baud.Value = "50 Kbps";
            if (eds.di.BaudRate_50 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            baud = new SupportedBaudRate();
            baud.Value = "125 Kbps";
            if (eds.di.BaudRate_125 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            baud = new SupportedBaudRate();
            baud.Value = "250 Kbps";
            if (eds.di.BaudRate_250 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            baud = new SupportedBaudRate();
            baud.Value = "500 Kbps";
            if (eds.di.BaudRate_500 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            baud = new SupportedBaudRate();
            baud.Value = "800 Kbps";
            if (eds.di.BaudRate_800 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            baud = new SupportedBaudRate();
            baud.Value = "1000 Kbps";
            if (eds.di.BaudRate_1000 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);


            dev.Other.DeviceIdentity = new DeviceIdentity();
            dev.Other.DeviceIdentity.ProductName = eds.di.ProductName;
            //dev.Other.DeviceIdentity.ProductText = new ProductText();
            //dev.Other.DeviceIdentity.ProductText.Description

            if (eds.di.concreteNodeId!=-1)
                dev.Other.DeviceIdentity.ConcreteNoideId = eds.di.concreteNodeId.ToString();

            dev.Other.DeviceIdentity.VendorName = eds.di.VendorName;

            //dev.Other.File = new Other.File();
            dev.Other.File = new File();

            dev.Other.File.FileName = eds.fi.FileName;

            dev.Other.File.FileCreationDate = eds.fi.CreationDate;
            dev.Other.File.FileCreationTime = eds.fi.CreationTime;
            dev.Other.File.FileCreator = eds.fi.CreatedBy;

            dev.Other.File.FileVersion = eds.fi.FileVersion.ToString();

            dev.Other.File.ExportFolder = eds.fi.exportFolder;

            return dev;
        }


        public EDSsharp convert(Device dev)
        {
            EDSsharp eds = new EDSsharp();
            
            foreach(CANopenObject coo in dev.CANopenObjectList.CANopenObject)
            {
                ODentry entry = new ODentry();
                entry.index = Convert.ToUInt16(coo.Index, 16);
                entry.parameter_name = coo.Name;
             
                if (coo.AccessType != null)
                {
                    string at = coo.AccessType;

                    //Nasty work around so we can use Enum types
                    at = at.Replace("const", "cons");
                    entry.accesstype = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), at);
                }

                if (coo.DataType != null)
                {
                    byte datatype = Convert.ToByte(coo.DataType, 16);
                    entry.datatype = (DataType)datatype;
                }
                else
                {
                    //CanOpenNode Project XML did not correctly set DataTypes for record sets

                    if (entry.index == 0x1018)
                        entry.datatype = DataType.IDENTITY;

                    if (entry.index >= 0x1200 && entry.index<0x1400) //check me is this the correct range??
                        entry.datatype = DataType.SDO_PARAMETER;


                    if (entry.index >= 0x1400 && entry.index < 0x1600) //check me is this the correct range??
                        entry.datatype = DataType.PDO_COMMUNICATION_PARAMETER;


                    if (entry.index >= 0x1600 && entry.index < 0x1800) //check me is this the correct range??
                        entry.datatype = DataType.PDO_MAPPING;


                    if (entry.index >= 0x1800 && entry.index < 0x1a00) //check me is this the correct range??
                        entry.datatype = DataType.PDO_COMMUNICATION_PARAMETER;


                    if (entry.index >= 0x1a00 && entry.index < 0x1c00) //check me is this the correct range??
                        entry.datatype = DataType.PDO_MAPPING;


                }

               
                entry.objecttype = (ObjectType)Enum.Parse(typeof(ObjectType),coo.ObjectType);

                entry.defaultvalue = coo.DefaultValue;
                //entry.nosubindexes = Convert.ToInt16(coo.SubNumber);

                if (coo.PDOmapping != null)
                    entry.PDOtype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), coo.PDOmapping);
                else
                    entry.PDOtype = PDOMappingType.no;

                entry.TPDODetectCos = coo.TPDOdetectCOS == "true";
                entry.AccessFunctionName = coo.AccessFunctionName;
                entry.AccessFunctionPreCode = coo.AccessFunctionPreCode;
                entry.Disabled = coo.Disabled == "true";

                if (coo.Description!=null)
                    entry.Description = coo.Description.Text; //FIXME URL/LANG

                if(coo.Label!=null)
                    entry.Label = coo.Label.Text; //FIXME LANG
                
                if(coo.MemoryType!=null)
                    entry.location = (StorageLocation)Enum.Parse(typeof(StorageLocation), coo.MemoryType);
                
                eds.ods.Add(entry.index, entry);

                if (entry.index == 0x1000 || entry.index==0x1001 || entry.index==0x1018)
                {
                    eds.md.objectlist.Add(eds.md.objectlist.Count+1,entry.index);
                }
                else
                if (entry.index >= 0x2000 && entry.index<0x6000)
                {
                    eds.mo.objectlist.Add(eds.mo.objectlist.Count+1,entry.index);
                }
                else
                {
                     eds.oo.objectlist.Add(eds.oo.objectlist.Count+1,entry.index);
                }

                
                foreach(CANopenSubObject coosub in coo.CANopenSubObject)
                {

                    ODentry subentry = new ODentry();

                    subentry.parameter_name = coosub.Name;
                    subentry.index = entry.index;

                    if(coosub.AccessType!=null)
                        subentry.accesstype = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), coosub.AccessType);

                    if (coosub.DataType != null)
                    {
                        byte datatype = Convert.ToByte(coosub.DataType, 16);
                        subentry.datatype = (DataType)datatype;
                    }

                    subentry.defaultvalue = coosub.DefaultValue;

                    subentry.subindex = Convert.ToUInt16(coosub.SubIndex, 16);
                    
                    if(coosub.PDOmapping!=null)
                        subentry.PDOtype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), coosub.PDOmapping);

                    if (entry.objecttype == ObjectType.ARRAY)
                    {
                        subentry.PDOtype = entry.PDOtype;
                    }

                    subentry.location = entry.location;
                    subentry.parent = entry;

                    subentry.objecttype = ObjectType.VAR;

                    if(coosub.TPDOdetectCOS!=null)
                    {
                        subentry.TPDODetectCos = coosub.TPDOdetectCOS == "true";  
                    }
                    else
                    {
                        if(coo.TPDOdetectCOS!=null)
                            subentry.TPDODetectCos = coo.TPDOdetectCOS == "true";
                    }
                       

                    entry.subobjects.Add(subentry.subindex,subentry);

                }
            }

            eds.du.Dummy0001 = dev.Other.DummyUsage.Dummy[0].Entry == "Dummy0001=1";
            eds.du.Dummy0002 = dev.Other.DummyUsage.Dummy[1].Entry == "Dummy0002=1";
            eds.du.Dummy0003 = dev.Other.DummyUsage.Dummy[2].Entry == "Dummy0003=1";
            eds.du.Dummy0004 = dev.Other.DummyUsage.Dummy[3].Entry == "Dummy0004=1";
            eds.du.Dummy0005 = dev.Other.DummyUsage.Dummy[4].Entry == "Dummy0005=1";
            eds.du.Dummy0006 = dev.Other.DummyUsage.Dummy[5].Entry == "Dummy0006=1";
            eds.du.Dummy0007 = dev.Other.DummyUsage.Dummy[6].Entry == "Dummy0007=1";

            foreach(SupportedBaudRate baud in dev.Other.BaudRate.SupportedBaudRate)
            {
                if (baud.Value == "10 Kbps")
                    eds.di.BaudRate_10 = true;
                if (baud.Value == "20 Kbps")
                    eds.di.BaudRate_20 = true;
                if (baud.Value == "50 Kbps")
                    eds.di.BaudRate_50 = true;
                if (baud.Value == "125 Kbps")
                    eds.di.BaudRate_125 = true;
                if (baud.Value == "250 Kbps")
                    eds.di.BaudRate_250 = true;
                if (baud.Value == "500 Kbps")
                    eds.di.BaudRate_500 = true;
                if (baud.Value == "800 Kbps")
                    eds.di.BaudRate_800 = true;
                if (baud.Value == "10000 Kbps")
                    eds.di.BaudRate_1000 = true;

            }

            eds.di.ProductName = dev.Other.DeviceIdentity.ProductName;
            //dev.Other.DeviceIdentity.ProductText
            eds.di.VendorName = dev.Other.DeviceIdentity.VendorName;

            if (dev.Other.DeviceIdentity.ConcreteNoideId != null)
            {
                eds.di.concreteNodeId = Convert.ToByte(dev.Other.DeviceIdentity.ConcreteNoideId);
            }
            else
            {
                eds.di.concreteNodeId = -1;
            }

            eds.fi.FileName = dev.Other.File.FileName;
            eds.fi.CreationDate = dev.Other.File.FileCreationDate;
            eds.fi.CreationTime = dev.Other.File.FileCreationTime;
            eds.fi.CreatedBy = dev.Other.File.FileCreator;
            eds.fi.exportFolder = dev.Other.File.ExportFolder;

            dev.Other.Capabilities = dev.Other.Capabilities;

            try
            {
                eds.fi.FileVersion = Convert.ToByte(dev.Other.File.FileVersion);
            }
            catch (Exception e)
            {
                if (dev.Other.File != null)
                    Warnings.warning_list.Add(String.Format("Unable to parse FileVersion\"{0}\" {1}", dev.Other.File.FileVersion,e.ToString()));

                eds.fi.FileVersion = 0;
            }

            eds.fi.EDSVersion = "4.0";
            
            //FIX me any other approprate defaults for eds here??

            eds.updatePDOcount();

            return eds;
        }

    }
}

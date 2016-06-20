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
            Device dev = new Device();
            dev.CANopenObjectList = new CANopenObjectList();

            /* OBJECT DICTIONARY */

            foreach(KeyValuePair<string,ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;

                if(od.subindex==-1)
                {
                    CANopenObject coo = new CANopenObject();
                    coo.Index =string.Format("{0:x4}",od.index);
                    coo.Name = od.parameter_name;
                    coo.ObjectType = od.objecttype.ToString();

                    if(od.objecttype==ObjectType.VAR)
                    {
                        coo.AccessType = od.accesstype.ToString();
                        coo.DataType = string.Format("0x{0:x2}",od.datatype);
                        coo.DefaultValue = od.defaultvalue;
                        coo.PDOmapping = od.PDOMapping.ToString();                 
                    }

                    if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
                    {
                        coo.SubNumber = od.nosubindexes.ToString(); //-1?? //check me 
                        coo.CANopenSubObject = new List<CANopenSubObject>();
                        for(int p=0;p<od.nosubindexes;p++)
                        {
                            CANopenSubObject sub = new CANopenSubObject();

                            ODentry subod;

                            string subidx = string.Format("{0:x4}/{1}", od.index,p);
                            if(eds.ods.ContainsKey(subidx))
                            {
                                subod = eds.ods[subidx];
                                sub.Name = subod.parameter_name;
                                sub.ObjectType = subod.objecttype.ToString();
                                sub.AccessType = subod.accesstype.ToString();
                                sub.DataType = string.Format("0x{0:x2}", subod.datatype);
                                sub.DefaultValue = subod.defaultvalue;
                                sub.PDOmapping = subod.PDOMapping.ToString();

                                coo.CANopenSubObject.Add(sub);
                            }                      
                        }
                    }

                    dev.CANopenObjectList.CANopenObject.Add(coo);
                }

            }
  

            /* DUMMY USAGE */
            
            dev.Other = new Other();
            dev.Other.DummyUsage = new DummyUsage();

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

            baud.Value="10 Kbps";
            if (eds.di.BaudRate_10 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);
            baud.Value = "20 Kbps";
            if (eds.di.BaudRate_20 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);
            baud.Value = "50 Kbps";
            if (eds.di.BaudRate_50 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);
            baud.Value = "125 Kbps";
            if (eds.di.BaudRate_125 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);
            baud.Value = "250 Kbps";
            if (eds.di.BaudRate_250 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);
            baud.Value = "500 Kbps";
            if (eds.di.BaudRate_500 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);
            baud.Value = "800 Kbps";
            if (eds.di.BaudRate_800 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);
            baud.Value = "1000 Kbps";
            if (eds.di.BaudRate_1000 == true)
                dev.Other.BaudRate.SupportedBaudRate.Add(baud);

            return dev;
        }


        public EDSsharp convert(Device dev)
        {
            EDSsharp eds = new EDSsharp();
            
            foreach(CANopenObject coo in dev.CANopenObjectList.CANopenObject)
            {
                ODentry entry = new ODentry();
                entry.index = Convert.ToInt16(coo.Index, 16);
                entry.parameter_name = coo.Name;

                if (coo.AccessType != null)
                {
                    string at = coo.AccessType;
                    at = at.Replace("const", "cons");
                    entry.accesstype = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), at);
                }


                if (coo.DataType != null)
                {
                    byte datatype = Convert.ToByte(coo.DataType, 16);
                    entry.datatype = (DataType)datatype;
                }


                entry.objecttype = (ObjectType)Enum.Parse(typeof(ObjectType),coo.ObjectType);


                entry.defaultvalue = coo.DefaultValue;
                entry.nosubindexes = Convert.ToInt16(coo.SubNumber);
                entry.subindex = -1;
                entry.PDOMapping = coo.PDOmapping!="no";

                eds.ods.Add(String.Format("{0:x4}", entry.index), entry);

                if (entry.index == 0x1000 || entry.index==0x1001 || entry.index==0x1018)
                {
                    eds.md.objectlist.Add(eds.md.objectlist.Count+1,entry.index);
                }
                else
                if (entry.index >= 0x2000 && entry.index<=0x6000)
                {
                    eds.mo.objectlist.Add(eds.mo.objectlist.Count+1,entry.index);
                }
                else
                {
                     eds.oo.objectlist.Add(eds.oo.objectlist.Count+1,entry.index);
                }

                
                foreach(CANopenSubObject coosub in coo.CANopenSubObject)
                {
                    int subno;
                    ODentry subentry = new ODentry();
                    //entry.index = Convert.ToInt16(coosub.Index, 16);

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
                    subentry.subindex = Convert.ToInt16(coosub.SubIndex, 16);
                    
                    if(coosub.PDOmapping!=null)
                        subentry.PDOMapping = coosub.PDOmapping != "no";

                    eds.ods.Add(String.Format("{0:x4}/{1}", entry.index, subentry.subindex), subentry);
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

            eds.fi.FileName = dev.Other.File.FileName;
            eds.fi.CreationDate = dev.Other.File.FileCreationDate;
            eds.fi.CreationTime = dev.Other.File.FileCreationTime;
            eds.fi.CreatedBy = dev.Other.File.FileCreator;

            try
            {
                eds.fi.FileVersion = Convert.ToByte(dev.Other.File.FileVersion);
            }
            catch (Exception e)
            {
                eds.fi.FileVersion = 0;
            }

            eds.fi.EDSVersion = "4.0";
            
            //FIX me any other approprate defaults for eds here??

            return eds;
        }

    }
}

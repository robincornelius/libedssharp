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
using System.Xml.Serialization;
using System.IO;
using XSDImport;
using System.Text.RegularExpressions; //and nope this is not anywhere near the xml parsing
using System.Collections.Generic;

namespace libEDSsharp
{
    public class CanOpenXDD
    {
        public ISO15745ProfileContainer dev;
        public EDSsharp readXML(string file)
        {

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ISO15745ProfileContainer));
                StreamReader reader = new StreamReader(file);
                dev = (ISO15745ProfileContainer)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception)
            {
                return null;
            }

            return convert(dev);

        }

        public List<EDSsharp> readMultiXML(string file )
        {

            List<EDSsharp> edss = new List<EDSsharp>();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OpenEDSProject));
                StreamReader reader = new StreamReader(file);
                OpenEDSProject oep = (OpenEDSProject)serializer.Deserialize(reader);

                foreach(ISO15745ProfileContainer cont in oep.ISO15745ProfileContainer)
                {
                    edss.Add(convert(cont));
                }

                reader.Close();

                return edss;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }    
        }

        public void writeMultiXML(string file, List<EDSsharp> edss)
        {

            List<ISO15745ProfileContainer> devs = new List<ISO15745ProfileContainer>();

            foreach (EDSsharp eds in edss)
            {
                ISO15745ProfileContainer dev = convert(eds);
                devs.Add(dev);
            }

            OpenEDSProject oep = new OpenEDSProject();
            oep.Version = "1.0";

            oep.ISO15745ProfileContainer = devs;

            XmlSerializer serializer = new XmlSerializer(typeof(OpenEDSProject));
            StreamWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, oep);
            writer.Close();
        }

        public void writeXML(string file, EDSsharp eds)
        {
            dev = convert(eds);
            XmlSerializer serializer = new XmlSerializer(typeof(ISO15745ProfileContainer));
            StreamWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, dev);
            writer.Close();
        }

        public void fillparamater(parameter p, ODentry od)
        {

            if (od.parent == null)
            {
                p.uniqueID = string.Format("UID_PARAM_{0:x4}", od.Index);
            }
            else
            {
                p.uniqueID = string.Format("UID_PARAM_{0:x4}{1:x2}", od.parent.Index, od.Subindex);
            }

            switch (od.accesstype)
            {
                case EDSsharp.AccessType.rw:
                    p.access = parameterTemplateAccess.readWrite;
                    break;

                case EDSsharp.AccessType.ro:
                    p.access = parameterTemplateAccess.read;
                    break;

                case EDSsharp.AccessType.wo:
                    p.access = parameterTemplateAccess.write;
                    break;

                case EDSsharp.AccessType.rwr:
                    p.access = parameterTemplateAccess.readWriteInput;
                    break;

                case EDSsharp.AccessType.rww:
                    p.access = parameterTemplateAccess.readWriteOutput;
                    break;

                case EDSsharp.AccessType.@const:
                    p.access = parameterTemplateAccess.@const;
                    break;

                //fixme no access not handled
                default:
                    p.access = parameterTemplateAccess.noAccess;
                    break;

            }

            p.Items = new object[2];

            vendorTextLabel lab = new vendorTextLabel();
            lab.lang = "en";
            lab.Value = od.parameter_name;
            p.Items[0] = lab;


            //FIXME we are currently writing the denotation value to both the object and the parameterList section
            //i'm not sure why two exist

            denotation denot = new denotation();
            vendorTextLabel lab2 = new vendorTextLabel();
            lab2.lang = "en";
            lab2.Value = od.denotation;
            denot.Items = new object[1];
            denot.Items[0] = lab2;
            p.denotation = denot;
            

            vendorTextDescription desc = new vendorTextDescription();
            desc.lang = "en"; //fixme we could and should do better than just English
            desc.Value = od.Description;
            p.Items[1] = desc;

            p.defaultValue = new defaultValue();
            p.defaultValue.value = od.defaultvalue;

        }


        public ISO15745ProfileContainer convert(EDSsharp eds)
        {
            dev = new ISO15745ProfileContainer();
            
            dev.ISO15745Profile = new ISO15745Profile[2];



            //Profile 0 ProfileBody_Device_CANopen
            dev.ISO15745Profile[0] = new ISO15745Profile();
            dev.ISO15745Profile[0].ProfileHeader = new ProfileHeader_DataType();

            dev.ISO15745Profile[0].ProfileHeader.ProfileIdentification = "CAN device profile";
            dev.ISO15745Profile[0].ProfileHeader.ProfileRevision = "1";
            dev.ISO15745Profile[0].ProfileHeader.ProfileClassID = ProfileClassID_DataType.Device;
            dev.ISO15745Profile[0].ProfileHeader.ProfileName = "";
            dev.ISO15745Profile[0].ProfileHeader.ProfileSource = "";

            dev.ISO15745Profile[0].ProfileHeader.ISO15745Reference = new ISO15745Reference_DataType();
            dev.ISO15745Profile[0].ProfileHeader.ISO15745Reference.ISO15745Part = "1";
            dev.ISO15745Profile[0].ProfileHeader.ISO15745Reference.ISO15745Edition = "1";
            dev.ISO15745Profile[0].ProfileHeader.ISO15745Reference.ProfileTechnology = "CANopen";

            dev.ISO15745Profile[0].ProfileBody = new ProfileBody_Device_CANopen();

            ProfileBody_Device_CANopen device = (ProfileBody_Device_CANopen)dev.ISO15745Profile[0].ProfileBody;

            device.DeviceIdentity = new DeviceIdentity();

            device.DeviceIdentity.vendorName = new vendorName();
            device.DeviceIdentity.vendorName.Value = eds.di.VendorName;
            device.DeviceIdentity.vendorName.readOnly = true;

            device.DeviceIdentity.vendorID = new vendorID();
            device.DeviceIdentity.vendorID.Value = eds.di.VendorNumber.ToString();
            device.DeviceIdentity.vendorID.readOnly = true;

            device.DeviceIdentity.deviceFamily = new deviceFamily();

            device.DeviceIdentity.productFamily = new productFamily();

            //device.DeviceIdentity.orderNumber = 

            device.fileCreationDate = eds.fi.CreationDateTime;
            device.fileCreationTime = eds.fi.CreationDateTime;
            device.fileCreationTimeSpecified = true;
            
            device.fileModificationDate = eds.fi.ModificationDateTime;
            device.fileModificationTime = eds.fi.ModificationDateTime;
            device.fileModificationDateSpecified = true;
            device.fileModificationTimeSpecified = true;

            device.fileCreator = eds.fi.CreatedBy;
            device.fileModifiedBy = eds.fi.ModifiedBy;

            device.supportedLanguages = "en";

            device.fileVersion = eds.fi.FileVersion.ToString();

            device.fileName = eds.fi.FileName;
            

            //device.DeviceIdentity.vendorText
            //device.DeviceIdentity.deviceFamily
            //device.DeviceIdentity.productFamily;

            device.DeviceIdentity.productName = new productName();
            device.DeviceIdentity.productName.Value = eds.di.ProductName;
            device.DeviceIdentity.productName.readOnly = true;

            device.DeviceIdentity.productID = new productID();
            device.DeviceIdentity.productID.Value = eds.di.ProductNumber.ToString();
            device.DeviceIdentity.productID.readOnly = true;

            device.DeviceIdentity.productText = new productText();
            device.DeviceIdentity.productText.Items = new object[1];


            vendorTextDescription des = new vendorTextDescription();
            des.lang = "en";

            des.Value = String.Format("FileDescription={0}|EdsVersion={1}|FileRevision={2}|RevisionNum={3}",eds.fi.Description,eds.fi.EDSVersion,eds.fi.FileVersion,eds.fi.FileRevision);

            device.DeviceIdentity.productText.Items[0] = des;
            device.DeviceIdentity.productText.readOnly = true;

            //device.DeviceIdentity.orderNumber

            device.DeviceIdentity.version = new version[3];

            device.DeviceIdentity.version[0] = new version();
            device.DeviceIdentity.version[0].readOnly = true;
            device.DeviceIdentity.version[0].versionType = versionVersionType.SW;

            device.DeviceIdentity.version[1] = new version();
            device.DeviceIdentity.version[1].readOnly = true;
            device.DeviceIdentity.version[1].versionType = versionVersionType.FW;

            device.DeviceIdentity.version[2] = new version();
            device.DeviceIdentity.version[2].readOnly = true;
            device.DeviceIdentity.version[2].versionType = versionVersionType.HW;

            //device.DeviceIdentity.specificationRevision.value = 
            device.DeviceIdentity.specificationRevision = new specificationRevision();
            device.DeviceIdentity.specificationRevision.readOnly = true;

            device.DeviceIdentity.instanceName = new instanceName();
            //device.DeviceIdentity.instanceName.Value = ;
            device.DeviceIdentity.instanceName.readOnly = true;


            device.DeviceManager = new DeviceManager();
            device.DeviceManager.indicatorList = new indicatorList();
            device.DeviceManager.indicatorList.LEDList = new LEDList();
            device.DeviceManager.indicatorList.LEDList.LED = new LED[1]; //fixme
            device.DeviceManager.indicatorList.LEDList.LED[0] = new LED();
            device.DeviceManager.indicatorList.LEDList.LED[0].LEDcolors = LEDLEDcolors.monocolor;
            device.DeviceManager.indicatorList.LEDList.LED[0].LEDtype = LEDLEDtype.device;
            device.DeviceManager.indicatorList.LEDList.LED[0].Items = new object[1];

            // LEDstate ls = new LEDstate();
            // ls.uniqueID = "LED_State_1";
            // ls.state = LEDstateState.off;
            // ls.LEDcolor = LEDstateLEDcolor.green;

            // device.DeviceManager.indicatorList.LEDList.LED[0].Items[0] = ls;

            device.DeviceFunction = new DeviceFunction[1];
            //fix me fll this in


            device.ApplicationProcess = new ApplicationProcess[1];
            device.ApplicationProcess[0] = new ApplicationProcess();

            device.ApplicationProcess[0].parameterList = new parameter[eds.GetNoEnabledObjects(true)];

            int ordinal = 0;

            foreach (ODentry od in eds.ods.Values)
            {

                if (od.Disabled)
                    continue;

                parameter p = new parameter();

                fillparamater(p, od);

                device.ApplicationProcess[0].parameterList[ordinal] = p;
                ordinal++;

                foreach (ODentry sub in od.subobjects.Values)
                {
                    p = new parameter();

                    fillparamater(p, sub);

                    device.ApplicationProcess[0].parameterList[ordinal] = p;
                    ordinal++;
                }

            }


            //Profile 1 ProfileClassID_DataType.CommunicationNetwork
            dev.ISO15745Profile[1] = new ISO15745Profile();

            dev.ISO15745Profile[1].ProfileHeader = new ProfileHeader_DataType();

            dev.ISO15745Profile[1].ProfileHeader.ProfileIdentification = "CAN comm net profile";
            dev.ISO15745Profile[1].ProfileHeader.ProfileRevision = "1";
            dev.ISO15745Profile[1].ProfileHeader.ProfileClassID = ProfileClassID_DataType.CommunicationNetwork;
            dev.ISO15745Profile[1].ProfileHeader.ProfileName = "";
            dev.ISO15745Profile[1].ProfileHeader.ProfileSource = "";

            dev.ISO15745Profile[1].ProfileHeader.ISO15745Reference = new ISO15745Reference_DataType();
            dev.ISO15745Profile[1].ProfileHeader.ISO15745Reference.ISO15745Part = "1";
            dev.ISO15745Profile[1].ProfileHeader.ISO15745Reference.ISO15745Edition = "1";
            dev.ISO15745Profile[1].ProfileHeader.ISO15745Reference.ProfileTechnology = "CANopen";

            dev.ISO15745Profile[1].ProfileBody = new ProfileBody_CommunicationNetwork_CANopen();
            ProfileBody_CommunicationNetwork_CANopen comnet = (ProfileBody_CommunicationNetwork_CANopen)dev.ISO15745Profile[1].ProfileBody;
            comnet.Items = new object[3];

            comnet.fileName = eds.fi.FileName;

            comnet.fileCreator = eds.fi.CreatedBy; //etc
            comnet.fileCreationDate = eds.fi.CreationDateTime;
            comnet.fileCreationTime = eds.fi.CreationDateTime;
            comnet.fileCreationTimeSpecified = true;

            comnet.fileModificationDate = eds.fi.ModificationDateTime;
            comnet.fileModificationTime = eds.fi.ModificationDateTime;
            comnet.fileModificationDateSpecified = true;

            comnet.fileVersion = eds.fi.FileVersion.ToString();

            comnet.supportedLanguages = "en";

            comnet.Items[0] = new ProfileBody_CommunicationNetwork_CANopenApplicationLayers();
            ProfileBody_CommunicationNetwork_CANopenApplicationLayers AppLayer = (ProfileBody_CommunicationNetwork_CANopenApplicationLayers)comnet.Items[0];

            comnet.Items[1] = new ProfileBody_CommunicationNetwork_CANopenTransportLayers();
            ProfileBody_CommunicationNetwork_CANopenTransportLayers TransportLayer = (ProfileBody_CommunicationNetwork_CANopenTransportLayers)comnet.Items[1];

            comnet.Items[2] = new ProfileBody_CommunicationNetwork_CANopenNetworkManagement();
            ProfileBody_CommunicationNetwork_CANopenNetworkManagement NetworkManagement = (ProfileBody_CommunicationNetwork_CANopenNetworkManagement)comnet.Items[2];

            AppLayer.CANopenObjectList = new CANopenObjectList();


            AppLayer.CANopenObjectList.CANopenObject = new CANopenObjectListCANopenObject[eds.GetNoEnabledObjects()];
            
            int count = 0;
            foreach (KeyValuePair<UInt16,ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                UInt16 subindex = kvp.Key;

                if (od.Disabled)
                    continue;

                AppLayer.CANopenObjectList.CANopenObject[count] = new CANopenObjectListCANopenObject();

                byte[] bytes = BitConverter.GetBytes((UInt16)od.Index);
                Array.Reverse(bytes);

                AppLayer.CANopenObjectList.CANopenObject[count].index = bytes;
                AppLayer.CANopenObjectList.CANopenObject[count].name = od.parameter_name;
                AppLayer.CANopenObjectList.CANopenObject[count].objectType = (byte)od.objecttype;

                bytes = BitConverter.GetBytes((UInt16)od.datatype);
                Array.Reverse(bytes);

                // hack - special handling for rrw / rww access type 
                // https://github.com/robincornelius/libedssharp/issues/128
                EDSsharp.AccessType accesstype = od.accesstype;
                PDOMappingType PDOtype = od.PDOtype;
                if (accesstype == EDSsharp.AccessType.rww) {
                    accesstype = EDSsharp.AccessType.rw;

                    // when optional, set it to the corresponding type
                    if (PDOtype == PDOMappingType.optional) {
                        PDOtype = PDOMappingType.RPDO;
                    }
                }
                if (accesstype == EDSsharp.AccessType.rwr) {
                    accesstype = EDSsharp.AccessType.rw;

                    // when optional, set it to the corresponding type
                    if (PDOtype == PDOMappingType.optional) {
                        PDOtype = PDOMappingType.TPDO;
                    }
                }


                if (od.objecttype != ObjectType.ARRAY && od.objecttype != ObjectType.REC)
                {
                    //#209 don't set data type for array or rec objects, the subobjects hold 
                    //the data type
                    AppLayer.CANopenObjectList.CANopenObject[count].dataType = bytes;
                    AppLayer.CANopenObjectList.CANopenObject[count].accessType = (CANopenObjectListCANopenObjectAccessType)Enum.Parse(typeof(CANopenObjectListCANopenObjectAccessType), accesstype.ToString());
                    AppLayer.CANopenObjectList.CANopenObject[count].accessTypeSpecified = true;

                }
                else
                {
                    AppLayer.CANopenObjectList.CANopenObject[count].accessTypeSpecified = false;
                }

                AppLayer.CANopenObjectList.CANopenObject[count].PDOmapping = (CANopenObjectListCANopenObjectPDOmapping)Enum.Parse(typeof(CANopenObjectListCANopenObjectPDOmapping),PDOtype.ToString());
                AppLayer.CANopenObjectList.CANopenObject[count].PDOmappingSpecified = true;

                AppLayer.CANopenObjectList.CANopenObject[count].uniqueIDRef = String.Format("UID_PARAM_{0:x4}", od.Index);

                AppLayer.CANopenObjectList.CANopenObject[count].denotation = od.denotation;
                AppLayer.CANopenObjectList.CANopenObject[count].edseditor_extenstion_storagelocation = od.StorageLocation;

                AppLayer.CANopenObjectList.CANopenObject[count].edseditor_extension_notifyonchange = od.TPDODetectCos;

                AppLayer.CANopenObjectList.CANopenObject[count].highLimit = od.HighLimit;
                AppLayer.CANopenObjectList.CANopenObject[count].lowLimit = od.LowLimit;
                AppLayer.CANopenObjectList.CANopenObject[count].actualValue = od.actualvalue;

                if (od.subobjects != null && od.subobjects.Count > 0)
                {
                    AppLayer.CANopenObjectList.CANopenObject[count].subNumber = (byte)od.subobjects.Count;
                    AppLayer.CANopenObjectList.CANopenObject[count].subNumberSpecified = true;

                    AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject = new CANopenObjectListCANopenObjectCANopenSubObject[od.subobjects.Count];

                    int subcount = 0;

                    foreach ( KeyValuePair<UInt16,ODentry> kvp2 in od.subobjects)
                    {
                        ODentry subod = kvp2.Value;
                        UInt16 subindex2 = kvp2.Key;

                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount] = new CANopenObjectListCANopenObjectCANopenSubObject();

                        bytes = BitConverter.GetBytes((UInt16)kvp2.Key);
                        Array.Reverse(bytes);

                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].subIndex = bytes;
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].name = subod.parameter_name;
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].objectType = (byte)subod.objecttype;

                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].denotation = subod.denotation;

                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].edseditor_extension_notifyonchange = subod.TPDODetectCos;

                        bytes = BitConverter.GetBytes((UInt16)subod.datatype);
                        Array.Reverse(bytes);

                        // hack - special handling for rrw / rww access type
                        // https://github.com/robincornelius/libedssharp/issues/128
                        accesstype = subod.accesstype;
                        PDOtype = subod.PDOtype;
                        if (accesstype == EDSsharp.AccessType.rww) {
                            accesstype = EDSsharp.AccessType.rw;

                            // when optional is set, 
                            if (PDOtype == PDOMappingType.optional) {
                                PDOtype = PDOMappingType.RPDO;
                            }
                        }
                        if (accesstype == EDSsharp.AccessType.rwr) {
                            accesstype = EDSsharp.AccessType.rw;

                            // when optional is set, 
                            if (PDOtype == PDOMappingType.optional) {
                                PDOtype = PDOMappingType.TPDO;
                            }
                        }
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].dataType = bytes;
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].PDOmapping = (CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping)Enum.Parse(typeof(CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping),PDOtype.ToString());
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].PDOmappingSpecified = true;
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].uniqueIDRef = String.Format("UID_PARAM_{0:x4}{1:x2}", od.Index, subindex2);
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].accessType = (CANopenObjectListCANopenObjectCANopenSubObjectAccessType)Enum.Parse(typeof(CANopenObjectListCANopenObjectCANopenSubObjectAccessType), accesstype.ToString());
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].accessTypeSpecified = true;

                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].highLimit = subod.HighLimit;
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].lowLimit = subod.LowLimit;
                        AppLayer.CANopenObjectList.CANopenObject[count].CANopenSubObject[subcount].actualValue = subod.actualvalue;


                        subcount++;
                    }
                }


                count++;
            }

            AppLayer.dummyUsage = new ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy[7];

            for (int x = 0; x < 7; x++)
            {
                AppLayer.dummyUsage[x] = new ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy();
            }

            //FIX ME this is terrible
            AppLayer.dummyUsage[0].entry = (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry)Enum.Parse(typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry), string.Format("Dummy0001{0}", eds.du.Dummy0001 == true ? "1" : "0"));
            AppLayer.dummyUsage[1].entry = (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry)Enum.Parse(typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry), string.Format("Dummy0002{0}", eds.du.Dummy0002 == true ? "1" : "0"));
            AppLayer.dummyUsage[2].entry = (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry)Enum.Parse(typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry), string.Format("Dummy0003{0}", eds.du.Dummy0003 == true ? "1" : "0"));
            AppLayer.dummyUsage[3].entry = (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry)Enum.Parse(typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry), string.Format("Dummy0004{0}", eds.du.Dummy0004 == true ? "1" : "0"));
            AppLayer.dummyUsage[4].entry = (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry)Enum.Parse(typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry), string.Format("Dummy0005{0}", eds.du.Dummy0005 == true ? "1" : "0"));
            AppLayer.dummyUsage[5].entry = (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry)Enum.Parse(typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry), string.Format("Dummy0006{0}", eds.du.Dummy0006 == true ? "1" : "0"));
            AppLayer.dummyUsage[6].entry = (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry)Enum.Parse(typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry), string.Format("Dummy0007{0}", eds.du.Dummy0007 == true ? "1" : "0"));

            TransportLayer.PhysicalLayer = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayer();

            //FIX me this is worse than above

            int bauds = 0;

            if (eds.di.BaudRate_10 == true)
                bauds++;
            if (eds.di.BaudRate_20 == true)
                bauds++;
            if (eds.di.BaudRate_50 == true)
                bauds++;
            if (eds.di.BaudRate_125 == true)
                bauds++;
            if (eds.di.BaudRate_250 == true)
                bauds++;
            if (eds.di.BaudRate_500 == true)
                bauds++;
            if (eds.di.BaudRate_800 == true)
                bauds++;
            if (eds.di.BaudRate_1000 == true)
                bauds++;

            //Fixme auto baudrate needs adding to system
            TransportLayer.PhysicalLayer.baudRate = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRate();
            TransportLayer.PhysicalLayer.baudRate.supportedBaudRate = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate[bauds];

            for (int x = 0; x < bauds; x++)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[x] = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate();
            }

            bauds = 0;

            if (eds.di.BaudRate_10 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item10Kbps;
                bauds++;
            }
            if (eds.di.BaudRate_20 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item20Kbps;
                bauds++;
            }
            if (eds.di.BaudRate_50 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item50Kbps;
                bauds++;
            }
            if (eds.di.BaudRate_125 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item125Kbps;
                bauds++;
            }
            if (eds.di.BaudRate_250 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item250Kbps;
                bauds++;
            }
            if (eds.di.BaudRate_500 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item500Kbps;
                bauds++;
            }
            if (eds.di.BaudRate_800 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item800Kbps;
                bauds++;
            }
            if (eds.di.BaudRate_1000 == true)
            {
                TransportLayer.PhysicalLayer.baudRate.supportedBaudRate[bauds].value = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item1000Kbps;
                bauds++;
            }


            NetworkManagement.CANopenGeneralFeatures = new ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenGeneralFeatures();

            NetworkManagement.CANopenGeneralFeatures.bootUpSlave = eds.di.SimpleBootUpSlave;
            //NetworkManagment.CANopenGeneralFeatures.dynamicChannels = eds.di.DynamicChannelsSupported;   //fix me count of dynamic channels not handled yet eds only has bool
            NetworkManagement.CANopenGeneralFeatures.granularity = eds.di.Granularity;
            NetworkManagement.CANopenGeneralFeatures.groupMessaging = eds.di.GroupMessaging;

            NetworkManagement.CANopenGeneralFeatures.layerSettingServiceSlave = eds.di.LSS_Supported && eds.di.LSS_Type == "Server";
            NetworkManagement.CANopenGeneralFeatures.nrOfRxPDO = eds.di.NrOfRXPDO;
            NetworkManagement.CANopenGeneralFeatures.nrOfTxPDO = eds.di.NrOfTXPDO;
            //extra items
            //NetworkManagment.CANopenGeneralFeatures.SDORequestingDevice;
            //NetworkManagment.CANopenGeneralFeatures.selfStartingDevice;

            NetworkManagement.CANopenMasterFeatures = new ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenMasterFeatures();

            NetworkManagement.CANopenMasterFeatures.bootUpMaster = eds.di.SimpleBootUpMaster;

            //Extra items
            //NetworkManagment.CANopenMasterFeatures.configurationManager;
            //NetworkManagment.CANopenMasterFeatures.flyingMaster;
            NetworkManagement.CANopenMasterFeatures.layerSettingServiceMaster = eds.di.LSS_Supported && eds.di.LSS_Type == "Client";
            //NetworkManagment.CANopenMasterFeatures.SDOManager;


            NetworkManagement.deviceCommissioning = new ProfileBody_CommunicationNetwork_CANopenNetworkManagementDeviceCommissioning();
            NetworkManagement.deviceCommissioning.actualBaudRate = eds.dc.BaudRate.ToString();
            NetworkManagement.deviceCommissioning.nodeID = eds.dc.NodeId;
            NetworkManagement.deviceCommissioning.networkName = eds.dc.NetworkName;
            NetworkManagement.deviceCommissioning.networkNumber = eds.dc.NetNumber;
            NetworkManagement.deviceCommissioning.CANopenManager = eds.dc.CANopenManager;

            //fixme unconnected
            //eds.dc.LSS_SerialNumber;



            return dev;
        }



        public EDSsharp convert(ISO15745ProfileContainer container)
        {
            EDSsharp eds = new EDSsharp();

            //Find Object Dictionary entries

           //fixme??
           // ProfileBody_DataType dt;


            ProfileBody_CommunicationNetwork_CANopen body_network = null;
            ProfileBody_Device_CANopen body_device = null;


            foreach (ISO15745Profile dev in container.ISO15745Profile)
            {
                if (dev.ProfileBody.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopen))
                {
                    body_network = (ProfileBody_CommunicationNetwork_CANopen)dev.ProfileBody;
                }

                if (dev.ProfileBody.GetType() == typeof(ProfileBody_Device_CANopen))
                {
                    body_device = (ProfileBody_Device_CANopen)dev.ProfileBody;

                }

            }

            //ProfileBody_CommunicationNetwork_CANopen
            if (body_network != null)
            {
                ProfileBody_CommunicationNetwork_CANopen obj = body_network;

                ProfileBody_CommunicationNetwork_CANopenApplicationLayers ApplicationLayers = null;
                ProfileBody_CommunicationNetwork_CANopenTransportLayers TransportLayers = null;
                ProfileBody_CommunicationNetwork_CANopenNetworkManagement NetworkManagment = null;

                foreach (object obj2 in obj.Items)
                {

                    if (obj2.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayers))
                        ApplicationLayers = (ProfileBody_CommunicationNetwork_CANopenApplicationLayers)obj2;

                    if (obj2.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenTransportLayers))
                        TransportLayers = (ProfileBody_CommunicationNetwork_CANopenTransportLayers)obj2;

                    if (obj2.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenNetworkManagement))
                        NetworkManagment = (ProfileBody_CommunicationNetwork_CANopenNetworkManagement)obj2;

                }

                if (ApplicationLayers != null)
                {

                    string vendorID = "";
                    //fixme
                    //string deviceFamily = "";
                    string productID = "";
                    //fixme
                    //string version = "";
                    DateTime buildDate;
                    string specificationRevision = "";

                    if (ApplicationLayers.identity != null)
                    {
                        if (ApplicationLayers.identity.vendorID != null)
                        {
                            vendorID = ApplicationLayers.identity.vendorID.Value;
                        }

                        if (ApplicationLayers.identity.deviceFamily != null)
                        {
                            //deviceFamily = ApplicationLayers.identity.deviceFamily.Items[]
                            //not really sure how to handle this. its a list of g_labels
                            //these contain label, description, language, URi etc could do with a simple class
                            //to wrap these in as they are used in a number of places
                        }

                        if (ApplicationLayers.identity.productID != null)
                        {
                            productID = ApplicationLayers.identity.productID.Value;
                        }

                        if (ApplicationLayers.identity.buildDate != null)
                        {
                            buildDate = ApplicationLayers.identity.buildDate;
                        }

                        if (ApplicationLayers.identity.specificationRevision != null)
                        {
                            specificationRevision = ApplicationLayers.identity.specificationRevision.Value;
                        }

                    }

                    if (ApplicationLayers.dummyUsage != null)
                    {
                        foreach (ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy dummy in ApplicationLayers.dummyUsage)
                        {
                            string pat = @"Dummy([0-9]{4})([0-1])";
                            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                            Match m = r.Match(dummy.entry.ToString());


                            if (m.Success)
                            {


                                int index = int.Parse(m.Groups[1].Value);

                                switch (index)
                                {
                                    case 1:
                                        eds.du.Dummy0001 = int.Parse(m.Groups[2].Value) == 1;
                                        break;
                                    case 2:
                                        eds.du.Dummy0002 = int.Parse(m.Groups[2].Value) == 1;
                                        break;
                                    case 3:
                                        eds.du.Dummy0003 = int.Parse(m.Groups[2].Value) == 1;
                                        break;
                                    case 4:
                                        eds.du.Dummy0004 = int.Parse(m.Groups[2].Value) == 1;
                                        break;
                                    case 5:
                                        eds.du.Dummy0005 = int.Parse(m.Groups[2].Value) == 1;
                                        break;
                                    case 6:
                                        eds.du.Dummy0006 = int.Parse(m.Groups[2].Value) == 1;
                                        break;
                                    case 7:
                                        eds.du.Dummy0007 = int.Parse(m.Groups[2].Value) == 1;
                                        break;

                                }

                            }

                        }

                    } //dummyusage != null

                    if (ApplicationLayers.dynamicChannels != null)
                    {

                    }

                    if (ApplicationLayers.conformanceClass != null)
                    {

                    }

                    if (ApplicationLayers.communicationEntityType != null)
                    {

                    }

                } //application layer

                if (TransportLayers != null)
                {
                    if(TransportLayers.PhysicalLayer!=null)
                    {
                        if (TransportLayers.PhysicalLayer.baudRate != null)
                        {
                            if (TransportLayers.PhysicalLayer.baudRate.supportedBaudRate != null)
                            {
                                foreach (ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate baud in TransportLayers.PhysicalLayer.baudRate.supportedBaudRate)
                                {

                                    if (baud.value.ToString() == "Item10Kbps")
                                        eds.di.BaudRate_10 = true;
                                    if (baud.value.ToString() == "Item20Kbps")
                                        eds.di.BaudRate_20 = true;
                                    if (baud.value.ToString() == "Item50Kbps")
                                        eds.di.BaudRate_50 = true;
                                    if (baud.value.ToString() == "Item125Kbps")
                                        eds.di.BaudRate_125 = true;
                                    if (baud.value.ToString() == "Item250Kbps")
                                        eds.di.BaudRate_250 = true;
                                    if (baud.value.ToString() == "Item500Kbps")
                                        eds.di.BaudRate_500 = true;
                                    if (baud.value.ToString() == "Item800Kbps")
                                        eds.di.BaudRate_800 = true;
                                    if (baud.value.ToString() == "Item1000Kbps")
                                        eds.di.BaudRate_1000 = true;

                                    //fixme "auto-baudRate" is a valid identifier here as well


                                }
                            }
                        }
                    }


                } //Transport layer

                eds.di.LSS_Supported = false;

                if (NetworkManagment != null)
                {
                    if (NetworkManagment.CANopenMasterFeatures != null)
                    {
                        eds.di.SimpleBootUpMaster = NetworkManagment.CANopenMasterFeatures.bootUpMaster;

                        //fixme Extra items
                        //NetworkManagment.CANopenMasterFeatures.configurationManager;
                        //NetworkManagment.CANopenMasterFeatures.flyingMaster;

                        //Fix me if Client and Server are set in XDD i can't deal with this and will default to Server
                        if (NetworkManagment.CANopenMasterFeatures.layerSettingServiceMaster)
                        {
                            eds.di.LSS_Supported = true;
                            eds.di.LSS_Type = "Client";
                        }

                        //NetworkManagment.CANopenMasterFeatures.SDOManager;
                    }

                    if (NetworkManagment.CANopenGeneralFeatures != null)
                    {
                        eds.di.SimpleBootUpSlave = NetworkManagment.CANopenGeneralFeatures.bootUpSlave;
                        eds.di.DynamicChannelsSupported = NetworkManagment.CANopenGeneralFeatures.dynamicChannels > 0;
                        //fix me count of dynamic channels not handled yet eds only has bool

                        eds.di.Granularity = NetworkManagment.CANopenGeneralFeatures.granularity;
                        eds.di.GroupMessaging = NetworkManagment.CANopenGeneralFeatures.groupMessaging;

                        //Fix me if Client and Server are set in XDD i can't deal with this and will default to Server
                        if (NetworkManagment.CANopenGeneralFeatures.layerSettingServiceSlave)
                        {
                            eds.di.LSS_Type = "Server";
                            eds.di.LSS_Supported = true;
                        }

                        eds.di.NrOfRXPDO = NetworkManagment.CANopenGeneralFeatures.nrOfRxPDO;
                        eds.di.NrOfTXPDO = NetworkManagment.CANopenGeneralFeatures.nrOfTxPDO;

                        //fixme extra items
                        //NetworkManagment.CANopenGeneralFeatures.SDORequestingDevice;
                        //NetworkManagment.CANopenGeneralFeatures.selfStartingDevice;

                    }

                    if (NetworkManagment.deviceCommissioning != null)
                    {
                        eds.dc.NodeId = NetworkManagment.deviceCommissioning.nodeID;
                        eds.dc.BaudRate = Convert.ToUInt16(NetworkManagment.deviceCommissioning.actualBaudRate);
                        eds.dc.CANopenManager = NetworkManagment.deviceCommissioning.CANopenManager;
                        eds.dc.NetworkName = NetworkManagment.deviceCommissioning.networkName;
                        eds.dc.NetNumber = Convert.ToUInt16(NetworkManagment.deviceCommissioning.networkNumber);
                        eds.dc.NodeName = NetworkManagment.deviceCommissioning.nodeName;

                    }
                }

                if (ApplicationLayers.CANopenObjectList.CANopenObject != null)
                    foreach (XSDImport.CANopenObjectListCANopenObject obj3 in ApplicationLayers.CANopenObjectList.CANopenObject)
                    {
                        ODentry entry = new ODentry();

                        UInt16 index;

                        if (obj3.index != null)
                        {
                            index = (UInt16)EDSsharp.ConvertToUInt16(obj3.index);
                            entry.Index = index;
                        }
                        else
                            continue; //unparseable

                        if (obj3.name != null)
                            entry.parameter_name = obj3.name;

                        entry.objecttype = (ObjectType)obj3.objectType;

                        if (obj3.dataType != null)
                            entry.datatype = (DataType)EDSsharp.ConvertToUInt16(obj3.dataType);

                        if (obj3.defaultValue != null)
                            entry.defaultvalue = obj3.defaultValue;

                        if (obj3.highLimit != null)
                            entry.HighLimit = obj3.highLimit;

                        if (obj3.lowLimit != null)
                            entry.LowLimit = obj3.lowLimit;

                        if (obj3.actualValue != null)
                            entry.actualvalue = obj3.actualValue;

                        if (obj3.denotation != null)
                            entry.denotation = obj3.denotation;

                        if (obj3.edseditor_extenstion_storagelocation != null)
                            entry.StorageLocation = obj3.edseditor_extenstion_storagelocation;

                        if (obj3.edseditor_extension_notifyonchange != null)
                            entry.TPDODetectCos = obj3.edseditor_extension_notifyonchange;

                            

                        //FIXME im not sure this is correct
                        if (obj3.objFlags != null)
                            entry.ObjFlags = obj3.objFlags[0];

                        entry.uniqueID = obj3.uniqueIDRef;

                        // https://github.com/robincornelius/libedssharp/issues/128
                        // Mapping of accesstype and pdo mappings have changed between EDS and XDD
                        // in EDS we have rw,wo, r and const which are the same in both standards, but EDS also
                        // has rww and rwr which are rw objects that can also be mapped to RPDOs (rww) or
                        // TPDOs (rwr) 

                        if (obj3.accessTypeSpecified)
                        {
                            entry.accesstype = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), obj3.accessType.ToString());
                        }
                        else
                        {
                            entry.accesstype = EDSsharp.AccessType.ro; //fixme sensible default required here??    
                        }

                        if(obj3.PDOmappingSpecified)
                        {

                            entry.PDOtype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), obj3.PDOmapping.ToString());

                            if (entry.accesstype == EDSsharp.AccessType.rw && entry.PDOtype == PDOMappingType.RPDO)
                            {
                                entry.accesstype = EDSsharp.AccessType.rww;
                            }

                            if (entry.accesstype == EDSsharp.AccessType.rw && entry.PDOtype == PDOMappingType.TPDO)
                            {
                                entry.accesstype = EDSsharp.AccessType.rwr;
                            }
                        }
                        else
                        {
                            entry.PDOtype = PDOMappingType.no; //fixme should this be @default??
                        }

                        eds.ods.Add(index, entry);

                        if (obj3.CANopenSubObject != null)
                        {
                            foreach (XSDImport.CANopenObjectListCANopenObjectCANopenSubObject subobj in obj3.CANopenSubObject)
                            {

                                DataType datatype;
                                EDSsharp.AccessType accesstype = EDSsharp.AccessType.ro; //fixme sensible default?
                                PDOMappingType pdotype = PDOMappingType.no;

                                if (subobj.dataType != null)
                                {
                                    datatype = (DataType)EDSsharp.ConvertToUInt16(subobj.dataType);
                                }
                                else
                                {
                                    datatype = entry.datatype;
                                }


                       
                                // https://github.com/robincornelius/libedssharp/issues/128
                                // Mapping of accesstype and pdo mappings have changed between EDS and XDD
                                // in EDS we have rw,wo, r and const which are the same in both standards, but EDS also
                                // has rww and rwr which are rw objects that can also be mapped to RPDOs (rww) or
                                // TPDOs (rwr) 


                                if (subobj.accessTypeSpecified == true)
                                {
                                    accesstype = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), subobj.accessType.ToString());
                                }
                                else
                                {
                                    accesstype = entry.accesstype;
                                }
                           
                                if (subobj.PDOmappingSpecified == true)
                                {

                                    pdotype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), subobj.PDOmapping.ToString());

                                    if(accesstype == EDSsharp.AccessType.rw && pdotype == PDOMappingType.RPDO)
                                    {
                                        accesstype = EDSsharp.AccessType.rww;
                                    }

                                    if (accesstype == EDSsharp.AccessType.rw && pdotype == PDOMappingType.TPDO)
                                    {
                                        accesstype = EDSsharp.AccessType.rwr;
                                    }

                                }
                                else
                                {
                                    pdotype = entry.PDOtype;
                                }


                                ODentry subentry = new ODentry(subobj.name, index, datatype, subobj.defaultValue, accesstype, pdotype, entry);


                                //extra items

                                if (subobj.edseditor_extension_notifyonchange != null)
                                    subentry.TPDODetectCos = subobj.edseditor_extension_notifyonchange;

                                if (subobj.lowLimit!=null)
                                    subentry.LowLimit = subobj.lowLimit;

                                if(subobj.highLimit!=null)
                                    subentry.HighLimit = subobj.highLimit;

                                if(subobj.actualValue!=null)
                                    subentry.actualvalue = subobj.actualValue;

                                if(subobj.denotation!=null)
                                    subentry.denotation = subobj.denotation;

                                if(subobj.objFlags!=null)
                                    subentry.ObjFlags = subobj.objFlags[0];


                                subentry.uniqueID = subobj.uniqueIDRef;

                                subentry.HighLimit = subobj.highLimit;
                                subentry.LowLimit = subobj.lowLimit;
                                subentry.actualvalue = subobj.actualValue;

                                //FIXME WTF is going on here??
                                entry.subobjects.Add(subobj.subIndex[1], subentry);

                            }
                        }


                }

            }

            //Process Device after network so we already have the ODEntries populated then can match bu uniqueID

            //ProfileBody_Device_CANopen
            if (body_device != null)
            {
                ProfileBody_Device_CANopen obj = body_device;

                if (obj.DeviceIdentity != null)
                {
                    eds.di.ProductName = obj.DeviceIdentity.productName.Value;
                    eds.di.ProductNumber = EDSsharp.ConvertToUInt32(obj.DeviceIdentity.productID.Value);
                    eds.di.VendorName = obj.DeviceIdentity.vendorName.Value;
                    eds.di.VendorNumber = EDSsharp.ConvertToUInt32(obj.DeviceIdentity.vendorID.Value);

                    foreach (object o in obj.DeviceIdentity.productText.Items)
                    {
                        //this is another g_label affair

                        if (o.GetType() == typeof(vendorTextDescription))
                        {
                            String desc = ((vendorTextDescription)o).Value;
                            string[] bits = desc.Split('|');

                            foreach(string bit in bits)
                            {
                                string[] keyvalue = bit.Split('=');
                                if(keyvalue.Length==2)
                                {
                                    switch(keyvalue[0])
                                    {
                                        case "FileDescription":
                                            eds.fi.Description = keyvalue[1];
                                            break;
                                        case "EdsVersion":
                                            eds.fi.EDSVersion = keyvalue[1];
                                            break;
                                        case "FileRevision":
                                            byte.TryParse(keyvalue[1],out eds.fi.FileVersion);
                                            break;
                                        case "RevisionNum":
                                            byte.TryParse(keyvalue[1], out eds.fi.FileRevision);                                            break;

                                                
                                    }
                                }
                            }
                        }

                        if (o.GetType() == typeof(vendorTextDescriptionRef))
                        {
                        }
                        if (o.GetType() == typeof(vendorTextLabel))
                        {
                        }
                        if (o.GetType() == typeof(vendorTextLabelRef))
                        {
                        }
                    }

                    //fixme i think date should be tested in a separate way
                    //as dates are supported without times
                    if (obj.fileCreationTimeSpecified)
                    {
                        eds.fi.CreationDateTime = obj.fileCreationDate.Add(obj.fileCreationTime.TimeOfDay);
                        eds.fi.CreationDate = eds.fi.CreationDateTime.ToString("MM-dd-yyyy");
                        eds.fi.CreationTime = eds.fi.CreationDateTime.ToString("h:mmtt");

                    }

                    if (obj.fileModificationDateSpecified)
                    {
                        eds.fi.ModificationDateTime = obj.fileModificationDate.Add(obj.fileCreationTime.TimeOfDay);
                        eds.fi.ModificationDate = eds.fi.ModificationDateTime.ToString("MM-dd-yyyy");
                        eds.fi.ModificationTime = eds.fi.ModificationDateTime.ToString("h:mmtt");

                    }

                    eds.fi.ModifiedBy = obj.fileModifiedBy;
                    eds.fi.CreatedBy = obj.fileCreator;
                }

                if (obj.DeviceManager != null)
                {

                }

                if (obj.DeviceFunction != null)
                {

                }

                if (obj.ApplicationProcess != null)
                {

                    if (obj.ApplicationProcess[0] != null)
                    {
                        foreach (parameter param in obj.ApplicationProcess[0].parameterList)
                        {

                            //match unique ID


                            ODentry od = eds.Getobject(param.uniqueID);

                            if (od == null)
                                continue;

                            //fix me defaultValue contains other stuff we might want
                            if (param.defaultValue != null)
                                od.defaultvalue = param.defaultValue.value;

                            //fix me, if more than one vendorTextDescription is present, eg
                            //multi language this will result in the last one being used
                            if (param.Items!=null && param.Items.Length>0)
                            {
                                foreach(object item in param.Items)
                                {
                                    if(item.GetType() == typeof(vendorTextDescription))
                                    {
                                        vendorTextDescription vtd = (vendorTextDescription)item;
                                        od.Description = vtd.Value;
                                    }

                                }

                            }

                            //FIXME: if we have a denotation set for an object in the <parameterList> section but it is not set on the object
                            //use the <parameterList> one. We may discover that this is used for something else and can be removed??
                            if ((od.denotation==null || od.denotation=="") && param.denotation!=null && param.denotation.Items.Length>0)
                            {
                                foreach (object item in param.denotation.Items)
                                {
                                    if (item.GetType() == typeof(vendorTextLabel))
                                    {
                                        vendorTextLabel vtd = (vendorTextLabel)item;
                                        od.denotation = vtd.Value;
                                    }
                                }
                            }





                        }

                    }





                }

            }

            return eds;

        }

    }

}

[XmlRoot(ElementName = "OpenEDSProject")]
public class OpenEDSProject
{
    [XmlElement(ElementName = "ISO15745ProfileContainer", Namespace = "http://www.canopen.org/xml/1.0")]
    public List<ISO15745ProfileContainer> ISO15745ProfileContainer { get; set; }
    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }

}


namespace XSDImport
{

    //------------------------------------------------------------------------------
    // <auto-generated>
    //     This code was generated by a tool.
    //     Runtime Version:4.0.30319.42000
    //
    //     Changes to this file may cause incorrect behavior and will be lost if
    //     the code is regenerated.
    // </auto-generated>
    //------------------------------------------------------------------------------

    using System.Xml.Serialization;

    // 
    // This source code was auto-generated by xsd, Version=4.6.1055.0.
    // 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class ISO15745ProfileContainer
    {

        private ISO15745Profile[] iSO15745ProfileField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ISO15745Profile")]
        public ISO15745Profile[] ISO15745Profile
        {
            get
            {
                return this.iSO15745ProfileField;
            }
            set
            {
                this.iSO15745ProfileField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class ISO15745Profile
    {

        private ProfileHeader_DataType profileHeaderField;

        private ProfileBody_DataType profileBodyField;

        /// <remarks/>
        public ProfileHeader_DataType ProfileHeader
        {
            get
            {
                return this.profileHeaderField;
            }
            set
            {
                this.profileHeaderField = value;
            }
        }

        /// <remarks/>
        public ProfileBody_DataType ProfileBody
        {
            get
            {
                return this.profileBodyField;
            }
            set
            {
                this.profileBodyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileHeader_DataType
    {

        private string profileIdentificationField;

        private string profileRevisionField;

        private string profileNameField;

        private string profileSourceField;

        private ProfileClassID_DataType profileClassIDField;

        private System.DateTime profileDateField;

        private bool profileDateFieldSpecified;

        private string additionalInformationField;

        private ISO15745Reference_DataType iSO15745ReferenceField;

        private string[] iASInterfaceTypeField;

        /// <remarks/>
        public string ProfileIdentification
        {
            get
            {
                return this.profileIdentificationField;
            }
            set
            {
                this.profileIdentificationField = value;
            }
        }

        /// <remarks/>
        public string ProfileRevision
        {
            get
            {
                return this.profileRevisionField;
            }
            set
            {
                this.profileRevisionField = value;
            }
        }

        /// <remarks/>
        public string ProfileName
        {
            get
            {
                return this.profileNameField;
            }
            set
            {
                this.profileNameField = value;
            }
        }

        /// <remarks/>
        public string ProfileSource
        {
            get
            {
                return this.profileSourceField;
            }
            set
            {
                this.profileSourceField = value;
            }
        }

        /// <remarks/>
        public ProfileClassID_DataType ProfileClassID
        {
            get
            {
                return this.profileClassIDField;
            }
            set
            {
                this.profileClassIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ProfileDate
        {
            get
            {
                return this.profileDateField;
            }
            set
            {
                this.profileDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ProfileDateSpecified
        {
            get
            {
                return this.profileDateFieldSpecified;
            }
            set
            {
                this.profileDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
        public string AdditionalInformation
        {
            get
            {
                return this.additionalInformationField;
            }
            set
            {
                this.additionalInformationField = value;
            }
        }

        /// <remarks/>
        public ISO15745Reference_DataType ISO15745Reference
        {
            get
            {
                return this.iSO15745ReferenceField;
            }
            set
            {
                this.iSO15745ReferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("IASInterfaceType")]
        public string[] IASInterfaceType
        {
            get
            {
                return this.iASInterfaceTypeField;
            }
            set
            {
                this.iASInterfaceTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0")]
    public enum ProfileClassID_DataType
    {

        /// <remarks/>
        AIP,

        /// <remarks/>
        Process,

        /// <remarks/>
        InformationExchange,

        /// <remarks/>
        Resource,

        /// <remarks/>
        Device,

        /// <remarks/>
        CommunicationNetwork,

        /// <remarks/>
        Equipment,

        /// <remarks/>
        Human,

        /// <remarks/>
        Material,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ISO15745Reference_DataType
    {

        private string iSO15745PartField;

        private string iSO15745EditionField;

        private string profileTechnologyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
        public string ISO15745Part
        {
            get
            {
                return this.iSO15745PartField;
            }
            set
            {
                this.iSO15745PartField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
        public string ISO15745Edition
        {
            get
            {
                return this.iSO15745EditionField;
            }
            set
            {
                this.iSO15745EditionField = value;
            }
        }

        /// <remarks/>
        public string ProfileTechnology
        {
            get
            {
                return this.profileTechnologyField;
            }
            set
            {
                this.profileTechnologyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileHandle_DataType
    {

        private string profileIdentificationField;

        private string profileRevisionField;

        private string profileLocationField;

        /// <remarks/>
        public string ProfileIdentification
        {
            get
            {
                return this.profileIdentificationField;
            }
            set
            {
                this.profileIdentificationField = value;
            }
        }

        /// <remarks/>
        public string ProfileRevision
        {
            get
            {
                return this.profileRevisionField;
            }
            set
            {
                this.profileRevisionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
        public string ProfileLocation
        {
            get
            {
                return this.profileLocationField;
            }
            set
            {
                this.profileLocationField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ProfileBody_CommunicationNetwork_CANopen))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ProfileBody_Device_CANopen))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0")]
    public abstract partial class ProfileBody_DataType
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopen : ProfileBody_DataType
    {

        private object[] itemsField;

        private string formatNameField;

        private string formatVersionField;

        private string fileNameField;

        private string fileCreatorField;

        private System.DateTime fileCreationDateField;

        private System.DateTime fileCreationTimeField;

        private bool fileCreationTimeFieldSpecified;

        private System.DateTime fileModificationDateField;

        private bool fileModificationDateFieldSpecified;

        private System.DateTime fileModificationTimeField;

        private bool fileModificationTimeFieldSpecified;

        private string fileModifiedByField;

        private string fileVersionField;

        private string supportedLanguagesField;

        public ProfileBody_CommunicationNetwork_CANopen()
        {
            this.formatNameField = "CANopen";
            this.formatVersionField = "1.0";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ApplicationLayers", typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayers))]
        [System.Xml.Serialization.XmlElementAttribute("ExternalProfileHandle", typeof(ProfileHandle_DataType))]
        [System.Xml.Serialization.XmlElementAttribute("NetworkManagement", typeof(ProfileBody_CommunicationNetwork_CANopenNetworkManagement))]
        [System.Xml.Serialization.XmlElementAttribute("TransportLayers", typeof(ProfileBody_CommunicationNetwork_CANopenTransportLayers))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formatName
        {
            get
            {
                return this.formatNameField;
            }
            set
            {
                this.formatNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formatVersion
        {
            get
            {
                return this.formatVersionField;
            }
            set
            {
                this.formatVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileCreator
        {
            get
            {
                return this.fileCreatorField;
            }
            set
            {
                this.fileCreatorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime fileCreationDate
        {
            get
            {
                return this.fileCreationDateField;
            }
            set
            {
                this.fileCreationDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "time")]
        public System.DateTime fileCreationTime
        {
            get
            {
                return this.fileCreationTimeField;
            }
            set
            {
                this.fileCreationTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fileCreationTimeSpecified
        {
            get
            {
                return this.fileCreationTimeFieldSpecified;
            }
            set
            {
                this.fileCreationTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime fileModificationDate
        {
            get
            {
                return this.fileModificationDateField;
            }
            set
            {
                this.fileModificationDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fileModificationDateSpecified
        {
            get
            {
                return this.fileModificationDateFieldSpecified;
            }
            set
            {
                this.fileModificationDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "time")]
        public System.DateTime fileModificationTime
        {
            get
            {
                return this.fileModificationTimeField;
            }
            set
            {
                this.fileModificationTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fileModificationTimeSpecified
        {
            get
            {
                return this.fileModificationTimeFieldSpecified;
            }
            set
            {
                this.fileModificationTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileModifiedBy
        {
            get
            {
                return this.fileModifiedByField;
            }
            set
            {
                this.fileModifiedByField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileVersion
        {
            get
            {
                return this.fileVersionField;
            }
            set
            {
                this.fileVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NMTOKENS")]
        public string supportedLanguages
        {
            get
            {
                return this.supportedLanguagesField;
            }
            set
            {
                this.supportedLanguagesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenApplicationLayers
    {

        private ProfileBody_CommunicationNetwork_CANopenApplicationLayersIdentity identityField;

        private CANopenObjectList cANopenObjectListField;

        private ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy[] dummyUsageField;

        private ProfileBody_CommunicationNetwork_CANopenApplicationLayersDynamicChannel[] dynamicChannelsField;

        private string conformanceClassField;

        private string communicationEntityTypeField;

        public ProfileBody_CommunicationNetwork_CANopenApplicationLayers()
        {
            this.communicationEntityTypeField = "slave";
        }

        /// <remarks/>
        public ProfileBody_CommunicationNetwork_CANopenApplicationLayersIdentity identity
        {
            get
            {
                return this.identityField;
            }
            set
            {
                this.identityField = value;
            }
        }

        /// <remarks/>
        public CANopenObjectList CANopenObjectList
        {
            get
            {
                return this.cANopenObjectListField;
            }
            set
            {
                this.cANopenObjectListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("dummy", IsNullable = false)]
        public ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy[] dummyUsage
        {
            get
            {
                return this.dummyUsageField;
            }
            set
            {
                this.dummyUsageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("dynamicChannel", IsNullable = false)]
        public ProfileBody_CommunicationNetwork_CANopenApplicationLayersDynamicChannel[] dynamicChannels
        {
            get
            {
                return this.dynamicChannelsField;
            }
            set
            {
                this.dynamicChannelsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string conformanceClass
        {
            get
            {
                return this.conformanceClassField;
            }
            set
            {
                this.conformanceClassField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NMTOKENS")]
        [System.ComponentModel.DefaultValueAttribute("slave")]
        public string communicationEntityType
        {
            get
            {
                return this.communicationEntityTypeField;
            }
            set
            {
                this.communicationEntityTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenApplicationLayersIdentity
    {

        private vendorID vendorIDField;

        private deviceFamily deviceFamilyField;

        private productID productIDField;

        private version[] versionField;

        private System.DateTime buildDateField;

        private bool buildDateFieldSpecified;

        private specificationRevision specificationRevisionField;

        /// <remarks/>
        public vendorID vendorID
        {
            get
            {
                return this.vendorIDField;
            }
            set
            {
                this.vendorIDField = value;
            }
        }

        /// <remarks/>
        public deviceFamily deviceFamily
        {
            get
            {
                return this.deviceFamilyField;
            }
            set
            {
                this.deviceFamilyField = value;
            }
        }

        /// <remarks/>
        public productID productID
        {
            get
            {
                return this.productIDField;
            }
            set
            {
                this.productIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("version")]
        public version[] version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime buildDate
        {
            get
            {
                return this.buildDateField;
            }
            set
            {
                this.buildDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool buildDateSpecified
        {
            get
            {
                return this.buildDateFieldSpecified;
            }
            set
            {
                this.buildDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        public specificationRevision specificationRevision
        {
            get
            {
                return this.specificationRevisionField;
            }
            set
            {
                this.specificationRevisionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class vendorID
    {

        private bool readOnlyField;

        private string valueField;

        public vendorID()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class deviceFamily
    {

        private object[] itemsField;

        private bool readOnlyField;

        public deviceFamily()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class vendorTextDescription
    {

        private string langField;

        private string uRIField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "language")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class vendorTextDescriptionRef
    {

        private string dictIDField;

        private string textIDField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string dictID
        {
            get
            {
                return this.dictIDField;
            }
            set
            {
                this.dictIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string textID
        {
            get
            {
                return this.textIDField;
            }
            set
            {
                this.textIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "anyURI")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class vendorTextLabel
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "language")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class vendorTextLabelRef
    {

        private string dictIDField;

        private string textIDField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string dictID
        {
            get
            {
                return this.dictIDField;
            }
            set
            {
                this.dictIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string textID
        {
            get
            {
                return this.textIDField;
            }
            set
            {
                this.textIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "anyURI")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class productID
    {

        private bool readOnlyField;

        private string valueField;

        public productID()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class version
    {

        private versionVersionType versionTypeField;

        private bool readOnlyField;

        private string valueField;

        public version()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public versionVersionType versionType
        {
            get
            {
                return this.versionTypeField;
            }
            set
            {
                this.versionTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum versionVersionType
    {

        /// <remarks/>
        SW,

        /// <remarks/>
        FW,

        /// <remarks/>
        HW,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class specificationRevision
    {

        private bool readOnlyField;

        private string valueField;

        public specificationRevision()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class CANopenObjectList
    {

        private CANopenObjectListCANopenObject[] cANopenObjectField;

        private uint mandatoryObjectsField;

        private bool mandatoryObjectsFieldSpecified;

        private uint optionalObjectsField;

        private bool optionalObjectsFieldSpecified;

        private uint manufacturerObjectsField;

        private bool manufacturerObjectsFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CANopenObject")]
        public CANopenObjectListCANopenObject[] CANopenObject
        {
            get
            {
                return this.cANopenObjectField;
            }
            set
            {
                this.cANopenObjectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint mandatoryObjects
        {
            get
            {
                return this.mandatoryObjectsField;
            }
            set
            {
                this.mandatoryObjectsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool mandatoryObjectsSpecified
        {
            get
            {
                return this.mandatoryObjectsFieldSpecified;
            }
            set
            {
                this.mandatoryObjectsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint optionalObjects
        {
            get
            {
                return this.optionalObjectsField;
            }
            set
            {
                this.optionalObjectsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool optionalObjectsSpecified
        {
            get
            {
                return this.optionalObjectsFieldSpecified;
            }
            set
            {
                this.optionalObjectsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint manufacturerObjects
        {
            get
            {
                return this.manufacturerObjectsField;
            }
            set
            {
                this.manufacturerObjectsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool manufacturerObjectsSpecified
        {
            get
            {
                return this.manufacturerObjectsFieldSpecified;
            }
            set
            {
                this.manufacturerObjectsFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class CANopenObjectListCANopenObject
    {

        private CANopenObjectListCANopenObjectCANopenSubObject[] cANopenSubObjectField;

        private byte[] indexField;

        private string nameField;

        private byte objectTypeField;

        private byte[] dataTypeField;

        private string lowLimitField;

        private string highLimitField;

        private CANopenObjectListCANopenObjectAccessType accessTypeField;

        private bool accessTypeFieldSpecified;

        private string defaultValueField;

        private string actualValueField;

        private string denotationField;

        private string edseditor_extension_storagelocationField;

        private bool edseditor_extension_notifyonchangeField;

        private CANopenObjectListCANopenObjectPDOmapping pDOmappingField;

        private bool pDOmappingFieldSpecified;

        private byte[] objFlagsField;

        private string uniqueIDRefField;

        private byte subNumberField;

        private bool subNumberFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CANopenSubObject")]
        public CANopenObjectListCANopenObjectCANopenSubObject[] CANopenSubObject
        {
            get
            {
                return this.cANopenSubObjectField;
            }
            set
            {
                this.cANopenSubObjectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte objectType
        {
            get
            {
                return this.objectTypeField;
            }
            set
            {
                this.objectTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] dataType
        {
            get
            {
                return this.dataTypeField;
            }
            set
            {
                this.dataTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string lowLimit
        {
            get
            {
                return this.lowLimitField;
            }
            set
            {
                this.lowLimitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string highLimit
        {
            get
            {
                return this.highLimitField;
            }
            set
            {
                this.highLimitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CANopenObjectListCANopenObjectAccessType accessType
        {
            get
            {
                return this.accessTypeField;
            }
            set
            {
                this.accessTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool accessTypeSpecified
        {
            get
            {
                return this.accessTypeFieldSpecified;
            }
            set
            {
                this.accessTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string actualValue
        {
            get
            {
                return this.actualValueField;
            }
            set
            {
                this.actualValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string denotation
        {
            get
            {
                return this.denotationField;
            }
            set
            {
                this.denotationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string edseditor_extenstion_storagelocation
        {
            get
            {
                return this.edseditor_extension_storagelocationField;
            }
            set
            {
                this.edseditor_extension_storagelocationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool edseditor_extension_notifyonchange
        {
            get
            {
                return this.edseditor_extension_notifyonchangeField;
            }
            set
            {
                this.edseditor_extension_notifyonchangeField = value;
            }
        }

      

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CANopenObjectListCANopenObjectPDOmapping PDOmapping
        {
            get
            {
                return this.pDOmappingField;
            }
            set
            {
                this.pDOmappingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PDOmappingSpecified
        {
            get
            {
                return this.pDOmappingFieldSpecified;
            }
            set
            {
                this.pDOmappingFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] objFlags
        {
            get
            {
                return this.objFlagsField;
            }
            set
            {
                this.objFlagsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string uniqueIDRef
        {
            get
            {
                return this.uniqueIDRefField;
            }
            set
            {
                this.uniqueIDRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte subNumber
        {
            get
            {
                return this.subNumberField;
            }
            set
            {
                this.subNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool subNumberSpecified
        {
            get
            {
                return this.subNumberFieldSpecified;
            }
            set
            {
                this.subNumberFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class CANopenObjectListCANopenObjectCANopenSubObject
    {

        private byte[] subIndexField;

        private string nameField;

        private byte objectTypeField;

        private byte[] dataTypeField;

        private string lowLimitField;

        private string highLimitField;

        private CANopenObjectListCANopenObjectCANopenSubObjectAccessType accessTypeField;

        private bool accessTypeFieldSpecified;

        private string defaultValueField;

        private string actualValueField;

        private string denotationField;

        private CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping pDOmappingField;

        private bool pDOmappingFieldSpecified;

        private byte[] objFlagsField;

        private string uniqueIDRefField;

        private bool edseditor_extension_notifyonchangeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool edseditor_extension_notifyonchange
        {
            get
            {
                return this.edseditor_extension_notifyonchangeField;
            }
            set
            {
                this.edseditor_extension_notifyonchangeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] subIndex
        {
            get
            {
                return this.subIndexField;
            }
            set
            {
                this.subIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte objectType
        {
            get
            {
                return this.objectTypeField;
            }
            set
            {
                this.objectTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] dataType
        {
            get
            {
                return this.dataTypeField;
            }
            set
            {
                this.dataTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string lowLimit
        {
            get
            {
                return this.lowLimitField;
            }
            set
            {
                this.lowLimitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string highLimit
        {
            get
            {
                return this.highLimitField;
            }
            set
            {
                this.highLimitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CANopenObjectListCANopenObjectCANopenSubObjectAccessType accessType
        {
            get
            {
                return this.accessTypeField;
            }
            set
            {
                this.accessTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool accessTypeSpecified
        {
            get
            {
                return this.accessTypeFieldSpecified;
            }
            set
            {
                this.accessTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string actualValue
        {
            get
            {
                return this.actualValueField;
            }
            set
            {
                this.actualValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string denotation
        {
            get
            {
                return this.denotationField;
            }
            set
            {
                this.denotationField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping PDOmapping
        {
            get
            {
                return this.pDOmappingField;
            }
            set
            {
                this.pDOmappingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PDOmappingSpecified
        {
            get
            {
                return this.pDOmappingFieldSpecified;
            }
            set
            {
                this.pDOmappingFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] objFlags
        {
            get
            {
                return this.objFlagsField;
            }
            set
            {
                this.objFlagsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string uniqueIDRef
        {
            get
            {
                return this.uniqueIDRefField;
            }
            set
            {
                this.uniqueIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum CANopenObjectListCANopenObjectCANopenSubObjectAccessType
    {

        /// <remarks/>
        ro,

        /// <remarks/>
        wo,

        /// <remarks/>
        rw,

        /// <remarks/>
        @const,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping
    {

        /// <remarks/>
        no,

        /// <remarks/>
        @default,

        /// <remarks/>
        optional,

        /// <remarks/>
        TPDO,

        /// <remarks/>
        RPDO,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum CANopenObjectListCANopenObjectAccessType
    {

        /// <remarks/>
        ro,

        /// <remarks/>
        wo,

        /// <remarks/>
        rw,

        /// <remarks/>
        @const,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum CANopenObjectListCANopenObjectPDOmapping
    {

        /// <remarks/>
        no,

        /// <remarks/>
        @default,

        /// <remarks/>
        optional,

        /// <remarks/>
        TPDO,

        /// <remarks/>
        RPDO,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy
    {

        private ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry entryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry entry
        {
            get
            {
                return this.entryField;
            }
            set
            {
                this.entryField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0001=0")]
        Dummy00010,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0002=0")]
        Dummy00020,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0003=0")]
        Dummy00030,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0004=0")]
        Dummy00040,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0005=0")]
        Dummy00050,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0006=0")]
        Dummy00060,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0007=0")]
        Dummy00070,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0001=1")]
        Dummy00011,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0002=1")]
        Dummy00021,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0003=1")]
        Dummy00031,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0004=1")]
        Dummy00041,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0005=1")]
        Dummy00051,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0006=1")]
        Dummy00061,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Dummy0007=1")]
        Dummy00071,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenApplicationLayersDynamicChannel
    {

        private byte[] dataTypeField;

        private ProfileBody_CommunicationNetwork_CANopenApplicationLayersDynamicChannelAccessType accessTypeField;

        private byte[] startIndexField;

        private byte[] endIndexField;

        private uint maxNumberField;

        private byte[] addressOffsetField;

        private byte bitAlignmentField;

        private bool bitAlignmentFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] dataType
        {
            get
            {
                return this.dataTypeField;
            }
            set
            {
                this.dataTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ProfileBody_CommunicationNetwork_CANopenApplicationLayersDynamicChannelAccessType accessType
        {
            get
            {
                return this.accessTypeField;
            }
            set
            {
                this.accessTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] startIndex
        {
            get
            {
                return this.startIndexField;
            }
            set
            {
                this.startIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] endIndex
        {
            get
            {
                return this.endIndexField;
            }
            set
            {
                this.endIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint maxNumber
        {
            get
            {
                return this.maxNumberField;
            }
            set
            {
                this.maxNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] addressOffset
        {
            get
            {
                return this.addressOffsetField;
            }
            set
            {
                this.addressOffsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte bitAlignment
        {
            get
            {
                return this.bitAlignmentField;
            }
            set
            {
                this.bitAlignmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool bitAlignmentSpecified
        {
            get
            {
                return this.bitAlignmentFieldSpecified;
            }
            set
            {
                this.bitAlignmentFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum ProfileBody_CommunicationNetwork_CANopenApplicationLayersDynamicChannelAccessType
    {

        /// <remarks/>
        readOnly,

        /// <remarks/>
        writeOnly,

        /// <remarks/>
        readWriteOutput,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenNetworkManagement
    {

        private ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenGeneralFeatures cANopenGeneralFeaturesField;

        private ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenMasterFeatures cANopenMasterFeaturesField;

        private ProfileBody_CommunicationNetwork_CANopenNetworkManagementDeviceCommissioning deviceCommissioningField;

        /// <remarks/>
        public ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenGeneralFeatures CANopenGeneralFeatures
        {
            get
            {
                return this.cANopenGeneralFeaturesField;
            }
            set
            {
                this.cANopenGeneralFeaturesField = value;
            }
        }

        /// <remarks/>
        public ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenMasterFeatures CANopenMasterFeatures
        {
            get
            {
                return this.cANopenMasterFeaturesField;
            }
            set
            {
                this.cANopenMasterFeaturesField = value;
            }
        }

        /// <remarks/>
        public ProfileBody_CommunicationNetwork_CANopenNetworkManagementDeviceCommissioning deviceCommissioning
        {
            get
            {
                return this.deviceCommissioningField;
            }
            set
            {
                this.deviceCommissioningField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenGeneralFeatures
    {

        private bool groupMessagingField;

        private byte dynamicChannelsField;

        private bool selfStartingDeviceField;

        private bool sDORequestingDeviceField;

        private byte granularityField;

        private ushort nrOfRxPDOField;

        private ushort nrOfTxPDOField;

        private bool bootUpSlaveField;

        private bool layerSettingServiceSlaveField;

        public ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenGeneralFeatures()
        {
            this.groupMessagingField = false;
            this.dynamicChannelsField = ((byte)(0));
            this.selfStartingDeviceField = false;
            this.sDORequestingDeviceField = false;
            this.nrOfRxPDOField = ((ushort)(0));
            this.nrOfTxPDOField = ((ushort)(0));
            this.bootUpSlaveField = false;
            this.layerSettingServiceSlaveField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool groupMessaging
        {
            get
            {
                return this.groupMessagingField;
            }
            set
            {
                this.groupMessagingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(byte), "0")]
        public byte dynamicChannels
        {
            get
            {
                return this.dynamicChannelsField;
            }
            set
            {
                this.dynamicChannelsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selfStartingDevice
        {
            get
            {
                return this.selfStartingDeviceField;
            }
            set
            {
                this.selfStartingDeviceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool SDORequestingDevice
        {
            get
            {
                return this.sDORequestingDeviceField;
            }
            set
            {
                this.sDORequestingDeviceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte granularity
        {
            get
            {
                return this.granularityField;
            }
            set
            {
                this.granularityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(ushort), "0")]
        public ushort nrOfRxPDO
        {
            get
            {
                return this.nrOfRxPDOField;
            }
            set
            {
                this.nrOfRxPDOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(ushort), "0")]
        public ushort nrOfTxPDO
        {
            get
            {
                return this.nrOfTxPDOField;
            }
            set
            {
                this.nrOfTxPDOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool bootUpSlave
        {
            get
            {
                return this.bootUpSlaveField;
            }
            set
            {
                this.bootUpSlaveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool layerSettingServiceSlave
        {
            get
            {
                return this.layerSettingServiceSlaveField;
            }
            set
            {
                this.layerSettingServiceSlaveField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenMasterFeatures
    {

        private bool bootUpMasterField;

        private bool flyingMasterField;

        private bool sDOManagerField;

        private bool configurationManagerField;

        private bool layerSettingServiceMasterField;

        public ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenMasterFeatures()
        {
            this.bootUpMasterField = false;
            this.flyingMasterField = false;
            this.sDOManagerField = false;
            this.configurationManagerField = false;
            this.layerSettingServiceMasterField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool bootUpMaster
        {
            get
            {
                return this.bootUpMasterField;
            }
            set
            {
                this.bootUpMasterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool flyingMaster
        {
            get
            {
                return this.flyingMasterField;
            }
            set
            {
                this.flyingMasterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool SDOManager
        {
            get
            {
                return this.sDOManagerField;
            }
            set
            {
                this.sDOManagerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool configurationManager
        {
            get
            {
                return this.configurationManagerField;
            }
            set
            {
                this.configurationManagerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool layerSettingServiceMaster
        {
            get
            {
                return this.layerSettingServiceMasterField;
            }
            set
            {
                this.layerSettingServiceMasterField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenNetworkManagementDeviceCommissioning
    {

        private byte nodeIDField;

        private string nodeNameField;

        private string actualBaudRateField;

        private ulong networkNumberField;

        private string networkNameField;

        private bool cANopenManagerField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte nodeID
        {
            get
            {
                return this.nodeIDField;
            }
            set
            {
                this.nodeIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nodeName
        {
            get
            {
                return this.nodeNameField;
            }
            set
            {
                this.nodeNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string actualBaudRate
        {
            get
            {
                return this.actualBaudRateField;
            }
            set
            {
                this.actualBaudRateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong networkNumber
        {
            get
            {
                return this.networkNumberField;
            }
            set
            {
                this.networkNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string networkName
        {
            get
            {
                return this.networkNameField;
            }
            set
            {
                this.networkNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool CANopenManager
        {
            get
            {
                return this.cANopenManagerField;
            }
            set
            {
                this.cANopenManagerField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenTransportLayers
    {

        private ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayer physicalLayerField;

        /// <remarks/>
        public ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayer PhysicalLayer
        {
            get
            {
                return this.physicalLayerField;
            }
            set
            {
                this.physicalLayerField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayer
    {

        private ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRate baudRateField;

        /// <remarks/>
        public ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRate baudRate
        {
            get
            {
                return this.baudRateField;
            }
            set
            {
                this.baudRateField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRate
    {

        private ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate[] supportedBaudRateField;

        private ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateDefaultValue defaultValueField;

        public ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRate()
        {
            this.defaultValueField = ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateDefaultValue.Item250Kbps;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("supportedBaudRate")]
        public ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate[] supportedBaudRate
        {
            get
            {
                return this.supportedBaudRateField;
            }
            set
            {
                this.supportedBaudRateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateDefaultValue.Item250Kbps)]
        public ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateDefaultValue defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate
    {

        private ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10 Kbps")]
        Item10Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20 Kbps")]
        Item20Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("50 Kbps")]
        Item50Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("100 Kbps")]
        Item100Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("125 Kbps")]
        Item125Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("250 Kbps")]
        Item250Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("500 Kbps")]
        Item500Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("800 Kbps")]
        Item800Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1000 Kbps")]
        Item1000Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("auto-baudRate")]
        autobaudRate,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateDefaultValue
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10 Kbps")]
        Item10Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20 Kbps")]
        Item20Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("50 Kbps")]
        Item50Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("100 Kbps")]
        Item100Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("125 Kbps")]
        Item125Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("250 Kbps")]
        Item250Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("500 Kbps")]
        Item500Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("800 Kbps")]
        Item800Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1000 Kbps")]
        Item1000Kbps,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("auto-baudRate")]
        autobaudRate,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class ProfileBody_Device_CANopen : ProfileBody_DataType
    {

        private DeviceIdentity deviceIdentityField;

        private DeviceManager deviceManagerField;

        private DeviceFunction[] deviceFunctionField;

        private ApplicationProcess[] applicationProcessField;

        private ProfileHandle_DataType[] externalProfileHandleField;

        private string formatNameField;

        private string formatVersionField;

        private string fileNameField;

        private string fileCreatorField;

        private System.DateTime fileCreationDateField;

        private System.DateTime fileCreationTimeField;

        private bool fileCreationTimeFieldSpecified;

        private System.DateTime fileModificationDateField;

        private bool fileModificationDateFieldSpecified;

        private System.DateTime fileModificationTimeField;

        private bool fileModificationTimeFieldSpecified;

        private string fileModifiedByField;

        private string fileVersionField;

        private string supportedLanguagesField;

        private ProfileBody_Device_CANopenDeviceClass deviceClassField;

        private bool deviceClassFieldSpecified;

        public ProfileBody_Device_CANopen()
        {
            this.formatNameField = "CANopen";
            this.formatVersionField = "1.0";
        }

        /// <remarks/>
        public DeviceIdentity DeviceIdentity
        {
            get
            {
                return this.deviceIdentityField;
            }
            set
            {
                this.deviceIdentityField = value;
            }
        }

        /// <remarks/>
        public DeviceManager DeviceManager
        {
            get
            {
                return this.deviceManagerField;
            }
            set
            {
                this.deviceManagerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DeviceFunction")]
        public DeviceFunction[] DeviceFunction
        {
            get
            {
                return this.deviceFunctionField;
            }
            set
            {
                this.deviceFunctionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ApplicationProcess")]
        public ApplicationProcess[] ApplicationProcess
        {
            get
            {
                return this.applicationProcessField;
            }
            set
            {
                this.applicationProcessField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ExternalProfileHandle")]
        public ProfileHandle_DataType[] ExternalProfileHandle
        {
            get
            {
                return this.externalProfileHandleField;
            }
            set
            {
                this.externalProfileHandleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formatName
        {
            get
            {
                return this.formatNameField;
            }
            set
            {
                this.formatNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formatVersion
        {
            get
            {
                return this.formatVersionField;
            }
            set
            {
                this.formatVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileCreator
        {
            get
            {
                return this.fileCreatorField;
            }
            set
            {
                this.fileCreatorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime fileCreationDate
        {
            get
            {
                return this.fileCreationDateField;
            }
            set
            {
                this.fileCreationDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "time")]
        public System.DateTime fileCreationTime
        {
            get
            {
                return this.fileCreationTimeField;
            }
            set
            {
                this.fileCreationTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fileCreationTimeSpecified
        {
            get
            {
                return this.fileCreationTimeFieldSpecified;
            }
            set
            {
                this.fileCreationTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime fileModificationDate
        {
            get
            {
                return this.fileModificationDateField;
            }
            set
            {
                this.fileModificationDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fileModificationDateSpecified
        {
            get
            {
                return this.fileModificationDateFieldSpecified;
            }
            set
            {
                this.fileModificationDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "time")]
        public System.DateTime fileModificationTime
        {
            get
            {
                return this.fileModificationTimeField;
            }
            set
            {
                this.fileModificationTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fileModificationTimeSpecified
        {
            get
            {
                return this.fileModificationTimeFieldSpecified;
            }
            set
            {
                this.fileModificationTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileModifiedBy
        {
            get
            {
                return this.fileModifiedByField;
            }
            set
            {
                this.fileModifiedByField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileVersion
        {
            get
            {
                return this.fileVersionField;
            }
            set
            {
                this.fileVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NMTOKENS")]
        public string supportedLanguages
        {
            get
            {
                return this.supportedLanguagesField;
            }
            set
            {
                this.supportedLanguagesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ProfileBody_Device_CANopenDeviceClass deviceClass
        {
            get
            {
                return this.deviceClassField;
            }
            set
            {
                this.deviceClassField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool deviceClassSpecified
        {
            get
            {
                return this.deviceClassFieldSpecified;
            }
            set
            {
                this.deviceClassFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class DeviceIdentity
    {

        private vendorName vendorNameField;

        private vendorID vendorIDField;

        private vendorText vendorTextField;

        private deviceFamily deviceFamilyField;

        private productFamily productFamilyField;

        private productName productNameField;

        private productID productIDField;

        private productText productTextField;

        private orderNumber[] orderNumberField;

        private version[] versionField;

        private System.DateTime buildDateField;

        private bool buildDateFieldSpecified;

        private specificationRevision specificationRevisionField;

        private instanceName instanceNameField;

        /// <remarks/>
        public vendorName vendorName
        {
            get
            {
                return this.vendorNameField;
            }
            set
            {
                this.vendorNameField = value;
            }
        }

        /// <remarks/>
        public vendorID vendorID
        {
            get
            {
                return this.vendorIDField;
            }
            set
            {
                this.vendorIDField = value;
            }
        }

        /// <remarks/>
        public vendorText vendorText
        {
            get
            {
                return this.vendorTextField;
            }
            set
            {
                this.vendorTextField = value;
            }
        }

        /// <remarks/>
        public deviceFamily deviceFamily
        {
            get
            {
                return this.deviceFamilyField;
            }
            set
            {
                this.deviceFamilyField = value;
            }
        }

        /// <remarks/>
        public productFamily productFamily
        {
            get
            {
                return this.productFamilyField;
            }
            set
            {
                this.productFamilyField = value;
            }
        }

        /// <remarks/>
        public productName productName
        {
            get
            {
                return this.productNameField;
            }
            set
            {
                this.productNameField = value;
            }
        }

        /// <remarks/>
        public productID productID
        {
            get
            {
                return this.productIDField;
            }
            set
            {
                this.productIDField = value;
            }
        }

        /// <remarks/>
        public productText productText
        {
            get
            {
                return this.productTextField;
            }
            set
            {
                this.productTextField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("orderNumber")]
        public orderNumber[] orderNumber
        {
            get
            {
                return this.orderNumberField;
            }
            set
            {
                this.orderNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("version")]
        public version[] version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime buildDate
        {
            get
            {
                return this.buildDateField;
            }
            set
            {
                this.buildDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool buildDateSpecified
        {
            get
            {
                return this.buildDateFieldSpecified;
            }
            set
            {
                this.buildDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        public specificationRevision specificationRevision
        {
            get
            {
                return this.specificationRevisionField;
            }
            set
            {
                this.specificationRevisionField = value;
            }
        }

        /// <remarks/>
        public instanceName instanceName
        {
            get
            {
                return this.instanceNameField;
            }
            set
            {
                this.instanceNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class vendorName
    {

        private bool readOnlyField;

        private string valueField;

        public vendorName()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class vendorText
    {

        private object[] itemsField;

        private bool readOnlyField;

        public vendorText()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class productFamily
    {

        private bool readOnlyField;

        private string valueField;

        public productFamily()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class productName
    {

        private bool readOnlyField;

        private string valueField;

        public productName()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class productText
    {

        private object[] itemsField;

        private bool readOnlyField;

        public productText()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class orderNumber
    {

        private bool readOnlyField;

        private string valueField;

        public orderNumber()
        {
            this.readOnlyField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(true)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class instanceName
    {

        private bool readOnlyField;

        private string valueField;

        public instanceName()
        {
            this.readOnlyField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool readOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class DeviceManager
    {

        private indicatorList indicatorListField;

        /// <remarks/>
        public indicatorList indicatorList
        {
            get
            {
                return this.indicatorListField;
            }
            set
            {
                this.indicatorListField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class indicatorList
    {

        private LEDList lEDListField;

        /// <remarks/>
        public LEDList LEDList
        {
            get
            {
                return this.lEDListField;
            }
            set
            {
                this.lEDListField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class LEDList
    {

        private LED[] lEDField;

        private combinedState[] combinedStateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("LED")]
        public LED[] LED
        {
            get
            {
                return this.lEDField;
            }
            set
            {
                this.lEDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("combinedState")]
        public combinedState[] combinedState
        {
            get
            {
                return this.combinedStateField;
            }
            set
            {
                this.combinedStateField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class LED
    {

        private object[] itemsField;

        private LEDstate[] lEDstateField;

        private LEDLEDcolors lEDcolorsField;

        private LEDLEDtype lEDtypeField;

        private bool lEDtypeFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("LEDstate")]
        public LEDstate[] LEDstate
        {
            get
            {
                return this.lEDstateField;
            }
            set
            {
                this.lEDstateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public LEDLEDcolors LEDcolors
        {
            get
            {
                return this.lEDcolorsField;
            }
            set
            {
                this.lEDcolorsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public LEDLEDtype LEDtype
        {
            get
            {
                return this.lEDtypeField;
            }
            set
            {
                this.lEDtypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LEDtypeSpecified
        {
            get
            {
                return this.lEDtypeFieldSpecified;
            }
            set
            {
                this.lEDtypeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class LEDstate
    {

        private object[] itemsField;

        private string uniqueIDField;

        private LEDstateState stateField;

        private LEDstateLEDcolor lEDcolorField;

        private uint flashingPeriodField;

        private bool flashingPeriodFieldSpecified;

        private byte impulsWidthField;

        private byte numberOfImpulsesField;

        public LEDstate()
        {
            this.impulsWidthField = ((byte)(50));
            this.numberOfImpulsesField = ((byte)(1));
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public LEDstateState state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public LEDstateLEDcolor LEDcolor
        {
            get
            {
                return this.lEDcolorField;
            }
            set
            {
                this.lEDcolorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint flashingPeriod
        {
            get
            {
                return this.flashingPeriodField;
            }
            set
            {
                this.flashingPeriodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool flashingPeriodSpecified
        {
            get
            {
                return this.flashingPeriodFieldSpecified;
            }
            set
            {
                this.flashingPeriodFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(byte), "50")]
        public byte impulsWidth
        {
            get
            {
                return this.impulsWidthField;
            }
            set
            {
                this.impulsWidthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(byte), "1")]
        public byte numberOfImpulses
        {
            get
            {
                return this.numberOfImpulsesField;
            }
            set
            {
                this.numberOfImpulsesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum LEDstateState
    {

        /// <remarks/>
        on,

        /// <remarks/>
        off,

        /// <remarks/>
        flashing,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum LEDstateLEDcolor
    {

        /// <remarks/>
        green,

        /// <remarks/>
        amber,

        /// <remarks/>
        red,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum LEDLEDcolors
    {

        /// <remarks/>
        monocolor,

        /// <remarks/>
        bicolor,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum LEDLEDtype
    {

        /// <remarks/>
        IO,

        /// <remarks/>
        device,

        /// <remarks/>
        communication,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class combinedState
    {

        private object[] itemsField;

        private combinedStateLEDstateRef[] lEDstateRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("LEDstateRef")]
        public combinedStateLEDstateRef[] LEDstateRef
        {
            get
            {
                return this.lEDstateRefField;
            }
            set
            {
                this.lEDstateRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class combinedStateLEDstateRef
    {

        private string stateIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string stateIDRef
        {
            get
            {
                return this.stateIDRefField;
            }
            set
            {
                this.stateIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class DeviceFunction
    {

        private capabilities capabilitiesField;

        private picture[] picturesListField;

        private dictionary[] dictionaryListField;

        /// <remarks/>
        public capabilities capabilities
        {
            get
            {
                return this.capabilitiesField;
            }
            set
            {
                this.capabilitiesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("picture", IsNullable = false)]
        public picture[] picturesList
        {
            get
            {
                return this.picturesListField;
            }
            set
            {
                this.picturesListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("dictionary", IsNullable = false)]
        public dictionary[] dictionaryList
        {
            get
            {
                return this.dictionaryListField;
            }
            set
            {
                this.dictionaryListField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class capabilities
    {

        private characteristicsList[] characteristicsListField;

        private compliantWith[] standardComplianceListField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("characteristicsList")]
        public characteristicsList[] characteristicsList
        {
            get
            {
                return this.characteristicsListField;
            }
            set
            {
                this.characteristicsListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("compliantWith", IsNullable = false)]
        public compliantWith[] standardComplianceList
        {
            get
            {
                return this.standardComplianceListField;
            }
            set
            {
                this.standardComplianceListField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class characteristicsList
    {

        private characteristicsListCategory categoryField;

        private characteristic[] characteristicField;

        /// <remarks/>
        public characteristicsListCategory category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("characteristic")]
        public characteristic[] characteristic
        {
            get
            {
                return this.characteristicField;
            }
            set
            {
                this.characteristicField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class characteristicsListCategory
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class characteristic
    {

        private characteristicName characteristicNameField;

        private characteristicContent[] characteristicContentField;

        /// <remarks/>
        public characteristicName characteristicName
        {
            get
            {
                return this.characteristicNameField;
            }
            set
            {
                this.characteristicNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("characteristicContent")]
        public characteristicContent[] characteristicContent
        {
            get
            {
                return this.characteristicContentField;
            }
            set
            {
                this.characteristicContentField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class characteristicName
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class characteristicContent
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class compliantWith
    {

        private object[] itemsField;

        private string nameField;

        private compliantWithRange rangeField;

        public compliantWith()
        {
            this.rangeField = compliantWithRange.international;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(compliantWithRange.international)]
        public compliantWithRange range
        {
            get
            {
                return this.rangeField;
            }
            set
            {
                this.rangeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum compliantWithRange
    {

        /// <remarks/>
        international,

        /// <remarks/>
        @internal,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class picture
    {

        private object[] itemsField;

        private string uRIField;

        private uint numberField;

        private bool numberFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool numberSpecified
        {
            get
            {
                return this.numberFieldSpecified;
            }
            set
            {
                this.numberFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class dictionary
    {

        private file fileField;

        private string langField;

        private string dictIDField;

        /// <remarks/>
        public file file
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "language")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string dictID
        {
            get
            {
                return this.dictIDField;
            }
            set
            {
                this.dictIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class file
    {

        private string uRIField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class ApplicationProcess
    {

        private dataTypeList dataTypeListField;

        private functionType[] functionTypeListField;

        private functionInstanceList functionInstanceListField;

        private templateList templateListField;

        private parameter[] parameterListField;

        private parameterGroup[] parameterGroupListField;

        /// <remarks/>
        public dataTypeList dataTypeList
        {
            get
            {
                return this.dataTypeListField;
            }
            set
            {
                this.dataTypeListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("functionType", IsNullable = false)]
        public functionType[] functionTypeList
        {
            get
            {
                return this.functionTypeListField;
            }
            set
            {
                this.functionTypeListField = value;
            }
        }

        /// <remarks/>
        public functionInstanceList functionInstanceList
        {
            get
            {
                return this.functionInstanceListField;
            }
            set
            {
                this.functionInstanceListField = value;
            }
        }

        /// <remarks/>
        public templateList templateList
        {
            get
            {
                return this.templateListField;
            }
            set
            {
                this.templateListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("parameter", IsNullable = false)]
        public parameter[] parameterList
        {
            get
            {
                return this.parameterListField;
            }
            set
            {
                this.parameterListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("parameterGroup", IsNullable = false)]
        public parameterGroup[] parameterGroupList
        {
            get
            {
                return this.parameterGroupListField;
            }
            set
            {
                this.parameterGroupListField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class dataTypeList
    {

        private array[] arrayField;

        private @struct[] structField;

        private @enum[] enumField;

        private derived[] derivedField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("array")]
        public array[] array
        {
            get
            {
                return this.arrayField;
            }
            set
            {
                this.arrayField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("struct")]
        public @struct[] @struct
        {
            get
            {
                return this.structField;
            }
            set
            {
                this.structField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("enum")]
        public @enum[] @enum
        {
            get
            {
                return this.enumField;
            }
            set
            {
                this.enumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("derived")]
        public derived[] derived
        {
            get
            {
                return this.derivedField;
            }
            set
            {
                this.derivedField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class array
    {

        private object[] itemsField;

        private subrange[] subrangeField;

        private object itemField;

        private ItemChoiceType itemElementNameField;

        private string nameField;

        private string uniqueIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subrange")]
        public subrange[] subrange
        {
            get
            {
                return this.subrangeField;
            }
            set
            {
                this.subrangeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BITSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BOOL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BYTE", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("CHAR", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("INT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LREAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("REAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("SINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("STRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UDINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("ULINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("USINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("dataTypeIDRef", typeof(dataTypeIDRef))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class subrange
    {

        private long lowerLimitField;

        private long upperLimitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long lowerLimit
        {
            get
            {
                return this.lowerLimitField;
            }
            set
            {
                this.lowerLimitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long upperLimit
        {
            get
            {
                return this.upperLimitField;
            }
            set
            {
                this.upperLimitField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class dataTypeIDRef
    {

        private string uniqueIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string uniqueIDRef
        {
            get
            {
                return this.uniqueIDRefField;
            }
            set
            {
                this.uniqueIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0", IncludeInSchema = false)]
    public enum ItemChoiceType
    {

        /// <remarks/>
        BITSTRING,

        /// <remarks/>
        BOOL,

        /// <remarks/>
        BYTE,

        /// <remarks/>
        CHAR,

        /// <remarks/>
        DINT,

        /// <remarks/>
        DWORD,

        /// <remarks/>
        INT,

        /// <remarks/>
        LINT,

        /// <remarks/>
        LREAL,

        /// <remarks/>
        LWORD,

        /// <remarks/>
        REAL,

        /// <remarks/>
        SINT,

        /// <remarks/>
        STRING,

        /// <remarks/>
        UDINT,

        /// <remarks/>
        UINT,

        /// <remarks/>
        ULINT,

        /// <remarks/>
        USINT,

        /// <remarks/>
        WORD,

        /// <remarks/>
        WSTRING,

        /// <remarks/>
        dataTypeIDRef,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class @struct
    {

        private object[] itemsField;

        private varDeclaration[] varDeclarationField;

        private string nameField;

        private string uniqueIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("varDeclaration")]
        public varDeclaration[] varDeclaration
        {
            get
            {
                return this.varDeclarationField;
            }
            set
            {
                this.varDeclarationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class varDeclaration
    {

        private object[] itemsField;

        private object itemField;

        private ItemChoiceType1 itemElementNameField;

        private string nameField;

        private string uniqueIDField;

        private string sizeField;

        private string initialValueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BITSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BOOL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BYTE", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("CHAR", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("INT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LREAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("REAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("SINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("STRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UDINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("ULINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("USINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("dataTypeIDRef", typeof(dataTypeIDRef))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType1 ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string initialValue
        {
            get
            {
                return this.initialValueField;
            }
            set
            {
                this.initialValueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0", IncludeInSchema = false)]
    public enum ItemChoiceType1
    {

        /// <remarks/>
        BITSTRING,

        /// <remarks/>
        BOOL,

        /// <remarks/>
        BYTE,

        /// <remarks/>
        CHAR,

        /// <remarks/>
        DINT,

        /// <remarks/>
        DWORD,

        /// <remarks/>
        INT,

        /// <remarks/>
        LINT,

        /// <remarks/>
        LREAL,

        /// <remarks/>
        LWORD,

        /// <remarks/>
        REAL,

        /// <remarks/>
        SINT,

        /// <remarks/>
        STRING,

        /// <remarks/>
        UDINT,

        /// <remarks/>
        UINT,

        /// <remarks/>
        ULINT,

        /// <remarks/>
        USINT,

        /// <remarks/>
        WORD,

        /// <remarks/>
        WSTRING,

        /// <remarks/>
        dataTypeIDRef,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class @enum
    {

        private object[] itemsField;

        private enumValue[] enumValueField;

        private object bOOLField;

        private object bITSTRINGField;

        private object bYTEField;

        private object cHARField;

        private object wORDField;

        private object dWORDField;

        private object lWORDField;

        private object sINTField;

        private object iNTField;

        private object dINTField;

        private object lINTField;

        private object uSINTField;

        private object uINTField;

        private object uDINTField;

        private object uLINTField;

        private object rEALField;

        private object lREALField;

        private object sTRINGField;

        private object wSTRINGField;

        private string nameField;

        private string uniqueIDField;

        private string sizeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("enumValue")]
        public enumValue[] enumValue
        {
            get
            {
                return this.enumValueField;
            }
            set
            {
                this.enumValueField = value;
            }
        }

        /// <remarks/>
        public object BOOL
        {
            get
            {
                return this.bOOLField;
            }
            set
            {
                this.bOOLField = value;
            }
        }

        /// <remarks/>
        public object BITSTRING
        {
            get
            {
                return this.bITSTRINGField;
            }
            set
            {
                this.bITSTRINGField = value;
            }
        }

        /// <remarks/>
        public object BYTE
        {
            get
            {
                return this.bYTEField;
            }
            set
            {
                this.bYTEField = value;
            }
        }

        /// <remarks/>
        public object CHAR
        {
            get
            {
                return this.cHARField;
            }
            set
            {
                this.cHARField = value;
            }
        }

        /// <remarks/>
        public object WORD
        {
            get
            {
                return this.wORDField;
            }
            set
            {
                this.wORDField = value;
            }
        }

        /// <remarks/>
        public object DWORD
        {
            get
            {
                return this.dWORDField;
            }
            set
            {
                this.dWORDField = value;
            }
        }

        /// <remarks/>
        public object LWORD
        {
            get
            {
                return this.lWORDField;
            }
            set
            {
                this.lWORDField = value;
            }
        }

        /// <remarks/>
        public object SINT
        {
            get
            {
                return this.sINTField;
            }
            set
            {
                this.sINTField = value;
            }
        }

        /// <remarks/>
        public object INT
        {
            get
            {
                return this.iNTField;
            }
            set
            {
                this.iNTField = value;
            }
        }

        /// <remarks/>
        public object DINT
        {
            get
            {
                return this.dINTField;
            }
            set
            {
                this.dINTField = value;
            }
        }

        /// <remarks/>
        public object LINT
        {
            get
            {
                return this.lINTField;
            }
            set
            {
                this.lINTField = value;
            }
        }

        /// <remarks/>
        public object USINT
        {
            get
            {
                return this.uSINTField;
            }
            set
            {
                this.uSINTField = value;
            }
        }

        /// <remarks/>
        public object UINT
        {
            get
            {
                return this.uINTField;
            }
            set
            {
                this.uINTField = value;
            }
        }

        /// <remarks/>
        public object UDINT
        {
            get
            {
                return this.uDINTField;
            }
            set
            {
                this.uDINTField = value;
            }
        }

        /// <remarks/>
        public object ULINT
        {
            get
            {
                return this.uLINTField;
            }
            set
            {
                this.uLINTField = value;
            }
        }

        /// <remarks/>
        public object REAL
        {
            get
            {
                return this.rEALField;
            }
            set
            {
                this.rEALField = value;
            }
        }

        /// <remarks/>
        public object LREAL
        {
            get
            {
                return this.lREALField;
            }
            set
            {
                this.lREALField = value;
            }
        }

        /// <remarks/>
        public object STRING
        {
            get
            {
                return this.sTRINGField;
            }
            set
            {
                this.sTRINGField = value;
            }
        }

        /// <remarks/>
        public object WSTRING
        {
            get
            {
                return this.wSTRINGField;
            }
            set
            {
                this.wSTRINGField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class enumValue
    {

        private object[] itemsField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class derived
    {

        private object[] itemsField;

        private count countField;

        private object itemField;

        private ItemChoiceType2 itemElementNameField;

        private string nameField;

        private string uniqueIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        public count count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BITSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BOOL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BYTE", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("CHAR", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("INT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LREAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("REAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("SINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("STRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UDINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("ULINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("USINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("dataTypeIDRef", typeof(dataTypeIDRef))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType2 ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class count
    {

        private object[] itemsField;

        private defaultValue defaultValueField;

        private allowedValues allowedValuesField;

        private string uniqueIDField;

        private countAccess accessField;

        public count()
        {
            this.accessField = countAccess.read;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        public defaultValue defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        public allowedValues allowedValues
        {
            get
            {
                return this.allowedValuesField;
            }
            set
            {
                this.allowedValuesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(countAccess.read)]
        public countAccess access
        {
            get
            {
                return this.accessField;
            }
            set
            {
                this.accessField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class defaultValue
    {

        private object[] itemsField;

        private string valueField;

        private string offsetField;

        private string multiplierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class allowedValues
    {

        private value[] valueField;

        private range[] rangeField;

        private string templateIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("value")]
        public value[] value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("range")]
        public range[] range
        {
            get
            {
                return this.rangeField;
            }
            set
            {
                this.rangeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string templateIDRef
        {
            get
            {
                return this.templateIDRefField;
            }
            set
            {
                this.templateIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class value
    {

        private object[] itemsField;

        private string value1Field;

        private string offsetField;

        private string multiplierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("value")]
        public string value1
        {
            get
            {
                return this.value1Field;
            }
            set
            {
                this.value1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class range
    {

        private rangeMinValue minValueField;

        private rangeMaxValue maxValueField;

        private rangeStep stepField;

        /// <remarks/>
        public rangeMinValue minValue
        {
            get
            {
                return this.minValueField;
            }
            set
            {
                this.minValueField = value;
            }
        }

        /// <remarks/>
        public rangeMaxValue maxValue
        {
            get
            {
                return this.maxValueField;
            }
            set
            {
                this.maxValueField = value;
            }
        }

        /// <remarks/>
        public rangeStep step
        {
            get
            {
                return this.stepField;
            }
            set
            {
                this.stepField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class rangeMinValue
    {

        private object[] itemsField;

        private string valueField;

        private string offsetField;

        private string multiplierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class rangeMaxValue
    {

        private object[] itemsField;

        private string valueField;

        private string offsetField;

        private string multiplierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public partial class rangeStep
    {

        private object[] itemsField;

        private string valueField;

        private string offsetField;

        private string multiplierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum countAccess
    {

        /// <remarks/>
        @const,

        /// <remarks/>
        read,

        /// <remarks/>
        write,

        /// <remarks/>
        readWrite,

        /// <remarks/>
        noAccess,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0", IncludeInSchema = false)]
    public enum ItemChoiceType2
    {

        /// <remarks/>
        BITSTRING,

        /// <remarks/>
        BOOL,

        /// <remarks/>
        BYTE,

        /// <remarks/>
        CHAR,

        /// <remarks/>
        DINT,

        /// <remarks/>
        DWORD,

        /// <remarks/>
        INT,

        /// <remarks/>
        LINT,

        /// <remarks/>
        LREAL,

        /// <remarks/>
        LWORD,

        /// <remarks/>
        REAL,

        /// <remarks/>
        SINT,

        /// <remarks/>
        STRING,

        /// <remarks/>
        UDINT,

        /// <remarks/>
        UINT,

        /// <remarks/>
        ULINT,

        /// <remarks/>
        USINT,

        /// <remarks/>
        WORD,

        /// <remarks/>
        WSTRING,

        /// <remarks/>
        dataTypeIDRef,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class functionType
    {

        private object[] itemsField;

        private versionInfo[] versionInfoField;

        private interfaceList interfaceListField;

        private functionInstanceList functionInstanceListField;

        private string nameField;

        private string uniqueIDField;

        private string packageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("versionInfo")]
        public versionInfo[] versionInfo
        {
            get
            {
                return this.versionInfoField;
            }
            set
            {
                this.versionInfoField = value;
            }
        }

        /// <remarks/>
        public interfaceList interfaceList
        {
            get
            {
                return this.interfaceListField;
            }
            set
            {
                this.interfaceListField = value;
            }
        }

        /// <remarks/>
        public functionInstanceList functionInstanceList
        {
            get
            {
                return this.functionInstanceListField;
            }
            set
            {
                this.functionInstanceListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string package
        {
            get
            {
                return this.packageField;
            }
            set
            {
                this.packageField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class versionInfo
    {

        private object[] itemsField;

        private string organizationField;

        private string versionField;

        private string authorField;

        private System.DateTime dateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string organization
        {
            get
            {
                return this.organizationField;
            }
            set
            {
                this.organizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string author
        {
            get
            {
                return this.authorField;
            }
            set
            {
                this.authorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class interfaceList
    {

        private varDeclaration[] inputVarsField;

        private varDeclaration[] outputVarsField;

        private varDeclaration[] configVarsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("varDeclaration", IsNullable = false)]
        public varDeclaration[] inputVars
        {
            get
            {
                return this.inputVarsField;
            }
            set
            {
                this.inputVarsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("varDeclaration", IsNullable = false)]
        public varDeclaration[] outputVars
        {
            get
            {
                return this.outputVarsField;
            }
            set
            {
                this.outputVarsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("varDeclaration", IsNullable = false)]
        public varDeclaration[] configVars
        {
            get
            {
                return this.configVarsField;
            }
            set
            {
                this.configVarsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class functionInstanceList
    {

        private functionInstance[] functionInstanceField;

        private connection[] connectionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("functionInstance")]
        public functionInstance[] functionInstance
        {
            get
            {
                return this.functionInstanceField;
            }
            set
            {
                this.functionInstanceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("connection")]
        public connection[] connection
        {
            get
            {
                return this.connectionField;
            }
            set
            {
                this.connectionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class functionInstance
    {

        private object[] itemsField;

        private string nameField;

        private string uniqueIDField;

        private string typeIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string typeIDRef
        {
            get
            {
                return this.typeIDRefField;
            }
            set
            {
                this.typeIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class connection
    {

        private string sourceField;

        private string destinationField;

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string destination
        {
            get
            {
                return this.destinationField;
            }
            set
            {
                this.destinationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class templateList
    {

        private parameterTemplate[] parameterTemplateField;

        private allowedValuesTemplate[] allowedValuesTemplateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parameterTemplate")]
        public parameterTemplate[] parameterTemplate
        {
            get
            {
                return this.parameterTemplateField;
            }
            set
            {
                this.parameterTemplateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("allowedValuesTemplate")]
        public allowedValuesTemplate[] allowedValuesTemplate
        {
            get
            {
                return this.allowedValuesTemplateField;
            }
            set
            {
                this.allowedValuesTemplateField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class parameterTemplate
    {

        private object itemField;

        private ItemChoiceType3 itemElementNameField;

        private conditionalSupport[] conditionalSupportField;

        private actualValue actualValueField;

        private defaultValue defaultValueField;

        private substituteValue substituteValueField;

        private allowedValues allowedValuesField;

        private unit unitField;

        private property[] propertyField;

        private string uniqueIDField;

        private parameterTemplateAccess accessField;

        private string accessListField;

        private parameterTemplateSupport supportField;

        private bool supportFieldSpecified;

        private bool persistentField;

        private string offsetField;

        private string multiplierField;

        private string templateIDRefField;

        public parameterTemplate()
        {
            this.accessField = parameterTemplateAccess.read;
            this.persistentField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BITSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BOOL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BYTE", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("CHAR", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("INT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LREAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("REAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("SINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("STRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UDINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("ULINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("USINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("dataTypeIDRef", typeof(dataTypeIDRef))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType3 ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("conditionalSupport")]
        public conditionalSupport[] conditionalSupport
        {
            get
            {
                return this.conditionalSupportField;
            }
            set
            {
                this.conditionalSupportField = value;
            }
        }

        /// <remarks/>
        public actualValue actualValue
        {
            get
            {
                return this.actualValueField;
            }
            set
            {
                this.actualValueField = value;
            }
        }

        /// <remarks/>
        public defaultValue defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        public substituteValue substituteValue
        {
            get
            {
                return this.substituteValueField;
            }
            set
            {
                this.substituteValueField = value;
            }
        }

        /// <remarks/>
        public allowedValues allowedValues
        {
            get
            {
                return this.allowedValuesField;
            }
            set
            {
                this.allowedValuesField = value;
            }
        }

        /// <remarks/>
        public unit unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("property")]
        public property[] property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(parameterTemplateAccess.read)]
        public parameterTemplateAccess access
        {
            get
            {
                return this.accessField;
            }
            set
            {
                this.accessField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NMTOKENS")]
        public string accessList
        {
            get
            {
                return this.accessListField;
            }
            set
            {
                this.accessListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public parameterTemplateSupport support
        {
            get
            {
                return this.supportField;
            }
            set
            {
                this.supportField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool supportSpecified
        {
            get
            {
                return this.supportFieldSpecified;
            }
            set
            {
                this.supportFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool persistent
        {
            get
            {
                return this.persistentField;
            }
            set
            {
                this.persistentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string templateIDRef
        {
            get
            {
                return this.templateIDRefField;
            }
            set
            {
                this.templateIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0", IncludeInSchema = false)]
    public enum ItemChoiceType3
    {

        /// <remarks/>
        BITSTRING,

        /// <remarks/>
        BOOL,

        /// <remarks/>
        BYTE,

        /// <remarks/>
        CHAR,

        /// <remarks/>
        DINT,

        /// <remarks/>
        DWORD,

        /// <remarks/>
        INT,

        /// <remarks/>
        LINT,

        /// <remarks/>
        LREAL,

        /// <remarks/>
        LWORD,

        /// <remarks/>
        REAL,

        /// <remarks/>
        SINT,

        /// <remarks/>
        STRING,

        /// <remarks/>
        UDINT,

        /// <remarks/>
        UINT,

        /// <remarks/>
        ULINT,

        /// <remarks/>
        USINT,

        /// <remarks/>
        WORD,

        /// <remarks/>
        WSTRING,

        /// <remarks/>
        dataTypeIDRef,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class conditionalSupport
    {

        private string paramIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string paramIDRef
        {
            get
            {
                return this.paramIDRefField;
            }
            set
            {
                this.paramIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class actualValue
    {

        private object[] itemsField;

        private string valueField;

        private string offsetField;

        private string multiplierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class substituteValue
    {

        private object[] itemsField;

        private string valueField;

        private string offsetField;

        private string multiplierField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class unit
    {

        private object[] itemsField;

        private string multiplierField;

        private string unitURIField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string unitURI
        {
            get
            {
                return this.unitURIField;
            }
            set
            {
                this.unitURIField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class property
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum parameterTemplateAccess
    {

        /// <remarks/>
        @const,

        /// <remarks/>
        read,

        /// <remarks/>
        write,

        /// <remarks/>
        readWrite,

        /// <remarks/>
        readWriteInput,

        /// <remarks/>
        readWriteOutput,

        /// <remarks/>
        noAccess,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum parameterTemplateSupport
    {

        /// <remarks/>
        mandatory,

        /// <remarks/>
        optional,

        /// <remarks/>
        conditional,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class allowedValuesTemplate
    {

        private value[] valueField;

        private range[] rangeField;

        private string uniqueIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("value")]
        public value[] value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("range")]
        public range[] range
        {
            get
            {
                return this.rangeField;
            }
            set
            {
                this.rangeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class parameter
    {

        private object[] itemsField;

        private object[] items1Field;

        private Items1ChoiceType[] items1ElementNameField;

        private conditionalSupport[] conditionalSupportField;

        private denotation denotationField;

        private actualValue actualValueField;

        private defaultValue defaultValueField;

        private substituteValue substituteValueField;

        private allowedValues allowedValuesField;

        private unit unitField;

        private property[] propertyField;

        private string uniqueIDField;

        private parameterTemplateAccess accessField;

        private string accessListField;

        private parameterTemplateSupport supportField;

        private bool supportFieldSpecified;

        private bool persistentField;

        private string offsetField;

        private string multiplierField;

        private string templateIDRefField;

        public parameter()
        {
            this.accessField = parameterTemplateAccess.read;
            this.persistentField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BITSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BOOL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("BYTE", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("CHAR", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("DWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("INT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LREAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("LWORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("REAL", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("SINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("STRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UDINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("UINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("ULINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("USINT", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WORD", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("WSTRING", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("dataTypeIDRef", typeof(dataTypeIDRef))]
        [System.Xml.Serialization.XmlElementAttribute("variableRef", typeof(variableRef))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("Items1ElementName")]
        public object[] Items1
        {
            get
            {
                return this.items1Field;
            }
            set
            {
                this.items1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Items1ElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Items1ChoiceType[] Items1ElementName
        {
            get
            {
                return this.items1ElementNameField;
            }
            set
            {
                this.items1ElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("conditionalSupport")]
        public conditionalSupport[] conditionalSupport
        {
            get
            {
                return this.conditionalSupportField;
            }
            set
            {
                this.conditionalSupportField = value;
            }
        }

        /// <remarks/>
        public denotation denotation
        {
            get
            {
                return this.denotationField;
            }
            set
            {
                this.denotationField = value;
            }
        }

        /// <remarks/>
        public actualValue actualValue
        {
            get
            {
                return this.actualValueField;
            }
            set
            {
                this.actualValueField = value;
            }
        }

        /// <remarks/>
        public defaultValue defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        public substituteValue substituteValue
        {
            get
            {
                return this.substituteValueField;
            }
            set
            {
                this.substituteValueField = value;
            }
        }

        /// <remarks/>
        public allowedValues allowedValues
        {
            get
            {
                return this.allowedValuesField;
            }
            set
            {
                this.allowedValuesField = value;
            }
        }

        /// <remarks/>
        public unit unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("property")]
        public property[] property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        //[System.ComponentModel.DefaultValueAttribute(parameterTemplateAccess.read)]
        public parameterTemplateAccess access
        {
            get
            {
                return this.accessField;
            }
            set
            {
                this.accessField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NMTOKENS")]
        public string accessList
        {
            get
            {
                return this.accessListField;
            }
            set
            {
                this.accessListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public parameterTemplateSupport support
        {
            get
            {
                return this.supportField;
            }
            set
            {
                this.supportField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool supportSpecified
        {
            get
            {
                return this.supportFieldSpecified;
            }
            set
            {
                this.supportFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool persistent
        {
            get
            {
                return this.persistentField;
            }
            set
            {
                this.persistentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string multiplier
        {
            get
            {
                return this.multiplierField;
            }
            set
            {
                this.multiplierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string templateIDRef
        {
            get
            {
                return this.templateIDRefField;
            }
            set
            {
                this.templateIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class variableRef
    {

        private instanceIDRef[] instanceIDRefField;

        private variableIDRef variableIDRefField;

        private memberRef[] memberRefField;

        private byte positionField;

        public variableRef()
        {
            this.positionField = ((byte)(1));
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("instanceIDRef")]
        public instanceIDRef[] instanceIDRef
        {
            get
            {
                return this.instanceIDRefField;
            }
            set
            {
                this.instanceIDRefField = value;
            }
        }

        /// <remarks/>
        public variableIDRef variableIDRef
        {
            get
            {
                return this.variableIDRefField;
            }
            set
            {
                this.variableIDRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("memberRef")]
        public memberRef[] memberRef
        {
            get
            {
                return this.memberRefField;
            }
            set
            {
                this.memberRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(byte), "1")]
        public byte position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class instanceIDRef
    {

        private string uniqueIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string uniqueIDRef
        {
            get
            {
                return this.uniqueIDRefField;
            }
            set
            {
                this.uniqueIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class variableIDRef
    {

        private string uniqueIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string uniqueIDRef
        {
            get
            {
                return this.uniqueIDRefField;
            }
            set
            {
                this.uniqueIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class memberRef
    {

        private string uniqueIDRefField;

        private long indexField;

        private bool indexFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string uniqueIDRef
        {
            get
            {
                return this.uniqueIDRefField;
            }
            set
            {
                this.uniqueIDRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool indexSpecified
        {
            get
            {
                return this.indexFieldSpecified;
            }
            set
            {
                this.indexFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.canopen.org/xml/1.0", IncludeInSchema = false)]
    public enum Items1ChoiceType
    {

        /// <remarks/>
        BITSTRING,

        /// <remarks/>
        BOOL,

        /// <remarks/>
        BYTE,

        /// <remarks/>
        CHAR,

        /// <remarks/>
        DINT,

        /// <remarks/>
        DWORD,

        /// <remarks/>
        INT,

        /// <remarks/>
        LINT,

        /// <remarks/>
        LREAL,

        /// <remarks/>
        LWORD,

        /// <remarks/>
        REAL,

        /// <remarks/>
        SINT,

        /// <remarks/>
        STRING,

        /// <remarks/>
        UDINT,

        /// <remarks/>
        UINT,

        /// <remarks/>
        ULINT,

        /// <remarks/>
        USINT,

        /// <remarks/>
        WORD,

        /// <remarks/>
        WSTRING,

        /// <remarks/>
        dataTypeIDRef,

        /// <remarks/>
        variableRef,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class denotation
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class parameterGroup
    {

        private object[] itemsField;

        private parameterGroup[] parameterGroup1Field;

        private parameterRef[] parameterRefField;

        private string uniqueIDField;

        private string kindOfAccessField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(vendorTextDescription))]
        [System.Xml.Serialization.XmlElementAttribute("descriptionRef", typeof(vendorTextDescriptionRef))]
        [System.Xml.Serialization.XmlElementAttribute("label", typeof(vendorTextLabel))]
        [System.Xml.Serialization.XmlElementAttribute("labelRef", typeof(vendorTextLabelRef))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parameterGroup")]
        public parameterGroup[] parameterGroup1
        {
            get
            {
                return this.parameterGroup1Field;
            }
            set
            {
                this.parameterGroup1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parameterRef")]
        public parameterRef[] parameterRef
        {
            get
            {
                return this.parameterRefField;
            }
            set
            {
                this.parameterRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string uniqueID
        {
            get
            {
                return this.uniqueIDField;
            }
            set
            {
                this.uniqueIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string kindOfAccess
        {
            get
            {
                return this.kindOfAccessField;
            }
            set
            {
                this.kindOfAccessField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class parameterRef
    {

        private string uniqueIDRefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string uniqueIDRef
        {
            get
            {
                return this.uniqueIDRefField;
            }
            set
            {
                this.uniqueIDRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    public enum ProfileBody_Device_CANopenDeviceClass
    {

        /// <remarks/>
        compact,

        /// <remarks/>
        modular,

        /// <remarks/>
        configurable,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class standardComplianceList
    {

        private compliantWith[] compliantWithField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("compliantWith")]
        public compliantWith[] compliantWith
        {
            get
            {
                return this.compliantWithField;
            }
            set
            {
                this.compliantWithField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class picturesList
    {

        private picture[] pictureField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("picture")]
        public picture[] picture
        {
            get
            {
                return this.pictureField;
            }
            set
            {
                this.pictureField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class dictionaryList
    {

        private dictionary[] dictionaryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("dictionary")]
        public dictionary[] dictionary
        {
            get
            {
                return this.dictionaryField;
            }
            set
            {
                this.dictionaryField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class functionTypeList
    {

        private functionType[] functionTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("functionType")]
        public functionType[] functionType
        {
            get
            {
                return this.functionTypeField;
            }
            set
            {
                this.functionTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class inputVars
    {

        private varDeclaration[] varDeclarationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("varDeclaration")]
        public varDeclaration[] varDeclaration
        {
            get
            {
                return this.varDeclarationField;
            }
            set
            {
                this.varDeclarationField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class outputVars
    {

        private varDeclaration[] varDeclarationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("varDeclaration")]
        public varDeclaration[] varDeclaration
        {
            get
            {
                return this.varDeclarationField;
            }
            set
            {
                this.varDeclarationField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class configVars
    {

        private varDeclaration[] varDeclarationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("varDeclaration")]
        public varDeclaration[] varDeclaration
        {
            get
            {
                return this.varDeclarationField;
            }
            set
            {
                this.varDeclarationField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class parameterList
    {

        private parameter[] parameterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parameter")]
        public parameter[] parameter
        {
            get
            {
                return this.parameterField;
            }
            set
            {
                this.parameterField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.canopen.org/xml/1.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.canopen.org/xml/1.0", IsNullable = false)]
    public partial class parameterGroupList
    {

        private parameterGroup[] parameterGroupField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parameterGroup")]
        public parameterGroup[] parameterGroup
        {
            get
            {
                return this.parameterGroupField;
            }
            set
            {
                this.parameterGroupField = value;
            }
        }
    }
}

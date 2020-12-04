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

    Copyright(c) 2016 - 2020 Robin Cornelius <robin.cornelius@gmail.com>
    Copyright(c) 2020 Janez Paternoster
*/


using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using CanOpenXSD_1_1;

namespace libEDSsharp
{
    public class CanOpenXDD_1_1
    {
        /// <summary>
        /// Read XDD file into EDSsharp object
        /// </summary>
        /// <param name="file">Name of the xdd file</param>
        /// <returns>EDSsharp object</returns>
        public EDSsharp ReadXML(string file)
        {
            ISO15745ProfileContainer dev;

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

            return Convert(dev);
        }

        /// <summary>
        /// Read custom multi xdd file (multiple standard xdd files inside one xml container)
        /// </summary>
        /// <param name="file">Name of the multi xdd file</param>
        /// <returns>List of EDSsharp objects</returns>
        public List<EDSsharp> ReadMultiXML(string file )
        {
            List<EDSsharp> edss = new List<EDSsharp>();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CanOpenProject_1_1));
                StreamReader reader = new StreamReader(file);
                CanOpenProject_1_1 oep = (CanOpenProject_1_1)serializer.Deserialize(reader);

                foreach(ISO15745ProfileContainer cont in oep.ISO15745ProfileContainer)
                {
                    edss.Add(Convert(cont));
                }

                reader.Close();

                return edss;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Write custom multi xdd file (multiple standard xdd files inside one xml container)
        /// </summary>
        /// <param name="file">Name of the multi xdd file</param>
        /// <param name="edss">List of EDSsharp objects</param>
        /// <param name="gitVersion">Git version string for documentation field</param>
        /// <param name="deviceCommissioning">If true, device commisioning, denotations and actual values will be included</param>
        public void WriteMultiXML(string file, List<EDSsharp> edss, string gitVersion, bool deviceCommissioning)
        {
            List<ISO15745ProfileContainer> devs = new List<ISO15745ProfileContainer>();

            foreach (EDSsharp eds in edss)
            {
                ISO15745ProfileContainer dev = Convert(eds, Path.GetFileName(file), deviceCommissioning, false);
                devs.Add(dev);
            }

            CanOpenProject_1_1 oep = new CanOpenProject_1_1
            {
                Version = "1.1",
                ISO15745ProfileContainer = devs
            };

            XmlSerializer serializer = new XmlSerializer(typeof(CanOpenProject_1_1));

            StreamWriter stream = new StreamWriter(file);
            XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true });
            writer.WriteStartDocument();
            writer.WriteComment(string.Format("CANopen Project file in custom format. It contains multiple standard CANopen device description files.", gitVersion));
            writer.WriteComment(string.Format("File is generated with libedssharp Object Dictionary Editor v{0}, URL: https://github.com/robincornelius/libedssharp", gitVersion));
            writer.WriteComment("File includes additional custom properties for CANopenNode, CANopen protocol stack, URL: https://github.com/CANopenNode/CANopenNode");
            serializer.Serialize(writer, oep);
            writer.WriteEndDocument();
            writer.Close();
            stream.Close();
        }

        /// <summary>
        /// Write XDD file from EDSsharp object
        /// </summary>
        /// <param name="file">Name of the xdd file</param>
        /// <param name="eds">EDSsharp object</param>
        /// <param name="gitVersion">Git version string for documentation field</param>
        /// <param name="deviceCommissioning">If true, device commisioning, denotations and actual values will be included</param>
        /// <param name="stripped">If true, then all CANopenNode specific parameters and all disabled objects will be stripped</param>
        public void WriteXML(string file, EDSsharp eds, string gitVersion, bool deviceCommissioning, bool stripped)
        {
            ISO15745ProfileContainer dev = Convert(eds, Path.GetFileName(file), deviceCommissioning, stripped);
            XmlSerializer serializer = new XmlSerializer(typeof(ISO15745ProfileContainer));

            StreamWriter stream = new StreamWriter(file);
            XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true });
            writer.WriteStartDocument();
            writer.WriteComment(string.Format("File is generated with libedssharp Object Dictionary Editor v{0}, URL: https://github.com/robincornelius/libedssharp", gitVersion));
            if (!stripped)
                writer.WriteComment("File includes additional custom properties for CANopenNode, CANopen protocol stack, URL: https://github.com/CANopenNode/CANopenNode");
            serializer.Serialize(writer, dev);
            writer.WriteEndDocument();
            writer.Close();
            stream.Close();
        }

        private parameterTemplateAccess ConvertAccessType(EDSsharp.AccessType edsAccessType)
        {
            switch (edsAccessType)
            {
                case EDSsharp.AccessType.@const: return parameterTemplateAccess.@const;
                case EDSsharp.AccessType.ro:     return parameterTemplateAccess.read;
                case EDSsharp.AccessType.rw:     return parameterTemplateAccess.readWrite;
                case EDSsharp.AccessType.rwr:    return parameterTemplateAccess.readWriteInput;
                case EDSsharp.AccessType.rww:    return parameterTemplateAccess.readWriteOutput;
                case EDSsharp.AccessType.wo:     return parameterTemplateAccess.write;
                default:                         return parameterTemplateAccess.noAccess;
            }
        }

        private EDSsharp.AccessType ConvertAccessType(parameterTemplateAccess xddAccessType)
        {
            switch (xddAccessType)
            {
                case parameterTemplateAccess.@const:          return EDSsharp.AccessType.@const;
                case parameterTemplateAccess.read:            return EDSsharp.AccessType.ro;
                case parameterTemplateAccess.readWrite:       return EDSsharp.AccessType.rw;
                case parameterTemplateAccess.readWriteInput:  return EDSsharp.AccessType.rwr;
                case parameterTemplateAccess.readWriteOutput: return EDSsharp.AccessType.rww;
                case parameterTemplateAccess.write:           return EDSsharp.AccessType.wo;
                default:                                      return EDSsharp.AccessType.UNKNOWN;
            }
        }

        private Items1ChoiceType ConvertDataType (ODentry od)
        {
            UInt32 byteLength;
            bool signed = false;
            var dt = od.datatype;
            if (dt == DataType.UNKNOWN && od.parent != null)
                dt = od.parent.datatype;

            switch (dt)
            {
                case DataType.BOOLEAN:        return Items1ChoiceType.BOOL;
                case DataType.INTEGER8:       return Items1ChoiceType.SINT;
                case DataType.INTEGER16:      return Items1ChoiceType.INT;
                case DataType.INTEGER32:      return Items1ChoiceType.DINT;
                case DataType.INTEGER64:      return Items1ChoiceType.LINT;
                case DataType.UNSIGNED8:      return Items1ChoiceType.USINT;
                case DataType.UNSIGNED16:     return Items1ChoiceType.UINT;
                case DataType.UNSIGNED32:     return Items1ChoiceType.UDINT;
                case DataType.UNSIGNED64:     return Items1ChoiceType.ULINT;
                case DataType.REAL32:         return Items1ChoiceType.REAL;
                case DataType.REAL64:         return Items1ChoiceType.LREAL;
                case DataType.VISIBLE_STRING: return Items1ChoiceType.STRING;
                case DataType.OCTET_STRING:   return Items1ChoiceType.BITSTRING;
                case DataType.UNICODE_STRING: return Items1ChoiceType.WSTRING;

                case DataType.DOMAIN:
                    od.defaultvalue = "";
                    return Items1ChoiceType.BITSTRING;

                default:
                    od.datatype = DataType.INTEGER32;
                    return Items1ChoiceType.DINT;

                // transform other non standard values to OCTET_STRING
                case DataType.INTEGER24:       byteLength = 3; signed = true; break;
                case DataType.INTEGER40:       byteLength = 5; signed = true; break;
                case DataType.INTEGER48:       byteLength = 6; signed = true; break;
                case DataType.INTEGER56:       byteLength = 7; signed = true; break;
                case DataType.UNSIGNED24:      byteLength = 3; break;
                case DataType.UNSIGNED40:      byteLength = 5; break;
                case DataType.UNSIGNED48:
                case DataType.TIME_OF_DAY:
                case DataType.TIME_DIFFERENCE: byteLength = 6; break;
                case DataType.UNSIGNED56:      byteLength = 7; break;
            }

            // set datatype OCTET_STRING and write default value as a sequence of bytes, little endian, like "56 34 12"
            UInt64 value;
            try
            {
                value = signed ? (UInt64)((Int64)new System.ComponentModel.Int64Converter().ConvertFromString(od.defaultvalue))
                               : (UInt64) new System.ComponentModel.UInt64Converter().ConvertFromString(od.defaultvalue);
            }
            catch (Exception)
            {
                value = 0;
            }

            List<string> bytes = new List<string>();
            for (UInt32 i = 0; i < byteLength; i++)
            {
                bytes.Add(String.Format("{0:X2}", value & 0xFF));
                value >>= 8;
            }
            od.datatype = DataType.OCTET_STRING;
            od.defaultvalue = string.Join(" ", bytes);

            return Items1ChoiceType.BITSTRING;
        }

        private DataType ConvertDataType(Items1ChoiceType choiceType, string defaultValue)
        {
            switch (choiceType)
            {
                case Items1ChoiceType.BOOL: return DataType.BOOLEAN;
                case Items1ChoiceType.CHAR:
                case Items1ChoiceType.SINT: return DataType.INTEGER8;
                case Items1ChoiceType.INT: return DataType.INTEGER16;
                case Items1ChoiceType.DINT: return DataType.INTEGER32;
                case Items1ChoiceType.LINT: return DataType.INTEGER64;
                case Items1ChoiceType.BYTE:
                case Items1ChoiceType.USINT: return DataType.UNSIGNED8;
                case Items1ChoiceType.WORD:
                case Items1ChoiceType.UINT: return DataType.UNSIGNED16;
                case Items1ChoiceType.DWORD:
                case Items1ChoiceType.UDINT: return DataType.UNSIGNED32;
                case Items1ChoiceType.LWORD:
                case Items1ChoiceType.ULINT: return DataType.UNSIGNED64;
                case Items1ChoiceType.REAL: return DataType.REAL32;
                case Items1ChoiceType.LREAL: return DataType.REAL64;
                case Items1ChoiceType.STRING: return DataType.VISIBLE_STRING;
                case Items1ChoiceType.WSTRING: return DataType.UNICODE_STRING;
                case Items1ChoiceType.BITSTRING:
                    return defaultValue == "" ? DataType.DOMAIN : DataType.OCTET_STRING;
                default:
                    return DataType.INTEGER32;
            }
        }

        private void WriteVar(parameter devPar, ODentry od)
        {
            if (od.accesstype == EDSsharp.AccessType.UNKNOWN && od.parent != null && od.parent.objecttype == ObjectType.ARRAY)
                od.accesstype = od.parent.accesstype;

            devPar.access = ConvertAccessType(od.accesstype);

            devPar.Items1 = new object[] { new object() };
            devPar.Items1ElementName = new Items1ChoiceType[] { ConvertDataType(od) };

            if (od.defaultvalue != null && od.defaultvalue != "")
                    devPar.defaultValue = new defaultValue { value = od.defaultvalue };

            if (od.LowLimit != null && od.LowLimit != "" && od.HighLimit != null && od.HighLimit != "")
            {
                devPar.allowedValues = new allowedValues
                {
                    range = new range[]
                    {
                        new range
                        {
                            minValue = new rangeMinValue { value = od.LowLimit },
                            maxValue = new rangeMaxValue { value = od.HighLimit }
                        }
                    }
                };
            }
        }

        private ISO15745ProfileContainer Convert(EDSsharp eds, string fileName, bool deviceCommissioning, bool stripped)
        {
            // Get xdd template from eds (if memorized on xdd file open)
            ISO15745ProfileContainer container = eds.xddTemplate;

            ProfileBody_Device_CANopen body_device = null;
            ProfileBody_CommunicationNetwork_CANopen body_network = null;

            List<string> mappingErrors = eds.VerifyPDOMapping();
            if (mappingErrors.Count > 0)
                Warnings.AddWarning($"Errors in PDO mappings:\r\n    " + string.Join("\r\n    ", mappingErrors), Warnings.warning_class.WARNING_BUILD);

            eds.fi.ModificationDateTime = DateTime.Now;

            #region base_elements
            // create required xml objects, where necessay
            if (container == null)
                container = new ISO15745ProfileContainer();
            if (container.ISO15745Profile == null
                || container.ISO15745Profile.Length < 2
                || container.ISO15745Profile[0] == null
                || container.ISO15745Profile[1] == null)
            {
                container.ISO15745Profile = new ISO15745Profile[]
                {
                    new ISO15745Profile(),
                    new ISO15745Profile(),
                };
            }

            // get headers and bodies
            if (container.ISO15745Profile[0].ProfileHeader != null
                && container.ISO15745Profile[0].ProfileBody != null
                && container.ISO15745Profile[1].ProfileHeader != null
                && container.ISO15745Profile[1].ProfileBody != null)
            {
                foreach (ISO15745Profile item in container.ISO15745Profile)
                {
                    if (item.ProfileBody.GetType() == typeof(ProfileBody_Device_CANopen))
                        body_device = (ProfileBody_Device_CANopen)item.ProfileBody;
                    else if (item.ProfileBody.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopen))
                        body_network = (ProfileBody_CommunicationNetwork_CANopen)item.ProfileBody;
                }
            }
            if (body_network == null || body_device == null)
            {
                container.ISO15745Profile[0].ProfileHeader = new ProfileHeader_DataType
                {
                    ProfileIdentification = "CANopen device profile",
                    ProfileRevision = "1.1",
                    ProfileName = "",
                    ProfileSource = "",
                    ProfileClassID = ProfileClassID_DataType.Device,
                    ISO15745Reference = new ISO15745Reference_DataType
                    {
                        ISO15745Part = "1",
                        ISO15745Edition = "1",
                        ProfileTechnology = "CANopen"
                    }
                };
                container.ISO15745Profile[0].ProfileBody = new ProfileBody_Device_CANopen();
                body_device = (ProfileBody_Device_CANopen)container.ISO15745Profile[0].ProfileBody;

                container.ISO15745Profile[1].ProfileHeader = new ProfileHeader_DataType
                {
                    ProfileIdentification = "CANopen communication network profile",
                    ProfileRevision = "1.1",
                    ProfileName = "",
                    ProfileSource = "",
                    ProfileClassID = ProfileClassID_DataType.CommunicationNetwork,
                    ISO15745Reference = new ISO15745Reference_DataType
                    {
                        ISO15745Part = "1",
                        ISO15745Edition = "1",
                        ProfileTechnology = "CANopen"
                    }
                };
                container.ISO15745Profile[1].ProfileBody = new ProfileBody_CommunicationNetwork_CANopen();
                body_network = (ProfileBody_CommunicationNetwork_CANopen)container.ISO15745Profile[1].ProfileBody;
            }
            #endregion

            #region ObjectDictionary
            var body_network_objectList = new List<CANopenObjectListCANopenObject>();
            var body_device_parameterList = new List<parameter>();
            var body_device_arrayList = new List<array>();
            var body_device_structList = new List<@struct>();

            foreach (ODentry od in eds.ods.Values)
            {
                if (stripped && od.prop.CO_disabled == true)
                    continue;

                string uid = string.Format("{0:X4}", od.Index);

                var netObj = new CANopenObjectListCANopenObject
                {
                    index = new byte[] { (byte)(od.Index >> 8), (byte)(od.Index & 0xFF) },
                    name = od.parameter_name,
                    objectType = (byte)od.objecttype,
                    uniqueIDRef = "UID_OBJ_" + uid
                };
                body_network_objectList.Add(netObj);

                var devPar = new parameter { uniqueID = "UID_OBJ_" + uid };
                if (od.Description != null && od.Description != "")
                {
                    devPar.Items = new object[] { new vendorTextDescription { lang = "en", Value = od.Description } };
                }
                else
                {
                    // Add at least label made from parameter name, because g_labels is required by schema
                    devPar.Items = new object[] { new vendorTextLabel { lang = "en", Value = od.parameter_name } };
                }
                if (!stripped)
                    devPar.property = od.prop.OdeXdd();
                if (deviceCommissioning && od.denotation != null && od.denotation != "")
                {
                    devPar.denotation = new denotation
                    {
                        Items = new object[] { new vendorTextLabel { lang = "en", Value = od.denotation } }
                    };
                }
                body_device_parameterList.Add(devPar);

                if (od.objecttype == ObjectType.VAR)
                {
                    try { netObj.PDOmapping = (CANopenObjectListCANopenObjectPDOmapping)Enum.Parse(typeof(CANopenObjectListCANopenObjectPDOmapping), od.PDOtype.ToString()); }
                    catch (Exception) { netObj.PDOmapping = CANopenObjectListCANopenObjectPDOmapping.no; }

                    netObj.PDOmappingSpecified = true;

                    WriteVar(devPar, od);
                    if (deviceCommissioning && od.actualvalue != null && od.actualvalue != "")
                        devPar.actualValue = new actualValue { value = od.actualvalue };
                }
                else if ((od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC) && od.subobjects != null && od.subobjects.Count > 0)
                {
                    netObj.subNumber = (byte)od.subobjects.Count;
                    netObj.subNumberSpecified = true;

                    var netSubObjList = new List<CANopenObjectListCANopenObjectCANopenSubObject>();
                    var devStructSubList = new List<varDeclaration>();

                    foreach (KeyValuePair<UInt16, ODentry> kvp in od.subobjects)
                    {
                        ODentry subod = kvp.Value;
                        UInt16 subindex = kvp.Key;
                        string subUid = string.Format("{0:X4}{1:X2}", od.Index, subindex);

                        var netSubObj = new CANopenObjectListCANopenObjectCANopenSubObject
                        {
                            subIndex = new byte[] { (byte)(subindex & 0xFF) },
                            name = subod.parameter_name,
                            objectType = (byte)ObjectType.VAR,
                            PDOmappingSpecified = true,
                            uniqueIDRef = "UID_SUB_" + subUid
                        };
                        try { netSubObj.PDOmapping = (CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping)Enum.Parse(typeof(CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping), subod.PDOtype.ToString()); }
                        catch (Exception) { netSubObj.PDOmapping = CANopenObjectListCANopenObjectCANopenSubObjectPDOmapping.no; }

                        var devSubPar = new parameter {
                            uniqueID = "UID_SUB_" + subUid
                        };
                        if (subod.Description != null && subod.Description != "")
                        {
                            devSubPar.Items = new object[] { new vendorTextDescription { lang = "en", Value = subod.Description } };
                        }
                        else
                        {
                            // Add at least label made from parameter name, because g_labels is required by schema
                            devSubPar.Items = new object[] { new vendorTextLabel { lang = "en", Value = subod.parameter_name } };
                        }
                        if (!stripped)
                            devSubPar.property = subod.prop.SubOdeXdd();
                        if (deviceCommissioning && subod.denotation != null && subod.denotation != "")
                        {
                            devPar.denotation = new denotation
                            {
                                Items = new object[] { new vendorTextLabel { lang = "en", Value = subod.denotation } }
                            };
                        }
                        WriteVar(devSubPar, subod);
                        if (deviceCommissioning && subod.actualvalue != null && subod.actualvalue != "")
                            devPar.actualValue = new actualValue { value = subod.actualvalue };

                        if (od.objecttype == ObjectType.REC)
                        {
                            devStructSubList.Add(new varDeclaration
                            {
                                name = subod.parameter_name,
                                uniqueID = "UID_RECSUB_" + subUid,
                                Item = new object(),
                                ItemElementName = (ItemChoiceType1)ConvertDataType(subod)
                            });
                        }

                        body_device_parameterList.Add(devSubPar);
                        netSubObjList.Add(netSubObj);
                    }

                    // add refference to data type definition and definition of array or struct data type (schema requirement)
                    if (od.objecttype == ObjectType.ARRAY)
                    {
                        devPar.Items1 = new object[] { new dataTypeIDRef { uniqueIDRef = "UID_ARR_" + uid } };
                        devPar.Items1ElementName = new Items1ChoiceType[] { Items1ChoiceType.dataTypeIDRef };
                        body_device_arrayList.Add(new array
                        {
                            uniqueID = "UID_ARR_" + uid,
                            name = od.parameter_name,
                            Item = new object(),
                            ItemElementName = (ItemChoiceType)ConvertDataType(od),
                            subrange = new subrange[] { new subrange { lowerLimit = 0, upperLimit = od.subobjects.Count - 1 } }
                        });
                    }
                    else
                    {
                        devPar.Items1 = new object[] { new dataTypeIDRef { uniqueIDRef = "UID_REC_" + uid } };
                        devPar.Items1ElementName = new Items1ChoiceType[] { Items1ChoiceType.dataTypeIDRef };
                        body_device_structList.Add(new @struct
                        {
                            uniqueID = "UID_REC_" + uid,
                            name = od.parameter_name,
                            varDeclaration = devStructSubList.ToArray()
                        });
                    }

                    netObj.CANopenSubObject = netSubObjList.ToArray();
                }
            }
            #endregion

            #region body_device
            body_device.fileName = fileName;
            body_device.fileCreator = eds.fi.CreatedBy;
            body_device.fileCreationDate = eds.fi.CreationDateTime;
            body_device.fileCreationTime = eds.fi.CreationDateTime;
            body_device.fileCreationTimeSpecified = true;
            body_device.fileVersion = eds.fi.FileVersion;
            body_device.fileModifiedBy = eds.fi.ModifiedBy;
            body_device.fileModificationDate = eds.fi.ModificationDateTime;
            body_device.fileModificationTime = eds.fi.ModificationDateTime;
            body_device.fileModificationDateSpecified = true;
            body_device.fileModificationTimeSpecified = true;
            body_device.supportedLanguages = "en";

            // Device identity
            if (body_device.DeviceIdentity == null)
                body_device.DeviceIdentity = new DeviceIdentity();
            body_device.DeviceIdentity.vendorName = new vendorName { Value = eds.di.VendorName };
            body_device.DeviceIdentity.vendorID = new vendorID { Value = eds.di.VendorNumber };
            body_device.DeviceIdentity.productName = new productName { Value = eds.di.ProductName };
            body_device.DeviceIdentity.productID = new productID { Value = eds.di.ProductNumber };
            if (eds.fi.Description != null && eds.fi.Description != "")
            {
                body_device.DeviceIdentity.productText = new productText
                {
                    Items = new object[]
                    {
                        new vendorTextDescription { lang = "en", Value = eds.fi.Description }
                    }
                };
            }

            // version is optional element, make a template if empty
            if (body_device.DeviceIdentity.version == null)
            {
                body_device.DeviceIdentity.version = new version[]
                {
                    new version { versionType = versionVersionType.SW, Value = "0" },
                    new version { versionType = versionVersionType.FW, Value = "0" },
                    new version { versionType = versionVersionType.HW, Value = "0" }
                };
            }

            // DeviceFunction is required by schema, make a template if empty.
            if (body_device.DeviceFunction == null)
            {
                // This is just a template for somehow complex xml structure. Users can edit the
                // xdd file directly to write characteristics of own devices or use generic xml
                // editing tool. External editing will be preserved anyway, if xdd file will be
                // later opened and saved back in EDSEditor.
                // EDSEditor curerently does not have interface for editing the characteristics.
                body_device.DeviceFunction = new DeviceFunction[]
                {
                    new DeviceFunction
                    {
                        capabilities = new capabilities
                        {
                            characteristicsList = new characteristicsList[]
                            {
                                new characteristicsList
                                {
                                    characteristic = new characteristic[]
                                    {
                                        new characteristic
                                        {
                                            characteristicName = new characteristicName
                                            {
                                                Items = new object[]
                                                {
                                                    new vendorTextLabel { lang = "en", Value = "SW library" }
                                                }
                                            },
                                            characteristicContent = new characteristicContent[]
                                            {
                                                new characteristicContent {
                                                    Items = new object[]
                                                    {
                                                        new vendorTextLabel { lang = "en", Value = "libedssharp" }
                                                    }
                                                },
                                                new characteristicContent {
                                                    Items = new object[]
                                                    {
                                                        new vendorTextLabel { lang = "en", Value = "CANopenNode" }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }

            // ApplicationProcess (insert object dictionary)
            if (body_device.ApplicationProcess == null)
                body_device.ApplicationProcess = new ApplicationProcess[1];
            if (body_device.ApplicationProcess[0] == null)
                body_device.ApplicationProcess[0] = new ApplicationProcess();
            body_device.ApplicationProcess[0].dataTypeList = new dataTypeList
            {
                array = body_device_arrayList.ToArray(),
                @struct = body_device_structList.ToArray()
            };
            body_device.ApplicationProcess[0].parameterList = body_device_parameterList.ToArray();
            #endregion

            #region body_network
            body_network.fileName = fileName;
            body_network.fileCreator = eds.fi.CreatedBy;
            body_network.fileCreationDate = eds.fi.CreationDateTime;
            body_network.fileCreationTime = eds.fi.CreationDateTime;
            body_network.fileCreationTimeSpecified = true;
            body_network.fileVersion = eds.fi.FileVersion;
            body_network.fileModificationDate = eds.fi.ModificationDateTime;
            body_network.fileModificationTime = eds.fi.ModificationDateTime;
            body_network.fileModificationDateSpecified = true;
            body_network.fileModificationTimeSpecified = true;
            body_network.supportedLanguages = "en";

            // base elements
            ProfileBody_CommunicationNetwork_CANopenApplicationLayers ApplicationLayers = null;
            ProfileBody_CommunicationNetwork_CANopenTransportLayers TransportLayers = null;
            ProfileBody_CommunicationNetwork_CANopenNetworkManagement NetworkManagement = null;
            if (body_network.Items != null && body_network.Items.Length >= 3)
            {
                foreach (object item in body_network.Items)
                {
                    if (item.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayers))
                        ApplicationLayers = (ProfileBody_CommunicationNetwork_CANopenApplicationLayers)item;
                    else if (item.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayers))
                        TransportLayers = (ProfileBody_CommunicationNetwork_CANopenTransportLayers)item;
                    else if (item.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayers))
                        NetworkManagement = (ProfileBody_CommunicationNetwork_CANopenNetworkManagement)item;
                }
            }
            if (ApplicationLayers == null || TransportLayers == null || NetworkManagement == null)
            {
                body_network.Items = new object[3];
                body_network.Items[0] = new ProfileBody_CommunicationNetwork_CANopenApplicationLayers();
                ApplicationLayers = (ProfileBody_CommunicationNetwork_CANopenApplicationLayers)body_network.Items[0];
                body_network.Items[1] = new ProfileBody_CommunicationNetwork_CANopenTransportLayers();
                TransportLayers = (ProfileBody_CommunicationNetwork_CANopenTransportLayers)body_network.Items[1];
                body_network.Items[2] = new ProfileBody_CommunicationNetwork_CANopenNetworkManagement();
                NetworkManagement = (ProfileBody_CommunicationNetwork_CANopenNetworkManagement)body_network.Items[2];
            }

            // ApplicationLayers -> dummyUsage
            ApplicationLayers.dummyUsage = new ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy[7];
            for (int x = 0; x < 7; x++)
                ApplicationLayers.dummyUsage[x] = new ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummy();

            ApplicationLayers.dummyUsage[0].entry = eds.du.Dummy0001
                ? ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00011
                : ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00010;
            ApplicationLayers.dummyUsage[1].entry = eds.du.Dummy0002
                ? ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00021
                : ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00020;
            ApplicationLayers.dummyUsage[2].entry = eds.du.Dummy0003
                ? ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00031
                : ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00030;
            ApplicationLayers.dummyUsage[3].entry = eds.du.Dummy0004
                ? ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00041
                : ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00040;
            ApplicationLayers.dummyUsage[4].entry = eds.du.Dummy0005
                ? ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00051
                : ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00050;
            ApplicationLayers.dummyUsage[5].entry = eds.du.Dummy0006
                ? ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00061
                : ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00060;
            ApplicationLayers.dummyUsage[6].entry = eds.du.Dummy0007
                ? ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00071
                : ProfileBody_CommunicationNetwork_CANopenApplicationLayersDummyEntry.Dummy00070;

            // ApplicationLayers -> CANopenObjectList (insert object dictionary)
            ApplicationLayers.CANopenObjectList = new CANopenObjectList
            {
                CANopenObject = body_network_objectList.ToArray()
            };

            // TransportLayers -> supportedBaudRate
            List<ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue> bauds;
            bauds = new List<ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue>();
            if (eds.di.BaudRate_10)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item10Kbps);
            if (eds.di.BaudRate_20)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item20Kbps);
            if (eds.di.BaudRate_50)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item50Kbps);
            if (eds.di.BaudRate_125)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item125Kbps);
            if (eds.di.BaudRate_250)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item250Kbps);
            if (eds.di.BaudRate_500)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item500Kbps);
            if (eds.di.BaudRate_800)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item800Kbps);
            if (eds.di.BaudRate_1000)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.Item1000Kbps);
            if (eds.di.BaudRate_auto)
                bauds.Add(ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRateValue.autobaudRate);
            TransportLayers.PhysicalLayer = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayer
            {
                baudRate = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRate
                {
                    supportedBaudRate = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate[bauds.Count]
                }
            };
            for (int i = 0; i < bauds.Count; i++)
            {
                TransportLayers.PhysicalLayer.baudRate.supportedBaudRate[i] = new ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate
                {
                    value = bauds[i]
                };
            }

            // NetworkManagement
            if (NetworkManagement.CANopenGeneralFeatures == null)
                NetworkManagement.CANopenGeneralFeatures = new ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenGeneralFeatures();
            NetworkManagement.CANopenGeneralFeatures.granularity = eds.di.Granularity; // required parameter
            NetworkManagement.CANopenGeneralFeatures.nrOfRxPDO = eds.di.NrOfRXPDO;
            NetworkManagement.CANopenGeneralFeatures.nrOfTxPDO = eds.di.NrOfTXPDO;
            NetworkManagement.CANopenGeneralFeatures.layerSettingServiceSlave = eds.di.LSS_Supported;
            // not handled by GUI
            NetworkManagement.CANopenGeneralFeatures.groupMessaging = eds.di.GroupMessaging;
            if (eds.di.DynamicChannelsSupported)
                NetworkManagement.CANopenGeneralFeatures.dynamicChannels = 1;
            NetworkManagement.CANopenGeneralFeatures.bootUpSlave = eds.di.SimpleBootUpSlave;

            if (NetworkManagement.CANopenMasterFeatures == null)
                NetworkManagement.CANopenMasterFeatures = new ProfileBody_CommunicationNetwork_CANopenNetworkManagementCANopenMasterFeatures();
            NetworkManagement.CANopenMasterFeatures.layerSettingServiceMaster = eds.di.LSS_Master;
            // not handled by GUI
            NetworkManagement.CANopenMasterFeatures.bootUpMaster = eds.di.SimpleBootUpMaster;

            if (deviceCommissioning)
            {
                NetworkManagement.deviceCommissioning = new ProfileBody_CommunicationNetwork_CANopenNetworkManagementDeviceCommissioning
                {
                    nodeID = eds.dc.NodeId,
                    nodeName = eds.dc.NodeName,
                    actualBaudRate = eds.dc.BaudRate.ToString(),
                    networkNumber = eds.dc.NetNumber,
                    networkName = eds.dc.NetworkName,
                    CANopenManager = eds.dc.CANopenManager
                };
            }
            else
            {
                NetworkManagement.deviceCommissioning = null;
            }
            #endregion

            return container;
        }

        private string G_label_getDescription(object[] items) {
            if (items != null)
            {
                foreach (object o in items)
                {
                    if (o.GetType() == typeof(vendorTextDescription))
                    {
                        return ((vendorTextDescription)o).Value ?? "";
                    }
                }
            }
            return "";
        }

        private EDSsharp Convert(ISO15745ProfileContainer container)
        {
            EDSsharp eds = new EDSsharp();

            ProfileBody_Device_CANopen body_device = null;
            ProfileBody_CommunicationNetwork_CANopen body_network = null;
            ProfileBody_CommunicationNetwork_CANopenApplicationLayers ApplicationLayers = null;
            var parameters = new Dictionary<string, parameter>();

            foreach (ISO15745Profile item in container.ISO15745Profile)
            {
                if (item.ProfileBody.GetType() == typeof(ProfileBody_Device_CANopen))
                    body_device = (ProfileBody_Device_CANopen)item.ProfileBody;
                else if (item.ProfileBody.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopen))
                    body_network = (ProfileBody_CommunicationNetwork_CANopen)item.ProfileBody;
            }

            if (body_device != null)
            {
                eds.fi.FileName = body_device.fileName ?? "";
                eds.fi.FileVersion = body_device.fileVersion ?? "";
                eds.fi.CreatedBy = body_device.fileCreator ?? "";
                eds.fi.ModifiedBy = body_device.fileModifiedBy ?? "";

                if (body_device.fileCreationTimeSpecified)
                {
                    eds.fi.CreationDateTime = body_device.fileCreationDate.Add(body_device.fileCreationTime.TimeOfDay);
                    eds.fi.CreationDate = eds.fi.CreationDateTime.ToString("MM-dd-yyyy");
                    eds.fi.CreationTime = eds.fi.CreationDateTime.ToString("h:mmtt");

                }
                if (body_device.fileModificationDateSpecified)
                {
                    eds.fi.ModificationDateTime = body_device.fileModificationDate.Add(body_device.fileModificationTime.TimeOfDay);
                    eds.fi.ModificationDate = eds.fi.ModificationDateTime.ToString("MM-dd-yyyy");
                    eds.fi.ModificationTime = eds.fi.ModificationDateTime.ToString("h:mmtt");
                }

                if (body_device.DeviceIdentity != null)
                {
                    if (body_device.DeviceIdentity.vendorName != null)
                        eds.di.VendorName = body_device.DeviceIdentity.vendorName.Value ?? "";
                    if (body_device.DeviceIdentity.vendorID != null)
                        eds.di.VendorNumber = body_device.DeviceIdentity.vendorID.Value ?? "";
                    if (body_device.DeviceIdentity.productName != null)
                        eds.di.ProductName = body_device.DeviceIdentity.productName.Value ?? "";
                    if (body_device.DeviceIdentity.productID != null)
                        eds.di.ProductNumber = body_device.DeviceIdentity.productID.Value ?? "";
                    if (body_device.DeviceIdentity.productText != null)
                        eds.fi.Description = G_label_getDescription(body_device.DeviceIdentity.productText.Items);
                }

                if (body_device.ApplicationProcess != null
                    && body_device.ApplicationProcess[0] != null
                    && body_device.ApplicationProcess[0].parameterList != null)
                {
                    foreach (parameter param in body_device.ApplicationProcess[0].parameterList)
                    {
                        if (!parameters.ContainsKey(param.uniqueID))
                            parameters.Add(param.uniqueID, param);
                    }
                }
            }

            if (body_network != null)
            {
                ProfileBody_CommunicationNetwork_CANopenTransportLayers TransportLayers = null;
                ProfileBody_CommunicationNetwork_CANopenNetworkManagement NetworkManagement = null;

                foreach (object item in body_network.Items)
                {
                    if (item.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenApplicationLayers))
                        ApplicationLayers = (ProfileBody_CommunicationNetwork_CANopenApplicationLayers)item;
                    else if (item.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenTransportLayers))
                        TransportLayers = (ProfileBody_CommunicationNetwork_CANopenTransportLayers)item;
                    else if (item.GetType() == typeof(ProfileBody_CommunicationNetwork_CANopenNetworkManagement))
                        NetworkManagement = (ProfileBody_CommunicationNetwork_CANopenNetworkManagement)item;
                }

                if (TransportLayers != null && TransportLayers.PhysicalLayer != null
                    && TransportLayers.PhysicalLayer.baudRate != null && TransportLayers.PhysicalLayer.baudRate.supportedBaudRate != null)
                {
                    foreach (ProfileBody_CommunicationNetwork_CANopenTransportLayersPhysicalLayerBaudRateSupportedBaudRate baud in TransportLayers.PhysicalLayer.baudRate.supportedBaudRate)
                    {
                        switch (baud.value.ToString())
                        {
                            case "Item10Kbps": eds.di.BaudRate_10 = true; break;
                            case "Item20Kbps": eds.di.BaudRate_20 = true; break;
                            case "Item50Kbps": eds.di.BaudRate_50 = true; break;
                            case "Item125Kbps": eds.di.BaudRate_125 = true; break;
                            case "Item250Kbps": eds.di.BaudRate_250 = true; break;
                            case "Item500Kbps": eds.di.BaudRate_500 = true; break;
                            case "Item800Kbps": eds.di.BaudRate_800 = true; break;
                            case "Item1000Kbps": eds.di.BaudRate_1000 = true; break;
                            case "autobaudRate": eds.di.BaudRate_auto = true; break;
                        }
                    }
                }

                if (NetworkManagement != null)
                {
                    if (NetworkManagement.CANopenGeneralFeatures != null)
                    {
                        eds.di.Granularity = NetworkManagement.CANopenGeneralFeatures.granularity;
                        eds.di.NrOfRXPDO = NetworkManagement.CANopenGeneralFeatures.nrOfRxPDO;
                        eds.di.NrOfTXPDO = NetworkManagement.CANopenGeneralFeatures.nrOfTxPDO;
                        eds.di.LSS_Supported = NetworkManagement.CANopenGeneralFeatures.layerSettingServiceSlave;
                        // not handled by GUI
                        eds.di.GroupMessaging = NetworkManagement.CANopenGeneralFeatures.groupMessaging;
                        eds.di.DynamicChannelsSupported = NetworkManagement.CANopenGeneralFeatures.dynamicChannels > 0;
                        eds.di.SimpleBootUpSlave = NetworkManagement.CANopenGeneralFeatures.bootUpSlave;

                    }

                    if (NetworkManagement.CANopenMasterFeatures != null)
                    {
                        eds.di.LSS_Master = NetworkManagement.CANopenMasterFeatures.layerSettingServiceMaster;
                        // not handled by GUI
                        eds.di.SimpleBootUpMaster = NetworkManagement.CANopenMasterFeatures.bootUpMaster;
                    }

                    if (NetworkManagement.deviceCommissioning != null)
                    {
                        eds.dc.NodeId = NetworkManagement.deviceCommissioning.nodeID;
                        try { eds.dc.BaudRate = System.Convert.ToUInt16(NetworkManagement.deviceCommissioning.actualBaudRate); }
                        catch (Exception) { eds.dc.BaudRate = 0; }
                        eds.dc.CANopenManager = NetworkManagement.deviceCommissioning.CANopenManager;
                        eds.dc.NetworkName = NetworkManagement.deviceCommissioning.networkName;
                        try { eds.dc.NetNumber = System.Convert.ToUInt16(NetworkManagement.deviceCommissioning.networkNumber); }
                        catch (Exception) { eds.dc.NetNumber = 0; }
                        eds.dc.NodeName = NetworkManagement.deviceCommissioning.nodeName;

                    }
                }

                if (ApplicationLayers != null)
                {
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
                                bool used = int.Parse(m.Groups[2].Value) == 1;

                                switch (index)
                                {
                                    case 1: eds.du.Dummy0001 = used; break;
                                    case 2: eds.du.Dummy0002 = used; break;
                                    case 3: eds.du.Dummy0003 = used; break;
                                    case 4: eds.du.Dummy0004 = used; break;
                                    case 5: eds.du.Dummy0005 = used; break;
                                    case 6: eds.du.Dummy0006 = used; break;
                                    case 7: eds.du.Dummy0007 = used; break;
                                }
                            }
                        }
                    }

                    if (ApplicationLayers.CANopenObjectList != null && ApplicationLayers.CANopenObjectList.CANopenObject != null)
                    {
                        foreach (CANopenObjectListCANopenObject netObj in ApplicationLayers.CANopenObjectList.CANopenObject)
                        {
                            if (netObj.index == null || netObj.index.Length != 2)
                                continue;

                            UInt16 index = (UInt16)((netObj.index[0] << 8) | netObj.index[1]);

                            EDSsharp.AccessType accessType;
                            if (netObj.accessTypeSpecified)
                            {
                                try { accessType = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), netObj.accessType.ToString()); }
                                catch (Exception) { accessType = EDSsharp.AccessType.ro; }
                            }
                            else {
                                accessType = EDSsharp.AccessType.ro;
                            }

                            PDOMappingType PDOtype;
                            if (netObj.PDOmappingSpecified)
                            {
                                try { PDOtype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), netObj.PDOmapping.ToString()); }
                                catch (Exception) { PDOtype = PDOMappingType.no; }
                            }
                            else
                            {
                                PDOtype = PDOMappingType.no;
                            }

                            if (accessType == EDSsharp.AccessType.rw)
                            {
                                if (PDOtype == PDOMappingType.RPDO)
                                    accessType = EDSsharp.AccessType.rww;
                                else if (PDOtype == PDOMappingType.TPDO)
                                    accessType = EDSsharp.AccessType.rwr;
                            }

                            ODentry od = new ODentry
                            {
                                Index = index,
                                parameter_name = netObj.name ?? "",
                                objecttype = (ObjectType)netObj.objectType,
                                PDOtype = PDOtype,
                                // following values are optional and may be overriden by parameters from body_device
                                accesstype = accessType,
                                datatype = netObj.dataType != null && netObj.dataType.Length == 1 ? (DataType)netObj.dataType[0] : DataType.UNKNOWN,
                                defaultvalue = netObj.defaultValue ?? "",
                                actualvalue = netObj.actualValue ?? "",
                                denotation = netObj.denotation ?? "",
                                LowLimit = netObj.lowLimit ?? "",
                                HighLimit = netObj.highLimit ?? "",
                                ObjFlags = netObj.objFlags != null && netObj.objFlags.Length == 2 ? netObj.objFlags[1] : (byte)0,
                                uniqueID = netObj.uniqueIDRef ?? ""
                            };

                            if (netObj.uniqueIDRef != null && netObj.uniqueIDRef != "" && parameters.ContainsKey(netObj.uniqueIDRef))
                            {
                                parameter devPar = parameters[netObj.uniqueIDRef];

                                od.Description = G_label_getDescription(devPar.Items);

                                od.accesstype = ConvertAccessType(devPar.access);

                                if (devPar.defaultValue != null && devPar.defaultValue.value != null)
                                    od.defaultvalue = devPar.defaultValue.value;

                                if (devPar.Items1 != null && devPar.Items1ElementName != null)
                                    od.datatype = ConvertDataType(devPar.Items1ElementName[0], od.defaultvalue);

                                if (devPar.actualValue != null && devPar.actualValue.value != null)
                                    od.actualvalue = devPar.actualValue.value;

                                if (devPar.denotation != null)
                                    od.denotation = G_label_getDescription(devPar.denotation.Items);

                                if (devPar.allowedValues != null && devPar.allowedValues.range != null && devPar.allowedValues.range[0] != null)
                                {
                                    range r = devPar.allowedValues.range[0];
                                    if (r.minValue != null) od.LowLimit = r.minValue.value ?? "";
                                    if (r.maxValue != null) od.HighLimit = r.maxValue.value ?? "";
                                }

                                od.prop.OdeXdd(devPar.property);
                            }

                            if (netObj.CANopenSubObject != null)
                            {
                                foreach (CANopenObjectListCANopenObjectCANopenSubObject netSubObj in netObj.CANopenSubObject)
                                {
                                    if (netSubObj.subIndex == null || netSubObj.subIndex.Length != 1)
                                        continue;

                                    UInt16 subIndex = (UInt16)netSubObj.subIndex[0];

                                    EDSsharp.AccessType subAccessType;
                                    if (netSubObj.accessTypeSpecified)
                                    {
                                        try { subAccessType = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), netSubObj.accessType.ToString()); }
                                        catch (Exception) { subAccessType = EDSsharp.AccessType.ro; }
                                    }
                                    else
                                    {
                                        subAccessType = EDSsharp.AccessType.ro;
                                    }

                                    PDOMappingType subPDOtype;
                                    if (netSubObj.PDOmappingSpecified)
                                    {
                                        try { subPDOtype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), netSubObj.PDOmapping.ToString()); }
                                        catch (Exception) { subPDOtype = PDOMappingType.no; }
                                    }
                                    else
                                    {
                                        subPDOtype = PDOMappingType.no;
                                    }

                                    if (subAccessType == EDSsharp.AccessType.rw)
                                    {
                                        if (subPDOtype == PDOMappingType.RPDO)
                                            subAccessType = EDSsharp.AccessType.rww;
                                        else if (subPDOtype == PDOMappingType.TPDO)
                                            subAccessType = EDSsharp.AccessType.rwr;
                                    }

                                    ODentry subod = new ODentry
                                    {
                                        parent = od,
                                        parameter_name = netSubObj.name ?? "",
                                        objecttype = (ObjectType)netSubObj.objectType,
                                        PDOtype = subPDOtype,
                                        // following values are optional and may be overriden by parameters from body_device
                                        accesstype = subAccessType,
                                        datatype = netSubObj.dataType != null && netSubObj.dataType.Length == 1 ? (DataType)netSubObj.dataType[0] : DataType.UNKNOWN,
                                        defaultvalue = netSubObj.defaultValue ?? "",
                                        actualvalue = netSubObj.actualValue ?? "",
                                        denotation = netSubObj.denotation ?? "",
                                        LowLimit = netSubObj.lowLimit ?? "",
                                        HighLimit = netSubObj.highLimit ?? "",
                                        ObjFlags = netSubObj.objFlags != null && netSubObj.objFlags.Length == 2 ? netSubObj.objFlags[1] : (byte)0,
                                        uniqueID = netSubObj.uniqueIDRef ?? ""
                                    };

                                    if (netSubObj.uniqueIDRef != null && netSubObj.uniqueIDRef != "" && parameters.ContainsKey(netSubObj.uniqueIDRef))
                                    {
                                        parameter devSubPar = parameters[netSubObj.uniqueIDRef];

                                        subod.Description = G_label_getDescription(devSubPar.Items);

                                        subod.accesstype = ConvertAccessType(devSubPar.access);

                                        if (devSubPar.defaultValue != null && devSubPar.defaultValue.value != null)
                                            subod.defaultvalue = devSubPar.defaultValue.value;

                                        if (devSubPar.Items1 != null && devSubPar.Items1ElementName != null)
                                            subod.datatype = ConvertDataType(devSubPar.Items1ElementName[0], subod.defaultvalue);

                                        if (devSubPar.actualValue != null && devSubPar.actualValue.value != null)
                                            subod.actualvalue = devSubPar.actualValue.value;

                                        if (devSubPar.denotation != null)
                                            subod.denotation = G_label_getDescription(devSubPar.denotation.Items);

                                        if (devSubPar.allowedValues != null && devSubPar.allowedValues.range != null && devSubPar.allowedValues.range[0] != null)
                                        {
                                            range r = devSubPar.allowedValues.range[0];
                                            if (r.minValue != null) subod.LowLimit = r.minValue.value ?? "";
                                            if (r.maxValue != null) subod.HighLimit = r.maxValue.value ?? "";
                                        }

                                        subod.prop.OdeXdd(devSubPar.property);
                                    }

                                    if (od.objecttype == ObjectType.ARRAY)
                                    {
                                        od.datatype = subod.datatype;
                                        od.accesstype = subod.accesstype;
                                        od.PDOtype = subod.PDOtype;
                                        od.prop.CO_accessSRDO = subod.prop.CO_accessSRDO;
                                    }

                                    if (!od.subobjects.ContainsKey(subIndex))
                                        od.subobjects.Add(subIndex, subod);
                                }
                            }
                            if (!eds.ods.ContainsKey(index))
                                eds.ods.Add(index, od);
                        }
                    }
                }
            }

            // Remove OD from the container and store container into eds to preserve unhandled objects
            if (body_device != null && body_device.ApplicationProcess != null && body_device.ApplicationProcess[0] != null)
            {
                body_device.ApplicationProcess[0].dataTypeList = null;
                body_device.ApplicationProcess[0].parameterList = null;
            }
            if (ApplicationLayers != null)
                ApplicationLayers.CANopenObjectList = null;
            eds.xddTemplate = container;

            return eds;
        }
    }
}

[XmlRoot(ElementName = "CanOpenProject_1_1")]
public class CanOpenProject_1_1
{
    [XmlElement(ElementName = "ISO15745ProfileContainer", Namespace = "http://www.canopen.org/xml/1.1")]
    public List<ISO15745ProfileContainer> ISO15745ProfileContainer { get; set; }
    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }
}

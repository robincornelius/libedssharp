using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections.Generic;
using CanOpenXDD;
using System.IO;

namespace libEDSsharp
{
    public class CanOpenXDDclass
    {
        public ISO15745ProfileContainer dev;
        public void readXML(string file)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(ISO15745ProfileContainer));
            StreamReader reader = new StreamReader(file);
            dev = (ISO15745ProfileContainer)serializer.Deserialize(reader);
            reader.Close();
        }

        public void writeXML(string file)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(ISO15745ProfileContainer));
            StreamWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, dev);
            writer.Close();
        }
    }
}

   /* 
    Licensed under the Apache License, Version 2.0
    
    http://www.apache.org/licenses/LICENSE-2.0
    */

namespace CanOpenXDD
{
    [XmlRoot(ElementName = "ISO15745Reference", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ISO15745Reference
    {
        [XmlElement(ElementName = "ISO15745Part", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ISO15745Part { get; set; }
        [XmlElement(ElementName = "ISO15745Edition", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ISO15745Edition { get; set; }
        [XmlElement(ElementName = "ProfileTechnology", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ProfileTechnology { get; set; }
    }

    [XmlRoot(ElementName = "ProfileHeader", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ProfileHeader
    {
        [XmlElement(ElementName = "ProfileIdentification", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ProfileIdentification { get; set; }
        [XmlElement(ElementName = "ProfileRevision", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ProfileRevision { get; set; }
        [XmlElement(ElementName = "ProfileName", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ProfileName { get; set; }
        [XmlElement(ElementName = "ProfileSource", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ProfileSource { get; set; }
        [XmlElement(ElementName = "ProfileClassID", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ProfileClassID { get; set; }
        [XmlElement(ElementName = "ISO15745Reference", Namespace = "http://www.canopen.org/xml/1.0")]
        public ISO15745Reference ISO15745Reference { get; set; }
    }

    [XmlRoot(ElementName = "vendorName", Namespace = "http://www.canopen.org/xml/1.0")]
    public class VendorName
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "vendorID", Namespace = "http://www.canopen.org/xml/1.0")]
    public class VendorID
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Label
    {
        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "vendorText", Namespace = "http://www.canopen.org/xml/1.0")]
    public class VendorText
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
    }

    [XmlRoot(ElementName = "deviceFamily", Namespace = "http://www.canopen.org/xml/1.0")]
    public class DeviceFamily
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
    }

    [XmlRoot(ElementName = "productFamily", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ProductFamily
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
    }

    [XmlRoot(ElementName = "productName", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ProductName
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "productID", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ProductID
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "description", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Description
    {
        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "productText", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ProductText
    {
        [XmlElement(ElementName = "description", Namespace = "http://www.canopen.org/xml/1.0")]
        public Description Description { get; set; }
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
    }

    [XmlRoot(ElementName = "orderNumber", Namespace = "http://www.canopen.org/xml/1.0")]
    public class OrderNumber
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "version", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Version
    {
        [XmlAttribute(AttributeName = "versionType")]
        public string VersionType { get; set; }
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
    }

    [XmlRoot(ElementName = "specificationRevision", Namespace = "http://www.canopen.org/xml/1.0")]
    public class SpecificationRevision
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
    }

    [XmlRoot(ElementName = "instanceName", Namespace = "http://www.canopen.org/xml/1.0")]
    public class InstanceName
    {
        [XmlAttribute(AttributeName = "readOnly")]
        public string ReadOnly { get; set; }
    }

    [XmlRoot(ElementName = "DeviceIdentity", Namespace = "http://www.canopen.org/xml/1.0")]
    public class DeviceIdentity
    {
        [XmlElement(ElementName = "vendorName", Namespace = "http://www.canopen.org/xml/1.0")]
        public VendorName VendorName { get; set; }
        [XmlElement(ElementName = "vendorID", Namespace = "http://www.canopen.org/xml/1.0")]
        public VendorID VendorID { get; set; }
        [XmlElement(ElementName = "vendorText", Namespace = "http://www.canopen.org/xml/1.0")]
        public VendorText VendorText { get; set; }
        [XmlElement(ElementName = "deviceFamily", Namespace = "http://www.canopen.org/xml/1.0")]
        public DeviceFamily DeviceFamily { get; set; }
        [XmlElement(ElementName = "productFamily", Namespace = "http://www.canopen.org/xml/1.0")]
        public ProductFamily ProductFamily { get; set; }
        [XmlElement(ElementName = "productName", Namespace = "http://www.canopen.org/xml/1.0")]
        public ProductName ProductName { get; set; }
        [XmlElement(ElementName = "productID", Namespace = "http://www.canopen.org/xml/1.0")]
        public ProductID ProductID { get; set; }
        [XmlElement(ElementName = "productText", Namespace = "http://www.canopen.org/xml/1.0")]
        public ProductText ProductText { get; set; }
        [XmlElement(ElementName = "orderNumber", Namespace = "http://www.canopen.org/xml/1.0")]
        public OrderNumber OrderNumber { get; set; }
        [XmlElement(ElementName = "version", Namespace = "http://www.canopen.org/xml/1.0")]
        public List<Version> Version { get; set; }
        [XmlElement(ElementName = "specificationRevision", Namespace = "http://www.canopen.org/xml/1.0")]
        public SpecificationRevision SpecificationRevision { get; set; }
        [XmlElement(ElementName = "instanceName", Namespace = "http://www.canopen.org/xml/1.0")]
        public InstanceName InstanceName { get; set; }
    }

    [XmlRoot(ElementName = "LEDstate", Namespace = "http://www.canopen.org/xml/1.0")]
    public class LEDstate
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
        [XmlAttribute(AttributeName = "uniqueID")]
        public string UniqueID { get; set; }
        [XmlAttribute(AttributeName = "state")]
        public string State { get; set; }
        [XmlAttribute(AttributeName = "LEDcolor")]
        public string LEDcolor { get; set; }
    }

    [XmlRoot(ElementName = "LED", Namespace = "http://www.canopen.org/xml/1.0")]
    public class LED
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
        [XmlElement(ElementName = "LEDstate", Namespace = "http://www.canopen.org/xml/1.0")]
        public LEDstate LEDstate { get; set; }
        [XmlAttribute(AttributeName = "LEDcolors")]
        public string LEDcolors { get; set; }
        [XmlAttribute(AttributeName = "LEDtype")]
        public string LEDtype { get; set; }
    }

    [XmlRoot(ElementName = "LEDList", Namespace = "http://www.canopen.org/xml/1.0")]
    public class LEDList
    {
        [XmlElement(ElementName = "LED", Namespace = "http://www.canopen.org/xml/1.0")]
        public LED LED { get; set; }
    }

    [XmlRoot(ElementName = "indicatorList", Namespace = "http://www.canopen.org/xml/1.0")]
    public class IndicatorList
    {
        [XmlElement(ElementName = "LEDList", Namespace = "http://www.canopen.org/xml/1.0")]
        public LEDList LEDList { get; set; }
    }

    [XmlRoot(ElementName = "DeviceManager", Namespace = "http://www.canopen.org/xml/1.0")]
    public class DeviceManager
    {
        [XmlElement(ElementName = "indicatorList", Namespace = "http://www.canopen.org/xml/1.0")]
        public IndicatorList IndicatorList { get; set; }
    }

    [XmlRoot(ElementName = "characteristicName", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CharacteristicName
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
    }

    [XmlRoot(ElementName = "characteristicContent", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CharacteristicContent
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
    }

    [XmlRoot(ElementName = "characteristic", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Characteristic
    {
        [XmlElement(ElementName = "characteristicName", Namespace = "http://www.canopen.org/xml/1.0")]
        public CharacteristicName CharacteristicName { get; set; }
        [XmlElement(ElementName = "characteristicContent", Namespace = "http://www.canopen.org/xml/1.0")]
        public CharacteristicContent CharacteristicContent { get; set; }
    }

    [XmlRoot(ElementName = "characteristicsList", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CharacteristicsList
    {
        [XmlElement(ElementName = "characteristic", Namespace = "http://www.canopen.org/xml/1.0")]
        public Characteristic Characteristic { get; set; }
    }

    [XmlRoot(ElementName = "capabilities", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Capabilities
    {
        [XmlElement(ElementName = "characteristicsList", Namespace = "http://www.canopen.org/xml/1.0")]
        public CharacteristicsList CharacteristicsList { get; set; }
    }

    [XmlRoot(ElementName = "DeviceFunction", Namespace = "http://www.canopen.org/xml/1.0")]
    public class DeviceFunction
    {
        [XmlElement(ElementName = "capabilities", Namespace = "http://www.canopen.org/xml/1.0")]
        public Capabilities Capabilities { get; set; }
    }

    [XmlRoot(ElementName = "denotation", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Denotation
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
    }

    [XmlRoot(ElementName = "defaultValue", Namespace = "http://www.canopen.org/xml/1.0")]
    public class DefaultValue
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "parameter", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Parameter
    {
        [XmlElement(ElementName = "label", Namespace = "http://www.canopen.org/xml/1.0")]
        public Label Label { get; set; }
        [XmlElement(ElementName = "UDINT", Namespace = "http://www.canopen.org/xml/1.0")]
        public string UDINT { get; set; }
        [XmlElement(ElementName = "denotation", Namespace = "http://www.canopen.org/xml/1.0")]
        public Denotation Denotation { get; set; }
        [XmlElement(ElementName = "defaultValue", Namespace = "http://www.canopen.org/xml/1.0")]
        public DefaultValue DefaultValue { get; set; }
        [XmlAttribute(AttributeName = "uniqueID")]
        public string UniqueID { get; set; }
        [XmlAttribute(AttributeName = "access")]
        public string Access { get; set; }
        [XmlElement(ElementName = "USINT", Namespace = "http://www.canopen.org/xml/1.0")]
        public string USINT { get; set; }
        [XmlElement(ElementName = "UINT", Namespace = "http://www.canopen.org/xml/1.0")]
        public string UINT { get; set; }
        [XmlElement(ElementName = "SINT", Namespace = "http://www.canopen.org/xml/1.0")]
        public string SINT { get; set; }
        [XmlElement(ElementName = "allowedValues", Namespace = "http://www.canopen.org/xml/1.0")]
        public AllowedValues AllowedValues { get; set; }
        [XmlElement(ElementName = "INT", Namespace = "http://www.canopen.org/xml/1.0")]
        public string INT { get; set; }
        [XmlElement(ElementName = "DINT", Namespace = "http://www.canopen.org/xml/1.0")]
        public string DINT { get; set; }
    }

    [XmlRoot(ElementName = "minValue", Namespace = "http://www.canopen.org/xml/1.0")]
    public class MinValue
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "maxValue", Namespace = "http://www.canopen.org/xml/1.0")]
    public class MaxValue
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "range", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Range
    {
        [XmlElement(ElementName = "minValue", Namespace = "http://www.canopen.org/xml/1.0")]
        public MinValue MinValue { get; set; }
        [XmlElement(ElementName = "maxValue", Namespace = "http://www.canopen.org/xml/1.0")]
        public MaxValue MaxValue { get; set; }
    }

    [XmlRoot(ElementName = "allowedValues", Namespace = "http://www.canopen.org/xml/1.0")]
    public class AllowedValues
    {
        [XmlElement(ElementName = "range", Namespace = "http://www.canopen.org/xml/1.0")]
        public Range Range { get; set; }
    }

    [XmlRoot(ElementName = "parameterList", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ParameterList
    {
        [XmlElement(ElementName = "parameter", Namespace = "http://www.canopen.org/xml/1.0")]
        public List<Parameter> Parameter { get; set; }
    }

    [XmlRoot(ElementName = "ApplicationProcess", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ApplicationProcess
    {
        [XmlElement(ElementName = "parameterList", Namespace = "http://www.canopen.org/xml/1.0")]
        public ParameterList ParameterList { get; set; }
    }

    [XmlRoot(ElementName = "ProfileBody", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ProfileBody
    {
        [XmlElement(ElementName = "DeviceIdentity", Namespace = "http://www.canopen.org/xml/1.0")]
        public DeviceIdentity DeviceIdentity { get; set; }
        [XmlElement(ElementName = "DeviceManager", Namespace = "http://www.canopen.org/xml/1.0")]
        public DeviceManager DeviceManager { get; set; }
        [XmlElement(ElementName = "DeviceFunction", Namespace = "http://www.canopen.org/xml/1.0")]
        public DeviceFunction DeviceFunction { get; set; }
        [XmlElement(ElementName = "ApplicationProcess", Namespace = "http://www.canopen.org/xml/1.0")]
        public ApplicationProcess ApplicationProcess { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "fileName")]
        public string FileName { get; set; }
        [XmlAttribute(AttributeName = "fileCreator")]
        public string FileCreator { get; set; }
        [XmlAttribute(AttributeName = "fileCreationDate")]
        public string FileCreationDate { get; set; }
        [XmlAttribute(AttributeName = "fileCreationTime")]
        public string FileCreationTime { get; set; }
        [XmlAttribute(AttributeName = "fileModificationDate")]
        public string FileModificationDate { get; set; }
        [XmlAttribute(AttributeName = "fileModificationTime")]
        public string FileModificationTime { get; set; }
        [XmlAttribute(AttributeName = "fileModifiedBy")]
        public string FileModifiedBy { get; set; }
        [XmlAttribute(AttributeName = "fileVersion")]
        public string FileVersion { get; set; }
        [XmlAttribute(AttributeName = "supportedLanguages")]
        public string SupportedLanguages { get; set; }
        [XmlElement(ElementName = "ApplicationLayers", Namespace = "http://www.canopen.org/xml/1.0")]
        public ApplicationLayers ApplicationLayers { get; set; }
        [XmlElement(ElementName = "TransportLayers", Namespace = "http://www.canopen.org/xml/1.0")]
        public TransportLayers TransportLayers { get; set; }
        [XmlElement(ElementName = "NetworkManagement", Namespace = "http://www.canopen.org/xml/1.0")]
        public NetworkManagement NetworkManagement { get; set; }
    }

    [XmlRoot(ElementName = "ISO15745Profile", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ISO15745Profile
    {
        [XmlElement(ElementName = "ProfileHeader", Namespace = "http://www.canopen.org/xml/1.0")]
        public ProfileHeader ProfileHeader { get; set; }
        [XmlElement(ElementName = "ProfileBody", Namespace = "http://www.canopen.org/xml/1.0")]
        public ProfileBody ProfileBody { get; set; }
    }

    [XmlRoot(ElementName = "identity", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Identity
    {
        [XmlElement(ElementName = "vendorID", Namespace = "http://www.canopen.org/xml/1.0")]
        public string VendorID { get; set; }
        [XmlElement(ElementName = "productID", Namespace = "http://www.canopen.org/xml/1.0")]
        public string ProductID { get; set; }
    }

    [XmlRoot(ElementName = "CANopenObject", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CANopenObject
    {
        [XmlAttribute(AttributeName = "index")]
        public string Index { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "objectType")]
        public string ObjectType { get; set; }
        [XmlAttribute(AttributeName = "dataType")]
        public string DataType { get; set; }
        [XmlAttribute(AttributeName = "PDOmapping")]
        public string PDOmapping { get; set; }
        [XmlAttribute(AttributeName = "uniqueIDRef")]
        public string UniqueIDRef { get; set; }
        [XmlElement(ElementName = "CANopenSubObject", Namespace = "http://www.canopen.org/xml/1.0")]
        public List<CANopenSubObject> CANopenSubObject { get; set; }
        [XmlAttribute(AttributeName = "subNumber")]
        public string SubNumber { get; set; }
        [XmlAttribute(AttributeName = "accessType")]
        public string AccessType { get; set; }
        [XmlAttribute(AttributeName = "defaultValue")]
        public string DefaultValue { get; set; }
    }

    [XmlRoot(ElementName = "CANopenSubObject", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CANopenSubObject
    {
        [XmlAttribute(AttributeName = "subIndex")]
        public string SubIndex { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "objectType")]
        public string ObjectType { get; set; }
        [XmlAttribute(AttributeName = "dataType")]
        public string DataType { get; set; }
        [XmlAttribute(AttributeName = "accessType")]
        public string AccessType { get; set; }
        [XmlAttribute(AttributeName = "defaultValue")]
        public string DefaultValue { get; set; }
        [XmlAttribute(AttributeName = "PDOmapping")]
        public string PDOmapping { get; set; }
        [XmlAttribute(AttributeName = "uniqueIDRef")]
        public string UniqueIDRef { get; set; }
        [XmlAttribute(AttributeName = "lowLimit")]
        public string LowLimit { get; set; }
        [XmlAttribute(AttributeName = "highLimit")]
        public string HighLimit { get; set; }
    }

    [XmlRoot(ElementName = "CANopenObjectList", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CANopenObjectList
    {
        [XmlElement(ElementName = "CANopenObject", Namespace = "http://www.canopen.org/xml/1.0")]
        public List<CANopenObject> CANopenObject { get; set; }
    }

    [XmlRoot(ElementName = "dummy", Namespace = "http://www.canopen.org/xml/1.0")]
    public class Dummy
    {
        [XmlAttribute(AttributeName = "entry")]
        public string Entry { get; set; }
    }

    [XmlRoot(ElementName = "dummyUsage", Namespace = "http://www.canopen.org/xml/1.0")]
    public class DummyUsage
    {
        [XmlElement(ElementName = "dummy", Namespace = "http://www.canopen.org/xml/1.0")]
        public List<Dummy> Dummy { get; set; }
    }

    [XmlRoot(ElementName = "ApplicationLayers", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ApplicationLayers
    {
        [XmlElement(ElementName = "identity", Namespace = "http://www.canopen.org/xml/1.0")]
        public Identity Identity { get; set; }
        [XmlElement(ElementName = "CANopenObjectList", Namespace = "http://www.canopen.org/xml/1.0")]
        public CANopenObjectList CANopenObjectList { get; set; }
        [XmlElement(ElementName = "dummyUsage", Namespace = "http://www.canopen.org/xml/1.0")]
        public DummyUsage DummyUsage { get; set; }
    }

    [XmlRoot(ElementName = "supportedBaudRate", Namespace = "http://www.canopen.org/xml/1.0")]
    public class SupportedBaudRate
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "baudRate", Namespace = "http://www.canopen.org/xml/1.0")]
    public class BaudRate
    {
        [XmlElement(ElementName = "supportedBaudRate", Namespace = "http://www.canopen.org/xml/1.0")]
        public List<SupportedBaudRate> SupportedBaudRate { get; set; }
    }

    [XmlRoot(ElementName = "PhysicalLayer", Namespace = "http://www.canopen.org/xml/1.0")]
    public class PhysicalLayer
    {
        [XmlElement(ElementName = "baudRate", Namespace = "http://www.canopen.org/xml/1.0")]
        public BaudRate BaudRate { get; set; }
    }

    [XmlRoot(ElementName = "TransportLayers", Namespace = "http://www.canopen.org/xml/1.0")]
    public class TransportLayers
    {
        [XmlElement(ElementName = "PhysicalLayer", Namespace = "http://www.canopen.org/xml/1.0")]
        public PhysicalLayer PhysicalLayer { get; set; }
    }

    [XmlRoot(ElementName = "CANopenGeneralFeatures", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CANopenGeneralFeatures
    {
        [XmlAttribute(AttributeName = "groupMessaging")]
        public string GroupMessaging { get; set; }
        [XmlAttribute(AttributeName = "dynamicChannels")]
        public string DynamicChannels { get; set; }
        [XmlAttribute(AttributeName = "selfStartingDevice")]
        public string SelfStartingDevice { get; set; }
        [XmlAttribute(AttributeName = "SDORequestingDevice")]
        public string SDORequestingDevice { get; set; }
        [XmlAttribute(AttributeName = "granularity")]
        public string Granularity { get; set; }
        [XmlAttribute(AttributeName = "nrOfRxPDO")]
        public string NrOfRxPDO { get; set; }
        [XmlAttribute(AttributeName = "nrOfTxPDO")]
        public string NrOfTxPDO { get; set; }
        [XmlAttribute(AttributeName = "bootUpSlave")]
        public string BootUpSlave { get; set; }
        [XmlAttribute(AttributeName = "layerSettingServiceSlave")]
        public string LayerSettingServiceSlave { get; set; }
    }

    [XmlRoot(ElementName = "CANopenMasterFeatures", Namespace = "http://www.canopen.org/xml/1.0")]
    public class CANopenMasterFeatures
    {
        [XmlAttribute(AttributeName = "bootUpMaster")]
        public string BootUpMaster { get; set; }
        [XmlAttribute(AttributeName = "flyingMaster")]
        public string FlyingMaster { get; set; }
        [XmlAttribute(AttributeName = "SDOManager")]
        public string SDOManager { get; set; }
        [XmlAttribute(AttributeName = "configurationManager")]
        public string ConfigurationManager { get; set; }
        [XmlAttribute(AttributeName = "layerSettingServiceMaster")]
        public string LayerSettingServiceMaster { get; set; }
    }

    [XmlRoot(ElementName = "deviceCommissioning", Namespace = "http://www.canopen.org/xml/1.0")]
    public class DeviceCommissioning
    {
        [XmlAttribute(AttributeName = "nodeID")]
        public string NodeID { get; set; }
        [XmlAttribute(AttributeName = "nodeName")]
        public string NodeName { get; set; }
        [XmlAttribute(AttributeName = "actualBaudRate")]
        public string ActualBaudRate { get; set; }
        [XmlAttribute(AttributeName = "networkNumber")]
        public string NetworkNumber { get; set; }
        [XmlAttribute(AttributeName = "networkName")]
        public string NetworkName { get; set; }
        [XmlAttribute(AttributeName = "CANopenManager")]
        public string CANopenManager { get; set; }
    }

    [XmlRoot(ElementName = "NetworkManagement", Namespace = "http://www.canopen.org/xml/1.0")]
    public class NetworkManagement
    {
        [XmlElement(ElementName = "CANopenGeneralFeatures", Namespace = "http://www.canopen.org/xml/1.0")]
        public CANopenGeneralFeatures CANopenGeneralFeatures { get; set; }
        [XmlElement(ElementName = "CANopenMasterFeatures", Namespace = "http://www.canopen.org/xml/1.0")]
        public CANopenMasterFeatures CANopenMasterFeatures { get; set; }
        [XmlElement(ElementName = "deviceCommissioning", Namespace = "http://www.canopen.org/xml/1.0")]
        public DeviceCommissioning DeviceCommissioning { get; set; }
    }

    [XmlRoot(ElementName = "ISO15745ProfileContainer", Namespace = "http://www.canopen.org/xml/1.0")]
    public class ISO15745ProfileContainer
    {
        [XmlElement(ElementName = "ISO15745Profile", Namespace = "http://www.canopen.org/xml/1.0")]
        public List<ISO15745Profile> ISO15745Profile { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
    }

}

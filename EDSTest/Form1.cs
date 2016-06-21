/*
    This file is part of libEDSsharp.

    libEDSsharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with libEDSsharp.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libEDSsharp;
using System.Globalization;
using Xml2CSharp;


namespace EDSTest
{
    public partial class Form1 : Form
    {
        EDSsharp eds;
        Device dev; //one day this will be multiple devices

        object selectedobject;
        CANopenObject selectedparent;


        public Form1()
        {
            InitializeComponent();

            comboBox_datatype.Items.Add("");

            foreach(DataType foo in Enum.GetValues(typeof(DataType )))
            {
                comboBox_datatype.Items.Add(foo.ToString());
            }

            comboBox_objecttype.Items.Add("");

            foreach (ObjectType foo in Enum.GetValues(typeof(ObjectType)))
            {
                comboBox_objecttype.Items.Add(foo.ToString());
            }

            comboBox_accesstype.Items.Add("");

            foreach (EDSsharp.AccessType foo in Enum.GetValues(typeof(EDSsharp.AccessType)))
            {
                comboBox_accesstype.Items.Add(foo.ToString());
            }


            comboBox_memory.Items.Add("");

            foreach (StorageLocation foo in Enum.GetValues(typeof(StorageLocation)))
            {
                comboBox_memory.Items.Add(foo.ToString());
            }

            comboBox_pdomap.Items.Add("");
            comboBox_pdomap.Items.Add("no");
            comboBox_pdomap.Items.Add("optional");
            comboBox_pdomap.Items.Add("yes"); //?? 
            
            
        }

        private void openEDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "Electronic Data Sheets (*.eds)|*.eds";
            if(odf.ShowDialog()==DialogResult.OK)
            {
                
                eds = new EDSsharp();
                //try
                {
                    eds.loadfile(odf.FileName);
                    Bridge bridge = new Bridge(); //tell me again why bridge is not static?
                    dev = bridge.convert(eds);
                    populateindexlists();
                }
                //catch(Exception ex)
                {
                //    MessageBox.Show(ex.ToString());
                }

            }
        }

        private void populateindexlists()
        {

            if (dev == null)
                return;

            foreach(CANopenObject canobj in dev.CANopenObjectList.CANopenObject)
            {

                UInt16 index = Convert.ToUInt16(canobj.Index, 16);
               
                ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}", index));
                lvi.SubItems.Add(canobj.Name);

                if (index == 0x1000 || index==0x1001 || index==0x1018)
                {
                    listView_mandatory_objects.Items.Add(lvi);
                }
                else if (index >= 0x2000 && index < 0x6000 )
                {
                    listView_manufacture_objects.Items.Add(lvi);
                }else 
                {
                    listView_optional_objects.Items.Add(lvi);
                }
            }
        }

        private void listView_mandatory_objects_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }


        private CANopenObject getobjectforindex(UInt16 index)
        {
            foreach (CANopenObject canobj in dev.CANopenObjectList.CANopenObject)
            {
                UInt16 obindex = Convert.ToUInt16(canobj.Index, 16);
                if (index == obindex)
                {
                    return canobj;
                }
            }

            return null;
        }

        private CANopenSubObject getobjectforsubindex(UInt16 index,UInt16 subidx,out CANopenObject parent)
        {
            parent = null;
            foreach (CANopenObject canobj in dev.CANopenObjectList.CANopenObject)
            {
                UInt16 obindex = Convert.ToUInt16(canobj.Index, 16);
                if (index == obindex)
                {
                    parent = canobj;
                    foreach(CANopenSubObject sub in canobj.CANopenSubObject)
                    {
                        if(Convert.ToInt16(sub.SubIndex,16)==subidx)
                        {
                            return sub;
                        }
                    }
                   
                }
            }

            return null;
        }


        private object getwhatweclickedon(string tag,out CANopenObject toplevel)
        {
            toplevel = null;

            object ret = null;
            string[] bits = tag.Split('/');

            Int16 index = -1;
            Int16 sub = -1;

            if (bits.Length == 1)
            {
                index = Convert.ToInt16(bits[0], 16); 
                toplevel = getobjectforindex((UInt16)index);
                ret = (object)toplevel;
            }
            else
            {
                index = Convert.ToInt16(bits[0], 16);
                sub = Convert.ToInt16(bits[1]);
                ret = (object)getobjectforsubindex((UInt16)index, (UInt16)sub,out toplevel);
            }

            return ret;
        }


        private void updateselectedindexdisplay(UInt16 index)
        {

            listViewDetails.Items.Clear();

            foreach (CANopenObject canobj in dev.CANopenObjectList.CANopenObject)
            {
                UInt16 obindex = Convert.ToUInt16(canobj.Index, 16);
                if (index == obindex)
                {
                    if (Convert.ToInt16(canobj.SubNumber) == 0)
                    {
                        ListViewItem lvi = new ListViewItem("0");
                        lvi.SubItems.Add(canobj.Name);

                        ObjectType ot = (ObjectType)Convert.ToInt16(canobj.ObjectType,16);
                        lvi.SubItems.Add(ot.ToString());
                        
                        DataType dt = (DataType)Convert.ToInt16(canobj.DataType, 16);
                        lvi.SubItems.Add(dt.ToString());

                        lvi.SubItems.Add(canobj.AccessType);
                        lvi.SubItems.Add(canobj.DefaultValue);
                        lvi.SubItems.Add(canobj.PDOmapping);

                        lvi.Tag = string.Format("{0:x4}",index);

                        listViewDetails.Items.Add(lvi);
                    }
                    else
                    {
                        ListViewItem lvi = new ListViewItem(" ");
                        lvi.SubItems.Add(canobj.Name);

                        ObjectType ot = (ObjectType)Convert.ToInt16(canobj.ObjectType, 16);
                        lvi.SubItems.Add(ot.ToString());

                        if (ot == ObjectType.REC)
                        {
                            DataType dt = (DataType)Convert.ToInt16(canobj.DataType, 16);
                            lvi.SubItems.Add(dt.ToString());

                            lvi.SubItems.Add(canobj.AccessType);
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add(canobj.PDOmapping);
                        }

                        lvi.Tag = string.Format("{0:x4}", index);
             
                        listViewDetails.Items.Add(lvi);

                        foreach(CANopenSubObject subobj in canobj.CANopenSubObject)
                        {
                            ListViewItem lvi2 = new ListViewItem(subobj.SubIndex);
                            lvi2.SubItems.Add(subobj.Name);

                            ObjectType ot2 = (ObjectType)Convert.ToInt16(subobj.ObjectType, 16);
                            lvi2.SubItems.Add(ot2.ToString());

                            if(subobj.DataType==null)
                            {
                                 lvi2.SubItems.Add(" -- ");
                            }
                            else
                            {
                                 DataType  dt = (DataType)Convert.ToInt16(subobj.DataType, 16);
                                 lvi2.SubItems.Add(dt.ToString());
                            }

                            if(subobj.AccessType==null)
                            {
                                lvi2.SubItems.Add(" -- ");
                            }
                            else
                            {
                                lvi2.SubItems.Add(subobj.AccessType);
                            }

                            lvi2.SubItems.Add(subobj.DefaultValue);

                            if (subobj.PDOmapping == null)
                            {
                                lvi2.SubItems.Add(" -- ");
                            }
                            else
                            {
                                lvi2.SubItems.Add(subobj.PDOmapping);
                            }

                            lvi2.Tag = string.Format("{0:x4}/{1:x}", index,subobj.SubIndex);
                            listViewDetails.Items.Add(lvi2);
                        }
                    }
                }
            }

        }

        private void listView_mandatory_objects_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView_mandatory_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);
            updateselectedindexdisplay(idx);

            selectedobject = selectedparent = getobjectforindex(idx);
            validateanddisplaydata();


        }

        private void listView_optionalobjects_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView_optional_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            updateselectedindexdisplay(idx);


            selectedobject = selectedparent = getobjectforindex(idx);
            validateanddisplaydata();
        }

        private void listView_manufacture_objects_MouseClick(object sender, MouseEventArgs e)
        {

            ListViewItem lvi = listView_manufacture_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            updateselectedindexdisplay(idx);

            selectedobject = selectedparent = getobjectforindex(idx);
            validateanddisplaydata();

        }

        private void exportCanOpenNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                CanOpenNodeExporter cone = new CanOpenNodeExporter();
                cone.export(fbd.SelectedPath, eds);
            }

        }

        private void openCanOpenNodeXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "XML (*.xml)|*.xml";
            if (odf.ShowDialog() == DialogResult.OK)
            {

                CanOpenXML coxml = new CanOpenXML();
                coxml.readXML(odf.FileName);

                Bridge b = new Bridge();

                eds = b.convert(coxml.dev);

                dev = coxml.dev;

                populateindexlists();

            }

        }

        private void listViewDetails_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listViewDetails.SelectedItems[0];
            string tag = (string)lvi.Tag;

            string[] bits = tag.Split('/');

            int index = -1;
            int sub = -1;

            if(bits.Length==1)
            {
                index = Convert.ToInt16(bits[0], 16);
            }
            else
            {
                index = Convert.ToInt16(bits[0], 16);
                sub = Convert.ToInt16(bits[1]);
            }

            foreach (CANopenObject canobj in dev.CANopenObjectList.CANopenObject)
            {
                UInt16 obindex = Convert.ToUInt16(canobj.Index, 16);
                if(index==obindex)
                {
                    if(sub==-1)
                    {
                        selectedobject = canobj;
                        validateanddisplaydata();
                        break;
                    }
                    else
                    {
                        foreach(CANopenSubObject subobj in canobj.CANopenSubObject)
                        {
                            if(Convert.ToInt16(subobj.SubIndex,16)==sub)
                            {
                                selectedobject = subobj;
                                validateanddisplaydata();
                                break;
                            }
                        }
                    }
                }

            }

        }

        private void validateanddisplaydata()
        {
            if(selectedobject.GetType()==typeof(CANopenObject))
            {
                CANopenObject obj = (CANopenObject)selectedobject;
                label_index.Text = obj.Index;
                textBox_name.Text = obj.Name;
                textBox_description.Text = obj.Description.Text.Replace("\n", "\r\n");

                if (obj.AccessType != null)
                {
                    EDSsharp.AccessType at = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), obj.AccessType.Replace("const", "cons"));
                    comboBox_accesstype.SelectedItem = at.ToString();
                }
                else
                {
                    comboBox_accesstype.SelectedItem = "";
                }

                DataType dt=DataType.UNKNOWN;
                if (obj.DataType != null)
                {
                    dt = (DataType)Convert.ToInt16(obj.DataType, 16);
                    comboBox_datatype.SelectedItem = dt.ToString();
                }
                else
                {
                    comboBox_datatype.SelectedItem = "";
                }

                ObjectType ot = ObjectType.UNKNOWN;

                if (obj.ObjectType != null)
                {
                    ot = (ObjectType)Convert.ToInt16(obj.ObjectType, 16);
                    comboBox_objecttype.SelectedItem = ot.ToString();
                }
                else
                {
                    comboBox_objecttype.SelectedItem = "";
                }

                if(obj.PDOmapping!=null)
                {
                    comboBox_pdomap.SelectedItem = obj.PDOmapping;
                }
                else
                {
                    comboBox_pdomap.SelectedItem = "";

                }

                checkBox_COS.Enabled = true;
                if(obj.TPDOdetectCOS!=null)
                {
                    checkBox_COS.Checked = obj.TPDOdetectCOS == "true";
                }

                if(obj.AccessFunctionName!=null)
                {
                    textBox_accessfunctionname.Text = obj.AccessFunctionName;
                }
                else
                {
                    textBox_accessfunctionname.Text = "";
                }


                if(obj.AccessFunctionPreCode!=null)
                {
                    textBox_precode.Text = obj.AccessFunctionPreCode;
                }
                else
                {
                    textBox_precode.Text = "";
                }

                if (obj.Disabled != null)
                {
                    checkBox_enabled.Enabled = true;
                    checkBox_enabled.Checked = obj.Disabled == "false";
                }
                else
                {
                    checkBox_enabled.Enabled = false;
                }

                if(obj.MemoryType!=null)
                {
                    comboBox_memory.SelectedItem = obj.MemoryType;
                }
                else
                {
                    comboBox_memory.SelectedItem = "";
                }


                textBox_accessfunctionname.Enabled = true;
                textBox_precode.Enabled = true;
                checkBox_enabled.Enabled = true;
                comboBox_memory.Enabled = true;


                textBox_defaultvalue.Enabled = true;

                comboBox_accesstype.Enabled = true;
                comboBox_datatype.Enabled = true;
                comboBox_objecttype.Enabled = false; 
                comboBox_pdomap.Enabled = true;
               

            }

            if (selectedobject.GetType() == typeof(CANopenSubObject))
            {
                 CANopenSubObject obj = (CANopenSubObject)selectedobject;;

                 ObjectType parent_ot = ObjectType.UNKNOWN;                 
                 CANopenObject parent = selectedparent;
                 if (parent.ObjectType != null)
                 {
                     parent_ot = (ObjectType)Convert.ToInt16(parent.ObjectType, 16);
                 }

                 textBox_name.Text = obj.Name;

                 if (obj.AccessType != null)
                 {
                     EDSsharp.AccessType at = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), obj.AccessType.Replace("const", "cons"));
                     comboBox_accesstype.SelectedItem = at.ToString();
                 }
                 else
                 {
                     comboBox_accesstype.SelectedItem = "";
                 }

                 if (obj.DataType != null)
                 {
                     DataType dt = (DataType)Convert.ToInt16(obj.DataType, 16);
                     comboBox_datatype.SelectedItem = dt.ToString();
                 }
                 else
                 {
                     comboBox_datatype.SelectedItem = "";
                 }

                 if (obj.ObjectType != null)
                 {
                     ObjectType ot = (ObjectType)Convert.ToInt16(obj.ObjectType, 16);
                     comboBox_objecttype.SelectedItem = ot.ToString();
                 }
                 else
                 {
                     comboBox_objecttype.SelectedItem = "";
                 }

                 if (obj.PDOmapping != null)
                 {
                     comboBox_pdomap.SelectedItem = obj.PDOmapping;
                 }
                 else
                 {
                     comboBox_pdomap.SelectedItem = "";

                 }

                if(obj.DefaultValue!=null)
                {
                    textBox_defaultvalue.Text = obj.DefaultValue;
                }

                //fixme should this be set by parent???
     
                checkBox_COS.Enabled = false;

                textBox_accessfunctionname.Enabled = false;
                textBox_precode.Enabled = false;
                checkBox_enabled.Enabled = false;
                comboBox_memory.Enabled=false;

                if((parent_ot==ObjectType.ARRAY && Convert.ToInt16(obj.SubIndex,16)==0) || parent_ot==ObjectType.REC)
                {
                        textBox_defaultvalue.Enabled = false;
                        comboBox_accesstype.Enabled = false;
                        comboBox_datatype.Enabled = false;
                        comboBox_objecttype.Enabled = false;
                        comboBox_pdomap.Enabled = false;

                    }
                    else
                    {
                        textBox_defaultvalue.Enabled = true;
                        comboBox_accesstype.Enabled = true;
                        comboBox_datatype.Enabled = true;
                        comboBox_objecttype.Enabled = false;
                        comboBox_pdomap.Enabled = true;
                    }
                
  
            }

        }
    }
}

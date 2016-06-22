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

        ODentry selectedobject;

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

                //fix me enable exceptions for production code
                
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

            if (eds == null)
                return;
            
            foreach(KeyValuePair<UInt16,ODentry> kvp in eds.ods)
            {

             
                UInt16 index = kvp.Value.index;
                ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}",   kvp.Value.index));
                lvi.SubItems.Add(kvp.Value.parameter_name);

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


        private void updateselectedindexdisplay(UInt16 index)
        {

            listViewDetails.Items.Clear();

            ODentry od = eds.ods[index];

            if(od.objecttype == ObjectType.VAR)
            {
                 ListViewItem lvi = new ListViewItem(" ");
                 lvi.SubItems.Add(od.parameter_name);
                 lvi.SubItems.Add(od.objecttype.ToString());
                 lvi.SubItems.Add(od.datatype.ToString());
                 lvi.SubItems.Add(od.accesstype.ToString());
                 lvi.SubItems.Add(od.defaultvalue);
                 lvi.SubItems.Add(od.PDOMapping.ToString());
                 lvi.Tag = od;

                 listViewDetails.Items.Add(lvi);
            }

            if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
            {
                ListViewItem lvi = new ListViewItem(" ");
                lvi.SubItems.Add(od.parameter_name);

                lvi.SubItems.Add(od.objecttype.ToString());

                if(od.objecttype==ObjectType.REC)
                {
                    if (od.subobjects.Count > 1)
                    {
                        lvi.SubItems.Add(od.subobjects[1].datatype.ToString());
                    }
                    else
                    {
                        lvi.SubItems.Add(" -- ");
                    }

                    lvi.SubItems.Add(od.accesstype.ToString());
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add(od.PDOMapping.ToString());
                }

                lvi.Tag = od;

                listViewDetails.Items.Add(lvi);

                foreach(KeyValuePair<UInt16,ODentry> kvp in od.subobjects)
                {
                    ODentry subod = kvp.Value;
                    int subindex = kvp.Key;

                    ListViewItem lvi2 = new ListViewItem(string.Format("{0:x}",subindex));
                    lvi2.SubItems.Add(subod.parameter_name);
                    lvi2.SubItems.Add(subod.objecttype.ToString());

                    
                    if (subod.datatype==DataType.UNKNOWN || (od.objecttype==ObjectType.REC && subod.subindex!=0))
                    {
                        lvi2.SubItems.Add(" -- ");
                    }
                    else
                    {
                        lvi2.SubItems.Add(subod.datatype.ToString());
                    }

                    if (subod.accesstype == EDSsharp.AccessType.UNKNOWN)
                    {
                        lvi2.SubItems.Add(" -- ");
                    }
                    else
                    {
                        lvi2.SubItems.Add(subod.accesstype.ToString());
                    }

                    lvi2.SubItems.Add(subod.defaultvalue);

                    //fixe me ??
                    lvi2.SubItems.Add(subod.PDOMapping.ToString());
                    
                    lvi2.Tag = subod;

                    listViewDetails.Items.Add(lvi2);

                }

            }

        }

        private void listView_mandatory_objects_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView_mandatory_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);
            updateselectedindexdisplay(idx);

            selectedobject = eds.ods[idx];
            validateanddisplaydata();


        }

        private void listView_optionalobjects_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView_optional_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            updateselectedindexdisplay(idx);


            selectedobject = eds.ods[idx];
            validateanddisplaydata();
        }

        private void listView_manufacture_objects_MouseClick(object sender, MouseEventArgs e)
        {

            ListViewItem lvi = listView_manufacture_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            updateselectedindexdisplay(idx);

            selectedobject = eds.ods[idx];
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

            ODentry od = (ODentry)lvi.Tag;

            selectedobject = od;
            validateanddisplaydata();

        }

        private void validateanddisplaydata()
        {

            ODentry od = (ODentry) selectedobject;

            
            label_index.Text = string.Format("0x{0:x4}",od.index);
            textBox_name.Text = od.parameter_name;
            comboBox_accesstype.SelectedItem = od.accesstype.ToString();

            if (od.datatype != DataType.UNKNOWN)
            {
                comboBox_datatype.SelectedItem = od.datatype.ToString();
            }
            else
            {
                if (od.objecttype == ObjectType.REC)
                {
                    if(od.subobjects.Count>=2)
                    {
                        comboBox_datatype.SelectedItem = od.subobjects[1].datatype.ToString();
                    }

                }
                else
                {
                    comboBox_datatype.SelectedItem = "";
                }
            }

            comboBox_objecttype.SelectedItem = od.objecttype.ToString();
            textBox_description.Text = od.Description.Replace("\n", "\r\n");
            //FIXME //comboBox_pdomap.SelectedItem = od.PDOmapping;

            checkBox_COS.Checked = od.TPDODetectCos;
            textBox_accessfunctionname.Text = od.AccessFunctionName;
            textBox_precode.Text = od.AccessFunctionPreCode;
            checkBox_enabled.Checked = !od.Disabled;

            comboBox_memory.SelectedItem = od.location.ToString();
     
            textBox_accessfunctionname.Enabled = true;
            textBox_precode.Enabled = true;
            checkBox_enabled.Enabled = true;
            comboBox_memory.Enabled = true;

            textBox_defaultvalue.Enabled = true;

            comboBox_accesstype.Enabled = true;
            comboBox_datatype.Enabled = true;
            comboBox_objecttype.Enabled = false; 
            comboBox_pdomap.Enabled = true;

            textBox_defaultvalue.Text = od.defaultvalue;

            if(od.parent!=null && ((od.parent.objecttype==ObjectType.ARRAY && od.subindex==0) ||(od.parent.objecttype==ObjectType.REC)))
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



            return;

        }
    }
}

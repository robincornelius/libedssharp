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

    Copyright(c) 2016 Robin Cornelius <robin.cornelius@gmail.com>
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libEDSsharp;
using System.Globalization;
using Xml2CSharp;

namespace EDSTest
{
    public partial class DeviceView : UserControl
    {

        public EDSsharp eds;

        ODentry selectedobject;
        ListViewItem selecteditem;

        public DeviceView()
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

        public void populatedeviceinfo()
        {
            textBox_productname.Text = eds.di.ProductName;
            textBox_productnumber.Text = eds.di.ProductNumber.ToString();
            textBox_vendorname.Text = eds.di.VendorName;
            textBox_vendornumber.Text = eds.di.VendorNumber.ToString();

            textBox_fileversion.Text = eds.fi.EDSVersion;
            textBox_modified_datetime.Text = eds.fi.ModificationDateTime.ToLongDateString();
            textBox_modifiedby.Text = eds.fi.ModifiedBy;
            
            textBox_filerevision.Text = eds.fi.FileRevision.ToString();
            textBox_fileversion.Text = eds.fi.FileVersion.ToString();

            textBox_createdby.Text = eds.fi.CreatedBy;
            textBox_create_datetime.Text = eds.fi.CreationDateTime.ToString();

            textBox_di_description.Text = eds.fi.Description;

            textBox_edsversionm.Text = eds.fi.EDSVersion;

            //textBox_fileversion.Text = eds.di

            checkBox_baud_10.Checked = eds.di.BaudRate_10;
            heckBox_baud_20.Checked = eds.di.BaudRate_20;
            heckBox_baud_50.Checked = eds.di.BaudRate_50;
            heckBox_baud_125.Checked = eds.di.BaudRate_125;
            heckBox_baud_250.Checked = eds.di.BaudRate_250;
            heckBox_baud_500.Checked = eds.di.BaudRate_500;
            heckBox_baud_800.Checked = eds.di.BaudRate_800;
            heckBox_baud_1000.Checked = eds.di.BaudRate_1000;

            checkBox_boot_master.Checked = eds.di.SimpleBootUpMaster;
            checkBox_bootslave.Checked = eds.di.SimpleBootUpSlave;
            checkBox_compactPDO.Checked = eds.di.CompactPDO;
            checkBox_group_msg.Checked = eds.di.GroupMessaging;
            checkBox_dynamicchan.Checked = eds.di.DynamicChannelsSupported;
            checkBox_lss.Checked = eds.di.LSS_Supported;
            textBox_Gran.Text = eds.di.Granularity.ToString();

            textBox_rxpdos.Text = eds.di.NrOfRXPDO.ToString();
            textBox_txpdos.Text = eds.di.NrOfTXPDO.ToString();



      
          
        }

        public void populateindexlists()
        {

            if (eds == null)
                return;

            populatedeviceinfo();

            listView_mandatory_objects.Items.Clear();
            listView_manufacture_objects.Items.Clear();
            listView_optional_objects.Items.Clear();

            foreach(KeyValuePair<UInt16,ODentry> kvp in eds.ods)
            {

             
                UInt16 index = kvp.Value.index;
                ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}",   kvp.Value.index));
                lvi.SubItems.Add(kvp.Value.parameter_name);
                lvi.Tag = kvp.Value;

                if (kvp.Value.Disabled == true)
                    lvi.ForeColor = Color.LightGray;

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

                if(od.objecttype==ObjectType.ARRAY)
                {
                    if (od.subobjects.Count > 1)
                    {
                        lvi.SubItems.Add(od.datatype.ToString());
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

                    
                    if (subod.datatype==DataType.UNKNOWN || (od.objecttype==ObjectType.ARRAY && subod.subindex!=0))
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

            if (e.Button == MouseButtons.Right)
            {

                if (listView_optional_objects.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    selecteditem = lvi;

                    ODentry od = (ODentry)lvi.Tag;
                    if (od.Disabled == true)
                    {
                        contextMenuStrip1.Items[2].Text = "Enable Object";
                    }
                    else
                    {
                        contextMenuStrip1.Items[2].Text = "Disable Object";
                    }

                    contextMenuStrip1.Show(Cursor.Position);
                }

                return;
            }

            updateselectedindexdisplay(idx);


            selectedobject = eds.ods[idx];
            validateanddisplaydata();
        }

        private void listView_manufacture_objects_MouseClick(object sender, MouseEventArgs e)
        {

            ListViewItem lvi = listView_manufacture_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            if (e.Button == MouseButtons.Right)
            {

                if (listView_manufacture_objects.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    selecteditem = lvi;
                    ODentry od = (ODentry)lvi.Tag;
                    if(od.Disabled==true)
                    {
                        contextMenuStrip1.Items[2].Text = "Enable Object";
                    }
                    else
                    {
                        contextMenuStrip1.Items[2].Text = "Disable Object";
                    }

                    contextMenuStrip1.Show(Cursor.Position);
                }

                return;
            }


            updateselectedindexdisplay(idx);

            selectedobject = eds.ods[idx];
            validateanddisplaydata();

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

        private void addNewObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
          NewIndex ni = new NewIndex(eds);

          if(ni.ShowDialog()==DialogResult.OK)
          {
              
              ODentry od = new ODentry();

              od.objecttype = ni.ot;
              od.index = ni.index;
              od.location = StorageLocation.RAM;
              od.defaultvalue = "";
              od.accesstype = EDSsharp.AccessType.rw;
              od.datatype = ni.dt;
              od.parameter_name = ni.name;

              if(od.objecttype == ObjectType.REC || od.objecttype==ObjectType.ARRAY)
              {
                  {
                      ODentry sod = new ODentry();

                      sod.objecttype = ObjectType.VAR;
                      sod.subindex = 0;
                      sod.index = ni.index;
                      sod.location = StorageLocation.RAM;
                      sod.defaultvalue = "";
                      sod.accesstype = EDSsharp.AccessType.ro;
                      sod.datatype = DataType.UNSIGNED8;

                      od.subobjects.Add(0, sod);
                  }

                  for (int p = 0; p < ni.nosubindexes; p++)
                  {
                      ODentry sod = new ODentry();

                      sod.objecttype = ObjectType.VAR;
                      sod.subindex = (UInt16)(p+1);
                      sod.index = ni.index;
                      sod.location = StorageLocation.RAM;
                      sod.defaultvalue = "";
                      sod.accesstype = EDSsharp.AccessType.rw;
                      sod.datatype = ni.dt;
                      sod.parent = od;

                      od.subobjects.Add((ushort)(p+1), sod);
                  }

              }

              eds.ods.Add(od.index, od);

              populateindexlists();
          }
           
        }

        private void deleteObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ListViewItem item = selecteditem;

            ODentry od = (ODentry)item.Tag;

            if(MessageBox.Show(string.Format("Really delete index 0x{0:x4} ?",od.index),"Are you sure?",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                eds.ods.Remove(od.index);
                populateindexlists();
            }


        }

        private void disableObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ListViewItem item = selecteditem;

            ODentry od = (ODentry)item.Tag;

            od.Disabled = !od.Disabled;
            populateindexlists();

        }

        private void button_update_devfile_info_Click(object sender, EventArgs e)
        {

            try
            {
                eds.di.ProductName = textBox_productname.Text;
                eds.di.ProductNumber = Convert.ToUInt32(textBox_productnumber.Text);

                eds.di.VendorName = textBox_vendorname.Text;
                eds.di.VendorNumber = Convert.ToUInt32(textBox_vendornumber.Text);

                eds.fi.EDSVersion = textBox_fileversion.Text;

                eds.fi.ModificationDateTime = DateTime.Parse(textBox_modified_datetime.Text);

                eds.fi.ModifiedBy = textBox_modifiedby.Text;

                eds.fi.FileRevision = Convert.ToByte(textBox_filerevision.Text);

                eds.fi.FileVersion = Convert.ToByte(textBox_fileversion.Text);


                eds.fi.CreatedBy = textBox_createdby.Text;
                eds.fi.CreationDateTime = DateTime.Parse(textBox_create_datetime.Text);

                eds.fi.Description = textBox_di_description.Text;

                eds.fi.EDSVersion = textBox_edsversionm.Text;


                eds.di.BaudRate_10 = checkBox_baud_10.Checked;
                eds.di.BaudRate_20 = heckBox_baud_20.Checked;
                eds.di.BaudRate_50 = heckBox_baud_50.Checked;
                eds.di.BaudRate_125 = heckBox_baud_125.Checked;
                eds.di.BaudRate_250 = heckBox_baud_250.Checked;
                eds.di.BaudRate_500 = heckBox_baud_500.Checked;
                eds.di.BaudRate_800 = heckBox_baud_800.Checked;
                eds.di.BaudRate_1000 = heckBox_baud_1000.Checked;

                eds.di.SimpleBootUpMaster = checkBox_boot_master.Checked;
                eds.di.SimpleBootUpSlave = checkBox_bootslave.Checked;
                eds.di.CompactPDO = checkBox_compactPDO.Checked;

                eds.di.GroupMessaging = checkBox_group_msg.Checked;
                eds.di.DynamicChannelsSupported = checkBox_dynamicchan.Checked;
                eds.di.LSS_Supported = checkBox_lss.Checked;
                eds.di.Granularity = Convert.ToByte(textBox_Gran.Text);

                //These are read only and auto calculated
                //textBox_rxpdos.Text = eds.di.NrOfRXPDO.ToString();
                //textBox_txpdos.Text = eds.di.NrOfTXPDO.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Update failed, reason :-\n" + ex.ToString());
            }

        }

     
    }
}

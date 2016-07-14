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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libEDSsharp;


namespace ODEditor
{
    public partial class DeviceODView : MyTabUserControl
    {
        public EDSsharp eds = null;

        ODentry selectedobject;
        ListViewItem selecteditem;
        ListViewItem selecteditemsub;

        public DeviceODView()
        {
            InitializeComponent();

            comboBox_datatype.Items.Add("");

            foreach (DataType foo in Enum.GetValues(typeof(DataType)))
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
 
        }

      
        private void button_save_changes_Click(object sender, EventArgs e)
        {
            if (selectedobject == null)
                return;

            //Allow everything to be updated and control what is allowed via enable/disable for the control

            selectedobject.parameter_name = textBox_name.Text;
            selectedobject.Description = textBox_description.Text;

            if (!(selectedobject.parent != null && selectedobject.parent.objecttype == ObjectType.ARRAY))
            {

                selectedobject.defaultvalue = textBox_defaultvalue.Text;
                selectedobject.TPDODetectCos = checkBox_COS.Checked;

                DataType dt = (DataType)Enum.Parse(typeof(DataType), comboBox_datatype.SelectedItem.ToString());
                selectedobject.datatype = dt;

                EDSsharp.AccessType at = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), comboBox_accesstype.SelectedItem.ToString());
                selectedobject.accesstype = at;

                selectedobject.PDOtype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), comboBox_pdomap.SelectedItem.ToString());

                selectedobject.AccessFunctionName = textBox_accessfunctionname.Text;
                selectedobject.AccessFunctionPreCode = textBox_precode.Text;
                selectedobject.Disabled = !checkBox_enabled.Checked;

                selectedobject.location = (StorageLocation)Enum.Parse(typeof(StorageLocation), comboBox_memory.SelectedItem.ToString());
            }

            updateselectedindexdisplay(selectedobject.index);
            validateanddisplaydata();

            //Update the PDO mappings as we may have new (or less) objects avaiable
            doUpdatePDOs();

        }

        public void updatedetailslist()
        {
            if (selectedobject == null)
                return;

            updateselectedindexdisplay(selectedobject.index);
        }

        public void validateanddisplaydata()
        {

            if (selectedobject == null)
                return;

            ODentry od = (ODentry)selectedobject;


            label_index.Text = string.Format("0x{0:x4}", od.index);
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
                    if (od.subobjects.Count >= 2)
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
            comboBox_pdomap.SelectedItem = od.PDOtype.ToString();

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

            checkBox_COS.Enabled = true;
            checkBox_enabled.Enabled = true;

            if (od.parent != null && ((od.parent.objecttype == ObjectType.ARRAY) || (od.parent.objecttype == ObjectType.REC && od.subindex == 0)))
            {
                textBox_defaultvalue.Enabled = false;
                comboBox_accesstype.Enabled = false;
                comboBox_datatype.Enabled = false;
                comboBox_objecttype.Enabled = false;
                comboBox_pdomap.Enabled = false;

                checkBox_enabled.Checked = false;
                textBox_accessfunctionname.Enabled = false;
                textBox_precode.Enabled = false;
                comboBox_memory.Enabled = false;

                checkBox_COS.Enabled = false;
                checkBox_enabled.Enabled = false;

                checkBox_COS.Checked = od.parent.TPDODetectCos;
                checkBox_enabled.Checked = !od.parent.Disabled;
               

            }
            else
            {
                textBox_defaultvalue.Enabled = true;
                comboBox_accesstype.Enabled = true;
                comboBox_datatype.Enabled = true;
                comboBox_objecttype.Enabled = false;
                comboBox_pdomap.Enabled = true;

                //checkBox_COS.Enabled = false;
                //checkBox_enabled.Enabled = false;

               
            }

            return;
        }



        ODentry selectedindexod = null;
        private void updateselectedindexdisplay(UInt16 index)
        {
            selectedindexod = eds.ods[index];
            updateselectedindexdisplay();
        }

        public void updateselectedindexdisplay()
        {
            
            listViewDetails.Items.Clear();

            if (selectedindexod == null)
                return;

            ODentry od = selectedindexod;

            if (od.objecttype == ObjectType.VAR)
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

                if (od.objecttype == ObjectType.ARRAY)
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

                foreach (KeyValuePair<UInt16, ODentry> kvp in od.subobjects)
                {
                    ODentry subod = kvp.Value;
                    int subindex = kvp.Key;

                    ListViewItem lvi2 = new ListViewItem(string.Format("{0:x}", subindex));
                    lvi2.SubItems.Add(subod.parameter_name);
                    lvi2.SubItems.Add(subod.objecttype.ToString());


                    if (subod.datatype == DataType.UNKNOWN || (od.objecttype == ObjectType.ARRAY && subod.subindex != 0))
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
                    lvi2.SubItems.Add(subod.PDOtype.ToString());

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

        private void list_mouseclick(ListView listview, MouseEventArgs e)
        {
            ListViewItem lvi = listview.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            if (e.Button == MouseButtons.Right)
            {

                if (listview.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    selecteditem = lvi;

                    ODentry od = (ODentry)lvi.Tag;
                    if (od.Disabled == true)
                    {
                        disableObjectToolStripMenuItem.Text = "Enable Object";
                    }
                    else
                    {
                        disableObjectToolStripMenuItem.Text = "Disable Object";
                    }

                    contextMenuStrip1.Show(Cursor.Position);
                }

                return;
            }

            updateselectedindexdisplay(idx);


            selectedobject = eds.ods[idx];
            validateanddisplaydata();
        }

        private void listView_MouseDown(ListView listview, MouseEventArgs e)
        {
            ListViewHitTestInfo HI = listview.HitTest(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                if (HI.Location == ListViewHitTestLocations.None)
                {
                    deleteObjectToolStripMenuItem.Enabled = false;
                    disableObjectToolStripMenuItem.Enabled = false;
                    contextMenuStrip1.Show(Cursor.Position);
                }
                else
                {
                    deleteObjectToolStripMenuItem.Enabled = true;
                    disableObjectToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void listView_optionalobjects_MouseClick(object sender, MouseEventArgs e)
        {
            list_mouseclick(listView_optional_objects, e);
        }


        private void listView_manufacture_objects_MouseDown(object sender, MouseEventArgs e)
        {
            listView_MouseDown(listView_manufacture_objects, e);
        }


        private void listView_manufacture_objects_MouseClick(object sender, MouseEventArgs e)
        {
            list_mouseclick(listView_manufacture_objects, e);
        }


        private void listView_optional_objects_MouseDown(object sender, MouseEventArgs e)
        {
            listView_MouseDown(listView_optional_objects, e);
        }

        private void listViewDetails_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listViewDetails.SelectedItems[0];

            if (listViewDetails.SelectedItems.Count == 0)
                return;

            selecteditemsub = lvi;

            ODentry od = (ODentry)lvi.Tag;

            if (e.Button == MouseButtons.Right)
            {
                ODentry parent = od;
                if (od.parent != null)
                    parent = od.parent;

                if (parent.objecttype == ObjectType.ARRAY || parent.objecttype == ObjectType.REC)
                {
                    if (od.subindex == 0 || od.parent == null)
                    {
                        contextMenu_array.Items[1].Enabled = false;
                    }
                    else
                    {
                        contextMenu_array.Items[1].Enabled = true;
                    }

                    if (listViewDetails.FocusedItem.Bounds.Contains(e.Location) == true)
                    {
                        contextMenu_array.Show(Cursor.Position);
                    }

                }
            }

            selectedobject = od;
            validateanddisplaydata();

        }

        public void populateindexlists()
        {

            if (eds == null)
                return;

            doUpdateDeviceInfo();
            doUpdatePDOs();

            listView_mandatory_objects.Items.Clear();
            listView_manufacture_objects.Items.Clear();
            listView_optional_objects.Items.Clear();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {


                UInt16 index = kvp.Value.index;
                ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}", kvp.Value.index));
                lvi.SubItems.Add(kvp.Value.parameter_name);
                lvi.Tag = kvp.Value;

                if (kvp.Value.Disabled == true)
                    lvi.ForeColor = Color.LightGray;

                if (index == 0x1000 || index == 0x1001 || index == 0x1018)
                {
                    listView_mandatory_objects.Items.Add(lvi);
                }
                else if (index >= 0x2000 && index < 0x6000)
                {
                    listView_manufacture_objects.Items.Add(lvi);
                }
                else
                {
                    listView_optional_objects.Items.Add(lvi);
                }

            }
        }

        private void addNewObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            NewIndex ni = new NewIndex(eds);

            if (ni.ShowDialog() == DialogResult.OK)
            {

                ODentry od = new ODentry();

                od.objecttype = ni.ot;
                od.index = ni.index;
                od.location = StorageLocation.RAM;
                od.defaultvalue = "";
                od.accesstype = EDSsharp.AccessType.rw;
                od.datatype = ni.dt;
                od.parameter_name = ni.name;

                if (od.objecttype == ObjectType.REC || od.objecttype == ObjectType.ARRAY)
                {
                    {
                        ODentry sod = new ODentry();

                        sod.objecttype = ObjectType.VAR;
                        sod.subindex = 0;
                        sod.index = ni.index;
                        sod.location = StorageLocation.RAM;
                        sod.defaultvalue = String.Format("{0}",ni.nosubindexes);
                        sod.accesstype = EDSsharp.AccessType.ro;
                        sod.datatype = DataType.UNSIGNED8;
                        sod.parent = od;
                        od.subobjects.Add(0, sod);
                    }

                    for (int p = 0; p < ni.nosubindexes; p++)
                    {
                        ODentry sod = new ODentry();

                        sod.objecttype = ObjectType.VAR;
                        sod.subindex = (UInt16)(p + 1);
                        sod.index = ni.index;
                        sod.location = StorageLocation.RAM;
                        sod.defaultvalue = "";
                        sod.accesstype = EDSsharp.AccessType.rw;
                        sod.datatype = ni.dt;
                        sod.parent = od;

                        od.subobjects.Add((ushort)(p + 1), sod);
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

            if (MessageBox.Show(string.Format("Really delete index 0x{0:x4} ?", od.index), "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

        private void addSubItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selecteditemsub.Tag != null)
            {
                ODentry od = (ODentry)selecteditemsub.Tag;

                DataType dt = DataType.UNKNOWN;

                if (od.parent.objecttype == ObjectType.ARRAY)
                {
                    ODentry newsub = new ODentry();
                    newsub.parent = od.parent;
                    newsub.datatype = DataType.UNKNOWN;
                    newsub.index = od.index;
                    newsub.objecttype = ObjectType.VAR;
                    newsub.subindex = (UInt16)od.parent.subobjects.Count;
                    od.parent.subobjects.Add((UInt16)(od.parent.subobjects.Count), newsub);

                    UInt16 def = Convert.ToUInt16(od.parent.subobjects[0].defaultvalue,16);
                    def++;
                    od.parent.subobjects[0].defaultvalue = def.ToString();


                }

                if (od.parent.objecttype == ObjectType.REC)
                {
                    dt = od.datatype;

                    NewIndex ni = new NewIndex(eds, dt, od.parent.objecttype);

                    if (ni.ShowDialog() == DialogResult.OK)
                    {
                        ODentry newsub = new ODentry();
                        newsub.parent = od.parent;
                        newsub.datatype = ni.dt;
                        newsub.index = od.index;
                        newsub.objecttype = ObjectType.VAR;
                        newsub.subindex = (UInt16)od.parent.subobjects.Count;
                        newsub.parameter_name = ni.name;

                        od.parent.subobjects.Add((UInt16)(od.parent.subobjects.Count), newsub);

                        UInt16 def = Convert.ToUInt16(od.parent.subobjects[0].defaultvalue,16);
                        def++;
                        od.parent.subobjects[0].defaultvalue = def.ToString();
                    }
                }

                updateselectedindexdisplay(selectedobject.index);
                validateanddisplaydata();

            }
        }

        private void removeSubItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selecteditemsub.Tag != null)
            {
                ODentry od = (ODentry)selecteditemsub.Tag;

                if (od.parent.objecttype == ObjectType.ARRAY)
                {
                    UInt16 count = Convert.ToUInt16(od.parent.subobjects[0].defaultvalue);
                    if (count > 0)
                        count--;
                    od.parent.subobjects[0].defaultvalue = count.ToString();
                }

                bool success = od.parent.subobjects.Remove(od.subindex);

                UInt16 countx = 0;

                SortedDictionary<UInt16, ODentry> newlist = new SortedDictionary<ushort, ODentry>();

                foreach (KeyValuePair<UInt16, ODentry> kvp in od.parent.subobjects)
                {
                    ODentry sub = kvp.Value;
                    sub.subindex = countx;
                    newlist.Add(countx, sub);
                    countx++;
                }

                od.parent.subobjects = newlist;

                updateselectedindexdisplay(selectedobject.index);
                validateanddisplaydata();
            }


        }


    }
}

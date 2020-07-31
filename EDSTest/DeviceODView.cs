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
using System.Reflection;
using System.Text.RegularExpressions;


namespace ODEditor
{

    public partial class DeviceODView : MyTabUserControl
    {
        public EDSsharp eds = null;

        ODentry selectedobject;
        ODentry lastselectedobject;
        ListViewItem selecteditem;
        ListViewItem selecteditemsub;

        ListView selectedList;

        public DeviceODView()
        {
            InitializeComponent();

 

            foreach (DataType foo in Enum.GetValues(typeof(DataType)))
            {
                comboBox_datatype.Items.Add(foo.ToString());
            }

            foreach (ObjectType foo in Enum.GetValues(typeof(ObjectType)))
            {
                comboBox_objecttype.Items.Add(foo.ToString());
            }

            foreach (EDSsharp.AccessType foo in Enum.GetValues(typeof(EDSsharp.AccessType)))
            {
                comboBox_accesstype.Items.Add(foo.ToString());
            }

            comboBox_accesstype.Items.Add("0x1003 rw/ro");
            comboBox_accesstype.Items.Add("0x1010 const/rw");
            comboBox_accesstype.Items.Add("0x1010 const/ro");

            comboBox_memory.Items.Add("Add...");

            comboBox_pdomap.Items.Add("no");
            comboBox_pdomap.Items.Add("optional");

            listView_mandatory_objects.DoubleBuffering(true);
            listView_manufacture_objects.DoubleBuffering(true);
            listView_optional_objects.DoubleBuffering(true);
            listViewDetails.DoubleBuffering(true);

            foreach(Control c in splitContainer4.Panel2.Controls)
            {
                if (c is CheckBox)
                {
                    ((CheckBox)c).CheckedChanged += DataDirty;
                }
                else
                {
                    c.TextChanged += DataDirty;
                }
            }

            registerCN(splitContainer4.Panel2.Controls);
            registerCN(groupBox1.Controls);

        }


        void registerCN(ControlCollection container)
        {
            foreach (Control c in container)
            {
                if (c is CheckBox)
                {
                    ((CheckBox)c).CheckedChanged += DataDirty;
                }
                else
                {
                    c.TextChanged += DataDirty;
                }
            }

        }

        bool updating = false;

        private void DataDirty(object sender, EventArgs e)
        {
            if (updating == true)
                return;

            button_save_changes.BackColor = Color.Red;


        }

        private void button_save_changes_Click(object sender, EventArgs e)
        {
            if (selectedobject == null)
                return;

            eds.Dirty = true;

            button_save_changes.BackColor = default(Color);

            //Allow everything to be updated and control what is allowed via enable/disable for the control

            selectedobject.parameter_name = textBox_name.Text;
            selectedobject.Description = textBox_description.Text;
            selectedobject.defaultvalue = textBox_defaultvalue.Text;
            selectedobject.denotation = textBox_denotation.Text;
            selectedobject.HighLimit = textBox_highvalue.Text;
            selectedobject.LowLimit = textBox_lowvalue.Text;
            selectedobject.actualvalue = textBox_actualvalue.Text;

            if (!(selectedobject.parent != null && selectedobject.parent.objecttype == ObjectType.ARRAY))
            {

                selectedobject.defaultvalue = textBox_defaultvalue.Text;
                selectedobject.TPDODetectCos = checkBox_COS.Checked;
                selectedobject.HighLimit = textBox_highvalue.Text;
                selectedobject.LowLimit = textBox_lowvalue.Text;
                selectedobject.actualvalue = textBox_actualvalue.Text;
                DataType dt = (DataType)Enum.Parse(typeof(DataType), comboBox_datatype.SelectedItem.ToString());
                selectedobject.datatype = dt;


                switch(comboBox_accesstype.SelectedItem.ToString())
                {
                    case "0x1003 rw/ro":
                        selectedobject.accesstype = EDSsharp.AccessType.rw;

                        foreach(KeyValuePair <UInt16,ODentry> kvp in selectedobject.subobjects)
                        {
                            ODentry od = kvp.Value;
                            UInt16 subindex = kvp.Key;

                            if (subindex == 0)
                                od.accesstype = EDSsharp.AccessType.rw;
                            else
                                od.accesstype = EDSsharp.AccessType.ro;

                        }

                        break;

                    case "0x1010 const/rw":
                        foreach (KeyValuePair<UInt16, ODentry> kvp in selectedobject.subobjects)
                        {
                            ODentry od = kvp.Value;
                            UInt16 subindex = kvp.Key;

                            if (subindex == 0)
                                od.accesstype = EDSsharp.AccessType.@const;
                            else
                                od.accesstype = EDSsharp.AccessType.rw;
                        }
                        break;

                    case "0x1010 const/ro":
                        foreach (KeyValuePair<UInt16, ODentry> kvp in selectedobject.subobjects)
                        {
                            ODentry od = kvp.Value;
                            UInt16 subindex = kvp.Key;
                            if (subindex == 0)
                                od.accesstype = EDSsharp.AccessType.@const;
                            else
                                od.accesstype = EDSsharp.AccessType.ro;
                        }
                        break;

                    default:
                        EDSsharp.AccessType at = (EDSsharp.AccessType)Enum.Parse(typeof(EDSsharp.AccessType), comboBox_accesstype.SelectedItem.ToString());
                        selectedobject.accesstype = at;
                        break;

                }
                
         

                selectedobject.PDOtype = (PDOMappingType)Enum.Parse(typeof(PDOMappingType), comboBox_pdomap.SelectedItem.ToString());

                selectedobject.Disabled = !checkBox_enabled.Checked;

                selectedobject.StorageLocation = comboBox_memory.SelectedItem.ToString();

            }

            if(selectedobject.parent == null && selectedobject.objecttype == ObjectType.ARRAY)
            {
                // Propagate changes through sub objects
                // We only really need to do this for PDOMapping to fix bug #13 see report
                // on git hub for discussion why other parameters are not propagated here
                // tl;dr; Limitations of CANopenNode object dictionary perms for sub array objects

                foreach (KeyValuePair<UInt16,ODentry>kvp in selectedobject.subobjects)
                {
                    ODentry subod = kvp.Value;
                    UInt16 subindex = kvp.Key;

                    subod.PDOtype = selectedobject.PDOtype;
                    switch(comboBox_accesstype.SelectedItem.ToString())
                    {
                        case "0x1003 rw/ro":
                        case "0x1010 const/rw":
                        case "0x1010 const/ro":
                            break;

                        default:
                            if (subindex != 0)
                                subod.accesstype = selectedobject.accesstype;
                            break;

                    }

                    if (kvp.Key != 0)
                        subod.datatype = selectedobject.datatype;
                }
            }

            //If we edit a parent REC object we also need to change the storage location of subobjects
            //this does occur implicitly anyway and it also occurs during load of file but the GUI was displaying
            //incorrect data in the shaded combo box item for storage location
            if (selectedobject.parent == null && selectedobject.objecttype == ObjectType.REC)
            {
                foreach (KeyValuePair<UInt16, ODentry> kvp in selectedobject.subobjects)
                {
                    ODentry subod = kvp.Value;
                    subod.StorageLocation = selectedobject.StorageLocation;
                }
            }


            updateselectedindexdisplay(selectedobject.Index, currentmodule);
            validateanddisplaydata();

            populateindexlists(); 

        }

        public void updatedetailslist()
        {
            if (selectedobject == null)
                return;

            updateselectedindexdisplay(selectedobject.Index, currentmodule);
        }

        public void validateanddisplaydata()
        {


            if (selectedobject == null)
                return;

            lastselectedobject = selectedobject;

            updating = true;


            ODentry od = (ODentry)selectedobject;


            if (currentmodule == 0)
            {
                label_index.Text = string.Format("0x{0:x4}", od.Index);
            }
            else
            {
                label_index.Text = string.Format("0x{0:x4} in module {1} -- {2}", od.Index,currentmodule,eds.modules[currentmodule].mi.ProductName);
            }

            textBox_name.Text = od.parameter_name;
            textBox_denotation.Text = od.denotation;

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
                        // BUG #70 Select the first non subindex count entry, note this may not be key[1] so we are using an ordinal hack
                        // to retrieve it.
                        // Whilst this will likely work forever, there is nothing stopping the implementation from being
                        // changed in the future and causing your code which uses this to break in horrible ways. You have been warned

                        comboBox_datatype.SelectedItem = od.subobjects.ElementAt(1).Value.datatype.ToString();
                    }

                }
                else
                {
                    if(od.parent!=null)
                        comboBox_datatype.SelectedItem = od.parent.datatype ;
                }
            }

            //Bug#25 set the combo box text to be the same as the selected item as this does not happen automatically
            //when the combo box is disabled
            if (comboBox_datatype.SelectedItem!=null)
                comboBox_datatype.Text = comboBox_datatype.SelectedItem.ToString();

            comboBox_objecttype.SelectedItem = od.objecttype.ToString();

            if (od.Description == null)
                textBox_description.Text = "";
            else
                textBox_description.Text = Regex.Replace(od.Description, "(?<!\r)\n", "\r\n");

            comboBox_pdomap.SelectedItem = od.PDOtype.ToString();

            checkBox_COS.Checked = od.TPDODetectCos;
          
            checkBox_enabled.Checked = !od.Disabled;

            comboBox_memory.SelectedItem = od.StorageLocation;

            checkBox_enabled.Enabled = true;
            comboBox_memory.Enabled = true;

            textBox_defaultvalue.Enabled = true;
            textBox_actualvalue.Enabled = true;
            textBox_highvalue.Enabled = true;
            textBox_lowvalue.Enabled = true;


            comboBox_accesstype.Enabled = true;
            comboBox_datatype.Enabled = true;
            comboBox_objecttype.Enabled = false;
            comboBox_pdomap.Enabled = true;

            textBox_defaultvalue.Text = od.defaultvalue;

            textBox_actualvalue.Text = od.actualvalue;
            textBox_highvalue.Text = od.HighLimit;
            textBox_lowvalue.Text = od.LowLimit;

            checkBox_COS.Enabled = true;
            checkBox_enabled.Enabled = true;

            if (od.parent == null)
            {
                //if we are a parent REC then 
                if (od.objecttype == ObjectType.REC)
                {
                    comboBox_accesstype.Enabled = false;
                    comboBox_pdomap.Enabled = false;
                    checkBox_COS.Enabled = false;
                    comboBox_datatype.Enabled = false;
                    textBox_defaultvalue.Enabled = false;
                    textBox_actualvalue.Enabled = false;
                    textBox_highvalue.Enabled = false;
                    textBox_lowvalue.Enabled = false;
                }

                updating = false;
                return; //nothing else to do at this point
            }

            //protect everything as default
            textBox_defaultvalue.Enabled = false;
            comboBox_accesstype.Enabled = false;
            comboBox_datatype.Enabled = false;
            comboBox_objecttype.Enabled = false;
            comboBox_pdomap.Enabled = false;
            checkBox_enabled.Checked = false;
            comboBox_memory.Enabled = false;
            checkBox_COS.Enabled = false;
            checkBox_enabled.Enabled = false;
          
            checkBox_enabled.Checked = !od.parent.Disabled;

            if (od.parent.objecttype == ObjectType.ARRAY && od.Subindex != 0)
            {
                textBox_defaultvalue.Enabled = true;
                checkBox_COS.Checked = od.parent.TPDODetectCos;

            }

            if (od.parent.objecttype == ObjectType.REC && od.Subindex != 0)
            {
                textBox_defaultvalue.Enabled = true;
                comboBox_datatype.Enabled = true;
                comboBox_pdomap.Enabled = true;
                comboBox_accesstype.Enabled = true;
                checkBox_COS.Enabled = true;

            }

            if (od.parent.objecttype == ObjectType.REC &&
                ((od.parent.Index >=0x1600 && od.parent.Index <= 0x17ff) || (od.parent.Index >= 0x1A00 && od.parent.Index <= 0x1Bff)
                || (od.parent.Index >= 0x1381 && od.parent.Index <= 0x13C0)) &&
                od.Subindex == 0)
            {
                //We are allowed to edit the no sub objects for the PDO mappings as its a requirement to support dynamic PDOs

                textBox_defaultvalue.Enabled = true;
                comboBox_accesstype.Enabled = true;
            }

            updating = false;

            return;
        }



        ODentry selectedindexod = null;
        UInt16 currentmodule = 0;

        private void updateselectedindexdisplay(UInt16 index,UInt16 mod)
        {

         
            selectedindexod = getOD(index,mod);
            currentmodule = mod;

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


                    if (subod.datatype == DataType.UNKNOWN || (od.objecttype == ObjectType.ARRAY && subindex != 0))
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

                    //fix me ??
                    lvi2.SubItems.Add(subod.PDOtype.ToString());

                    lvi2.Tag = subod;

                    listViewDetails.Items.Add(lvi2);

                }

            }

        }

        private void listView_mandatory_objects_MouseClick(object sender, MouseEventArgs e)
        {
            selectedList = listView_mandatory_objects;
            ListViewItem lvi = listView_mandatory_objects.SelectedItems[0];

            if (checkdirty())
                return;

            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);
            updateselectedindexdisplay(idx, currentmodule);

            selectedobject = eds.ods[idx];
            validateanddisplaydata();

            listView_mandatory_objects.HideSelection = false;
            listView_manufacture_objects.HideSelection = true;
            listView_optional_objects.HideSelection = true;
        }

        private void list_mouseclick(ListView listview, MouseEventArgs e)
        {
            selectedList = listview;
            if (listview.SelectedItems.Count == 0)
                return;

            if (checkdirty())
                return;

            deleteObjectToolStripMenuItem.Text = listview.SelectedItems.Count > 1 ? "&Delete Objects" : "&Delete Object";

            ListViewItem lvi = listview.SelectedItems[0];

            currentmodule = 0;

            UInt16 idx;
            if (lvi.Text.Contains('('))
            {
                int i = 1+lvi.Text.IndexOf(' ');
                string id = lvi.Text.Substring(i, lvi.Text.Length - i);
                idx = Convert.ToUInt16(id, 16);

                string mods = lvi.Text.Substring(1, i - 3);

                currentmodule = Convert.ToUInt16(mods, 10);

            }
            else
            {
                idx = Convert.ToUInt16(lvi.Text, 16);
            }

            if (e.Button == MouseButtons.Right)
            {
                if (currentmodule != 0)
                    return;

                if (listview.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    selecteditem = lvi;

                    ODentry od = (ODentry)lvi.Tag;

                    string objString = listview.SelectedItems.Count > 1 ? "Objects" : "Object";
                    
                    string objEnableString = od.Disabled ? "&Enable" : "Disabl&e";
                    disableObjectToolStripMenuItem.Text = string.Format("{0} {1}", objEnableString, objString);
                    contextMenuStrip1.Show(Cursor.Position);
                }

                return;
            }

            updateselectedindexdisplay(idx, currentmodule);

            selectedobject = getOD(idx, currentmodule);

            validateanddisplaydata();

            listView_mandatory_objects.HideSelection = true;
            listView_manufacture_objects.HideSelection = true;
            listView_optional_objects.HideSelection = true;
            listview.HideSelection = false;
        }

        private void listView_MouseDown(ListView listview, MouseEventArgs e)
        {
            selectedList = listview;

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
            selectedList = listViewDetails;
            ListViewItem lvi = listViewDetails.SelectedItems[0];

            if (listViewDetails.SelectedItems.Count == 0)
                return;

            if (checkdirty())
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
                    if (od.Subindex == 0 || od.parent == null)
                    {
                        contextMenu_array.Items[2].Enabled = false;
                    }
                    else
                    {
                        contextMenu_array.Items[2].Enabled = true;
                    }

                    //Only show the special subindex adjust menu for subindex 0 of arrays
                    if((od.parent!=null) && (parent.objecttype == ObjectType.ARRAY) && (od.Subindex==0))
                    {
                        contextMenu_array.Items[0].Enabled = true;
                        contextMenu_array.Items[0].Visible = true;
                    }
                    else
                    {
                        contextMenu_array.Items[0].Enabled = false;
                        contextMenu_array.Items[0].Visible = false;
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

        public void populatememorytypes()
        {
            if (eds == null)
                return;

            foreach (string location in eds.storageLocation)
            {
                if (location == "Unused")
                {
                    continue;
                }

                /* add string to the second to last position (before "add...") */
                /* Ensuring that it does not already exist */
                if (!comboBox_memory.Items.Contains(location.ToString()))
                {
                    comboBox_memory.Items.Insert(comboBox_memory.Items.Count - 1, location.ToString());
                }
            }
        }
        
        public void populateindexlists()
        {

            if (eds == null)
                return;

            doUpdateDeviceInfo();
            doUpdatePDOs();

            /* save scroll positions */
            int listview_mandatory_position = 0;
            int listview_manufacture_position = 0;
            int listview_optional_position = 0;

            if (listView_mandatory_objects.TopItem != null)
                listview_mandatory_position = listView_mandatory_objects.TopItem.Index;
            if (listView_manufacture_objects.TopItem != null)
                listview_manufacture_position = listView_manufacture_objects.TopItem.Index;
            if (listView_optional_objects.TopItem != null)
                listview_optional_position = listView_optional_objects.TopItem.Index;

            /* prevent flickering */
            listView_mandatory_objects.BeginUpdate();
            listView_manufacture_objects.BeginUpdate();
            listView_optional_objects.BeginUpdate();

            listView_mandatory_objects.Items.Clear();
            listView_manufacture_objects.Items.Clear();
            listView_optional_objects.Items.Clear();

            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {


                UInt16 index = kvp.Value.Index;
                ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}", kvp.Value.Index));
                lvi.SubItems.Add(kvp.Value.parameter_name);
                lvi.Tag = kvp.Value;
                if (selectedobject != null)
                    if (index == selectedobject.Index)
                        lvi.Selected = true;

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


          
            foreach (libEDSsharp.Module m in eds.modules.Values)
            {
                foreach (KeyValuePair<UInt16, ODentry> kvp in m.modulesubext)
                {

                
                    UInt16 index = kvp.Value.Index;
                    ListViewItem lvi = new ListViewItem(string.Format("({0}) 0x{1:x4}", m.moduleindex,kvp.Value.Index));
                    lvi.SubItems.Add(kvp.Value.parameter_name);
                    lvi.Tag = kvp.Value;
                    if (selectedobject != null)
                        if (index == selectedobject.Index)
                            lvi.Selected = true;

                    lvi.ForeColor = Color.Blue;

                    if (index >= 0x2000 && index < 0x6000)
                    {
                        listView_manufacture_objects.Items.Add(lvi);
                    }
                    else
                    {
                        listView_optional_objects.Items.Add(lvi);
                    }

                }
            }




            listView_mandatory_objects.EndUpdate();
            listView_manufacture_objects.EndUpdate();
            listView_optional_objects.EndUpdate();

            /* reset scroll position and selection */
            if (listview_mandatory_position != 0 && listView_mandatory_objects.Items.Count > 0)
                listView_mandatory_objects.TopItem = listView_mandatory_objects.Items[listview_mandatory_position];
            if (listview_manufacture_position != 0 && listView_manufacture_objects.Items.Count > 0)
                listView_manufacture_objects.TopItem = listView_manufacture_objects.Items[listview_manufacture_position];
            if (listview_optional_position != 0 && listView_optional_objects.Items.Count > 0)
                listView_optional_objects.TopItem = listView_optional_objects.Items[listview_optional_position];


        }

        private void addNewObjectFromDialog()
        {
            NewIndex ni = new NewIndex(eds);

            if (ni.ShowDialog() == DialogResult.OK)
            {

                eds.Dirty = true;

                ODentry od = new ODentry();

                od.objecttype = ni.ot;
                od.Index = ni.index;
                od.StorageLocation = "RAM";
                od.defaultvalue = "";
                od.accesstype = EDSsharp.AccessType.rw;
                od.datatype = ni.dt;
                od.parameter_name = ni.name;

                if (od.objecttype == ObjectType.REC || od.objecttype == ObjectType.ARRAY)
                {
                    {
                        ODentry sod = new ODentry();

                        sod.objecttype = ObjectType.VAR;
                        sod.parent = od;
                        sod.StorageLocation = "RAM";
                        sod.defaultvalue = String.Format("{0}", ni.nosubindexes);
                        sod.accesstype = EDSsharp.AccessType.ro;
                        sod.datatype = DataType.UNSIGNED8;

                        sod.parameter_name = "max sub-index";


                        od.subobjects.Add(0, sod);
                    }

                    for (int p = 0; p < ni.nosubindexes; p++)
                    {
                        ODentry sod = new ODentry();

                        sod.objecttype = ObjectType.VAR;
                        sod.parent = od;
                        sod.StorageLocation = "RAM";
                        sod.defaultvalue = "";
                        sod.accesstype = EDSsharp.AccessType.rw;
                        sod.datatype = ni.dt;

                        od.subobjects.Add((ushort)(p + 1), sod);
                    }

                }

                eds.ods.Add(od.Index, od);

                //Now switch to it as well Bug #26

                updateselectedindexdisplay(od.Index, currentmodule);
                selectedobject = eds.ods[od.Index];
                validateanddisplaydata();


                populateindexlists();
            }
        }

        private void addNewObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewObjectFromDialog();
        }

        private void disableSelectedObjects(ListView objectList)
        {
            ListView.SelectedListViewItemCollection selectedItems = objectList.SelectedItems;
            if (selectedItems.Count > 0)
            {
                foreach (ListViewItem item in selectedItems)
                {
                    ODentry od = (ODentry)item.Tag;

                    eds.Dirty = true;
                    od.Disabled = !od.Disabled;
                    populateindexlists();
                }

                
            }
        }

        private void deleteSelectedObjects(ListView objectList)
        {
            ListView.SelectedListViewItemCollection selectedItems = objectList.SelectedItems;
            if (selectedItems.Count > 0)
            {
                DialogResult confirmDelete;

                if(selectedItems.Count == 1)
                {
                    confirmDelete = MessageBox.Show("Do you really want to delete the selected item?", "Are you sure?", MessageBoxButtons.YesNo);
                }
                else
                {
                    confirmDelete = MessageBox.Show(string.Format("Do you really want to delete the selected {0} items?", selectedItems.Count), "Are you sure?", MessageBoxButtons.YesNo);
                }

                if (confirmDelete == DialogResult.Yes)
                {


                    foreach (ListViewItem item in selectedItems)
                    {
                        selecteditem = item;
                        ODentry od = (ODentry)item.Tag;

                        //Check object is not used in a PDO before deleting


                        for (UInt16 idx = 0x1600; idx < 0x1a00 + 0x01ff; idx++)
                        {

                            //Cheat as we want to only map 1600-17FF and 1a00-1bff
                            if (idx == 0x1800)
                                idx = 0x1a00;

                            if (eds.ods.ContainsKey(idx))
                            {
                                ODentry pdood = eds.ods[idx];
                                for (byte subno = 1; subno < pdood.Nosubindexes; subno++)
                                {
                                    try
                                    {
                                        UInt16 odindex = Convert.ToUInt16(pdood.subobjects[subno].defaultvalue.Substring(0, 4), 16);
                                        if (odindex == od.Index)
                                        {
                                            MessageBox.Show(string.Format("Cannot delete OD entry it is mapped in PDO {0:4x}", pdood.Index));
                                            return;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        //Failed to parse the PDO
                                    }
                                }

                                eds.Dirty = true;
                                if (currentmodule == 0)
                                {
                                    eds.ods.Remove(od.Index);
                                }

                                populateindexlists();
                            }
                        }
                    }
                }
            }
        }

        private void deleteObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteSelectedObjects(selectedList);
        }

        private void deleteSelectedIndexes(ListView objectList, bool shiftUp)
        {
            ListView.SelectedListViewItemCollection selectedItems = objectList.SelectedItems;
            if (selectedItems.Count > 0)
            {
                if (shiftUp == false)
                {
                    foreach (ListViewItem item in selectedItems)
                    {
                        if (item.Tag != null)
                        {
                            ODentry od = (ODentry)item.Tag;
                            if (od.parent != null)
                                od = od.parent;

                            if (od.objecttype == ObjectType.ARRAY)
                            {
                                ODentry newsub = new ODentry();
                                newsub.parent = od;
                                newsub.datatype = od.datatype;
                                newsub.accesstype = od.accesstype;
                                newsub.PDOtype = od.PDOtype;
                                newsub.objecttype = ObjectType.VAR;
                                od.subobjects.Add((UInt16)(od.subobjects.Count), newsub);

                                UInt16 def = EDSsharp.ConvertToUInt16(od.subobjects[0].defaultvalue);

                                def++;
                                od.subobjects[0].defaultvalue = def.ToString();


                            }

                            if (od.objecttype == ObjectType.REC)
                            {
                                DataType dt = od.datatype;

                                NewIndex ni = new NewIndex(eds, dt, od.objecttype, od);

                                if (ni.ShowDialog() == DialogResult.OK)
                                {
                                    ODentry newsub = new ODentry();
                                    newsub.parent = od;
                                    newsub.datatype = ni.dt;
                                    newsub.accesstype = od.accesstype;
                                    newsub.PDOtype = od.PDOtype;
                                    newsub.objecttype = ObjectType.VAR;
                                    newsub.parameter_name = ni.name;

                                    od.subobjects.Add((UInt16)(od.subobjects.Count), newsub);

                                    UInt16 def = EDSsharp.ConvertToUInt16(od.subobjects[0].defaultvalue);
                                    def++;
                                    od.subobjects[0].defaultvalue = def.ToString();
                                }
                            }

                            eds.Dirty = true;
                            updateselectedindexdisplay(selectedobject.Index, currentmodule);
                            validateanddisplaydata();

                        }

                    }
                }
                else
                {
                    foreach (ListViewItem item in selectedItems)
                    {
                        if (item.Tag != null)
                        {
                            ODentry od = (ODentry)item.Tag;

                            if (od.parent.objecttype == ObjectType.ARRAY || od.parent.objecttype == ObjectType.REC)
                            {
                                UInt16 count = EDSsharp.ConvertToUInt16(od.parent.subobjects[0].defaultvalue);
                                if (count > 0)
                                    count--;
                                od.parent.subobjects[0].defaultvalue = count.ToString();
                            }

                            bool success = od.parent.subobjects.Remove(od.Subindex);

                            
                            UInt16 countx = 0;

                            SortedDictionary<UInt16, ODentry> newlist = new SortedDictionary<ushort, ODentry>();

                            foreach (KeyValuePair<UInt16, ODentry> kvp in od.parent.subobjects)
                            {
                                ODentry sub = kvp.Value;
                                newlist.Add(countx, sub);
                                countx++;
                            }

                            od.parent.subobjects = newlist;
                            

                            eds.Dirty = true;
                            updateselectedindexdisplay(selectedobject.Index, currentmodule);
                            validateanddisplaydata();
                        }
                    }
                }
            }
        }

        private void disableObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disableSelectedObjects(selectedList);
        }

        private void addNewSubItemFromDialog()
        {
            if (selecteditemsub.Tag != null)
            {
                ODentry od = (ODentry)selecteditemsub.Tag;

                if (od.parent != null)
                    od = od.parent;

                if (od.objecttype == ObjectType.ARRAY)
                {
                    ODentry newsub = new ODentry();
                    newsub.parent = od;
                    newsub.datatype = od.datatype;
                    newsub.accesstype = od.accesstype;
                    newsub.PDOtype = od.PDOtype;
                    newsub.objecttype = ObjectType.VAR;
                    od.subobjects.Add((UInt16)(od.subobjects.Count), newsub);

                    UInt16 def = EDSsharp.ConvertToUInt16(od.subobjects[0].defaultvalue);

                    def++;
                    od.subobjects[0].defaultvalue = def.ToString();


                }

                if (od.objecttype == ObjectType.REC)
                {
                    DataType dt = od.datatype;

                    NewIndex ni = new NewIndex(eds, dt, od.objecttype, od);

                    if (ni.ShowDialog() == DialogResult.OK)
                    {
                        ODentry newsub = new ODentry();
                        newsub.parent = od;
                        newsub.datatype = ni.dt;
                        newsub.accesstype = od.accesstype;
                        newsub.PDOtype = od.PDOtype;
                        newsub.objecttype = ObjectType.VAR;
                        newsub.parameter_name = ni.name;

                        od.subobjects.Add((UInt16)(od.subobjects.Count), newsub);

                        UInt16 def = EDSsharp.ConvertToUInt16(od.subobjects[0].defaultvalue);
                        def++;
                        od.subobjects[0].defaultvalue = def.ToString();
                    }
                }

                eds.Dirty = true;
                updateselectedindexdisplay(selectedobject.Index, currentmodule);
                validateanddisplaydata();

            }
        }

        private void addSubItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewSubItemFromDialog();
        }

        private void removeSubItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //deleteSelectedIndexes(selectedList, true);
            if (selecteditemsub.Tag != null)
            {
                ODentry od = (ODentry)selecteditemsub.Tag;

                if (od.parent.objecttype == ObjectType.ARRAY || od.parent.objecttype == ObjectType.REC)
                {
                    UInt16 count = EDSsharp.ConvertToUInt16(od.parent.subobjects[0].defaultvalue);
                    if (count > 0)
                        count--;
                    od.parent.subobjects[0].defaultvalue = count.ToString();
                }

                bool success = od.parent.subobjects.Remove(od.Subindex);

                UInt16 countx = 0;

                SortedDictionary<UInt16, ODentry> newlist = new SortedDictionary<ushort, ODentry>();

                foreach (KeyValuePair<UInt16, ODentry> kvp in od.parent.subobjects)
                {
                    ODentry sub = kvp.Value;
                    newlist.Add(countx, sub);
                    countx++;
                }

                od.parent.subobjects = newlist;

                eds.Dirty = true;
                updateselectedindexdisplay(selectedobject.Index, currentmodule);
                validateanddisplaydata();
            }
        }

        private void listView_optional_objects_SelectedIndexChanged(object sender, EventArgs e)
        {
            list_mouseclick(listView_optional_objects, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        }

        private void listView_mandatory_objects_SelectedIndexChanged(object sender, EventArgs e)
        {
            list_mouseclick(listView_mandatory_objects, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        }

        private void listView_manufacture_objects_SelectedIndexChanged(object sender, EventArgs e)
        {
            list_mouseclick(listView_manufacture_objects, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        }

        private void changeMaxSubIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Change the max subindex, it is allowed to have a different max subindex to the physical array size
            //as depending on implementation it might not be a simple array behind the scenes. Even 0x1010,0x1011 
            //do this on their implementation in CANopenNode

            if (selecteditemsub.Tag != null)
            {
                ODentry od = (ODentry)selecteditemsub.Tag;

                if (od.parent.objecttype == ObjectType.ARRAY && od.Subindex==0)
                {
                    MaxSubIndexFrm frm = new MaxSubIndexFrm(od.Nosubindexes);

                    if(frm.ShowDialog()==DialogResult.OK)
                    {
                        od.defaultvalue = string.Format("0x{0:x2}",frm.maxsubindex);
                        updateselectedindexdisplay(selectedobject.Index, currentmodule);
                        validateanddisplaydata();
                    }
                }
            }


            
        }

        private void listViewDetails_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listViewDetails.SelectedItems.Count == 0)
                return;

            if (checkdirty())
                return;

            ListViewItem lvi = listViewDetails.SelectedItems[0];

            selecteditemsub = lvi;
            selectedobject = (ODentry)lvi.Tag;
            validateanddisplaydata();
        }

        private bool checkdirty()
        {

            if (button_save_changes.BackColor == Color.Red)
            {
                if (button_save_changes.BackColor == Color.Red)
                {
                    if (MessageBox.Show(String.Format("Unsaved changes on Index 0x{0:x4}/{1:x2}\nDo you wish to change objects and loose your changes", lastselectedobject.Index, lastselectedobject.Subindex), "Unsaved changes",MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return true;
                    }
                    else
                    {
                        button_save_changes.BackColor = default(Color);
                    }

                }
            }

            return false;
        }

        private void comboBox_memory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_memory.SelectedItem!=null && comboBox_memory.SelectedItem.ToString() == "Add...")
            {
                NewMemoryType memory = new NewMemoryType();
                if (memory.ShowDialog() == DialogResult.OK)
                {
                    if (comboBox_memory.FindStringExact(memory.name) == -1)
                    {
                        /* add string to the second to last position (before "add...") */
                        comboBox_memory.Items.Insert(comboBox_memory.Items.Count - 1, memory.name);
                        /* add new memory location to eds back end */
                        eds.storageLocation.Add(memory.name);
                    }
                }
            }
        }

        private ODentry getOD(UInt16 index, UInt16 selectedmodule)
        {
            if (selectedmodule == 0)
            {
                if (eds.ods.ContainsKey(index))
                {
                    return eds.ods[index];
                }
            }
            else
            {

                if (eds.modules.ContainsKey(selectedmodule))
                {
                    return eds.modules[selectedmodule].modulesubext[index];
                }
  
            }

            return null; ;

        }

        private void removeSubItemleaveGapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //deleteSelectedIndexes(selectedList, false);
            if (selecteditemsub.Tag != null)
            {
                ODentry od = (ODentry)selecteditemsub.Tag;

                if (od.parent.objecttype == ObjectType.ARRAY || od.parent.objecttype == ObjectType.REC)
                {
                    UInt16 count = EDSsharp.ConvertToUInt16(od.parent.subobjects[0].defaultvalue);
                    if (count > 0)
                        count--;
                    od.parent.subobjects[0].defaultvalue = count.ToString();
                }

                bool success = od.parent.subobjects.Remove(od.Subindex);

                /*
                UInt16 countx = 0;
                SortedDictionary<UInt16, ODentry> newlist = new SortedDictionary<ushort, ODentry>();
                foreach (KeyValuePair<UInt16, ODentry> kvp in od.parent.subobjects)
                {
                    ODentry sub = kvp.Value;
                    newlist.Add(countx, sub);
                    countx++;
                }
                
                od.parent.subobjects = newlist;
                */

                eds.Dirty = true;
                updateselectedindexdisplay(selectedobject.Index, currentmodule);
                validateanddisplaydata();
            }


        }

        private void selectAllItemsInList(ListView selectedList)
        {
            foreach(ListViewItem item in selectedList.Items)
            {
                item.Selected = true;
            }
        }

        private void listView_KeyDown_Handler(ListView selectedList, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case (Keys.Delete):
                    deleteSelectedObjects(selectedList);
                    break;
                case (Keys.A):
                    if (e.Modifiers == Keys.Control)
                    {
                        selectAllItemsInList(selectedList);
                    }
                    break;
                default:
                    break;
            }
        }

        //specialized KeyDown event handler for the object dictionary lists. 
        //Usage: call this function before the general list KeyDown handler. If this function returns true, the keydown event was handled and the generic one should not be called (this allows default behavior to be overridden)
        private bool listView_Objects_KeyDown_Handler(ListView selectedList, KeyEventArgs e)
        {
            bool keyDownHandled = false;
            switch (e.KeyCode)
            {
                case (Keys.N):
                    if (e.Modifiers == (Keys.Control | Keys.Shift))
                    {
                        keyDownHandled = true;
                        addNewObjectFromDialog();
                    }
                    break;
                default:
                    break;
            }
            return keyDownHandled;
        }

        private bool listView_Subindexes_KeyDown_Handler(ListView selectedList, KeyEventArgs e)
        {
            bool keyDownHandled = false;
            switch (e.KeyCode)
            {
                case (Keys.N):
                    if (e.Modifiers == (Keys.Control | Keys.Shift))
                    {
                        keyDownHandled = true;
                        addNewSubItemFromDialog();
                    }
                    break;
                default:
                    break;
            }
            return keyDownHandled;
        }

        private void listView_manufacture_objects_KeyDown(object sender, KeyEventArgs e)
        {
            if(listView_Objects_KeyDown_Handler(listView_manufacture_objects, e) == false)
            {
                listView_KeyDown_Handler(listView_manufacture_objects, e);
            }
            
        }

        private void listView_optional_objects_KeyDown(object sender, KeyEventArgs e)
        {
            if (listView_Objects_KeyDown_Handler(listView_optional_objects, e) == false)
            {
                listView_KeyDown_Handler(listView_optional_objects, e);
            }
        }

        private void listView_mandatory_objects_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode != Keys.Delete)
            {
                listView_KeyDown_Handler(listView_mandatory_objects, e);
            }
        }

        private void listViewDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (listView_Subindexes_KeyDown_Handler(listView_optional_objects, e) == false)
            {
                listView_KeyDown_Handler(listView_optional_objects, e);
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectAllItemsInList(selectedList);
        }
    }

    public static class ControlExtensions
    {
        public static void DoubleBuffering(this Control control, bool enable)
        {
            var method = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer, enable });
        }
    }

  

}

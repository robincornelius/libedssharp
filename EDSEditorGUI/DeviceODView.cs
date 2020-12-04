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
    Copyright(c) 2020 Janez Paternoster
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using libEDSsharp;

namespace ODEditor
{

    public partial class DeviceODView : MyTabUserControl
    {
        EDSsharp eds = null;

        ODentry selectedObject;
        ODentry lastSelectedObject;
        ListView selectedList;
        bool justUpdating = false;
        readonly bool CANopenNodeV4;

        public DeviceODView()
        {
            ExporterFactory.Exporter type = (ExporterFactory.Exporter)Properties.Settings.Default.ExporterType;
            CANopenNodeV4 = (type == ExporterFactory.Exporter.CANOPENNODE_V4);

            InitializeComponent();

            if (CANopenNodeV4)
            {
                comboBox_dataType.Items.Add("BOOLEAN");
                comboBox_dataType.Items.Add("INTEGER8");
                comboBox_dataType.Items.Add("INTEGER16");
                comboBox_dataType.Items.Add("INTEGER32");
                comboBox_dataType.Items.Add("INTEGER64");
                comboBox_dataType.Items.Add("UNSIGNED8");
                comboBox_dataType.Items.Add("UNSIGNED16");
                comboBox_dataType.Items.Add("UNSIGNED32");
                comboBox_dataType.Items.Add("UNSIGNED64");
                comboBox_dataType.Items.Add("REAL32");
                comboBox_dataType.Items.Add("REAL64");
                comboBox_dataType.Items.Add("VISIBLE_STRING");
                comboBox_dataType.Items.Add("OCTET_STRING");
                comboBox_dataType.Items.Add("UNICODE_STRING");
                comboBox_dataType.Items.Add("DOMAIN");

                comboBox_objectType.Items.Add("VAR");
                comboBox_objectType.Items.Add("ARRAY");
                comboBox_objectType.Items.Add("RECORD");

                foreach (AccessSDO foo in Enum.GetValues(typeof(AccessSDO)))
                    comboBox_accessSDO.Items.Add(foo.ToString());

                foreach (AccessPDO foo in Enum.GetValues(typeof(AccessPDO)))
                    comboBox_accessPDO.Items.Add(foo.ToString());
            }
            else
            {
                foreach (DataType foo in Enum.GetValues(typeof(DataType)))
                    comboBox_dataType.Items.Add(foo.ToString());
                foreach (ObjectType foo in Enum.GetValues(typeof(ObjectType)))
                    comboBox_objectType.Items.Add(foo.ToString());
                foreach (EDSsharp.AccessType foo in Enum.GetValues(typeof(EDSsharp.AccessType)))
                    comboBox_accessSDO.Items.Add(foo.ToString());

                comboBox_accessSDO.Items.Add("0x1003 rw/ro");
                comboBox_accessSDO.Items.Add("0x1010 const/rw");
                comboBox_accessSDO.Items.Add("0x1010 const/ro");

                comboBox_accessPDO.Items.Add("no");
                comboBox_accessPDO.Items.Add("optional");
            }

            foreach (AccessSRDO foo in Enum.GetValues(typeof(AccessSRDO)))
                comboBox_accessSRDO.Items.Add(foo.ToString());

            // other elements may be added in PopulateObjectLists()
            comboBox_countLabel.Items.Add("");
            comboBox_countLabel.Items.Add("Add...");
            comboBox_countLabel.SelectedIndexChanged += new System.EventHandler(this.ComboBox_countLabel_Add);
            comboBox_storageGroup.Items.Add("Add...");
            comboBox_storageGroup.SelectedIndexChanged += new System.EventHandler(this.ComboBox_storageGroup_Add);

            listView_communication_objects.DoubleBuffering(true);
            listView_deviceProfile_objects.DoubleBuffering(true);
            listView_manufacturer_objects.DoubleBuffering(true);
            listView_subObjects.DoubleBuffering(true);
        }

        private bool Checkdirty()
        {
            if (button_saveChanges.BackColor == Color.Red)
            {
                if (lastSelectedObject != null && MessageBox.Show(String.Format("Unsaved changes on Index 0x{0:X4}/{1:X2}.\nDo you wish to switch object and loose your changes?", lastSelectedObject.Index, lastSelectedObject.Subindex), "Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return true;
                }
                button_saveChanges.BackColor = default;
            }

            return false;
        }

        private void ComboBoxSet(ComboBox comboBox, string item)
        {
            if (item == null)
                item = "";

            if (!comboBox.Items.Contains(item))
                comboBox.Items.Add(item);

            comboBox.SelectedItem = item;
        }

        public void PopulateObjectLists(EDSsharp eds_target)
        {
            if (eds_target == null)
                return;

            eds = eds_target;

            doUpdateDeviceInfo();
            doUpdatePDOs();

            /* save scroll positions */
            int listview_communication_position = 0;
            int listview_manufacturer_position = 0;
            int listview_deviceProfile_position = 0;

            if (listView_communication_objects.TopItem != null)
                listview_communication_position = listView_communication_objects.TopItem.Index;
            if (listView_manufacturer_objects.TopItem != null)
                listview_manufacturer_position = listView_manufacturer_objects.TopItem.Index;
            if (listView_deviceProfile_objects.TopItem != null)
                listview_deviceProfile_position = listView_deviceProfile_objects.TopItem.Index;

            /* prevent flickering */
            listView_communication_objects.BeginUpdate();
            listView_manufacturer_objects.BeginUpdate();
            listView_deviceProfile_objects.BeginUpdate();

            listView_communication_objects.Items.Clear();
            listView_manufacturer_objects.Items.Clear();
            listView_deviceProfile_objects.Items.Clear();

            foreach (ODentry od in eds.ods.Values)
            {
                UInt16 index = od.Index;
                ListViewItem lvi = new ListViewItem(new string[] {
                    string.Format("0x{0:X4}", index),
                    od.parameter_name
                });

                lvi.Tag = od;
                if (selectedObject != null && index == selectedObject.Index)
                    lvi.Selected = true;
                if (od.prop.CO_disabled == true)
                    lvi.ForeColor = Color.LightGray;

                if (index <= 0x1000 || index < 0x2000)
                    listView_communication_objects.Items.Add(lvi);
                else if (index >= 0x2000 && index < 0x6000)
                    listView_manufacturer_objects.Items.Add(lvi);
                else
                    listView_deviceProfile_objects.Items.Add(lvi);

                string countLabel = od.prop.CO_countLabel;
                if (!comboBox_countLabel.Items.Contains(countLabel))
                    comboBox_countLabel.Items.Insert(comboBox_countLabel.Items.Count - 1, countLabel);

                string storageGroup = od.prop.CO_storageGroup;
                if (!comboBox_storageGroup.Items.Contains(storageGroup))
                    comboBox_storageGroup.Items.Insert(comboBox_storageGroup.Items.Count - 1, storageGroup);
            }

            listView_communication_objects.EndUpdate();
            listView_manufacturer_objects.EndUpdate();
            listView_deviceProfile_objects.EndUpdate();

            /* reset scroll position and selection */
            if (listview_communication_position != 0 && listView_communication_objects.Items.Count > 0)
                listView_communication_objects.TopItem = listView_communication_objects.Items[listview_communication_position];
            if (listview_manufacturer_position != 0 && listView_manufacturer_objects.Items.Count > 0)
                listView_manufacturer_objects.TopItem = listView_manufacturer_objects.Items[listview_manufacturer_position];
            if (listview_deviceProfile_position != 0 && listView_deviceProfile_objects.Items.Count > 0)
                listView_deviceProfile_objects.TopItem = listView_deviceProfile_objects.Items[listview_deviceProfile_position];
        }

        public void PopulateSubList()
        {
            listView_subObjects.Items.Clear();

            if (selectedObject == null)
                return;

            ODentry od = selectedObject.parent == null ? selectedObject : selectedObject.parent;

            if (od.objecttype == ObjectType.VAR)
            {
                ListViewItem lvi = new ListViewItem(new string[] {
                    " ", // subindex
                    od.parameter_name,
                    od.ObjectTypeString(),
                    od.datatype.ToString(),
                    od.AccessSDO().ToString(),
                    od.AccessPDO().ToString(),
                    od.prop.CO_accessSRDO.ToString(),
                    od.defaultvalue
                });
                lvi.Tag = od;
                listView_subObjects.Items.Add(lvi);
            }
            else if (od.objecttype == ObjectType.ARRAY || od.objecttype == ObjectType.REC)
            {
                ListViewItem lvi = new ListViewItem(new string[]{
                    " ",
                    od.parameter_name,
                    od.ObjectTypeString()
                });
                lvi.Tag = od;
                listView_subObjects.Items.Add(lvi);

                foreach (KeyValuePair<UInt16, ODentry> kvp in od.subobjects)
                {
                    ODentry subod = kvp.Value;
                    int subindex = kvp.Key;

                    ListViewItem lvi2 = new ListViewItem(new string[] {
                        string.Format("0x{0:X2}", subindex),
                        subod.parameter_name,
                        subod.ObjectTypeString(),
                        (subod.datatype != DataType.UNKNOWN) ? subod.datatype.ToString() : od.datatype.ToString(),
                        subod.AccessSDO().ToString(),
                        subod.AccessPDO().ToString(),
                        subod.prop.CO_accessSRDO.ToString(),
                        subod.defaultvalue
                    });
                    lvi2.Tag = subod;
                    listView_subObjects.Items.Add(lvi2);
                }
            }
        }

        public void PopulateObject()
        {
            justUpdating = true;
            lastSelectedObject = selectedObject;

            if (selectedObject == null)
            {
                textBox_index.Text = "";
                textBox_subIndex.Text = "";
                textBox_name.Text = "";
                textBox_denotation.Text = "";
                textBox_description.Text = "";
                justUpdating = false;
                return;
            }

            ODentry od = selectedObject;

            textBox_index.Text = string.Format("0x{0:X4}", od.Index);
            textBox_name.Text = od.parameter_name;
            textBox_denotation.Text = od.denotation;
            textBox_description.Text = (od.Description == null) ? "" : Regex.Replace(od.Description, "(?<!\r)\n", "\r\n");

            comboBox_objectType.SelectedItem = od.ObjectTypeString();

            if (od.objecttype == ObjectType.VAR)
            {
                comboBox_dataType.Enabled = true;
                comboBox_accessSDO.Enabled = true;
                comboBox_accessPDO.Enabled = true;
                comboBox_accessSRDO.Enabled = true;

                textBox_defaultValue.Enabled = true;
                textBox_actualValue.Enabled = true;
                textBox_highLimit.Enabled = true;
                textBox_lowLimit.Enabled = true;
                textBox_stringLengthMin.Enabled = true;

                string dataType = (od.datatype == DataType.UNKNOWN && od.parent != null)
                                ? od.parent.datatype.ToString()
                                : od.datatype.ToString();
                ComboBoxSet(comboBox_dataType, dataType);
                comboBox_accessSDO.SelectedItem = od.AccessSDO().ToString();
                comboBox_accessPDO.SelectedItem = od.AccessPDO().ToString();
                comboBox_accessSRDO.SelectedItem = od.prop.CO_accessSRDO.ToString();

                textBox_defaultValue.Text = od.defaultvalue;
                textBox_actualValue.Text = od.actualvalue;
                textBox_highLimit.Text = od.HighLimit;
                textBox_lowLimit.Text = od.LowLimit;
                textBox_stringLengthMin.Text = od.prop.CO_stringLengthMin.ToString();
            }
            else
            {
                comboBox_dataType.SelectedItem = null;
                comboBox_accessSDO.SelectedItem = null;
                comboBox_accessPDO.SelectedItem = null;
                comboBox_accessSRDO.SelectedItem = null;

                textBox_defaultValue.Text = "";
                textBox_actualValue.Text = "";
                textBox_highLimit.Text = "";
                textBox_lowLimit.Text = "";
                textBox_stringLengthMin.Text = "";

                comboBox_dataType.Enabled = false;
                comboBox_accessSDO.Enabled = false;
                comboBox_accessPDO.Enabled = false;
                comboBox_accessSRDO.Enabled = false;

                textBox_defaultValue.Enabled = false;
                textBox_actualValue.Enabled = false;
                textBox_highLimit.Enabled = false;
                textBox_lowLimit.Enabled = false;
                textBox_stringLengthMin.Enabled = false;
            }

            ODentry odBase;
            if (od.parent == null)
            {
                odBase = od;
                textBox_subIndex.Text = "";
                comboBox_countLabel.Enabled = true;
                comboBox_storageGroup.Enabled = true;
                checkBox_enabled.Enabled = true;
                checkBox_ioExtension.Enabled = true;
                checkBox_pdoFlags.Enabled = true;
            }
            else
            {
                odBase = od.parent;
                textBox_subIndex.Text = string.Format("0x{0:X2}", od.Subindex);
                comboBox_countLabel.Enabled = false;
                comboBox_storageGroup.Enabled = false;
                checkBox_enabled.Enabled = false;
                checkBox_ioExtension.Enabled = false;
                checkBox_pdoFlags.Enabled = false;
            }

            ComboBoxSet(comboBox_countLabel, odBase.prop.CO_countLabel);
            ComboBoxSet(comboBox_storageGroup, odBase.prop.CO_storageGroup);
            checkBox_enabled.Checked = !odBase.prop.CO_disabled;
            checkBox_ioExtension.Checked = odBase.prop.CO_extensionIO;
            checkBox_pdoFlags.Checked = odBase.prop.CO_flagsPDO;

            justUpdating = false;
            return;
        }

        private void DataDirty(object sender, EventArgs e)
        {
            if (!justUpdating)
                button_saveChanges.BackColor = Color.Red;
        }

        private void Button_saveChanges_Click(object sender, EventArgs e)
        {
            if (selectedObject == null)
                return;

            eds.Dirty = true;
            button_saveChanges.BackColor = default;
            ODentry od = selectedObject;

            od.parameter_name = textBox_name.Text;
            od.denotation = textBox_denotation.Text;
            od.Description = textBox_description.Text;
            od.ObjectTypeString(od.parent == null ? comboBox_objectType.SelectedItem.ToString() : "VAR");

            if (od.objecttype == ObjectType.VAR)
            {
                // dataType
                try
                {
                    od.datatype = (DataType)Enum.Parse(typeof(DataType), comboBox_dataType.SelectedItem.ToString());
                }
                catch (Exception) {
                    od.datatype = DataType.UNKNOWN;
                }

                AccessSDO accessSDO;
                try
                {
                    accessSDO = (AccessSDO)Enum.Parse(typeof(AccessSDO), comboBox_accessSDO.SelectedItem.ToString());
                }
                catch (Exception) {
                    accessSDO = AccessSDO.ro;
                }

                AccessPDO accessPDO;
                try
                {
                    accessPDO = (AccessPDO)Enum.Parse(typeof(AccessPDO), comboBox_accessPDO.SelectedItem.ToString());
                }
                catch (Exception)
                {
                    accessPDO = AccessPDO.no;
                }

                od.AccessSDO(accessSDO, accessPDO);
                od.AccessPDO(accessPDO);

                // CO_accessSRDO
                try
                {
                    od.prop.CO_accessSRDO = (AccessSRDO)Enum.Parse(typeof(AccessSRDO), comboBox_accessSRDO.SelectedItem.ToString());
                }
                catch (Exception)
                {
                    od.prop.CO_accessSRDO = AccessSRDO.no;
                }

                od.defaultvalue = textBox_defaultValue.Text;
                od.actualvalue = textBox_actualValue.Text;
                od.HighLimit = textBox_highLimit.Text;
                od.LowLimit = textBox_lowLimit.Text;

                // CO_stringLengthMin
                if (od.datatype == DataType.VISIBLE_STRING || od.datatype == DataType.UNICODE_STRING || od.datatype == DataType.OCTET_STRING)
                {
                    try
                    {
                        od.prop.CO_stringLengthMin = (uint)new System.ComponentModel.UInt32Converter().ConvertFromString(textBox_stringLengthMin.Text);
                    }
                    catch (Exception)
                    {
                        od.prop.CO_stringLengthMin = 0;
                    }
                }
                else
                {
                    od.prop.CO_stringLengthMin = 0;
                }

                // some propeties in all array sub elements (and base element) must be equal
                if (od.parent != null && od.parent.objecttype == ObjectType.ARRAY && od.Subindex > 0)
                {
                    foreach (ODentry subod in od.parent.subobjects.Values)
                    {
                        if (subod.Subindex > 0)
                        {
                            subod.datatype = od.datatype;
                            subod.accesstype = od.accesstype;
                            subod.PDOtype = od.PDOtype;
                            subod.prop.CO_accessSRDO = od.prop.CO_accessSRDO;
                        }
                    }
                    od.parent.datatype = od.datatype;
                    od.parent.accesstype = od.accesstype;
                    od.parent.PDOtype = od.PDOtype;
                    od.parent.prop.CO_accessSRDO = od.prop.CO_accessSRDO;
                }
            }

            if (od.parent == null)
            {
                od.prop.CO_countLabel = comboBox_countLabel.SelectedItem.ToString();
                od.prop.CO_storageGroup = comboBox_storageGroup.SelectedItem.ToString();
                od.prop.CO_disabled = !checkBox_enabled.Checked;
                od.prop.CO_extensionIO = checkBox_ioExtension.Checked;
                od.prop.CO_flagsPDO = checkBox_pdoFlags.Checked;
            }

            PopulateObjectLists(eds);
            PopulateSubList();
            PopulateObject();
        }

        private void ListView_objects_MouseClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ODentry od = listview.SelectedItems.Count > 0 ? (ODentry)listview.SelectedItems[0].Tag : null;

            if ((od != selectedObject || e.Button == MouseButtons.Right) && !Checkdirty())
            {
                selectedList = listview;
                selectedObject = od;

                if (e.Button == MouseButtons.Right)
                {
                    contextMenu_object.Show(Cursor.Position);
                }

                PopulateObject();
                PopulateSubList();
            }
            listView_communication_objects.HideSelection = true;
            listView_deviceProfile_objects.HideSelection = true;
            listView_manufacturer_objects.HideSelection = true;
        }

        private void ListView_objects_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView_objects_MouseClick(sender, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        }

        private void ListView_objects_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ((ListView)sender).SelectedItems.Clear();
            ListView_objects_MouseClick(sender, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));
        }

        private void ListView_subObjects_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView_subObjects.SelectedItems.Count == 0)
                return;

            ODentry od = (ODentry)listView_subObjects.SelectedItems[0].Tag;

            if ((od != selectedObject || e.Button == MouseButtons.Right) && !Checkdirty())
            {
                if (e.Button == MouseButtons.Right)
                {
                    ODentry parent = od.parent == null ? od : od.parent;

                    if (parent.objecttype == ObjectType.ARRAY || parent.objecttype == ObjectType.REC)
                    {
                        contextMenu_subObject_removeSubItemToolStripMenuItem.Enabled = od.Subindex > 0 && od.parent != null;
                        contextMenu_subObject_removeSubItemLeaveGapToolStripMenuItem.Enabled = parent.objecttype == ObjectType.REC && od.Subindex > 0 && od.parent != null;

                        if (listView_subObjects.FocusedItem.Bounds.Contains(e.Location) == true)
                        {
                            contextMenu_subObject.Show(Cursor.Position);
                        }
                    }
                }
                selectedObject = od;
                PopulateObject();
            }
        }

        private void ListView_subObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView_subObjects_MouseClick(sender, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        }

        private void ComboBox_countLabel_Add(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.SelectedItem != null && comboBox.SelectedItem.ToString() == "Add...")
            {
                NewItem dialog = new NewItem("Add Count Label");
                if (dialog.ShowDialog() == DialogResult.OK && comboBox.FindStringExact(dialog.name) == -1)
                {
                    comboBox.Items.Insert(comboBox.Items.Count - 1, dialog.name);
                    comboBox.SelectedItem = dialog.name;
                }
            }
        }

        private void ComboBox_storageGroup_Add(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.SelectedItem!=null && comboBox.SelectedItem.ToString() == "Add...")
            {
                NewItem dialog = new NewItem("Add Storage Group");
                if (dialog.ShowDialog() == DialogResult.OK && comboBox.FindStringExact(dialog.name) == -1)
                {
                    comboBox.Items.Insert(comboBox.Items.Count - 1, dialog.name);
                    comboBox.SelectedItem = dialog.name;
                    /* add new dialog location to eds back end */
                    eds.CO_storageGroups.Add(dialog.name);
                }
            }
        }

        private void ContextMenu_object_clone_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var srcObjects = new SortedDictionary<UInt16, ODentry>();
            foreach (ListViewItem item in selectedList.SelectedItems)
            {
                ODentry od = (ODentry)item.Tag;
                srcObjects.Add(od.Index, od);
            }

            if (srcObjects.Count > 0)
            {
                InsertObjects insObjForm = new InsertObjects(eds, srcObjects, "1");

                if (insObjForm.ShowDialog() == DialogResult.OK)
                {
                    selectedObject = null;
                    eds.Dirty = true;
                    PopulateObjectLists(eds);
                    PopulateSubList();
                    PopulateObject();
                }
            }
        }

        private void ContextMenu_object_add_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewIndex ni = new NewIndex(eds, (UInt16)(selectedObject == null ? 0x2000 : selectedObject.Index + 1));

            if (ni.ShowDialog() == DialogResult.OK)
            {
                selectedObject = ni.od;
                eds.Dirty = true;
                PopulateObjectLists(eds);
                PopulateSubList();
                PopulateObject();
            }
        }

        private void ContextMenu_object_delete_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = selectedList.SelectedItems;
            if (selectedItems.Count > 0)
            {
                DialogResult confirmDelete = MessageBox.Show(string.Format("Do you really want to delete the selected {0} items?", selectedItems.Count), "Are you sure?", MessageBoxButtons.YesNo);

                if (confirmDelete == DialogResult.Yes)
                {
                    foreach (ListViewItem item in selectedItems)
                    {
                        ODentry od = (ODentry)item.Tag;
                        eds.ods.Remove(od.Index);
                    }

                    eds.Dirty = true;
                    selectedObject = null;
                    PopulateObjectLists(eds);
                    PopulateSubList();
                    PopulateObject();
                }
            }
        }

        private void ContextMenu_object_toggle_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = selectedList.SelectedItems;

            justUpdating = true;
            foreach (ListViewItem item in selectedItems)
            {
                ODentry od = (ODentry)item.Tag;

                od.prop.CO_disabled = !od.prop.CO_disabled;
            }
            justUpdating = false;
            eds.Dirty = true;
            PopulateObjectLists(eds);
            PopulateObject();
        }

        private void ContextMenu_subObject_add_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = listView_subObjects.SelectedItems;

            ODentry newOd = null;

            foreach (ListViewItem item in selectedItems)
            {
                ODentry od = (ODentry)item.Tag;
                newOd = od.AddSubEntry();
            }

            eds.Dirty = true;
            selectedObject = newOd;
            PopulateSubList();
            PopulateObject();
        }

        private void ContextMenu_subObject_remove_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = listView_subObjects.SelectedItems;
            bool renumber = sender == contextMenu_subObject_removeSubItemToolStripMenuItem;
            bool update = false;

            foreach (ListViewItem item in selectedItems)
            {
                ODentry od = (ODentry)item.Tag;
                od.RemoveSubEntry(renumber);
                update = true;
            }

            if (update)
            {
                eds.Dirty = true;
                selectedObject = selectedObject.parent;
                PopulateSubList();
                PopulateObject();
            }
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

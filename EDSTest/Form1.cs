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

namespace EDSTest
{
    public partial class Form1 : Form
    {
        EDSsharp eds;
        public Form1()
        {
            InitializeComponent();
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
            foreach (KeyValuePair<string,ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                if (eds.md.objectlist.ContainsValue(od.index) && od.subindex==-1)
                {
                    ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}",od.index));
                    lvi.SubItems.Add(od.parameter_name);
                    listView_mandatory_objects.Items.Add(lvi);
                }

                if (eds.oo.objectlist.ContainsValue(od.index) && od.subindex == -1)
                {
                    ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}", od.index));
                    lvi.SubItems.Add(od.parameter_name);
                    listView_optional_objects.Items.Add(lvi);
                }

                if (eds.mo.objectlist.ContainsValue(od.index) && od.subindex == -1)
                {
                    ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}", od.index));
                    lvi.SubItems.Add(od.parameter_name);
                    listView_manufacture_objects.Items.Add(lvi);
                }
            }

        }

        private void listView_mandatory_objects_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void updateselectedindexdisplay(UInt16 index)
        {
            ODentry od = eds.getentryforindex(index,-1);

            listView1.Items.Clear();

            if(od.objecttype==ObjectType.VAR)
            {
                ListViewItem lvi = new ListViewItem("0");
                lvi.SubItems.Add(od.parameter_name);
                lvi.SubItems.Add(od.objecttype.ToString());
                lvi.SubItems.Add(od.datatype.ToString());
                lvi.SubItems.Add(od.accesstype.ToString());
                lvi.SubItems.Add(od.defaultvalue);
                lvi.SubItems.Add(od.PDOMapping.ToString());

                listView1.Items.Add(lvi);
            }
            else
            {
                ListViewItem lvi = new ListViewItem(" ");
                lvi.SubItems.Add(od.parameter_name);
                lvi.SubItems.Add(od.objecttype.ToString());

                listView1.Items.Add(lvi);

                for(Int16 i=0;i<od.nosubindexes;i++)
                {
                    ODentry od2 = eds.getentryforindex(index,i);
                    ListViewItem lvi2 = new ListViewItem((i).ToString());
                    lvi2.SubItems.Add(od2.parameter_name);
                    lvi2.SubItems.Add(od2.objecttype.ToString());
                    lvi2.SubItems.Add(od2.datatype.ToString());
                    lvi2.SubItems.Add(od2.accesstype.ToString());
                    lvi2.SubItems.Add(od2.defaultvalue);
                    lvi2.SubItems.Add(od2.PDOMapping.ToString());
                    listView1.Items.Add(lvi2);

                }

            }

            

        }

        private void listView_mandatory_objects_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView_mandatory_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            updateselectedindexdisplay(idx);
        }

        private void listView_optionalobjects_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView_optional_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            updateselectedindexdisplay(idx);
        }

        private void listView_manufacture_objects_MouseClick(object sender, MouseEventArgs e)
        {

            ListViewItem lvi = listView_manufacture_objects.SelectedItems[0];
            UInt16 idx = Convert.ToUInt16(lvi.Text, 16);

            updateselectedindexdisplay(idx);
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

                populateindexlists();

            }

        }
    }
}

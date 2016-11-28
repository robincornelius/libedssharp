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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using libEDSsharp;
using System.Globalization;
using Xml2CSharp;


namespace ODEditor
{
    public partial class ODEditor_MainForm : Form
    {

        private List<string> _mru = new List<string>();
        private string appdatafolder;

        public static Dictionary<UInt16, EDSsharp> TXCobMap = new Dictionary<UInt16, EDSsharp>();
        List<EDSsharp> network = new List<EDSsharp>();

        public ODEditor_MainForm()
        {
            InitializeComponent();
            loadprofiles();

            insertToolStripMenuItem.Enabled = false;


        }

        private void loadprofiles()
        {
            string[] profilelist = Directory.GetFiles(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+Path.DirectorySeparatorChar+"Profiles");
            ToolStripMenuItem[] items = new ToolStripMenuItem[profilelist.Length];

            int x = 0;
            foreach(string file in profilelist)
            {
            
                ToolStripMenuItem i = new ToolStripMenuItem();
                i.Name = Path.GetFileName(file);
                i.Text = Path.GetFileName(file);
                i.Click += ProfileAddClick;
                i.Image = Properties.Resources.InsertColumn_5626;
                items[x++] = i;   
            }

            insertToolStripMenuItem.DropDownItems.AddRange(items);
        
        }

        void ProfileAddClick(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];
               
                ToolStripMenuItem item = (ToolStripMenuItem)sender;

                string filename = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Profiles" + Path.DirectorySeparatorChar + item.Name;

                CanOpenXML coxml = new CanOpenXML();
                coxml.readXML(filename);

                Bridge b = new Bridge();

                EDSsharp eds;
                Device dev; //one day this will be multiple devices

                eds = b.convert(coxml.dev);
                dev = coxml.dev;

                foreach(KeyValuePair<UInt16,ODentry> kvp in eds.ods)
                {
                    if (!dv.eds.ods.ContainsKey(kvp.Key))
                        dv.eds.ods.Add(kvp.Key, kvp.Value);
                }

                
                dv.dispatch_updateOD();

            }
        }

        private void openEDSToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "Electronic Data Sheets (*.eds)|*.eds";
            if (odf.ShowDialog() == DialogResult.OK)
            {
                openEDSfile(odf.FileName);
                addtoMRU(odf.FileName);
            }
        }

        private void openEDSfile(string path)
        {
            Warnings.warning_list.Clear();

            try
            {
                EDSsharp eds = new EDSsharp();
                Device dev; 

                eds.loadfile(path);
                Bridge bridge = new Bridge(); //tell me again why bridge is not static?
                dev = bridge.convert(eds);

                DeviceView device = new DeviceView();

                device.eds = eds;
                tabControl1.TabPages.Add(eds.di.ProductName);
                tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(device);
                device.Dock = DockStyle.Fill;

                device.dispatch_updateOD();

                network.Add(eds);
            }
            catch (Exception ex)
            {
                Warnings.warning_list.Add(ex.ToString());
            }

            if (Warnings.warning_list.Count != 0)
            {
                WarningsFrm frm = new WarningsFrm();
                frm.ShowDialog();
            }
        }

        private void exportCanOpenNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.CheckFileExists = false;

                
                sfd.FileName = "CO_OD.c";
                sfd.InitialDirectory = dv.eds.fi.exportFolder;
                sfd.RestoreDirectory = true;

                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string savePath = Path.GetDirectoryName(sfd.FileName);
                    dv.eds.fi.exportFolder = savePath;

                    Warnings.warning_list.Clear();

                    CanOpenNodeExporter cone = new CanOpenNodeExporter();
                    cone.export(savePath, dv.eds);

                    if (Warnings.warning_list.Count != 0)
                    {
                        WarningsFrm frm = new WarningsFrm();
                        frm.ShowDialog();
                    }

                }
            }

             

        }

        private void openCanOpenNodeXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "XML (*.xml)|*.xml";
            if (odf.ShowDialog() == DialogResult.OK)
            {
                openXMLfile(odf.FileName);
                addtoMRU(odf.FileName);
            }

        }

        private void openXMLfile(string path)
        {

            try
            {
                EDSsharp eds;
                Device dev; //one day this will be multiple devices

                CanOpenXML coxml = new CanOpenXML();
                coxml.readXML(path);

                Bridge b = new Bridge();

                eds = b.convert(coxml.dev);
                eds.filename = path;

                dev = coxml.dev;

                tabControl1.TabPages.Add(eds.di.ProductName);

                DeviceView device = new DeviceView();

                device.eds = eds;
                tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(device);
                device.Dock = DockStyle.Fill;

                device.dispatch_updateOD();

                network.Add(eds);

            }
            catch (Exception ex)
            {
                Warnings.warning_list.Add(ex.ToString());
            }

            if (Warnings.warning_list.Count != 0)
            {
                WarningsFrm frm = new WarningsFrm();
                frm.ShowDialog();
            }

        }


        private void tabControl1_DrawItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabControl1.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {

                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.Red);
                g.FillRectangle(Brushes.Gray, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                // tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(device);

                DeviceView device = (DeviceView)tabControl1.SelectedTab.Controls[0];
                network.Remove(device.eds);

                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            }

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveEDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "Electronic Data Sheets (*.eds)|*.eds";

                sfd.InitialDirectory = Path.GetDirectoryName(dv.eds.filename);
                sfd.RestoreDirectory = true;
                sfd.FileName = Path.GetFileNameWithoutExtension(dv.eds.filename);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    dv.eds.savefile(sfd.FileName);
                }

            }
        }

        private void saveProjectXMLToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (tabControl1.SelectedTab != null)
                {
                    DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];
                    SaveFileDialog sfd = new SaveFileDialog();

                    sfd.Filter = "Canopen Node XML (*.xml)|*.xml";

                    sfd.InitialDirectory = Path.GetDirectoryName(dv.eds.filename);
                    sfd.RestoreDirectory = true;
                    sfd.FileName = Path.GetFileNameWithoutExtension(dv.eds.filename);

                if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        //dv.eds.savefile(sfd.FileName);

                        Bridge b = new Bridge();
                        Device d = b.convert(dv.eds);

                        CanOpenXML coxml = new CanOpenXML();
                        coxml.dev = d;

                        coxml.writeXML(sfd.FileName);
    
                    }

                }
            }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EDSsharp eds = new EDSsharp();
            eds.di.ProductName = "New Product";

            tabControl1.TabPages.Add(eds.di.ProductName);

            DeviceView device = new DeviceView();

            device.eds = eds;
            tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(device);
            device.Dock = DockStyle.Fill;

            device.dispatch_updateOD();
        }

        private void tabControl1_ControlsChanged(object sender, ControlEventArgs e)
        {
            enablesavemenus(tabControl1.TabCount > 0);  
        }

        private void tabControl1_Controlsremoved(object sender, ControlEventArgs e)
        {
            //Because
            enablesavemenus(tabControl1.TabCount > 1);
        }

        private void enablesavemenus(bool enable)
        {
            insertToolStripMenuItem.Enabled = enable;
            saveEDSToolStripMenuItem.Enabled = enable;
            saveProjectXMLToolStripMenuItem.Enabled = enable;
            exportCanOpenNodeToolStripMenuItem.Enabled = enable;
            closeFileToolStripMenuItem.Enabled = enable;
            saveNetworkXmlToolStripMenuItem.Enabled = enable;
            documentationToolStripMenuItem.Enabled = enable;
            networkPDOToolStripMenuItem.Enabled = enable;

        }

        void OpenRecentFile(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var filepath = (string)menuItem.Tag;

            string ext = Path.GetExtension(filepath);

            if (ext != null)
                ext = ext.ToLower();

            if ( ext == ".xml" )
                openXMLfile(filepath);
            if ( ext == ".eds" )
                openEDSfile(filepath);
            if (ext == ".nxml")
                openNetworkfile(filepath);

        }

        private void ODEditor_MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var mruFilePath = Path.Combine(appdatafolder, "MRU.txt");
            System.IO.File.WriteAllLines(mruFilePath, _mru);
        }

        private void ODEditor_MainForm_Load(object sender, EventArgs e)
        {
            //First lets create an appdata folder

            // The folder for the roaming current user 
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            appdatafolder = Path.Combine(folder, "EDSEditor");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(appdatafolder))
                Directory.CreateDirectory(appdatafolder);

            var mruFilePath = Path.Combine(appdatafolder, "MRU.txt");
            if (System.IO.File.Exists(mruFilePath))
                _mru.AddRange(System.IO.File.ReadAllLines(mruFilePath));

            populateMRU();
        }

        private void addtoMRU(string path)
        {
            // if it already exists remove it then let it readd itsself
            // so it will be promoted to the top of the list
            if (_mru.Contains(path))
                _mru.Remove(path);

            _mru.Insert(0, path);

            if (_mru.Count > 10)
                _mru.RemoveAt(10);

            populateMRU();

        }

        private void populateMRU()
        {

            mnuRecentlyUsed.DropDownItems.Clear();

            foreach (var path in _mru)
            {
                var item = new ToolStripMenuItem(path);
                item.Tag = path;
                item.Click += OpenRecentFile;
                switch(Path.GetExtension(path))
                {
                    case ".xml":
                        item.Image = Properties.Resources.GenericVSEditor_9905;
                        break;
                    case ".eds":
                        item.Image = Properties.Resources.EventLog_5735;
                        break;
                    case ".nxml":
                        item.Image = Properties.Resources.Index_8287_16x;
                        break;
                }

                mnuRecentlyUsed.DropDownItems.Add(item);
            }
        }

        private void saveNetworkXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "CanOpen network XML (*.nxml)|*.nxml";


            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //dv.eds.savefile(sfd.FileName);

                NetworkXML net = new NetworkXML();
                net.writeXML(sfd.FileName, network);
                addtoMRU(sfd.FileName);

            }

        }

        private void loadNetworkXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "CanOpen network XML (*.nxml)|*.nxml";
            if (odf.ShowDialog() == DialogResult.OK)
            {
                openNetworkfile(odf.FileName);
            }
        }

        private void openNetworkfile(string file)
        {
            NetworkXML net = new NetworkXML();
            List<Device> devs = net.readXML(file);

            foreach (Device d in devs)
            {
                Bridge b = new Bridge();

                EDSsharp eds = b.convert(d);
                //eds.filename = path;  //fixme

                tabControl1.TabPages.Add(eds.di.ProductName);
                

                DeviceView device = new DeviceView();

                device.eds = eds;
                tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(device);
                device.Dock = DockStyle.Fill;

                network.Add(eds);

                device.dispatch_updateOD();

                addtoMRU(file);
            }
        }

        private void networkPDOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string temp = Path.GetTempFileName();
            NetworkPDOreport npr = new NetworkPDOreport();
            npr.gennetpdodoc(temp, network);

            ReportView rv = new ReportView(temp);

            rv.Show();
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                Warnings.warning_list.Clear();

                if (tabControl1.SelectedTab != null)
                {
                    DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];
                    SaveFileDialog sfd = new SaveFileDialog();

                    string temp = Path.GetTempFileName();

                    this.UseWaitCursor = true;

                    DocumentationGen docgen = new DocumentationGen();
                    docgen.genhtmldoc(temp, dv.eds);
                    ReportView rv = new ReportView(temp);
                    rv.Show();

                    this.UseWaitCursor = false;

                }
            }
            catch (Exception ex)
            {
                Warnings.warning_list.Add(ex.ToString());
            }

            if (Warnings.warning_list.Count != 0)
            {
                WarningsFrm frm = new WarningsFrm();
                frm.ShowDialog();
            }
        }
    }
}

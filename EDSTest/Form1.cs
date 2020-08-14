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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using libEDSsharp;
using Xml2CSharp;
using XSDImport;

namespace ODEditor
{
    public partial class ODEditor_MainForm : Form
    {

        private List<string> _mru = new List<string>();
        private string appdatafolder;

        private string networkfilename;

        private string gitVersion;

        private string toolTipsString;   //used for holding the tooltip message for file drag and drop events

        public static Dictionary<UInt32, EDSsharp> TXCobMap = new Dictionary<UInt32, EDSsharp>();
        List<EDSsharp> network = new List<EDSsharp>();

        public ODEditor_MainForm()
        {
            InitializeComponent();
            loadprofiles();
            insertToolStripMenuItem.Enabled = false;
            Warnings.warning_mask = Properties.Settings.Default.WarningMask;
        }

        

        private void loadprofiles()
        {

            // load default profiles from the install directory
            // load user profiles from the My Documents\.edseditor\profiles\ folder
            // Personal is my documents in windows and ~ in mono

            try
            {

                List<string> profilelist = Directory.GetFiles(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Profiles").ToList();
                string homepath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".edseditor");
                homepath = Path.Combine(homepath, "profiles");

                if (Directory.Exists(homepath))
                {
                    profilelist.AddRange(Directory.GetFiles(homepath).ToList());
                }

                int count = 0;
                //some attempt to validate files

                foreach (string file in profilelist)
                {
                    if (Path.GetExtension(file) == ".xml")
                        count++;
                }


                ToolStripMenuItem[] items = new ToolStripMenuItem[count];

                int x = 0;
                foreach (string file in profilelist)
                {
                    if (Path.GetExtension(file) == ".xml")
                    {
                        ToolStripMenuItem i = new ToolStripMenuItem();
                        i.Name = Path.GetFileName(file);
                        i.Text = Path.GetFileName(file);
                        i.Click += ProfileAddClick;
                        i.Image = Properties.Resources.InsertColumn_5626;
                        items[x++] = i;
                    }
                }

                insertToolStripMenuItem.DropDownItems.AddRange(items);
            }
            catch (Exception e)
            {
                MessageBox.Show("Loading profiles has failed for the following reason :\n" + e.ToString());
            }
        
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
                dv.dispatch_updatePDOinfo();

                dv.eds.UpdatePDOcount();
                dv.dispatch_updatedevice();

            }
        }

        private void openEDSToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "Electronic Data Sheets (*.eds)|*.eds";
            if (odf.ShowDialog() == DialogResult.OK)
            {
                openEDSfile(odf.FileName,InfoSection.Filetype.File_EDS);
                addtoMRU(odf.FileName);
            }
        }

        private void openEDSfile(string path,InfoSection.Filetype ft)
        {
            Warnings.warning_list.Clear();

            try
            {
                EDSsharp eds = new EDSsharp();
                Device dev; 

                eds.Loadfile(path);
                Bridge bridge = new Bridge(); //tell me again why bridge is not static?
                dev = bridge.convert(eds);

                DeviceView device = new DeviceView();

                eds.OnDataDirty += Eds_onDataDirty;

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
                frm.Show();
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

                    ExporterFactory.Exporter type = (ExporterFactory.Exporter)Properties.Settings.Default.ExporterType;

                    IExporter exporter = ExporterFactory.getExporter(type);

                    try
                    {
                        exporter.export(savePath, Path.GetFileNameWithoutExtension(sfd.FileName), this.gitVersion, dv.eds);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Export failed see detailed reason below :-\n" + ex.ToString());
                    }

                    if (Warnings.warning_list.Count != 0)
                    {
                        WarningsFrm frm = new WarningsFrm();
                        frm.Show();
                    }

                    dv.dispatch_updateOD();

                }
            }
        }

        private void openCanOpenNodeXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Warnings.warning_list.Clear();

            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "All supported files (*.eds;*.xml;*.xdd;*.dcf)|*.eds;*.xml;*.xdd;*.dcf|Electronic Data Sheets (*.eds)|*.eds|Device Configuration Files (*.dcf)|*.dcf|CANopen Xml Data sheet (*.xdd)|*.xdd|CANopenNode XML (*.xml)|*.xml";
            if (odf.ShowDialog() == DialogResult.OK)
            {

                switch(Path.GetExtension(odf.FileName).ToLower())
                {
                    case ".xdd":
                        openXDDfile(odf.FileName);
                        break;

                    case ".xml":
                        openXMLfile(odf.FileName);
                        break;

                    case ".eds":
                        openEDSfile(odf.FileName, InfoSection.Filetype.File_EDS);
                        break;

                    case ".dcf":
                        openEDSfile(odf.FileName, InfoSection.Filetype.File_DCF);
                        break;

                    default:
                        return;

                }
              
                addtoMRU(odf.FileName);
            }

        }

        private void openXDDfile(string path)
        {
            try
            {
                EDSsharp eds;

                //fixme
                //ISO15745ProfileContainer devs; //one day this will be multiple devices

                CanOpenXDD coxml = new CanOpenXDD();
                eds = coxml.readXML(path);

                if (eds == null)
                    return;

                eds.xddfilename = path;

                tabControl1.TabPages.Add(eds.di.ProductName);

                DeviceView device = new DeviceView();

                eds.OnDataDirty += Eds_onDataDirty;

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
                frm.Show();
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
                eds.xmlfilename = path;  

                dev = coxml.dev;

                tabControl1.TabPages.Add(eds.di.ProductName);

                DeviceView device = new DeviceView();

                eds.OnDataDirty += Eds_onDataDirty;

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
                frm.Show();
            }

        }

        private void Eds_onDataDirty(bool dirty, EDSsharp sender)
        {
            foreach(TabPage page in tabControl1.TabPages)
            {
                foreach(Control c in page.Controls)
                {
                    if(c.GetType() == typeof(DeviceView))
                    {
                        DeviceView d = (DeviceView)c;
                        if (d.eds.Dirty == true)
                        {
                            page.BackColor = Color.Red;
                        }
                        else
                        {
                            page.BackColor = default(Color);
                        }
                    }

                }

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

                if(device.eds.Dirty==true)
                {
                    if (MessageBox.Show( "All unsaved changes will be lost\n continue?", "Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }

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

                sfd.InitialDirectory = Path.GetDirectoryName(dv.eds.edsfilename);
                sfd.RestoreDirectory = true;
                sfd.FileName = Path.GetFileNameWithoutExtension(dv.eds.edsfilename);

                if (sfd.ShowDialog() == DialogResult.OK)
                {

                    if (dv.eds.edsfilename != sfd.FileName)
                        dv.eds.Dirty = true;

                    dv.eds.Savefile(sfd.FileName,InfoSection.Filetype.File_EDS);

                    dv.eds.edsfilename = sfd.FileName;
                    dv.dispatch_updateOD();
                }

              

            }
        }

        private void exportDeviceFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "CANopen Node XML (*.xml)|*.xml|Electronic Data Sheets (*.eds)|*.eds|Device Configuration Files (*.dcf)|*.dcf";

                sfd.InitialDirectory = Path.GetDirectoryName(dv.eds.xmlfilename);
                sfd.RestoreDirectory = true;
                sfd.FileName = Path.GetFileNameWithoutExtension(dv.eds.xmlfilename);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    dosave(dv, sfd.FileName);
                }
            }
        }


        private void saveProjectXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];

                if (dv.eds.xddfilename!=null && dv.eds.xddfilename != "")
                {
                    dosave(dv, dv.eds.xddfilename);
                }
                else
                {
                    saveAsToolStripMenuItem_Click(sender, e);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "CANopen XDD (*.xdd)|*.xdd";

                sfd.InitialDirectory = Path.GetDirectoryName(dv.eds.xmlfilename);
                sfd.RestoreDirectory = true;
                sfd.FileName = Path.GetFileNameWithoutExtension(dv.eds.xmlfilename);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    dosave(dv, sfd.FileName);                   
                }
            }

        }


        void dosave(DeviceView dv,string FileName)
        {

            switch (Path.GetExtension(FileName))
            {
                case ".eds":
                    dv.eds.Savefile(FileName, InfoSection.Filetype.File_EDS);
                    dv.eds.edsfilename = FileName;
                    break;

                case ".dcf":
                    dv.eds.Savefile(FileName, InfoSection.Filetype.File_DCF);
                    dv.eds.dcffilename = FileName;
                    break;

                case ".xml":
                    Bridge b = new Bridge();
                    Device d = b.convert(dv.eds);

                    CanOpenXML coxml = new CanOpenXML();
                    coxml.dev = d;
                    coxml.writeXML(FileName);
                    dv.eds.xmlfilename = FileName;
                    break;

                case ".xdd":
                    CanOpenXDD coxdd = new CanOpenXDD();
                    coxdd.writeXML(FileName, dv.eds);
                    dv.eds.xddfilename = FileName;
                    dv.eds.Dirty = false;
                    break;
            }

            dv.dispatch_updateOD();

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EDSsharp eds = new EDSsharp();
            eds.di.ProductName = "New Product";

            tabControl1.TabPages.Add(eds.di.ProductName);

            DeviceView device = new DeviceView();

            device.eds = eds;
            eds.OnDataDirty += Eds_onDataDirty;

            tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(device);
            device.Dock = DockStyle.Fill;

            device.dispatch_updateOD();

            network.Add(eds);
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
            saveProjectXMLToolStripMenuItem.Enabled = enable;
            exportCanOpenNodeToolStripMenuItem.Enabled = enable;
            closeFileToolStripMenuItem.Enabled = enable;
            saveNetworkXmlToolStripMenuItem.Enabled = enable;
            documentationToolStripMenuItem.Enabled = enable;
            networkPDOToolStripMenuItem.Enabled = enable;
            saveExportAllToolStripMenuItem.Enabled = enable;
            exportDeviceFileToolStripMenuItem.Enabled = enable;
            saveAsToolStripMenuItem.Enabled = true;

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
            if (ext == ".xdd")
                openXDDfile(filepath);
            if ( ext == ".eds" )
                openEDSfile(filepath, InfoSection.Filetype.File_EDS);
            if (ext == ".dcf")
                openEDSfile(filepath, InfoSection.Filetype.File_DCF);
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
            //read git version string, show in title bar 
            //(https://stackoverflow.com/a/15145121)
            string gitVersion = String.Empty;
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("ODEditor." + "version.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                gitVersion = reader.ReadToEnd().TrimEnd('\n');
            }
            if (gitVersion == "")
            {
                gitVersion = "Unknown";
            }
            this.Text += "v" + gitVersion;
            this.gitVersion = gitVersion;

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
            // if it already exists remove it then let it re-add itself
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

            sfd.Filter = "CANopen Network XDD (*.nxdd)|*.nxdd|CANopen network XML (*.nxml)|*.nxml";

            sfd.InitialDirectory = Path.GetDirectoryName(networkfilename);
            sfd.RestoreDirectory = true;
            sfd.FileName = Path.GetFileNameWithoutExtension(networkfilename);

            if (sfd.ShowDialog() == DialogResult.OK)
            {

                switch (Path.GetExtension(sfd.FileName).ToLower())
                {
                    case ".nxml":
                        NetworkXML net = new NetworkXML();
                        net.writeXML(sfd.FileName, network);
                        addtoMRU(sfd.FileName);
                        break;

                    case ".nxdd":
                        CanOpenXDD xdd = new CanOpenXDD();
                        xdd.writeMultiXML(sfd.FileName, network);
                        break;

                }

            }

        }

        private void loadNetworkXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog odf = new OpenFileDialog();
            odf.Filter = "CANopen Network XDD (*.nxdd)|*.nxdd|CANopen network XML (*.nxml)|*.nxml";
            if (odf.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(odf.FileName).ToLower())
                {
                    case ".nxml":
                        openNetworkfile(odf.FileName);
                        break;

                    case ".nxdd":

                        openXDDNetworkfile(odf.FileName);

                        break;

                }

               
            }
        }

        private void openXDDNetworkfile(string file)
        {
            CanOpenXDD xdd = new CanOpenXDD();
            List<EDSsharp> edss = xdd.readMultiXML(file);

            if (edss == null)
                return;

            foreach (EDSsharp eds in edss)
            {

                tabControl1.TabPages.Add(eds.di.ProductName);

                DeviceView device = new DeviceView();

                device.eds = eds;
                tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(device);
                device.Dock = DockStyle.Fill;

                network.Add(eds);
                eds.OnDataDirty += Eds_onDataDirty;

                device.dispatch_updateOD();

            }

            addtoMRU(file);
            networkfilename = file;

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
                eds.OnDataDirty += Eds_onDataDirty;

                device.dispatch_updateOD();

                addtoMRU(file);
            }

            networkfilename = file;
        }

        private void networkPDOToolStripMenuItem_Click(object sender, EventArgs e)
        {


            string dir = GetTemporaryDirectory();

            string csspath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".edseditor");
            csspath = Path.Combine(csspath, "style.css");

            if (!System.IO.File.Exists(csspath))
            {
                csspath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "style.css");
            }

            if (System.IO.File.Exists(csspath))
            {
                System.IO.File.Copy(csspath, dir + Path.DirectorySeparatorChar + "style.css");
            }

            string temp = dir + Path.DirectorySeparatorChar + "network.html";

            NetworkPDOreport npr = new NetworkPDOreport();
            npr.gennetpdodoc(temp, network);

            if (IsRunningOnMono())
            {
                System.Diagnostics.Process.Start("file://"+temp);
            }
            else
            {
                ReportView rv = new ReportView(temp);
                rv.Show();
            }
        }

        public string GetTemporaryDirectory()
        {
            string tempDirectory;

            do
            {
                tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            } while (Directory.Exists(tempDirectory));

            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
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

                    string dir = GetTemporaryDirectory();


                    string csspath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".edseditor");
                    csspath = Path.Combine(csspath, "style.css");

                    if (!System.IO.File.Exists(csspath))
                    {
                        csspath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "style.css");
                    }

                    if (System.IO.File.Exists(csspath))
                    {
                        System.IO.File.Copy(csspath, dir + Path.DirectorySeparatorChar + "style.css");
                    }

                    string temp = dir + Path.DirectorySeparatorChar + "documentation.html";
                    string temp2 = dir + Path.DirectorySeparatorChar + "documentation.md";

                    this.UseWaitCursor = true;

                    DocumentationGen docgen = new DocumentationGen();
                    docgen.genhtmldoc(temp, dv.eds);
                    docgen.genmddoc(temp2, dv.eds);
                    System.Diagnostics.Process.Start("file://" + temp2);
                    if (IsRunningOnMono())
                    {
                        System.Diagnostics.Process.Start("file://" + temp);
                    }
                    else
                    {
                        ReportView rv = new ReportView(temp);
                        rv.Show();
                    }

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
                frm.Show();
            }
        }

        private void saveExportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string temp;
            //Attempt to save EDS,XML and export the CANopen dictionary

            if (tabControl1.SelectedTab != null)
            {
                DeviceView dv = (DeviceView)tabControl1.SelectedTab.Controls[0];
                SaveFileDialog sfd = new SaveFileDialog();

                //save eds xml and export CO_OD.c and CO_OD.h

                if (dv.eds.edsfilename == null || dv.eds.edsfilename == "")
                {
                    MessageBox.Show("Please manually save as EDS at least once");
                    return;
                }
                temp = Path.GetDirectoryName(dv.eds.edsfilename);
                if (Directory.Exists (temp) != true) 
                {
                    MessageBox.Show("File path was removed. Please manually save as EDS once");
                    return;
                }

                if (dv.eds.xmlfilename == null || dv.eds.xmlfilename == "")
                {
                    MessageBox.Show("Please manually save as XML at least once");
                    return;
                }
                temp = Path.GetDirectoryName(dv.eds.xmlfilename);
                if (Directory.Exists (temp) != true) 
                {
                    MessageBox.Show("File path was removed. Please manually save as XML once");
                    return;
                }



                if (dv.eds.fi.exportFolder == null || dv.eds.fi.exportFolder == "")
                {
                    MessageBox.Show("Please export CO_OD.c/h at least once");
                    return;
                }
                if (Directory.Exists (dv.eds.fi.exportFolder) != true) 
                {
                    MessageBox.Show("File path was removed. Please export CO_OD.c/h once");
                    return;
                }

                //export XML
                Bridge b = new Bridge();
                Device d = b.convert(dv.eds);

                CanOpenXML coxml = new CanOpenXML();
                coxml.dev = d;

                coxml.writeXML(dv.eds.xmlfilename);

                if (dv.eds.xddfilename != null)
                {
                    CanOpenXDD coxdd = new CanOpenXDD();
                    coxdd.writeXML(dv.eds.xddfilename, dv.eds);
                }

                //export EDS
                dv.eds.Savefile(dv.eds.edsfilename, InfoSection.Filetype.File_EDS);

                //export CO_OD.c and CO_OD.h
                ExporterFactory.Exporter type = (ExporterFactory.Exporter)Properties.Settings.Default.ExporterType;
                IExporter cone = ExporterFactory.getExporter(type);

                try
                {
                    cone.export(dv.eds.fi.exportFolder, "", this.gitVersion, dv.eds);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Export failed, error message:\n" + ex.ToString());
                    return;
                }

                dv.eds.Dirty = false;

                if (Warnings.warning_list.Count != 0)
                {
                    WarningsFrm frm = new WarningsFrm();
                    frm.Show();
                }





            }
        }

        private void ODEditor_MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            foreach (TabPage page in tabControl1.TabPages)
            {
                foreach (Control c in page.Controls)
                {
                    if (c.GetType() == typeof(DeviceView))
                    {
                        DeviceView d = (DeviceView)c;
                        if (d.eds.Dirty == true)
                        {
                           if(MessageBox.Show("Warning you have unsaved changes\n Do you wish to continue","Unsaved changes",MessageBoxButtons.YesNo)==DialogResult.No)
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                       
                    }

                }

            }
        }

        private bool fileTypeSupported(string fileName) //there is a better way to do this, but it's easy to copy/paste from the file open code. bigger fish to fry...
        {
            bool typeSupported = false;
            if (Path.HasExtension(fileName))
            {
                switch (Path.GetExtension(fileName).ToLower())
                {
                    case ".xdd":
                        typeSupported = true;
                        break;

                    case ".xml":
                        typeSupported = true;
                        break;

                    case ".eds":
                        typeSupported = true;
                        break;

                    case ".dcf":
                        typeSupported = true;
                        break;

                    case ".nxml":
                        typeSupported = true;
                        break;

                    case ".nxdd":
                        typeSupported = true;
                        break;

                    default:
                        typeSupported = false;
                        break;

                }
            }

            return typeSupported;

        }

        private void ODEditor_MainForm_DragEnter(object sender, DragEventArgs e)
        {
            this.Activate();
            bool unsupportedFile = false;
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null && data.GetType()==typeof(String))
            {                
                var rawFileNames = data as string[];
                if (rawFileNames.Length > 0)
                {
                    var fileNames = rawFileNames.Distinct();
                    foreach (string fileName in fileNames)
                    {
                        if(fileTypeSupported(fileName) == false)
                        {
                            unsupportedFile = true;
                            break;
                        }
                    }

                }

                toolTipsString = (unsupportedFile ? "1 or more files not supported" : "Drop files here to open");

                if (unsupportedFile)
                {
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    e.Effect = DragDropEffects.All;
                }

                enableDragDropTooltip();

            }
                
            else
            {
               e.Effect = DragDropEffects.None;
                //disableDragDropTooltip();
                enableDragDropTooltip();
            }
                
        }

        private void enableDragDropTooltip()
        {
            toolTip1.Active = true;
            toolTip1.ReshowDelay = 0;
            toolTip1.InitialDelay = 0;
            toolTip1.UseAnimation = false;
            toolTip1.UseFading = false;
            toolTip1.Show(toolTipsString, this, this.PointToClient(Cursor.Position).X, this.PointToClient(Cursor.Position).Y);
        }

        private void disableDragDropTooltip()
        {
            toolTip1.Active = false;
        }

        private void ODEditor_MainForm_DragLeave(object sender, EventArgs e)
        {
            disableDragDropTooltip();
        }

        private void ODEditor_MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var rawFileNames = data as string[];

                if (rawFileNames.Length > 0)
                {
                    var fileNames = rawFileNames.Distinct();
                    foreach (string fileName in fileNames)
                    {
                        switch (Path.GetExtension(fileName).ToLower())
                        {
                            case ".xdd":
                                openXDDfile(fileName);
                                break;

                            case ".xml":
                                openXMLfile(fileName);
                                break;

                            case ".eds":
                                openEDSfile(fileName, InfoSection.Filetype.File_EDS);
                                break;

                            case ".dcf":
                                openEDSfile(fileName, InfoSection.Filetype.File_DCF);
                                break;

                            case ".nxml":
                                openNetworkfile(fileName);
                                break;

                            case ".nxdd":
                                openXDDNetworkfile(fileName);
                                break;

                            default:
                                break;

                        }

                        addtoMRU(fileName);
                    }
                }
            }
            disableDragDropTooltip();
        }

        private void ODEditor_MainForm_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if(e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                disableDragDropTooltip();
            }
        }

        private void ODEditor_MainForm_Leave(object sender, EventArgs e)
        {
            disableDragDropTooltip();
        }

        private void ODEditor_MainForm_DragOver(object sender, DragEventArgs e)
        {
            enableDragDropTooltip();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences p = new Preferences();
            p.ShowDialog();
        }
    }
}

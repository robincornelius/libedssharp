namespace ODEditor
{
    partial class ODEditor_MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ODEditor_MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openEDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveEDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openCanOpenNodeXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.loadNetworkXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNetworkXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRecentlyUsed = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportCanOpenNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkPDOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.reportsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1199, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openEDSToolStripMenuItem,
            this.saveEDSToolStripMenuItem,
            this.toolStripSeparator3,
            this.openCanOpenNodeXMLToolStripMenuItem,
            this.saveProjectXMLToolStripMenuItem,
            this.toolStripSeparator5,
            this.loadNetworkXmlToolStripMenuItem,
            this.saveNetworkXmlToolStripMenuItem,
            this.toolStripSeparator6,
            this.mnuRecentlyUsed,
            this.toolStripSeparator1,
            this.exportCanOpenNodeToolStripMenuItem,
            this.toolStripSeparator2,
            this.closeFileToolStripMenuItem,
            this.toolStripSeparator4,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::ODEditor.Properties.Resources.NewFile_6276;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openEDSToolStripMenuItem
            // 
            this.openEDSToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Open_6529;
            this.openEDSToolStripMenuItem.Name = "openEDSToolStripMenuItem";
            this.openEDSToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.openEDSToolStripMenuItem.Text = "Open EDS";
            this.openEDSToolStripMenuItem.Click += new System.EventHandler(this.openEDSToolStripMenuItem_Click);
            // 
            // saveEDSToolStripMenuItem
            // 
            this.saveEDSToolStripMenuItem.Enabled = false;
            this.saveEDSToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Save_6530;
            this.saveEDSToolStripMenuItem.Name = "saveEDSToolStripMenuItem";
            this.saveEDSToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.saveEDSToolStripMenuItem.Text = "Save EDS";
            this.saveEDSToolStripMenuItem.Click += new System.EventHandler(this.saveEDSToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(207, 6);
            // 
            // openCanOpenNodeXMLToolStripMenuItem
            // 
            this.openCanOpenNodeXMLToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Open_6529;
            this.openCanOpenNodeXMLToolStripMenuItem.Name = "openCanOpenNodeXMLToolStripMenuItem";
            this.openCanOpenNodeXMLToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.openCanOpenNodeXMLToolStripMenuItem.Text = "Open Device XML";
            this.openCanOpenNodeXMLToolStripMenuItem.Click += new System.EventHandler(this.openCanOpenNodeXMLToolStripMenuItem_Click);
            // 
            // saveProjectXMLToolStripMenuItem
            // 
            this.saveProjectXMLToolStripMenuItem.Enabled = false;
            this.saveProjectXMLToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Save_6530;
            this.saveProjectXMLToolStripMenuItem.Name = "saveProjectXMLToolStripMenuItem";
            this.saveProjectXMLToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.saveProjectXMLToolStripMenuItem.Text = "Save Device XML";
            this.saveProjectXMLToolStripMenuItem.Click += new System.EventHandler(this.saveProjectXMLToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(207, 6);
            // 
            // loadNetworkXmlToolStripMenuItem
            // 
            this.loadNetworkXmlToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Open_6529;
            this.loadNetworkXmlToolStripMenuItem.Name = "loadNetworkXmlToolStripMenuItem";
            this.loadNetworkXmlToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.loadNetworkXmlToolStripMenuItem.Text = "Open Network XML";
            this.loadNetworkXmlToolStripMenuItem.Click += new System.EventHandler(this.loadNetworkXmlToolStripMenuItem_Click);
            // 
            // saveNetworkXmlToolStripMenuItem
            // 
            this.saveNetworkXmlToolStripMenuItem.Enabled = false;
            this.saveNetworkXmlToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Save_6530;
            this.saveNetworkXmlToolStripMenuItem.Name = "saveNetworkXmlToolStripMenuItem";
            this.saveNetworkXmlToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.saveNetworkXmlToolStripMenuItem.Text = "Save Network XML";
            this.saveNetworkXmlToolStripMenuItem.Click += new System.EventHandler(this.saveNetworkXmlToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(207, 6);
            // 
            // mnuRecentlyUsed
            // 
            this.mnuRecentlyUsed.Name = "mnuRecentlyUsed";
            this.mnuRecentlyUsed.Size = new System.Drawing.Size(210, 22);
            this.mnuRecentlyUsed.Text = "Recent Files";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(207, 6);
            // 
            // exportCanOpenNodeToolStripMenuItem
            // 
            this.exportCanOpenNodeToolStripMenuItem.Enabled = false;
            this.exportCanOpenNodeToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Compile_191;
            this.exportCanOpenNodeToolStripMenuItem.Name = "exportCanOpenNodeToolStripMenuItem";
            this.exportCanOpenNodeToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.exportCanOpenNodeToolStripMenuItem.Text = "Export CanOpenNode c/h";
            this.exportCanOpenNodeToolStripMenuItem.Click += new System.EventHandler(this.exportCanOpenNodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(207, 6);
            // 
            // closeFileToolStripMenuItem
            // 
            this.closeFileToolStripMenuItem.Enabled = false;
            this.closeFileToolStripMenuItem.Image = global::ODEditor.Properties.Resources.Close_6519;
            this.closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
            this.closeFileToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.closeFileToolStripMenuItem.Text = "Close file";
            this.closeFileToolStripMenuItem.Click += new System.EventHandler(this.closeFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(207, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Image = global::ODEditor.Properties.Resources._305_Close_16x16_72;
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.insertToolStripMenuItem.Text = "Insert Profile";
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentationToolStripMenuItem,
            this.networkPDOToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // documentationToolStripMenuItem
            // 
            this.documentationToolStripMenuItem.Enabled = false;
            this.documentationToolStripMenuItem.Image = global::ODEditor.Properties.Resources.ExporttoScript_9881;
            this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            this.documentationToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.documentationToolStripMenuItem.Text = "Documentation";
            this.documentationToolStripMenuItem.Click += new System.EventHandler(this.documentationToolStripMenuItem_Click);
            // 
            // networkPDOToolStripMenuItem
            // 
            this.networkPDOToolStripMenuItem.Enabled = false;
            this.networkPDOToolStripMenuItem.Image = global::ODEditor.Properties.Resources.ExporttoScript_9881;
            this.networkPDOToolStripMenuItem.Name = "networkPDOToolStripMenuItem";
            this.networkPDOToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.networkPDOToolStripMenuItem.Text = "Network PDO";
            this.networkPDOToolStripMenuItem.Click += new System.EventHandler(this.networkPDOToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ItemSize = new System.Drawing.Size(24, 120);
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1199, 737);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 2;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.tabControl1_ControlsChanged);
            this.tabControl1.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.tabControl1_Controlsremoved);
            // 
            // ODEditor_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 761);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ODEditor_MainForm";
            this.Text = "Object Dictionary Editor v0.4";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ODEditor_MainForm_FormClosed);
            this.Load += new System.EventHandler(this.ODEditor_MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openEDSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCanOpenNodeXMLToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem closeFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveEDSToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveProjectXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mnuRecentlyUsed;
        private System.Windows.Forms.ToolStripMenuItem saveNetworkXmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadNetworkXmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem exportCanOpenNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkPDOToolStripMenuItem;
    }
}


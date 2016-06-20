namespace EDSTest
{
    partial class Form1
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listView_manufacture_objects = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView_optional_objects = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView_mandatory_objects = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openEDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCanOpenNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCanOpenNodeXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.listView_manufacture_objects);
            this.splitContainer1.Panel1.Controls.Add(this.listView_optional_objects);
            this.splitContainer1.Panel1.Controls.Add(this.listView_mandatory_objects);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Size = new System.Drawing.Size(1116, 764);
            this.splitContainer1.SplitterDistance = 286;
            this.splitContainer1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 423);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Manufacturer Objects";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Optional Objects";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Compulsory Objects";
            // 
            // listView_manufacture_objects
            // 
            this.listView_manufacture_objects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.listView_manufacture_objects.FullRowSelect = true;
            this.listView_manufacture_objects.Location = new System.Drawing.Point(12, 439);
            this.listView_manufacture_objects.Name = "listView_manufacture_objects";
            this.listView_manufacture_objects.Size = new System.Drawing.Size(269, 313);
            this.listView_manufacture_objects.TabIndex = 2;
            this.listView_manufacture_objects.UseCompatibleStateImageBehavior = false;
            this.listView_manufacture_objects.View = System.Windows.Forms.View.Details;
            this.listView_manufacture_objects.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_manufacture_objects_MouseClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Index";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Name";
            this.columnHeader6.Width = 200;
            // 
            // listView_optional_objects
            // 
            this.listView_optional_objects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listView_optional_objects.FullRowSelect = true;
            this.listView_optional_objects.Location = new System.Drawing.Point(12, 206);
            this.listView_optional_objects.Name = "listView_optional_objects";
            this.listView_optional_objects.Size = new System.Drawing.Size(269, 214);
            this.listView_optional_objects.TabIndex = 1;
            this.listView_optional_objects.UseCompatibleStateImageBehavior = false;
            this.listView_optional_objects.View = System.Windows.Forms.View.Details;
            this.listView_optional_objects.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_optionalobjects_MouseClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Index";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 200;
            // 
            // listView_mandatory_objects
            // 
            this.listView_mandatory_objects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_mandatory_objects.FullRowSelect = true;
            this.listView_mandatory_objects.Location = new System.Drawing.Point(12, 25);
            this.listView_mandatory_objects.Name = "listView_mandatory_objects";
            this.listView_mandatory_objects.Size = new System.Drawing.Size(269, 161);
            this.listView_mandatory_objects.TabIndex = 0;
            this.listView_mandatory_objects.UseCompatibleStateImageBehavior = false;
            this.listView_mandatory_objects.View = System.Windows.Forms.View.Details;
            this.listView_mandatory_objects.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_mandatory_objects_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Index";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 200;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader14});
            this.listView1.Location = new System.Drawing.Point(6, 25);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(820, 417);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Subindex";
            this.columnHeader7.Width = 66;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Name";
            this.columnHeader8.Width = 245;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Obj Type";
            this.columnHeader9.Width = 65;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Data Type";
            this.columnHeader10.Width = 160;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Access";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Default";
            this.columnHeader12.Width = 102;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "PDO Map";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1116, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openEDSToolStripMenuItem,
            this.exportCanOpenNodeToolStripMenuItem,
            this.openCanOpenNodeXMLToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openEDSToolStripMenuItem
            // 
            this.openEDSToolStripMenuItem.Name = "openEDSToolStripMenuItem";
            this.openEDSToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.openEDSToolStripMenuItem.Text = "Open EDS";
            this.openEDSToolStripMenuItem.Click += new System.EventHandler(this.openEDSToolStripMenuItem_Click);
            // 
            // exportCanOpenNodeToolStripMenuItem
            // 
            this.exportCanOpenNodeToolStripMenuItem.Name = "exportCanOpenNodeToolStripMenuItem";
            this.exportCanOpenNodeToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.exportCanOpenNodeToolStripMenuItem.Text = "Export CanOpenNode c/h";
            this.exportCanOpenNodeToolStripMenuItem.Click += new System.EventHandler(this.exportCanOpenNodeToolStripMenuItem_Click);
            // 
            // openCanOpenNodeXMLToolStripMenuItem
            // 
            this.openCanOpenNodeXMLToolStripMenuItem.Name = "openCanOpenNodeXMLToolStripMenuItem";
            this.openCanOpenNodeXMLToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.openCanOpenNodeXMLToolStripMenuItem.Text = "Open CanOpenNode XML";
            this.openCanOpenNodeXMLToolStripMenuItem.Click += new System.EventHandler(this.openCanOpenNodeXMLToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1116, 788);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Object Dictionary Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView_manufacture_objects;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ListView listView_optional_objects;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ListView listView_mandatory_objects;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openEDSToolStripMenuItem;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem exportCanOpenNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCanOpenNodeXMLToolStripMenuItem;

    }
}


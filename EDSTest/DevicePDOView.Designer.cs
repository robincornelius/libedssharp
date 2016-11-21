namespace ODEditor
{
    partial class DevicePDOView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label_availableobjects = new System.Windows.Forms.Label();
            this.listView_TXPDO = new System.Windows.Forms.ListView();
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_savepdochanges = new System.Windows.Forms.Button();
            this.button_deletePDO = new System.Windows.Forms.Button();
            this.textBox_slot = new System.Windows.Forms.TextBox();
            this.textBox_syncstart = new System.Windows.Forms.TextBox();
            this.textBox_eventtimer = new System.Windows.Forms.TextBox();
            this.textBox_inhibit = new System.Windows.Forms.TextBox();
            this.textBox_type = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_cob = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listView_TXCOBmap = new CustomListView.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_addPDO = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 238);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Assigned PDO mapping";
            // 
            // label_availableobjects
            // 
            this.label_availableobjects.AutoSize = true;
            this.label_availableobjects.Location = new System.Drawing.Point(13, 13);
            this.label_availableobjects.Name = "label_availableobjects";
            this.label_availableobjects.Size = new System.Drawing.Size(130, 13);
            this.label_availableobjects.TabIndex = 6;
            this.label_availableobjects.Text = "Available Objects for PDO";
            // 
            // listView_TXPDO
            // 
            this.listView_TXPDO.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader13,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17});
            this.listView_TXPDO.FullRowSelect = true;
            this.listView_TXPDO.Location = new System.Drawing.Point(16, 29);
            this.listView_TXPDO.MultiSelect = false;
            this.listView_TXPDO.Name = "listView_TXPDO";
            this.listView_TXPDO.Size = new System.Drawing.Size(492, 199);
            this.listView_TXPDO.TabIndex = 5;
            this.listView_TXPDO.UseCompatibleStateImageBehavior = false;
            this.listView_TXPDO.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Index";
            this.columnHeader13.Width = 55;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Sub";
            this.columnHeader15.Width = 40;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Name";
            this.columnHeader16.Width = 206;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Datatype";
            this.columnHeader17.Width = 183;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_savepdochanges);
            this.groupBox1.Controls.Add(this.button_deletePDO);
            this.groupBox1.Controls.Add(this.textBox_slot);
            this.groupBox1.Controls.Add(this.textBox_syncstart);
            this.groupBox1.Controls.Add(this.textBox_eventtimer);
            this.groupBox1.Controls.Add(this.textBox_inhibit);
            this.groupBox1.Controls.Add(this.textBox_type);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_cob);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(521, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 199);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Commuincation parameters";
            // 
            // button_savepdochanges
            // 
            this.button_savepdochanges.Image = global::ODEditor.Properties.Resources.Save_6530;
            this.button_savepdochanges.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_savepdochanges.Location = new System.Drawing.Point(192, 161);
            this.button_savepdochanges.Name = "button_savepdochanges";
            this.button_savepdochanges.Size = new System.Drawing.Size(101, 27);
            this.button_savepdochanges.TabIndex = 15;
            this.button_savepdochanges.Text = "Save ";
            this.button_savepdochanges.UseVisualStyleBackColor = true;
            this.button_savepdochanges.Click += new System.EventHandler(this.button_savepdochanges_Click);
            // 
            // button_deletePDO
            // 
            this.button_deletePDO.Image = global::ODEditor.Properties.Resources.Remove_16xLG;
            this.button_deletePDO.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_deletePDO.Location = new System.Drawing.Point(192, 128);
            this.button_deletePDO.Name = "button_deletePDO";
            this.button_deletePDO.Size = new System.Drawing.Size(101, 27);
            this.button_deletePDO.TabIndex = 14;
            this.button_deletePDO.Text = "Delete PDO";
            this.button_deletePDO.UseVisualStyleBackColor = true;
            this.button_deletePDO.Click += new System.EventHandler(this.button_deletePDO_Click);
            // 
            // textBox_slot
            // 
            this.textBox_slot.Location = new System.Drawing.Point(80, 21);
            this.textBox_slot.Name = "textBox_slot";
            this.textBox_slot.ReadOnly = true;
            this.textBox_slot.Size = new System.Drawing.Size(96, 20);
            this.textBox_slot.TabIndex = 11;
            // 
            // textBox_syncstart
            // 
            this.textBox_syncstart.Location = new System.Drawing.Point(80, 168);
            this.textBox_syncstart.Name = "textBox_syncstart";
            this.textBox_syncstart.Size = new System.Drawing.Size(96, 20);
            this.textBox_syncstart.TabIndex = 10;
            // 
            // textBox_eventtimer
            // 
            this.textBox_eventtimer.Location = new System.Drawing.Point(80, 142);
            this.textBox_eventtimer.Name = "textBox_eventtimer";
            this.textBox_eventtimer.Size = new System.Drawing.Size(96, 20);
            this.textBox_eventtimer.TabIndex = 9;
            // 
            // textBox_inhibit
            // 
            this.textBox_inhibit.Location = new System.Drawing.Point(80, 111);
            this.textBox_inhibit.Name = "textBox_inhibit";
            this.textBox_inhibit.Size = new System.Drawing.Size(96, 20);
            this.textBox_inhibit.TabIndex = 8;
            // 
            // textBox_type
            // 
            this.textBox_type.Location = new System.Drawing.Point(80, 78);
            this.textBox_type.Name = "textBox_type";
            this.textBox_type.Size = new System.Drawing.Size(96, 20);
            this.textBox_type.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sync start";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Inhibit";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Event Timer";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Type";
            // 
            // textBox_cob
            // 
            this.textBox_cob.Location = new System.Drawing.Point(80, 47);
            this.textBox_cob.Name = "textBox_cob";
            this.textBox_cob.Size = new System.Drawing.Size(96, 20);
            this.textBox_cob.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "COB";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Slot";
            // 
            // listView_TXCOBmap
            // 
            this.listView_TXCOBmap.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader10,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listView_TXCOBmap.FullRowSelect = true;
            this.listView_TXCOBmap.HideSelection = false;
            this.listView_TXCOBmap.Location = new System.Drawing.Point(16, 255);
            this.listView_TXCOBmap.Name = "listView_TXCOBmap";
            this.listView_TXCOBmap.Size = new System.Drawing.Size(954, 307);
            this.listView_TXCOBmap.TabIndex = 10;
            this.listView_TXCOBmap.UseCompatibleStateImageBehavior = false;
            this.listView_TXCOBmap.View = System.Windows.Forms.View.Details;
            this.listView_TXCOBmap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_TXCOBmap_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "COB";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Slot";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Byte 0";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Byte 1";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Byte 2";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Byte 3";
            this.columnHeader5.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Byte 4";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Byte 5";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Byte 6";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Byte7";
            this.columnHeader9.Width = 100;
            // 
            // button_addPDO
            // 
            this.button_addPDO.Image = global::ODEditor.Properties.Resources.action_add_16xLG;
            this.button_addPDO.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_addPDO.Location = new System.Drawing.Point(832, 38);
            this.button_addPDO.Name = "button_addPDO";
            this.button_addPDO.Size = new System.Drawing.Size(117, 27);
            this.button_addPDO.TabIndex = 13;
            this.button_addPDO.Text = "Add new PDO";
            this.button_addPDO.UseVisualStyleBackColor = true;
            this.button_addPDO.Click += new System.EventHandler(this.button_addPDO_Click);
            // 
            // DevicePDOView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_addPDO);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView_TXCOBmap);
            this.Controls.Add(this.label_availableobjects);
            this.Controls.Add(this.listView_TXPDO);
            this.Name = "DevicePDOView";
            this.Size = new System.Drawing.Size(973, 695);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_availableobjects;
        private System.Windows.Forms.ListView listView_TXPDO;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private CustomListView.ListViewEx listView_TXCOBmap;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_cob;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_syncstart;
        private System.Windows.Forms.TextBox textBox_eventtimer;
        private System.Windows.Forms.TextBox textBox_inhibit;
        private System.Windows.Forms.TextBox textBox_type;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_slot;
        private System.Windows.Forms.Button button_addPDO;
        private System.Windows.Forms.Button button_savepdochanges;
        private System.Windows.Forms.Button button_deletePDO;
    }
}

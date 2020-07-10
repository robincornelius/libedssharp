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
            this.checkBox_invalidpdo = new System.Windows.Forms.CheckBox();
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
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.label1.Location = new System.Drawing.Point(23, 293);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Assigned PDO mapping";
            // 
            // label_availableobjects
            // 
            this.label_availableobjects.AutoSize = true;
            this.label_availableobjects.Location = new System.Drawing.Point(17, 16);
            this.label_availableobjects.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_availableobjects.Name = "label_availableobjects";
            this.label_availableobjects.Size = new System.Drawing.Size(172, 17);
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
            this.listView_TXPDO.Location = new System.Drawing.Point(21, 36);
            this.listView_TXPDO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listView_TXPDO.MultiSelect = false;
            this.listView_TXPDO.Name = "listView_TXPDO";
            this.listView_TXPDO.Size = new System.Drawing.Size(655, 244);
            this.listView_TXPDO.TabIndex = 0;
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
            this.groupBox1.Controls.Add(this.checkBox_invalidpdo);
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
            this.groupBox1.Location = new System.Drawing.Point(695, 34);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(393, 245);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication parameters";
            // 
            // checkBox_invalidpdo
            // 
            this.checkBox_invalidpdo.AutoSize = true;
            this.checkBox_invalidpdo.Location = new System.Drawing.Point(256, 60);
            this.checkBox_invalidpdo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBox_invalidpdo.Name = "checkBox_invalidpdo";
            this.checkBox_invalidpdo.Size = new System.Drawing.Size(70, 21);
            this.checkBox_invalidpdo.TabIndex = 2;
            this.checkBox_invalidpdo.Text = "Invalid";
            this.checkBox_invalidpdo.UseVisualStyleBackColor = true;
            this.checkBox_invalidpdo.CheckedChanged += new System.EventHandler(this.CheckBox_invalidpdo_CheckedChanged);
            // 
            // button_savepdochanges
            // 
            this.button_savepdochanges.Image = global::ODEditor.Properties.Resources.Save_6530;
            this.button_savepdochanges.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_savepdochanges.Location = new System.Drawing.Point(256, 198);
            this.button_savepdochanges.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_savepdochanges.Name = "button_savepdochanges";
            this.button_savepdochanges.Size = new System.Drawing.Size(135, 33);
            this.button_savepdochanges.TabIndex = 8;
            this.button_savepdochanges.Text = "Save ";
            this.button_savepdochanges.UseVisualStyleBackColor = true;
            this.button_savepdochanges.Click += new System.EventHandler(this.Button_savepdochanges_Click);
            // 
            // button_deletePDO
            // 
            this.button_deletePDO.Image = global::ODEditor.Properties.Resources.Remove_16xLG;
            this.button_deletePDO.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_deletePDO.Location = new System.Drawing.Point(256, 158);
            this.button_deletePDO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_deletePDO.Name = "button_deletePDO";
            this.button_deletePDO.Size = new System.Drawing.Size(135, 33);
            this.button_deletePDO.TabIndex = 7;
            this.button_deletePDO.Text = "Delete PDO";
            this.button_deletePDO.UseVisualStyleBackColor = true;
            this.button_deletePDO.Click += new System.EventHandler(this.Button_deletePDO_Click);
            // 
            // textBox_slot
            // 
            this.textBox_slot.Location = new System.Drawing.Point(107, 26);
            this.textBox_slot.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_slot.Name = "textBox_slot";
            this.textBox_slot.ReadOnly = true;
            this.textBox_slot.Size = new System.Drawing.Size(127, 22);
            this.textBox_slot.TabIndex = 0;
            // 
            // textBox_syncstart
            // 
            this.textBox_syncstart.Location = new System.Drawing.Point(107, 207);
            this.textBox_syncstart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_syncstart.Name = "textBox_syncstart";
            this.textBox_syncstart.Size = new System.Drawing.Size(127, 22);
            this.textBox_syncstart.TabIndex = 6;
            // 
            // textBox_eventtimer
            // 
            this.textBox_eventtimer.Location = new System.Drawing.Point(107, 175);
            this.textBox_eventtimer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_eventtimer.Name = "textBox_eventtimer";
            this.textBox_eventtimer.Size = new System.Drawing.Size(127, 22);
            this.textBox_eventtimer.TabIndex = 5;
            // 
            // textBox_inhibit
            // 
            this.textBox_inhibit.Location = new System.Drawing.Point(107, 137);
            this.textBox_inhibit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_inhibit.Name = "textBox_inhibit";
            this.textBox_inhibit.Size = new System.Drawing.Size(127, 22);
            this.textBox_inhibit.TabIndex = 4;
            // 
            // textBox_type
            // 
            this.textBox_type.Location = new System.Drawing.Point(107, 96);
            this.textBox_type.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_type.Name = "textBox_type";
            this.textBox_type.Size = new System.Drawing.Size(127, 22);
            this.textBox_type.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 213);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 17);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sync start";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 140);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Inhibit";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 178);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Event Timer";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 100);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Type";
            // 
            // textBox_cob
            // 
            this.textBox_cob.Location = new System.Drawing.Point(107, 58);
            this.textBox_cob.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_cob.Name = "textBox_cob";
            this.textBox_cob.Size = new System.Drawing.Size(127, 22);
            this.textBox_cob.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "COB";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Slot";
            // 
            // listView_TXCOBmap
            // 
            this.listView_TXCOBmap.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
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
            this.listView_TXCOBmap.Location = new System.Drawing.Point(21, 313);
            this.listView_TXCOBmap.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listView_TXCOBmap.Name = "listView_TXCOBmap";
            this.listView_TXCOBmap.Size = new System.Drawing.Size(1329, 377);
            this.listView_TXCOBmap.TabIndex = 2;
            this.listView_TXCOBmap.UseCompatibleStateImageBehavior = false;
            this.listView_TXCOBmap.View = System.Windows.Forms.View.Details;
            this.listView_TXCOBmap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListView_TXCOBmap_MouseClick);
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Num";
            this.columnHeader11.Width = 38;
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 2;
            this.columnHeader1.Text = "COB";
            this.columnHeader1.Width = 88;
            // 
            // columnHeader10
            // 
            this.columnHeader10.DisplayIndex = 1;
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
            this.columnHeader9.Width = 105;
            // 
            // button_addPDO
            // 
            this.button_addPDO.Image = global::ODEditor.Properties.Resources.action_add_16xLG;
            this.button_addPDO.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_addPDO.Location = new System.Drawing.Point(1109, 47);
            this.button_addPDO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_addPDO.Name = "button_addPDO";
            this.button_addPDO.Size = new System.Drawing.Size(156, 33);
            this.button_addPDO.TabIndex = 1;
            this.button_addPDO.Text = "Add new PDO";
            this.button_addPDO.UseVisualStyleBackColor = true;
            this.button_addPDO.Click += new System.EventHandler(this.Button_addPDO_Click);
            // 
            // DevicePDOView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_addPDO);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView_TXCOBmap);
            this.Controls.Add(this.label_availableobjects);
            this.Controls.Add(this.listView_TXPDO);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DevicePDOView";
            this.Size = new System.Drawing.Size(1356, 855);
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
        private System.Windows.Forms.CheckBox checkBox_invalidpdo;
        private System.Windows.Forms.ColumnHeader columnHeader11;
    }
}

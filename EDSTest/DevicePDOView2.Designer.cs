namespace ODEditor
{
    partial class DevicePDOView2
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
            this.components = new System.ComponentModel.Container();
            this.button_addPDO = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_mapping = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
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
            this.label_availableobjects = new System.Windows.Forms.Label();
            this.listView_TXPDO = new System.Windows.Forms.ListView();
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grid1 = new SourceGrid.Grid();
            this.button_down = new System.Windows.Forms.Button();
            this.button_up = new System.Windows.Forms.Button();
            this.contextMenuStrip_removeitem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_removeitem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_insert = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip_removeitem.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_addPDO
            // 
            this.button_addPDO.Image = global::ODEditor.Properties.Resources.action_add_16xLG;
            this.button_addPDO.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_addPDO.Location = new System.Drawing.Point(1178, 32);
            this.button_addPDO.Margin = new System.Windows.Forms.Padding(4);
            this.button_addPDO.Name = "button_addPDO";
            this.button_addPDO.Size = new System.Drawing.Size(156, 33);
            this.button_addPDO.TabIndex = 15;
            this.button_addPDO.Text = "Add new PDO";
            this.button_addPDO.UseVisualStyleBackColor = true;
            this.button_addPDO.Click += new System.EventHandler(this.button_addPDO_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_mapping);
            this.groupBox1.Controls.Add(this.label1);
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
            this.groupBox1.Location = new System.Drawing.Point(704, 19);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(453, 254);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication parameters";
            // 
            // textBox_mapping
            // 
            this.textBox_mapping.Location = new System.Drawing.Point(130, 58);
            this.textBox_mapping.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_mapping.Name = "textBox_mapping";
            this.textBox_mapping.ReadOnly = true;
            this.textBox_mapping.Size = new System.Drawing.Size(127, 22);
            this.textBox_mapping.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Mapping";
            // 
            // checkBox_invalidpdo
            // 
            this.checkBox_invalidpdo.AutoSize = true;
            this.checkBox_invalidpdo.Location = new System.Drawing.Point(277, 94);
            this.checkBox_invalidpdo.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox_invalidpdo.Name = "checkBox_invalidpdo";
            this.checkBox_invalidpdo.Size = new System.Drawing.Size(70, 21);
            this.checkBox_invalidpdo.TabIndex = 2;
            this.checkBox_invalidpdo.Text = "Invalid";
            this.checkBox_invalidpdo.UseVisualStyleBackColor = true;
            this.checkBox_invalidpdo.CheckedChanged += new System.EventHandler(this.checkBox_invalidpdo_CheckedChanged);
            // 
            // button_savepdochanges
            // 
            this.button_savepdochanges.Image = global::ODEditor.Properties.Resources.Save_6530;
            this.button_savepdochanges.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_savepdochanges.Location = new System.Drawing.Point(277, 198);
            this.button_savepdochanges.Margin = new System.Windows.Forms.Padding(4);
            this.button_savepdochanges.Name = "button_savepdochanges";
            this.button_savepdochanges.Size = new System.Drawing.Size(135, 33);
            this.button_savepdochanges.TabIndex = 8;
            this.button_savepdochanges.Text = "Save ";
            this.button_savepdochanges.UseVisualStyleBackColor = true;
            this.button_savepdochanges.Click += new System.EventHandler(this.button_savepdochanges_Click_1);
            // 
            // button_deletePDO
            // 
            this.button_deletePDO.Image = global::ODEditor.Properties.Resources.Remove_16xLG;
            this.button_deletePDO.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_deletePDO.Location = new System.Drawing.Point(277, 158);
            this.button_deletePDO.Margin = new System.Windows.Forms.Padding(4);
            this.button_deletePDO.Name = "button_deletePDO";
            this.button_deletePDO.Size = new System.Drawing.Size(135, 33);
            this.button_deletePDO.TabIndex = 7;
            this.button_deletePDO.Text = "Delete PDO";
            this.button_deletePDO.UseVisualStyleBackColor = true;
            this.button_deletePDO.Click += new System.EventHandler(this.button_deletePDO_Click);
            // 
            // textBox_slot
            // 
            this.textBox_slot.Location = new System.Drawing.Point(130, 25);
            this.textBox_slot.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_slot.Name = "textBox_slot";
            this.textBox_slot.Size = new System.Drawing.Size(127, 22);
            this.textBox_slot.TabIndex = 0;
            // 
            // textBox_syncstart
            // 
            this.textBox_syncstart.Location = new System.Drawing.Point(130, 223);
            this.textBox_syncstart.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_syncstart.Name = "textBox_syncstart";
            this.textBox_syncstart.Size = new System.Drawing.Size(127, 22);
            this.textBox_syncstart.TabIndex = 6;
            // 
            // textBox_eventtimer
            // 
            this.textBox_eventtimer.Location = new System.Drawing.Point(130, 190);
            this.textBox_eventtimer.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_eventtimer.Name = "textBox_eventtimer";
            this.textBox_eventtimer.Size = new System.Drawing.Size(127, 22);
            this.textBox_eventtimer.TabIndex = 5;
            // 
            // textBox_inhibit
            // 
            this.textBox_inhibit.Location = new System.Drawing.Point(130, 157);
            this.textBox_inhibit.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_inhibit.Name = "textBox_inhibit";
            this.textBox_inhibit.Size = new System.Drawing.Size(127, 22);
            this.textBox_inhibit.TabIndex = 4;
            // 
            // textBox_type
            // 
            this.textBox_type.Location = new System.Drawing.Point(130, 124);
            this.textBox_type.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_type.Name = "textBox_type";
            this.textBox_type.Size = new System.Drawing.Size(127, 22);
            this.textBox_type.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 227);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 17);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sync start";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 161);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Inhibit";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 194);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Event Timer";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 128);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Type";
            // 
            // textBox_cob
            // 
            this.textBox_cob.Location = new System.Drawing.Point(130, 91);
            this.textBox_cob.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_cob.Name = "textBox_cob";
            this.textBox_cob.Size = new System.Drawing.Size(127, 22);
            this.textBox_cob.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 95);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "COB";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 29);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Communication";
            // 
            // label_availableobjects
            // 
            this.label_availableobjects.AutoSize = true;
            this.label_availableobjects.Location = new System.Drawing.Point(37, 9);
            this.label_availableobjects.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_availableobjects.Name = "label_availableobjects";
            this.label_availableobjects.Size = new System.Drawing.Size(172, 17);
            this.label_availableobjects.TabIndex = 18;
            this.label_availableobjects.Text = "Available Objects for PDO";
            // 
            // listView_TXPDO
            // 
            this.listView_TXPDO.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader13,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader1});
            this.listView_TXPDO.FullRowSelect = true;
            this.listView_TXPDO.HideSelection = false;
            this.listView_TXPDO.Location = new System.Drawing.Point(41, 29);
            this.listView_TXPDO.Margin = new System.Windows.Forms.Padding(4);
            this.listView_TXPDO.MultiSelect = false;
            this.listView_TXPDO.Name = "listView_TXPDO";
            this.listView_TXPDO.Size = new System.Drawing.Size(655, 244);
            this.listView_TXPDO.TabIndex = 17;
            this.listView_TXPDO.UseCompatibleStateImageBehavior = false;
            this.listView_TXPDO.View = System.Windows.Forms.View.Details;
            this.listView_TXPDO.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView_TXPDO_ItemDrag);
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
            // grid1
            // 
            this.grid1.AllowDrop = true;
            this.grid1.DefaultWidth = 18;
            this.grid1.EnableSort = true;
            this.grid1.Location = new System.Drawing.Point(40, 300);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(1377, 354);
            this.grid1.TabIndex = 21;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            this.grid1.DragDrop += new System.Windows.Forms.DragEventHandler(this.grid1_DragDrop);
            this.grid1.DragEnter += new System.Windows.Forms.DragEventHandler(this.grid1_DragEnter);
            this.grid1.DragOver += new System.Windows.Forms.DragEventHandler(this.grid1_DragOver);
            // 
            // button_down
            // 
            this.button_down.Location = new System.Drawing.Point(42, 659);
            this.button_down.Name = "button_down";
            this.button_down.Size = new System.Drawing.Size(92, 37);
            this.button_down.TabIndex = 22;
            this.button_down.Text = "Zoom Out";
            this.button_down.UseVisualStyleBackColor = true;
            this.button_down.Click += new System.EventHandler(this.button_down_Click);
            // 
            // button_up
            // 
            this.button_up.Location = new System.Drawing.Point(140, 659);
            this.button_up.Name = "button_up";
            this.button_up.Size = new System.Drawing.Size(100, 37);
            this.button_up.TabIndex = 23;
            this.button_up.Text = "Zoom In";
            this.button_up.UseVisualStyleBackColor = true;
            this.button_up.Click += new System.EventHandler(this.button_up_Click);
            // 
            // contextMenuStrip_removeitem
            // 
            this.contextMenuStrip_removeitem.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_removeitem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_removeitem,
            this.toolStripMenuItem_insert});
            this.contextMenuStrip_removeitem.Name = "contextMenuStrip_removeitem";
            this.contextMenuStrip_removeitem.Size = new System.Drawing.Size(167, 52);
            this.contextMenuStrip_removeitem.Text = "Remove Item";
            // 
            // toolStripMenuItem_removeitem
            // 
            this.toolStripMenuItem_removeitem.Name = "toolStripMenuItem_removeitem";
            this.toolStripMenuItem_removeitem.Size = new System.Drawing.Size(166, 24);
            this.toolStripMenuItem_removeitem.Tag = "remove";
            this.toolStripMenuItem_removeitem.Text = "Remove Item";
            // 
            // toolStripMenuItem_insert
            // 
            this.toolStripMenuItem_insert.Name = "toolStripMenuItem_insert";
            this.toolStripMenuItem_insert.Size = new System.Drawing.Size(166, 24);
            this.toolStripMenuItem_insert.Tag = "insert";
            this.toolStripMenuItem_insert.Text = "Insert Item";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Bits";
            // 
            // DevicePDOView2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.button_up);
            this.Controls.Add(this.button_down);
            this.Controls.Add(this.grid1);
            this.Controls.Add(this.label_availableobjects);
            this.Controls.Add(this.listView_TXPDO);
            this.Controls.Add(this.button_addPDO);
            this.Controls.Add(this.groupBox1);
            this.Name = "DevicePDOView2";
            this.Size = new System.Drawing.Size(1423, 701);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip_removeitem.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_addPDO;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_invalidpdo;
        private System.Windows.Forms.Button button_deletePDO;
        private System.Windows.Forms.TextBox textBox_slot;
        private System.Windows.Forms.TextBox textBox_syncstart;
        private System.Windows.Forms.TextBox textBox_eventtimer;
        private System.Windows.Forms.TextBox textBox_inhibit;
        private System.Windows.Forms.TextBox textBox_type;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_cob;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_availableobjects;
        private System.Windows.Forms.ListView listView_TXPDO;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private SourceGrid.Grid grid1;
        private System.Windows.Forms.Button button_down;
        private System.Windows.Forms.Button button_up;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_removeitem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_removeitem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_insert;
        private System.Windows.Forms.TextBox textBox_mapping;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_savepdochanges;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

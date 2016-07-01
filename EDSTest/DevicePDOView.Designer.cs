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
            this.listView_TXPDOslots = new System.Windows.Forms.ListView();
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader23 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label34 = new System.Windows.Forms.Label();
            this.listView_configuredTXPDO = new System.Windows.Forms.ListView();
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader21 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader22 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label33 = new System.Windows.Forms.Label();
            this.listView_TXPDO = new System.Windows.Forms.ListView();
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView_TXPDOslots
            // 
            this.listView_TXPDOslots.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader18,
            this.columnHeader23});
            this.listView_TXPDOslots.FullRowSelect = true;
            this.listView_TXPDOslots.Location = new System.Drawing.Point(16, 272);
            this.listView_TXPDOslots.Name = "listView_TXPDOslots";
            this.listView_TXPDOslots.Size = new System.Drawing.Size(314, 199);
            this.listView_TXPDOslots.TabIndex = 9;
            this.listView_TXPDOslots.UseCompatibleStateImageBehavior = false;
            this.listView_TXPDOslots.View = System.Windows.Forms.View.Details;
            this.listView_TXPDOslots.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_TXPDOslots_MouseClick);
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Index";
            // 
            // columnHeader23
            // 
            this.columnHeader23.Text = "COB";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(384, 256);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(101, 13);
            this.label34.TabIndex = 8;
            this.label34.Text = "Configured TX PDO";
            // 
            // listView_configuredTXPDO
            // 
            this.listView_configuredTXPDO.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader19,
            this.columnHeader20,
            this.columnHeader21,
            this.columnHeader22});
            this.listView_configuredTXPDO.Location = new System.Drawing.Point(387, 272);
            this.listView_configuredTXPDO.Name = "listView_configuredTXPDO";
            this.listView_configuredTXPDO.Size = new System.Drawing.Size(314, 199);
            this.listView_configuredTXPDO.TabIndex = 7;
            this.listView_configuredTXPDO.UseCompatibleStateImageBehavior = false;
            this.listView_configuredTXPDO.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Index";
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "Sub";
            this.columnHeader20.Width = 39;
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "Name";
            this.columnHeader21.Width = 151;
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "Bytes";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(13, 13);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(147, 13);
            this.label33.TabIndex = 6;
            this.label33.Text = "Available Objects for TX PDO";
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
            // DevicePDOView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView_TXPDOslots);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.listView_configuredTXPDO);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.listView_TXPDO);
            this.Name = "DevicePDOView";
            this.Size = new System.Drawing.Size(760, 542);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView_TXPDOslots;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader23;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.ListView listView_configuredTXPDO;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.ColumnHeader columnHeader21;
        private System.Windows.Forms.ColumnHeader columnHeader22;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.ListView listView_TXPDO;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
    }
}

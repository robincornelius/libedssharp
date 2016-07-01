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
            this.label33 = new System.Windows.Forms.Label();
            this.listView_TXPDO = new System.Windows.Forms.ListView();
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView_TXCOBmap = new CustomListView.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
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
            // listView_TXCOBmap
            // 
            this.listView_TXCOBmap.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listView_TXCOBmap.FullRowSelect = true;
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
            // DevicePDOView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView_TXCOBmap);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.listView_TXPDO);
            this.Name = "DevicePDOView";
            this.Size = new System.Drawing.Size(973, 695);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label33;
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
    }
}

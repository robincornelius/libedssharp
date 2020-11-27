namespace ODEditor
{
    partial class ReportView
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.toolStripButton_print = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_preview = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_save = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.webBrowser1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(952, 541);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(952, 566);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_print,
            this.toolStripButton_preview,
            this.toolStripButton_save});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(81, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "Print";
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(952, 541);
            this.webBrowser1.TabIndex = 0;
            // 
            // toolStripButton_print
            // 
            this.toolStripButton_print.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_print.Image = global::ODEditor.Properties.Resources.Print_11009;
            this.toolStripButton_print.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_print.Name = "toolStripButton_print";
            this.toolStripButton_print.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_print.Text = "Print";
            this.toolStripButton_print.Click += new System.EventHandler(this.toolStripButton_print_Click);
            // 
            // toolStripButton_preview
            // 
            this.toolStripButton_preview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_preview.Image = global::ODEditor.Properties.Resources.PrintPreviewControl_698;
            this.toolStripButton_preview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_preview.Name = "toolStripButton_preview";
            this.toolStripButton_preview.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_preview.Text = "Print preview";
            this.toolStripButton_preview.Click += new System.EventHandler(this.toolStripButton_preview_Click);
            // 
            // toolStripButton_save
            // 
            this.toolStripButton_save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_save.Image = global::ODEditor.Properties.Resources.Save_6530;
            this.toolStripButton_save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_save.Name = "toolStripButton_save";
            this.toolStripButton_save.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_save.Text = "Save";
            this.toolStripButton_save.Click += new System.EventHandler(this.toolStripButton_save_Click);
            // 
            // ReportView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 566);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "ReportView";
            this.Text = "ReportView";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_print;
        private System.Windows.Forms.ToolStripButton toolStripButton_preview;
        private System.Windows.Forms.ToolStripButton toolStripButton_save;
    }
}
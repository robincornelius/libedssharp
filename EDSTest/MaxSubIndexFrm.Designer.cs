namespace ODEditor
{
    partial class MaxSubIndexFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaxSubIndexFrm));
            this.numericUpDown_maxsubindex = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_noelements = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maxsubindex)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown_maxsubindex
            // 
            this.numericUpDown_maxsubindex.Location = new System.Drawing.Point(143, 72);
            this.numericUpDown_maxsubindex.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown_maxsubindex.Name = "numericUpDown_maxsubindex";
            this.numericUpDown_maxsubindex.Size = new System.Drawing.Size(69, 20);
            this.numericUpDown_maxsubindex.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Max subindex";
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(30, 140);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(84, 26);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(167, 140);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(84, 26);
            this.button_cancel.TabIndex = 3;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Number of elements";
            // 
            // textBox_noelements
            // 
            this.textBox_noelements.Location = new System.Drawing.Point(143, 44);
            this.textBox_noelements.Name = "textBox_noelements";
            this.textBox_noelements.ReadOnly = true;
            this.textBox_noelements.Size = new System.Drawing.Size(69, 20);
            this.textBox_noelements.TabIndex = 5;
            // 
            // MaxSubIndexFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 178);
            this.Controls.Add(this.textBox_noelements);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown_maxsubindex);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MaxSubIndexFrm";
            this.Text = "Change max subindex";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maxsubindex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown_maxsubindex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_noelements;
    }
}
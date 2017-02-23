namespace ODEditor
{
    partial class DeviceInfoView
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
            this.textBox_concretenodeid = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.button_update_devfile_info = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox_txpdos = new System.Windows.Forms.TextBox();
            this.textBox_rxpdos = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox_productnumber = new System.Windows.Forms.TextBox();
            this.textBox_productname = new System.Windows.Forms.TextBox();
            this.textBox_vendornumber = new System.Windows.Forms.TextBox();
            this.textBox_vendorname = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_modifiedby = new System.Windows.Forms.TextBox();
            this.textBox_modified_datetime = new System.Windows.Forms.TextBox();
            this.textBox_createdby = new System.Windows.Forms.TextBox();
            this.textBox_create_datetime = new System.Windows.Forms.TextBox();
            this.textBox_di_description = new System.Windows.Forms.TextBox();
            this.textBox_edsversionm = new System.Windows.Forms.TextBox();
            this.textBox_filerevision = new System.Windows.Forms.TextBox();
            this.textBox_fileversion = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_Gran = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.checkBox_lss = new System.Windows.Forms.CheckBox();
            this.checkBox_boot_master = new System.Windows.Forms.CheckBox();
            this.checkBox_group_msg = new System.Windows.Forms.CheckBox();
            this.checkBox_bootslave = new System.Windows.Forms.CheckBox();
            this.checkBox_compactPDO = new System.Windows.Forms.CheckBox();
            this.checkBox_dynamicchan = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.heckBox_baud_125 = new System.Windows.Forms.CheckBox();
            this.checkBox_baud_10 = new System.Windows.Forms.CheckBox();
            this.heckBox_baud_20 = new System.Windows.Forms.CheckBox();
            this.heckBox_baud_50 = new System.Windows.Forms.CheckBox();
            this.heckBox_baud_250 = new System.Windows.Forms.CheckBox();
            this.heckBox_baud_1000 = new System.Windows.Forms.CheckBox();
            this.heckBox_baud_500 = new System.Windows.Forms.CheckBox();
            this.heckBox_baud_800 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_devicefilename = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_deviceedsname = new System.Windows.Forms.TextBox();
            this.textBox_exportfolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_concretenodeid
            // 
            this.textBox_concretenodeid.Location = new System.Drawing.Point(502, 246);
            this.textBox_concretenodeid.Name = "textBox_concretenodeid";
            this.textBox_concretenodeid.Size = new System.Drawing.Size(80, 20);
            this.textBox_concretenodeid.TabIndex = 54;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(405, 253);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(91, 13);
            this.label32.TabIndex = 53;
            this.label32.Text = "Concrete node ID";
            // 
            // button_update_devfile_info
            // 
            this.button_update_devfile_info.Location = new System.Drawing.Point(513, 312);
            this.button_update_devfile_info.Name = "button_update_devfile_info";
            this.button_update_devfile_info.Size = new System.Drawing.Size(113, 35);
            this.button_update_devfile_info.TabIndex = 52;
            this.button_update_devfile_info.Text = "Update";
            this.button_update_devfile_info.UseVisualStyleBackColor = true;
            this.button_update_devfile_info.Click += new System.EventHandler(this.button_update_devfile_info_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox_txpdos);
            this.groupBox5.Controls.Add(this.textBox_rxpdos);
            this.groupBox5.Controls.Add(this.label30);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Location = new System.Drawing.Point(13, 395);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(377, 118);
            this.groupBox5.TabIndex = 51;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "PDOs";
            // 
            // textBox_txpdos
            // 
            this.textBox_txpdos.Location = new System.Drawing.Point(100, 49);
            this.textBox_txpdos.Name = "textBox_txpdos";
            this.textBox_txpdos.ReadOnly = true;
            this.textBox_txpdos.Size = new System.Drawing.Size(101, 20);
            this.textBox_txpdos.TabIndex = 41;
            // 
            // textBox_rxpdos
            // 
            this.textBox_rxpdos.Location = new System.Drawing.Point(100, 24);
            this.textBox_rxpdos.Name = "textBox_rxpdos";
            this.textBox_rxpdos.ReadOnly = true;
            this.textBox_rxpdos.Size = new System.Drawing.Size(101, 20);
            this.textBox_rxpdos.TabIndex = 40;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(12, 52);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(69, 13);
            this.label30.TabIndex = 39;
            this.label30.Text = "No TX PDOs";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 27);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 38;
            this.label17.Text = "No RX PDOs";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox_productnumber);
            this.groupBox4.Controls.Add(this.textBox_productname);
            this.groupBox4.Controls.Add(this.textBox_vendornumber);
            this.groupBox4.Controls.Add(this.textBox_vendorname);
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.label28);
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Location = new System.Drawing.Point(13, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(377, 134);
            this.groupBox4.TabIndex = 50;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Device Info";
            // 
            // textBox_productnumber
            // 
            this.textBox_productnumber.Location = new System.Drawing.Point(137, 45);
            this.textBox_productnumber.Name = "textBox_productnumber";
            this.textBox_productnumber.Size = new System.Drawing.Size(226, 20);
            this.textBox_productnumber.TabIndex = 41;
            // 
            // textBox_productname
            // 
            this.textBox_productname.Location = new System.Drawing.Point(137, 19);
            this.textBox_productname.Name = "textBox_productname";
            this.textBox_productname.Size = new System.Drawing.Size(226, 20);
            this.textBox_productname.TabIndex = 40;
            // 
            // textBox_vendornumber
            // 
            this.textBox_vendornumber.Location = new System.Drawing.Point(137, 101);
            this.textBox_vendornumber.Name = "textBox_vendornumber";
            this.textBox_vendornumber.Size = new System.Drawing.Size(226, 20);
            this.textBox_vendornumber.TabIndex = 39;
            // 
            // textBox_vendorname
            // 
            this.textBox_vendorname.Location = new System.Drawing.Point(137, 75);
            this.textBox_vendorname.Name = "textBox_vendorname";
            this.textBox_vendorname.Size = new System.Drawing.Size(226, 20);
            this.textBox_vendorname.TabIndex = 38;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(9, 48);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(82, 13);
            this.label29.TabIndex = 37;
            this.label29.Text = "Product number";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(9, 25);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(73, 13);
            this.label28.TabIndex = 36;
            this.label28.Text = "Product name";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(9, 104);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(79, 13);
            this.label27.TabIndex = 35;
            this.label27.Text = "Vendor number";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(9, 78);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(70, 13);
            this.label26.TabIndex = 34;
            this.label26.Text = "Vendor name";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox_modifiedby);
            this.groupBox3.Controls.Add(this.textBox_modified_datetime);
            this.groupBox3.Controls.Add(this.textBox_createdby);
            this.groupBox3.Controls.Add(this.textBox_create_datetime);
            this.groupBox3.Controls.Add(this.textBox_di_description);
            this.groupBox3.Controls.Add(this.textBox_edsversionm);
            this.groupBox3.Controls.Add(this.textBox_filerevision);
            this.groupBox3.Controls.Add(this.textBox_fileversion);
            this.groupBox3.Controls.Add(this.label25);
            this.groupBox3.Controls.Add(this.label24);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Location = new System.Drawing.Point(13, 156);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(377, 220);
            this.groupBox3.TabIndex = 49;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "EDS File Info";
            // 
            // textBox_modifiedby
            // 
            this.textBox_modifiedby.Location = new System.Drawing.Point(139, 189);
            this.textBox_modifiedby.Name = "textBox_modifiedby";
            this.textBox_modifiedby.Size = new System.Drawing.Size(226, 20);
            this.textBox_modifiedby.TabIndex = 33;
            // 
            // textBox_modified_datetime
            // 
            this.textBox_modified_datetime.Location = new System.Drawing.Point(139, 164);
            this.textBox_modified_datetime.Name = "textBox_modified_datetime";
            this.textBox_modified_datetime.Size = new System.Drawing.Size(226, 20);
            this.textBox_modified_datetime.TabIndex = 32;
            // 
            // textBox_createdby
            // 
            this.textBox_createdby.Location = new System.Drawing.Point(139, 141);
            this.textBox_createdby.Name = "textBox_createdby";
            this.textBox_createdby.Size = new System.Drawing.Size(226, 20);
            this.textBox_createdby.TabIndex = 31;
            // 
            // textBox_create_datetime
            // 
            this.textBox_create_datetime.Location = new System.Drawing.Point(139, 116);
            this.textBox_create_datetime.Name = "textBox_create_datetime";
            this.textBox_create_datetime.Size = new System.Drawing.Size(226, 20);
            this.textBox_create_datetime.TabIndex = 30;
            // 
            // textBox_di_description
            // 
            this.textBox_di_description.Location = new System.Drawing.Point(139, 92);
            this.textBox_di_description.Name = "textBox_di_description";
            this.textBox_di_description.Size = new System.Drawing.Size(226, 20);
            this.textBox_di_description.TabIndex = 29;
            // 
            // textBox_edsversionm
            // 
            this.textBox_edsversionm.Location = new System.Drawing.Point(139, 69);
            this.textBox_edsversionm.Name = "textBox_edsversionm";
            this.textBox_edsversionm.Size = new System.Drawing.Size(226, 20);
            this.textBox_edsversionm.TabIndex = 28;
            // 
            // textBox_filerevision
            // 
            this.textBox_filerevision.Location = new System.Drawing.Point(139, 44);
            this.textBox_filerevision.Name = "textBox_filerevision";
            this.textBox_filerevision.Size = new System.Drawing.Size(226, 20);
            this.textBox_filerevision.TabIndex = 27;
            // 
            // textBox_fileversion
            // 
            this.textBox_fileversion.Location = new System.Drawing.Point(139, 22);
            this.textBox_fileversion.Name = "textBox_fileversion";
            this.textBox_fileversion.Size = new System.Drawing.Size(226, 20);
            this.textBox_fileversion.TabIndex = 26;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(11, 192);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(61, 13);
            this.label25.TabIndex = 25;
            this.label25.Text = "Modified by";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(11, 167);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(118, 13);
            this.label24.TabIndex = 24;
            this.label24.Text = "Modification Date/Time";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(11, 144);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(59, 13);
            this.label23.TabIndex = 23;
            this.label23.Text = "Created By";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(11, 119);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(100, 13);
            this.label22.TabIndex = 22;
            this.label22.Text = "Creation Date/Time";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(11, 95);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(60, 13);
            this.label21.TabIndex = 21;
            this.label21.Text = "Description";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(11, 72);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(66, 13);
            this.label20.TabIndex = 20;
            this.label20.Text = "EDS version";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(11, 47);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(62, 13);
            this.label19.TabIndex = 19;
            this.label19.Text = "File revision";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 25);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(60, 13);
            this.label18.TabIndex = 18;
            this.label18.Text = "File version";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_Gran);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Controls.Add(this.checkBox_lss);
            this.groupBox2.Controls.Add(this.checkBox_boot_master);
            this.groupBox2.Controls.Add(this.checkBox_group_msg);
            this.groupBox2.Controls.Add(this.checkBox_bootslave);
            this.groupBox2.Controls.Add(this.checkBox_compactPDO);
            this.groupBox2.Controls.Add(this.checkBox_dynamicchan);
            this.groupBox2.Location = new System.Drawing.Point(522, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 224);
            this.groupBox2.TabIndex = 48;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Device settings";
            // 
            // textBox_Gran
            // 
            this.textBox_Gran.Location = new System.Drawing.Point(68, 166);
            this.textBox_Gran.Name = "textBox_Gran";
            this.textBox_Gran.Size = new System.Drawing.Size(59, 20);
            this.textBox_Gran.TabIndex = 44;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(3, 169);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(57, 13);
            this.label31.TabIndex = 41;
            this.label31.Text = "Granularity";
            // 
            // checkBox_lss
            // 
            this.checkBox_lss.AutoSize = true;
            this.checkBox_lss.Location = new System.Drawing.Point(6, 140);
            this.checkBox_lss.Name = "checkBox_lss";
            this.checkBox_lss.Size = new System.Drawing.Size(98, 17);
            this.checkBox_lss.TabIndex = 40;
            this.checkBox_lss.Text = "LSS Supported";
            this.checkBox_lss.UseVisualStyleBackColor = true;
            // 
            // checkBox_boot_master
            // 
            this.checkBox_boot_master.AutoSize = true;
            this.checkBox_boot_master.Location = new System.Drawing.Point(6, 48);
            this.checkBox_boot_master.Name = "checkBox_boot_master";
            this.checkBox_boot_master.Size = new System.Drawing.Size(127, 17);
            this.checkBox_boot_master.TabIndex = 36;
            this.checkBox_boot_master.Text = "Simple bootup master";
            this.checkBox_boot_master.UseVisualStyleBackColor = true;
            // 
            // checkBox_group_msg
            // 
            this.checkBox_group_msg.AutoSize = true;
            this.checkBox_group_msg.Location = new System.Drawing.Point(6, 117);
            this.checkBox_group_msg.Name = "checkBox_group_msg";
            this.checkBox_group_msg.Size = new System.Drawing.Size(109, 17);
            this.checkBox_group_msg.TabIndex = 39;
            this.checkBox_group_msg.Text = "Group Messaging";
            this.checkBox_group_msg.UseVisualStyleBackColor = true;
            // 
            // checkBox_bootslave
            // 
            this.checkBox_bootslave.AutoSize = true;
            this.checkBox_bootslave.Location = new System.Drawing.Point(6, 25);
            this.checkBox_bootslave.Name = "checkBox_bootslave";
            this.checkBox_bootslave.Size = new System.Drawing.Size(121, 17);
            this.checkBox_bootslave.TabIndex = 35;
            this.checkBox_bootslave.Text = "Simple bootup slave";
            this.checkBox_bootslave.UseVisualStyleBackColor = true;
            // 
            // checkBox_compactPDO
            // 
            this.checkBox_compactPDO.AutoSize = true;
            this.checkBox_compactPDO.Location = new System.Drawing.Point(6, 94);
            this.checkBox_compactPDO.Name = "checkBox_compactPDO";
            this.checkBox_compactPDO.Size = new System.Drawing.Size(94, 17);
            this.checkBox_compactPDO.TabIndex = 38;
            this.checkBox_compactPDO.Text = "Compact PDO";
            this.checkBox_compactPDO.UseVisualStyleBackColor = true;
            // 
            // checkBox_dynamicchan
            // 
            this.checkBox_dynamicchan.AutoSize = true;
            this.checkBox_dynamicchan.Location = new System.Drawing.Point(6, 71);
            this.checkBox_dynamicchan.Name = "checkBox_dynamicchan";
            this.checkBox_dynamicchan.Size = new System.Drawing.Size(163, 17);
            this.checkBox_dynamicchan.TabIndex = 37;
            this.checkBox_dynamicchan.Text = "Dynamic channels supported";
            this.checkBox_dynamicchan.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.heckBox_baud_125);
            this.groupBox1.Controls.Add(this.checkBox_baud_10);
            this.groupBox1.Controls.Add(this.heckBox_baud_20);
            this.groupBox1.Controls.Add(this.heckBox_baud_50);
            this.groupBox1.Controls.Add(this.heckBox_baud_250);
            this.groupBox1.Controls.Add(this.heckBox_baud_1000);
            this.groupBox1.Controls.Add(this.heckBox_baud_500);
            this.groupBox1.Controls.Add(this.heckBox_baud_800);
            this.groupBox1.Location = new System.Drawing.Point(396, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(120, 224);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Baudrates";
            // 
            // heckBox_baud_125
            // 
            this.heckBox_baud_125.AutoSize = true;
            this.heckBox_baud_125.Location = new System.Drawing.Point(18, 100);
            this.heckBox_baud_125.Name = "heckBox_baud_125";
            this.heckBox_baud_125.Size = new System.Drawing.Size(70, 17);
            this.heckBox_baud_125.TabIndex = 25;
            this.heckBox_baud_125.Text = "125 kbps";
            this.heckBox_baud_125.UseVisualStyleBackColor = true;
            // 
            // checkBox_baud_10
            // 
            this.checkBox_baud_10.AutoSize = true;
            this.checkBox_baud_10.Location = new System.Drawing.Point(18, 25);
            this.checkBox_baud_10.Name = "checkBox_baud_10";
            this.checkBox_baud_10.Size = new System.Drawing.Size(64, 17);
            this.checkBox_baud_10.TabIndex = 22;
            this.checkBox_baud_10.Text = "10 kbps";
            this.checkBox_baud_10.UseVisualStyleBackColor = true;
            // 
            // heckBox_baud_20
            // 
            this.heckBox_baud_20.AutoSize = true;
            this.heckBox_baud_20.Location = new System.Drawing.Point(18, 48);
            this.heckBox_baud_20.Name = "heckBox_baud_20";
            this.heckBox_baud_20.Size = new System.Drawing.Size(64, 17);
            this.heckBox_baud_20.TabIndex = 23;
            this.heckBox_baud_20.Text = "20 kbps";
            this.heckBox_baud_20.UseVisualStyleBackColor = true;
            // 
            // heckBox_baud_50
            // 
            this.heckBox_baud_50.AutoSize = true;
            this.heckBox_baud_50.Location = new System.Drawing.Point(18, 73);
            this.heckBox_baud_50.Name = "heckBox_baud_50";
            this.heckBox_baud_50.Size = new System.Drawing.Size(64, 17);
            this.heckBox_baud_50.TabIndex = 24;
            this.heckBox_baud_50.Text = "50 kbps";
            this.heckBox_baud_50.UseVisualStyleBackColor = true;
            // 
            // heckBox_baud_250
            // 
            this.heckBox_baud_250.AutoSize = true;
            this.heckBox_baud_250.Location = new System.Drawing.Point(18, 123);
            this.heckBox_baud_250.Name = "heckBox_baud_250";
            this.heckBox_baud_250.Size = new System.Drawing.Size(70, 17);
            this.heckBox_baud_250.TabIndex = 26;
            this.heckBox_baud_250.Text = "250 kbps";
            this.heckBox_baud_250.UseVisualStyleBackColor = true;
            // 
            // heckBox_baud_1000
            // 
            this.heckBox_baud_1000.AutoSize = true;
            this.heckBox_baud_1000.Location = new System.Drawing.Point(18, 192);
            this.heckBox_baud_1000.Name = "heckBox_baud_1000";
            this.heckBox_baud_1000.Size = new System.Drawing.Size(76, 17);
            this.heckBox_baud_1000.TabIndex = 29;
            this.heckBox_baud_1000.Text = "1000 kbps";
            this.heckBox_baud_1000.UseVisualStyleBackColor = true;
            // 
            // heckBox_baud_500
            // 
            this.heckBox_baud_500.AutoSize = true;
            this.heckBox_baud_500.Location = new System.Drawing.Point(18, 146);
            this.heckBox_baud_500.Name = "heckBox_baud_500";
            this.heckBox_baud_500.Size = new System.Drawing.Size(70, 17);
            this.heckBox_baud_500.TabIndex = 27;
            this.heckBox_baud_500.Text = "500 kbps";
            this.heckBox_baud_500.UseVisualStyleBackColor = true;
            // 
            // heckBox_baud_800
            // 
            this.heckBox_baud_800.AutoSize = true;
            this.heckBox_baud_800.Location = new System.Drawing.Point(18, 169);
            this.heckBox_baud_800.Name = "heckBox_baud_800";
            this.heckBox_baud_800.Size = new System.Drawing.Size(70, 17);
            this.heckBox_baud_800.TabIndex = 28;
            this.heckBox_baud_800.Text = "800 kbps";
            this.heckBox_baud_800.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(398, 379);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 55;
            this.label1.Text = "Device XML file name";
            // 
            // textBox_devicefilename
            // 
            this.textBox_devicefilename.Location = new System.Drawing.Point(400, 395);
            this.textBox_devicefilename.Name = "textBox_devicefilename";
            this.textBox_devicefilename.ReadOnly = true;
            this.textBox_devicefilename.Size = new System.Drawing.Size(335, 20);
            this.textBox_devicefilename.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(398, 426);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 56;
            this.label2.Text = "Device EDS file name";
            // 
            // textBox_deviceedsname
            // 
            this.textBox_deviceedsname.Location = new System.Drawing.Point(400, 440);
            this.textBox_deviceedsname.Name = "textBox_deviceedsname";
            this.textBox_deviceedsname.ReadOnly = true;
            this.textBox_deviceedsname.Size = new System.Drawing.Size(335, 20);
            this.textBox_deviceedsname.TabIndex = 57;
            // 
            // textBox_exportfolder
            // 
            this.textBox_exportfolder.Location = new System.Drawing.Point(400, 483);
            this.textBox_exportfolder.Name = "textBox_exportfolder";
            this.textBox_exportfolder.ReadOnly = true;
            this.textBox_exportfolder.Size = new System.Drawing.Size(335, 20);
            this.textBox_exportfolder.TabIndex = 58;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(398, 467);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 59;
            this.label3.Text = "Export location";
            // 
            // DeviceInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_exportfolder);
            this.Controls.Add(this.textBox_deviceedsname);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_devicefilename);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_concretenodeid);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.button_update_devfile_info);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "DeviceInfoView";
            this.Size = new System.Drawing.Size(754, 525);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_concretenodeid;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Button button_update_devfile_info;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBox_txpdos;
        private System.Windows.Forms.TextBox textBox_rxpdos;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox_productnumber;
        private System.Windows.Forms.TextBox textBox_productname;
        private System.Windows.Forms.TextBox textBox_vendornumber;
        private System.Windows.Forms.TextBox textBox_vendorname;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox_modifiedby;
        private System.Windows.Forms.TextBox textBox_modified_datetime;
        private System.Windows.Forms.TextBox textBox_createdby;
        private System.Windows.Forms.TextBox textBox_create_datetime;
        private System.Windows.Forms.TextBox textBox_di_description;
        private System.Windows.Forms.TextBox textBox_edsversionm;
        private System.Windows.Forms.TextBox textBox_filerevision;
        private System.Windows.Forms.TextBox textBox_fileversion;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_Gran;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox checkBox_lss;
        private System.Windows.Forms.CheckBox checkBox_boot_master;
        private System.Windows.Forms.CheckBox checkBox_group_msg;
        private System.Windows.Forms.CheckBox checkBox_bootslave;
        private System.Windows.Forms.CheckBox checkBox_compactPDO;
        private System.Windows.Forms.CheckBox checkBox_dynamicchan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox heckBox_baud_125;
        private System.Windows.Forms.CheckBox checkBox_baud_10;
        private System.Windows.Forms.CheckBox heckBox_baud_20;
        private System.Windows.Forms.CheckBox heckBox_baud_50;
        private System.Windows.Forms.CheckBox heckBox_baud_250;
        private System.Windows.Forms.CheckBox heckBox_baud_1000;
        private System.Windows.Forms.CheckBox heckBox_baud_500;
        private System.Windows.Forms.CheckBox heckBox_baud_800;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_devicefilename;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_deviceedsname;
        private System.Windows.Forms.TextBox textBox_exportfolder;
        private System.Windows.Forms.Label label3;
    }
}

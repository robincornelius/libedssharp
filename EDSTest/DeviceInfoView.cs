/*
    This file is part of libEDSsharp.

    libEDSsharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    libEDSsharp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with libEDSsharp.  If not, see <http://www.gnu.org/licenses/>.

    Copyright(c) 2016 - 2019 Robin Cornelius <robin.cornelius@gmail.com>
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libEDSsharp;

namespace ODEditor
{
    public partial class DeviceInfoView : MyTabUserControl
    {
        public EDSsharp eds = null;

        public DeviceInfoView()
        {
            InitializeComponent();
        }

        public void populatedeviceinfo()
        {
            if (eds == null)
                return;

            textBox_productname.Text = eds.di.ProductName;
            textBox_productnumber.Text = eds.di.ProductNumber.ToString();
            textBox_vendorname.Text = eds.di.VendorName;
            textBox_vendornumber.Text = eds.di.VendorNumber.ToString();

            textBox_fileversion.Text = eds.fi.EDSVersion;
            textBox_modified_datetime.Text = eds.fi.ModificationDateTime.ToLongDateString();
            textBox_modifiedby.Text = eds.fi.ModifiedBy;

            textBox_filerevision.Text = eds.fi.FileRevision.ToString();
            textBox_fileversion.Text = eds.fi.FileVersion.ToString();

            textBox_createdby.Text = eds.fi.CreatedBy;
            textBox_create_datetime.Text = eds.fi.CreationDateTime.ToString();

            textBox_di_description.Text = eds.fi.Description;

            textBox_edsversionm.Text = eds.fi.EDSVersion;

            //textBox_fileversion.Text = eds.di

            checkBox_baud_10.Checked = eds.di.BaudRate_10;
            heckBox_baud_20.Checked = eds.di.BaudRate_20;
            heckBox_baud_50.Checked = eds.di.BaudRate_50;
            heckBox_baud_125.Checked = eds.di.BaudRate_125;
            heckBox_baud_250.Checked = eds.di.BaudRate_250;
            heckBox_baud_500.Checked = eds.di.BaudRate_500;
            heckBox_baud_800.Checked = eds.di.BaudRate_800;
            heckBox_baud_1000.Checked = eds.di.BaudRate_1000;

            checkBox_boot_master.Checked = eds.di.SimpleBootUpMaster;
            checkBox_bootslave.Checked = eds.di.SimpleBootUpSlave;
            textBox_compactPDO.Text = eds.di.CompactPDO.ToString();
            checkBox_group_msg.Checked = eds.di.GroupMessaging;
            checkBox_dynamicchan.Checked = eds.di.DynamicChannelsSupported;
            checkBox_lss.Checked = eds.di.LSS_Supported;
            if (eds.di.LSS_Type == "Client")
            {
                comboBox_lss.SelectedItem = "Client";
            }
            else
            {
                comboBox_lss.SelectedItem = "Server";
            }
            textBox_Gran.Text = eds.di.Granularity.ToString();

            textBox_rxpdos.Text = eds.di.NrOfRXPDO.ToString();
            textBox_txpdos.Text = eds.di.NrOfTXPDO.ToString();

            if (eds.dc.NodeId == 0)
            {
                textBox_concretenodeid.Text = "";
            }
            else
            {
                textBox_concretenodeid.Text = eds.dc.NodeId.ToString();
            }


            if(eds.edsfilename!=null)
                textBox_deviceedsname.Text = eds.edsfilename;

            if (eds.dcffilename != null)
                textBox_devicedcfname.Text = eds.dcffilename;

            if (eds.xmlfilename != null)
                textBox_devicefilename.Text = eds.xmlfilename;

            if(eds.fi.exportFolder !=null)
                textBox_exportfolder.Text = eds.fi.exportFolder;


            //DCF support

            if(eds.dc!=null)
            {
                textBox_concretenodeid.Text = eds.dc.NodeId.ToString();
                textBox_nodename.Text = eds.dc.NodeName;
                textBox_baudrate.Text = eds.dc.BaudRate.ToString();
                textBox_netnum.Text = eds.dc.NetNumber.ToString();
                checkBox_canopenmanager.Checked = eds.dc.CANopenManager;
                textBox_lssserial.Text = eds.dc.LSS_SerialNumber.ToString();

            }



        }

        private void button_update_devfile_info_Click(object sender, EventArgs e)
        {
            if (eds == null)
                return;

            try
            {
                eds.di.ProductName = textBox_productname.Text;
                eds.di.ProductNumber = Convert.ToUInt32(textBox_productnumber.Text);

                eds.di.VendorName = textBox_vendorname.Text;
                eds.di.VendorNumber = Convert.ToUInt32(textBox_vendornumber.Text);

                eds.fi.EDSVersion = textBox_fileversion.Text;

                eds.fi.ModificationDateTime = DateTime.Now;
                textBox_modified_datetime.Text = DateTime.Now.ToString();

                eds.fi.ModifiedBy = textBox_modifiedby.Text;

                eds.fi.FileRevision = Convert.ToByte(textBox_filerevision.Text);

                eds.fi.FileVersion = Convert.ToByte(textBox_fileversion.Text);


                eds.fi.CreatedBy = textBox_createdby.Text;
                eds.fi.CreationDateTime = DateTime.Parse(textBox_create_datetime.Text);

                eds.fi.Description = textBox_di_description.Text;

                eds.fi.EDSVersion = textBox_edsversionm.Text;


                eds.di.BaudRate_10 = checkBox_baud_10.Checked;
                eds.di.BaudRate_20 = heckBox_baud_20.Checked;
                eds.di.BaudRate_50 = heckBox_baud_50.Checked;
                eds.di.BaudRate_125 = heckBox_baud_125.Checked;
                eds.di.BaudRate_250 = heckBox_baud_250.Checked;
                eds.di.BaudRate_500 = heckBox_baud_500.Checked;
                eds.di.BaudRate_800 = heckBox_baud_800.Checked;
                eds.di.BaudRate_1000 = heckBox_baud_1000.Checked;

                eds.di.SimpleBootUpMaster = checkBox_boot_master.Checked;
                eds.di.SimpleBootUpSlave = checkBox_bootslave.Checked;

                eds.di.GroupMessaging = checkBox_group_msg.Checked;
                eds.di.DynamicChannelsSupported = checkBox_dynamicchan.Checked;
                eds.di.LSS_Supported = checkBox_lss.Checked;
                eds.di.LSS_Type = comboBox_lss.SelectedItem.ToString();
                eds.di.Granularity = Convert.ToByte(textBox_Gran.Text);

                eds.dc.NodeId = 0;
                if (textBox_concretenodeid.Text != "")
                {
                    eds.dc.NodeId = Convert.ToByte(textBox_concretenodeid.Text);
                }


                doUpdatePDOs();

                //These are read only and auto calculated
                //textBox_rxpdos.Text = eds.di.NrOfRXPDO.ToString();
                //textBox_txpdos.Text = eds.di.NrOfTXPDO.ToString();


                //DCF support

                eds.dc.NodeId = Convert.ToByte(textBox_concretenodeid.Text);
                eds.dc.NodeName = textBox_nodename.Text;
                eds.dc.BaudRate = Convert.ToUInt16(textBox_baudrate.Text);
                eds.dc.NetNumber = Convert.ToUInt32(textBox_netnum.Text);
                eds.dc.CANopenManager = checkBox_canopenmanager.Checked;
                eds.dc.LSS_SerialNumber = Convert.ToUInt32(textBox_lssserial.Text);

                eds.Dirty = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed, reason :-\n" + ex.ToString());
            }

        }

        private void checkBox_lss_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_lss.Checked)
            {
                comboBox_lss.Enabled = true;
            }
            else
            {
                comboBox_lss.Enabled = false;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

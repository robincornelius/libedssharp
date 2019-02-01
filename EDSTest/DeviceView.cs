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
using System.Globalization;
using Xml2CSharp;

namespace ODEditor
{
    public partial class DeviceView : MyTabUserControl
    {

        public EDSsharp eds;

        public DeviceView()
        {
          
            InitializeComponent();
            
            foreach(TabPage tp in tabControl1.TabPages)
            {
                foreach(Object o in tp.Controls)
                {
                    if (o is MyTabUserControl)
                    {
                        MyTabUserControl t = (MyTabUserControl)o;

                        t.UpdateDeviceInfo += dispatch_updatedevice;
                        t.UpdateOD += dispatch_updateOD;
                        t.UpdatePDOs += dispatch_updatePDOinfo;
                    }
                }
            }

            devicePDOView1.Init(true);
            devicePDOView2.Init(false);
     
        }

        #region UpdateDispatchEvents

        // This region handles update requests that are dispatched to the various user controls on the tabs

        public void dispatch_updatedevice()
        {
            if (eds == null)
                return;

            deviceInfoView.eds = eds;
            deviceInfoView.populatedeviceinfo();


            moduleInfo1.eds = eds;
            moduleInfo1.populatemoduleinfo();
        }

        public void dispatch_updatePDOinfo()
        {
            if (eds == null)
                return;

            devicePDOView1.eds = eds;
            devicePDOView1.UpdatePDOinfo();

            devicePDOView2.eds = eds;
            devicePDOView2.UpdatePDOinfo();

        }

        public void dispatch_updateOD()
        {
            if (eds == null)
                return;

            deviceODView1.eds = eds;
            deviceODView1.populatememorytypes();
            deviceODView1.populateindexlists();
            deviceODView1.validateanddisplaydata();
            deviceODView1.updateselectedindexdisplay();



        }

        #endregion

        private void deviceInfoView_Load(object sender, EventArgs e)
        {

        }
  
 
    }
}

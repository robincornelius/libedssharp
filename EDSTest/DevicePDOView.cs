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
    public partial class DevicePDOView : UserControl
    {
        public EDSsharp eds = null;

        public DevicePDOView()
        {
            InitializeComponent();
        }

        public void updatePDOinfo()
        {
            listView_TXPDO.Items.Clear();
            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                int index = kvp.Key;

                if (od.Disabled == true)
                    continue;

                if (od.objecttype == ObjectType.VAR && (od.PDOtype == PDOMappingType.optional || od.PDOtype == PDOMappingType.TPDO))
                {
                    addTXPDOoption(od);
                }

                foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                {
                    ODentry odsub = kvp2.Value;

                    if (odsub.PDOtype == PDOMappingType.optional || odsub.PDOtype == PDOMappingType.TPDO)
                    {
                        addTXPDOoption(odsub);
                    }
                }

            }

            listView_TXPDOslots.Items.Clear();

            for (UInt16 idx = 0x1800; idx < 0x18ff; idx++)
            {
                if (eds.ods.ContainsKey(idx))
                {
                    ODentry od = eds.ods[idx];
                    if (od.Disabled == true)
                        continue;

                    ListViewItem lvi = new ListViewItem(String.Format("0x{0:x4}", idx));
                    lvi.Tag = od;

                    //fixme process $NODEID etc
                    lvi.SubItems.Add(od.subobjects[1].defaultvalue);

                    listView_TXPDOslots.Items.Add(lvi);
                }
            }




        }

        private void addTXPDOoption(ODentry od)
        {

            ListViewItem lvi = new ListViewItem(String.Format("0x{0:x4}", od.index));
            lvi.SubItems.Add(String.Format("0x{0:x2}", od.subindex));
            lvi.SubItems.Add(od.parameter_name);
            lvi.SubItems.Add(od.datatype.ToString());
            lvi.Tag = (object)od;

            listView_TXPDO.Items.Add(lvi);


        }

        private void listView_TXPDOslots_MouseClick(object sender, MouseEventArgs e)
        {

            listView_configuredTXPDO.Items.Clear();
            if (listView_TXPDOslots.SelectedItems.Count > 0)
            {
                ODentry od = (ODentry)listView_TXPDOslots.SelectedItems[0].Tag;

                UInt16 idx = (UInt16)(od.index + 0x200);

                ODentry oddef = eds.ods[idx];

                foreach (KeyValuePair<UInt16, ODentry> kvp in oddef.subobjects)
                {
                    ODentry sub = kvp.Value;
                    if (sub.subindex == 0)
                        continue;

                    UInt32 data = Convert.ToUInt32(sub.defaultvalue, 16);

                    if (data == 0)
                        continue;

                    //format is 0x6000 01 08
                    byte datasize = (byte)(data & 0x000000FF);
                    UInt16 pdoindex = (UInt16)((data >> 16) & 0x0000FFFF);
                    byte pdosub = (byte)((data >> 8) & 0x000000FF);

                    ListViewItem lvi = new ListViewItem(string.Format("0x{0:x4}", pdoindex));
                    lvi.SubItems.Add(string.Format("0x{0:x2}", pdosub));

                    //fixme sanity checking here please
                    if (!eds.ods.ContainsKey(pdoindex))
                        continue;

                    ODentry targetod = eds.ods[pdoindex];

                    if (pdosub != 0)
                    {
                        //fixme sanity checking here please
                        if (!targetod.subobjects.ContainsKey(pdosub))
                            continue;
                        targetod = targetod.subobjects[pdosub];
                    }

                    lvi.SubItems.Add(string.Format("{0}", targetod.parameter_name));

                    lvi.SubItems.Add(string.Format("{0}", datasize / 8));

                    listView_configuredTXPDO.Items.Add(lvi);


                }
            }

        }
 
    }
}

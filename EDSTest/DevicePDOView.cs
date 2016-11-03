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
using System.Collections.Specialized;

namespace ODEditor
{
    public partial class DevicePDOView : MyTabUserControl
    {
        public EDSsharp eds = null;
        StringCollection TXchoices = new StringCollection();
        bool isTXPDO;

        UInt16 startcob = 0x1800;
      
        public DevicePDOView()
        {
          
            InitializeComponent();
            listView_TXCOBmap.onComboBoxIndexChanged += listView_TXCOBmap_onComboBoxIndexChanged;
        }

        public void init(bool isTXPDO)
        {
            this.isTXPDO = isTXPDO;

            if (isTXPDO == false)
            {
                startcob = 0x1400;
            }

        }

        public void updatePDOinfo()
        {

            listView_TXCOBmap.FullRowSelect = true;
            listView_TXCOBmap.GridLines = true;

            TXchoices.Clear();

            TXchoices.Add(String.Format("empty"));

            listView_TXPDO.Items.Clear();
            foreach (KeyValuePair<UInt16, ODentry> kvp in eds.ods)
            {
                ODentry od = kvp.Value;
                int index = kvp.Key;

                if (od.Disabled == true)
                    continue;

                if (od.objecttype == ObjectType.VAR && (od.PDOtype == PDOMappingType.optional || (isTXPDO && (od.PDOtype == PDOMappingType.TPDO)) || (!isTXPDO && (od.PDOtype == PDOMappingType.RPDO))))
                {
                    addTXPDOoption(od);
                }

                foreach (KeyValuePair<UInt16, ODentry> kvp2 in od.subobjects)
                {
                    ODentry odsub = kvp2.Value;

                    if (odsub.subindex == 0)
                        continue;

                    if (odsub.PDOtype == PDOMappingType.optional || (isTXPDO && (odsub.PDOtype == PDOMappingType.TPDO)) || (!isTXPDO && (odsub.PDOtype == PDOMappingType.RPDO)))
                    {
                        addTXPDOoption(odsub);
                    }
                }

            }

            // Clean out any existing TX cob entries for this device.
            List<UInt16> removes = new List<ushort>();
            foreach (KeyValuePair<UInt16, EDSsharp> kvp in ODEditor_MainForm.TXCobMap)
            {
                if (kvp.Value == eds)
                    removes.Add(kvp.Key);

            }
            foreach (UInt16 u in removes)
                ODEditor_MainForm.TXCobMap.Remove(u);

            int row=0;

            listView_TXCOBmap.Items.Clear();

      

         
            for (UInt16 idx = startcob; idx < startcob+0x01ff; idx++)
            {
                if (eds.ods.ContainsKey(idx))
                {
                    ODentry od = eds.ods[idx];
                    if (od.Disabled == true)
                        continue;

                    //protect againt not completed new CommunicationParamater sections
                    //we probably could do better and do more checking but as long as
                    //we protect against the subobjects[1] read in a few lines all else is
                    //good
                    if (od.subobjects.Count < 1)
                        continue;

                    ListViewItem lvi = new ListViewItem(String.Format("0x{0:x4}", idx));
                    lvi.Tag = od;

                   

                    UInt16 cob = eds.GetNodeID(od.subobjects[1].defaultvalue);
                    lvi.SubItems.Add(String.Format("0x{0:x3}",cob));

                    if (!ODEditor_MainForm.TXCobMap.ContainsKey(cob))
                        ODEditor_MainForm.TXCobMap.Add(cob, eds);

                    ListViewItem lvi2 = new ListViewItem(String.Format("0x{0:x4}", cob));
                    lvi2.SubItems.Add(string.Format("{0:x4}",idx));       
                    lvi2.SubItems.Add("   ");
                    lvi2.SubItems.Add("   ");
                    lvi2.SubItems.Add("   ");
                    lvi2.SubItems.Add("   ");
                    lvi2.SubItems.Add("   ");
                    lvi2.SubItems.Add("   ");
                    lvi2.SubItems.Add("   ");
                    lvi2.SubItems.Add("   ");


                    listView_TXCOBmap.Items.Add(lvi2);

                    updatePDOTXslot(od, row);


                    row++;

                  
                }
            }

        }

        private void addTXPDOoption(ODentry od)
        {

            TXchoices.Add(String.Format("0x{0:x4}/{1:x2}/", od.index, od.subindex) + od.parameter_name);

            ListViewItem lvi = new ListViewItem(String.Format("0x{0:x4}", od.index));
            lvi.SubItems.Add(String.Format("0x{0:x2}", od.subindex));
            lvi.SubItems.Add(od.parameter_name);

            DataType dt = od.datatype;
            if (dt == DataType.UNKNOWN || od.parent !=null)
                dt = od.parent.datatype;
            lvi.SubItems.Add(dt.ToString());

            lvi.Tag = (object)od;

            listView_TXPDO.Items.Add(lvi);

        }

        void updatePDOTXslot(ODentry od , int row)
        {
           
            UInt16 idx = (UInt16)(od.index + 0x200);

            ODentry oddef = eds.ods[idx];

            int byteoff = 0;

            foreach (KeyValuePair<UInt16, ODentry> kvp in oddef.subobjects)
            {
                if (byteoff >= 8)
                    continue;

                ODentry sub = kvp.Value;
                if (sub.subindex == 0)
                    continue;



                UInt32 data = Convert.ToUInt32(sub.defaultvalue, EDSsharp.getbase(sub.defaultvalue));

                if (data == 0) //FIX ME also include dummy usage here
                {
                    listView_TXCOBmap.AddComboBoxCell(row, byteoff + 2, TXchoices);
                    listView_TXCOBmap.Items[row].SubItems[byteoff + 2].Text = "empty";
                    byteoff++;
                    continue;
                }           

                //format is 0x6000 01 08
                byte datasize = (byte)(data & 0x000000FF);
                UInt16 pdoindex = (UInt16)((data >> 16) & 0x0000FFFF);
                byte pdosub = (byte)((data >> 8) & 0x000000FF);

                //fixme sanity checking here please
                if (!eds.ods.ContainsKey(pdoindex))
                    continue;

                ODentry targetod = eds.ods[pdoindex];

                if(pdosub!=0)
                {
                    targetod = targetod.subobjects[pdosub];
                }

                listView_TXCOBmap.AddComboBoxCell(row, byteoff+2, TXchoices);
   
                String target = String.Format("0x{0:x4}/{1:x2}/", targetod.index, targetod.subindex) + targetod.parameter_name;
                listView_TXCOBmap.Items[row].SubItems[byteoff+2].Text = target;

                int PDOdatasize = targetod.sizeofdatatype();
               
                while (PDOdatasize != 1)
                {
                    listView_TXCOBmap.Items[row].SubItems[byteoff + PDOdatasize+1].Text = " - ";
                    PDOdatasize--;

                }

                byteoff += targetod.sizeofdatatype();

              

            }
        }

        void listView_TXCOBmap_onComboBoxIndexChanged(int row, int col, string Text)
        {
          
            //row+0x1a00 will be the slot to adjust



            UInt16 slot = (UInt16)(0x200 + Convert.ToUInt16(listView_TXCOBmap.Items[row].SubItems[1].Text, 16));
            ODentry slotod = eds.ods[slot];

            //Now rebuild the entire slot working out data size as we go

            for(byte p=1;p<slotod.subobjects.Count;p++)
            {
                slotod.subobjects[p].defaultvalue = "0x00000000";
            }

            byte subcount = 1;
            int totaldatalength = 0;

            ListViewItem item = listView_TXCOBmap.Items[row];
            foreach(ListViewItem.ListViewSubItem subitem in item.SubItems)
            {
                if (subitem.Text == "" || subitem.Text == " - " || subitem.Text == "   ")
                    continue;

                string[] bits = subitem.Text.Split('/');
                if (bits.Length != 3) //ignore the first column
                    continue;
                UInt16 index = Convert.ToUInt16(bits[0], 16);
                Byte sub = Convert.ToByte(bits[1], 16);

                ODentry od = eds.ods[index];
                if(sub!=0)
                    od = od.subobjects[sub];

                //fixme for non basic types will this work?? i think
                //its not even allowed for PDO but need trap in code to
                //prevent this and throw error here
                int datalength = 8* od.sizeofdatatype();

                totaldatalength += datalength;

                if(totaldatalength>64)
                {
                    MessageBox.Show(String.Format("Too much data in TX PDO {0}", slotod.index));
                    break;
                }

                string value = string.Format("0x{0:x4}{1:x2}{2:x2}", index, sub, datalength);

                if (subcount >= slotod.subobjects.Count())
                {
                    MessageBox.Show("PDO Mapping array is too small, please add more elements in OD editor");
                    break;
                }

                slotod.subobjects[subcount].defaultvalue = value;

                subcount++;
                
            }

            //write out the number of objects used into the sub object count [0]
            slotod.subobjects[0].defaultvalue = string.Format("{0}", subcount-1);

            updatePDOinfo();
            doUpdateOD();
            

        }

        private void listView_TXCOBmap_MouseClick(object sender, MouseEventArgs e)
        {

        }
 
    }
}

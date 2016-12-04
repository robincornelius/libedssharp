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

            listView_TXPDO.DoubleBuffering(true);
            listView_TXCOBmap.DoubleBuffering(true);

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

            textBox_eventtimer.Enabled = false;
            textBox_inhibit.Enabled = false;
            textBox_syncstart.Enabled = false;
            textBox_type.Enabled = false;
            textBox_cob.Enabled = false;

            button_deletePDO.Enabled = false;
            button_savepdochanges.Enabled = false;

            listView_TXCOBmap.FullRowSelect = true;
            listView_TXCOBmap.GridLines = true;

            TXchoices.Clear();

            TXchoices.Add(String.Format("empty"));

            //TXchoices.Add(string.Format("Dummy Bool")); //not sure how this works at all for a bit???

            TXchoices.Add(string.Format("0x002/00/Dummy Int8"));
            TXchoices.Add(string.Format("0x003/00/Dummy Int16"));
            TXchoices.Add(string.Format("0x004/00/Dummy Int32"));
            TXchoices.Add(string.Format("0x005/00/Dummy UInt8"));
            TXchoices.Add(string.Format("0x006/00/Dummy UInt16"));
            TXchoices.Add(string.Format("0x007/00/Dummy UInt32"));


            listView_TXPDO.BeginUpdate();

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

            listView_TXPDO.EndUpdate();

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

            listView_TXCOBmap.BeginUpdate();
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
                    if (od.subobjects.Count <= 1)
                        continue;

                    ListViewItem lvi = new ListViewItem(String.Format("0x{0:x3}", idx));
                    lvi.Tag = od;

                   

                    UInt16 cob = eds.GetNodeID(od.subobjects[1].defaultvalue);
                    lvi.SubItems.Add(String.Format("0x{0:x3}",cob));

                    if (!ODEditor_MainForm.TXCobMap.ContainsKey(cob))
                        ODEditor_MainForm.TXCobMap.Add(cob, eds);

                    ListViewItem lvi2 = new ListViewItem(String.Format("0x{0:x3}", cob));
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

            listView_TXCOBmap.EndUpdate();

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

            if (!eds.ods.ContainsKey(idx))
                return;

            ODentry oddef = eds.ods[idx];

            int byteoff = 0;

            foreach (KeyValuePair<UInt16, ODentry> kvp in oddef.subobjects)
            {
                if (byteoff >= 8)
                    continue;

                ODentry sub = kvp.Value;
                if (sub.subindex == 0)
                    continue;

                UInt32 data = 0;
                if (sub.defaultvalue != "")
                    data = Convert.ToUInt32(sub.defaultvalue, EDSsharp.getbase(sub.defaultvalue));

                if (data == 0)
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

                String target = "";
                int PDOdatasize = 0;

                //dummy objects
                if (pdoindex>=0x0002 && pdoindex<=0x007)
                {
                    //the dummy objects
                    switch (pdoindex)
                    {
                        case 0x002:
                            target = "0x0002/00/Dummy Int8";
                            PDOdatasize = 1;
                            break;
                        case 0x003:
                            target = "0x0002/00/Dummy Int16";
                            PDOdatasize = 2;
                            break;
                        case 0x004:
                            target = "0x0002/00/Dummy Int32";
                            PDOdatasize = 4;
                            break;
                        case 0x005:
                            target = "0x0002/00/Dummy UInt8";
                            PDOdatasize = 1;
                            break;
                        case 0x006:
                            target = "0x0002/00/Dummy UInt16";
                            PDOdatasize = 2;
                            break;
                        case 0x007:
                            target = "0x0002/00/Dummy UInt32";
                            PDOdatasize = 4;
                            break;     
                    }

                    if (PDOdatasize == 0)
                        continue;
                 
                }
                else
                {
                    //fixme sanity checking here please
                    if (!eds.ods.ContainsKey(pdoindex))
                        continue;

                    ODentry targetod = eds.ods[pdoindex];

                    if (pdosub != 0)
                    {
                        //FIXME direct sub array access, unprotected and will fault with holes in range
                        targetod = targetod.subobjects[pdosub];
                    }

                    target = String.Format("0x{0:x4}/{1:x2}/", targetod.index, targetod.subindex) + targetod.parameter_name;
                    PDOdatasize = targetod.sizeofdatatype();
                }
                

                listView_TXCOBmap.AddComboBoxCell(row, byteoff+2, TXchoices);
                listView_TXCOBmap.Items[row].SubItems[byteoff+2].Text = target;

                int oldPDOdatasize = PDOdatasize;

                while (oldPDOdatasize != 1)
                {
                    listView_TXCOBmap.Items[row].SubItems[byteoff + oldPDOdatasize + 1].Text = " - ";
                    oldPDOdatasize--;

                }

                byteoff += PDOdatasize;

            }
        }

        void listView_TXCOBmap_onComboBoxIndexChanged(int row, int col, string Text)
        {

            //row+0x1a00 will be the slot to adjust

            eds.dirty = true;

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


                int datalength = 0;

                if (index >= 0x002 && index <= 0x007)
                {
                    //the dummy objects
                    switch(index)
                    {
                        case 0x002:
                            datalength = 8;
                            break;
                        case 0x003:
                            datalength = 16;
                            break;
                        case 0x004:
                            datalength = 32;
                            break;
                        case 0x005:
                            datalength = 8;
                            break;
                        case 0x006:
                            datalength = 16;
                            break;
                        case 0x007:
                            datalength = 32;
                            break;

                    }

                }
                else
                {

                    ODentry od = eds.ods[index];
                    if (sub != 0)
                        od = od.subobjects[sub];

                    //fixme for non basic types will this work?? i think
                    //its not even allowed for PDO but need trap in code to
                    //prevent this and throw error here
                    datalength = 8 * od.sizeofdatatype();
                }

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
            if (listView_TXCOBmap.SelectedItems.Count != 1)
                return;

            UInt16 index = Convert.ToUInt16(listView_TXCOBmap.SelectedItems[0].SubItems[1].Text, 16);
            ODentry od = eds.ods[index];

            textBox_slot.Text = string.Format("0x{0:x4}",od.index);

            if (od.nosubindexes > 1)
             textBox_cob.Text = od.subobjects[1].defaultvalue;

            if (od.nosubindexes > 2)
                textBox_type.Text = od.subobjects[2].defaultvalue;

            if (od.nosubindexes > 3)
                textBox_inhibit.Text = od.subobjects[3].defaultvalue;

            if (od.nosubindexes > 5)
                textBox_eventtimer.Text = od.subobjects[5].defaultvalue;

            if (od.nosubindexes > 6)
                textBox_syncstart.Text = od.subobjects[6].defaultvalue;


            if (isTXPDO)
            {
                textBox_eventtimer.Enabled = true;
                textBox_inhibit.Enabled = true;
                textBox_syncstart.Enabled = true;
            }
            textBox_type.Enabled = true;
            textBox_cob.Enabled = true;

            button_deletePDO.Enabled = true;
            button_savepdochanges.Enabled = true;

        }

        private void button_addPDO_Click(object sender, EventArgs e)
        {

            UInt16 trycreateindex = 0;

            if (this.isTXPDO)
            {
                trycreateindex = 0x1800;
            }
            else
            {
                trycreateindex = 0x1400;
            }

            for(UInt16 cob = trycreateindex; cob< (UInt16)(trycreateindex+0x0200); cob++)
            {
                if(!eds.ods.ContainsKey(cob))
                {
                    trycreateindex = cob;
                    break;
                }
            }

            if(!eds.createPDO(!this.isTXPDO,trycreateindex))
            {
                MessageBox.Show(String.Format("Failed to create PDO at index {0}", trycreateindex));
            }
            else
            {
                doUpdatePDOs();
                doUpdateOD();

            }

            eds.dirty = true;


        }

        private void button_deletePDO_Click(object sender, EventArgs e)
        {

            try
            {
                UInt16 index = (UInt16)(Convert.ToUInt16(textBox_slot.Text, 16));

                if (MessageBox.Show(String.Format("Really delete PDO params 0x{0:x4} and mapping 0x{1:x4} ?", index, 0x200 + index), "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    eds.ods.Remove(index);
                    eds.ods.Remove((UInt16)(index + 0x200));

                    doUpdatePDOs();
                    doUpdateOD();
                }

                eds.dirty = true;

            }
            catch (Exception ex)
            {

            }

        }
        private void button_savepdochanges_Click(object sender, EventArgs e)
        {
            try
            {
                UInt16 index = Convert.ToUInt16(textBox_slot.Text, 16);

                if (!eds.ods.ContainsKey(index))
                {
                    MessageBox.Show("Error finding communication paramaters");
                    return;
                }

                if (isTXPDO && eds.ods[index].nosubindexes != 7)
                {
                    MessageBox.Show("Error with communication paramaters, manual edit required of OD");
                    return;
                }


                if (!isTXPDO && eds.ods[index].nosubindexes != 3)
                {
                    MessageBox.Show("Error with communication paramaters, manual edit required of OD");
                    return;
                }

                UInt16 newnode = eds.GetNodeID(textBox_cob.Text);
                if(newnode < 0x180 || newnode >0x57F)
                {
                    MessageBox.Show("PDO COBs should be between 0x180 and 0x57F");
                    return;
                }

                int dummy;
                if(!int.TryParse(textBox_type.Text,out dummy) || dummy<0 || dummy>255)
                {
                    MessageBox.Show("Type should be a number between 0 and 255");
                    return;
                }


                if (isTXPDO)
                {
                    if (!int.TryParse(textBox_inhibit.Text, out dummy) || dummy < 0 || dummy > 65535)
                    {
                        MessageBox.Show("Inhibit should be a number between 0 and 65535");
                        return;
                    }

                    if (!int.TryParse(textBox_eventtimer.Text, out dummy) || dummy < 0 || dummy > 65535)
                    {
                        MessageBox.Show("Event timer should be a number between 0 and 65535");
                        return;
                    }

                    if (!int.TryParse(textBox_syncstart.Text, out dummy) || dummy < 0 || dummy > 255)
                    {
                        MessageBox.Show("Syncstart should be a number between 0 and 255");
                        return;
                    }

                    eds.ods[index].subobjects[3].defaultvalue = textBox_inhibit.Text;
                    eds.ods[index].subobjects[5].defaultvalue = textBox_eventtimer.Text;
                    eds.ods[index].subobjects[6].defaultvalue = textBox_syncstart.Text;
                }

                eds.ods[index].subobjects[1].defaultvalue = textBox_cob.Text;
                eds.ods[index].subobjects[2].defaultvalue = textBox_type.Text;


                doUpdatePDOs();
                doUpdateOD();

                eds.dirty = true;
            }
            catch (Exception ex)
            {

            }
        }
    }
}

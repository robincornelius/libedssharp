using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODEditor
{
    public partial class MaxSubIndexFrm : Form
    {
        public int maxsubindex = 0;
        public MaxSubIndexFrm(int noelements)
        {
            InitializeComponent();
            textBox_noelements.Text = noelements.ToString();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            maxsubindex = (int)numericUpDown_maxsubindex.Value;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}

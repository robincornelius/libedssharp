using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libEDSsharp;

namespace ODEditor
{
    public partial class WarningsFrm : Form
    {
        public WarningsFrm()
        {
            InitializeComponent();

            foreach(string s in Warnings.warning_list)
            {
                textBox1.AppendText(s+"\r\n");
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

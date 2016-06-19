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
using System.Globalization;

namespace EDSTest
{
    public partial class Form1 : Form
    {
        EDSsharp eds;
        public Form1()
        {
            InitializeComponent();
            eds = new EDSsharp();

            eds.loadfile(@"C:\code\canfestival\canfestival-3-asc\objdictgen\examples\example_objdict.eds");

            textBox1.AppendText(eds.di.ToString());
            textBox1.AppendText(eds.du.ToString());
            textBox1.AppendText(eds.md.ToString());
            textBox1.AppendText(eds.oo.ToString());
            textBox1.AppendText(eds.c.ToString());

            foreach (ODentry o in eds.ods)
            {
                textBox1.AppendText(o.ToString());
            }

            textBox1.Text = textBox1.Text.Replace("\n", "\r\n");

        }
    }
}

/*
    This file is part of libEDSsharp.

    libEDSsharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with libEDSsharp.  If not, see <http://www.gnu.org/licenses/>.
*/

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
            eds.savefile(@"C:\code\canfestival\canfestival-3-asc\objdictgen\examples\example_objdict2.eds");

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

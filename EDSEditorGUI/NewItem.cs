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
    Copyright(c) 2017 Neuberger Gebäudeautomation martin.wagner@neuberger.net>
*/

using System;
using System.Windows.Forms;

namespace ODEditor
{
    public partial class NewItem : Form
    {
        public string name;

        public NewItem(string title)
        {
            InitializeComponent();
            Text = title;

            DialogResult = DialogResult.Cancel;
        }

        private void Create()
        {
            name = textBox_name.Text;

            if (name == "")
            {
                MessageBox.Show("Please specify a name");
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button_create_Click(object sender, EventArgs e)
        {
            Create();
        }

        private void textBox_name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x1B)
            {
                Close();
            }
            else if (e.KeyChar == '\r')
            {
                Create();
            }
        }
    }
}

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
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();

            comboBox_exporter.DataSource = Enum.GetValues(typeof(ExporterFactory.Exporter));

            comboBox_exporter.SelectedItem = (ExporterFactory.Exporter)Properties.Settings.Default.ExporterType;
          

        }

        private void button_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_save_Click(object sender, EventArgs e)
        {

            ExporterFactory.Exporter exporter;
            Enum.TryParse<ExporterFactory.Exporter>(comboBox_exporter.SelectedValue.ToString(), out exporter);
            Properties.Settings.Default.ExporterType = (int)exporter;

            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TubeScanner
{
    public partial class Configuration : Form
    {
        private const int defaultInterval = 10;

        public static int interval = defaultInterval;

        public Configuration()
        {
            InitializeComponent();
            cb_interval.SelectedIndex = 9;
        }

        private void cb_interval_SelectedIndexChanged(object sender, EventArgs e)
        {
            string value = cb_interval.SelectedItem.ToString();
        }
    }
}

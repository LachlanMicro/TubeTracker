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
        public static int interval;

        public Configuration()
        {
            InitializeComponent();
           //setDefaultSettings();
        }


        /* Disables the close (X) button on window */
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ClassStyle = param.ClassStyle | CP_NOCLOSE_BUTTON;
                return param;
            }
        }

        public void setDefaultSettings()
        {
            /* Select default value for interval combo box- middle of values (10) */
            cb_interval.SelectedIndex = 0; //cb_interval.Items.Count / 2;

            interval = (Int32.Parse(cb_interval.SelectedItem.ToString()));
        }

        /* When interval dropdown value changed, alter the variable accordingly */
        private void cb_interval_SelectedIndexChanged(object sender, EventArgs e)
        {
            interval = (Int32.Parse(cb_interval.SelectedItem.ToString()));
        }

        /* Back to startup */
        private void btn_back_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
            setDefaultSettings();
        }
    }
}

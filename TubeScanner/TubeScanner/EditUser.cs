using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TubeScanner.Classes;

namespace TubeScanner
{
    public partial class EditUser : Form
    {
        UserAccounts_SQLite userAccounts = new UserAccounts_SQLite();
        public string editedUser = "";
        public string editedPassword = "";
        public int editedAccess = 0;
        public bool _modifyAccess = false;
        public bool _modifyUsername = false;

        public EditUser(string initialName, string initialPassword, int initialAccess, bool modifyAccess, bool modifyUsername)
        {
            InitializeComponent();
            editedUser = initialName;
            editedPassword = initialPassword;
            editedAccess = initialAccess;
            _modifyAccess = modifyAccess;
            _modifyUsername = modifyUsername;

            textBox1.Text = initialName;
            textBox2.Text = Cryptography.Decrypt(initialPassword);
            comboBox1.SelectedIndex = initialAccess;

            if (!_modifyAccess)
            {
                comboBox1.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
            }

            if (!_modifyUsername)
            {
                textBox1.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.SelectedIndex = 0;
            this.Hide();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                if (textBox2.Text.Length > 0)
                {
                    if (textBox2.Text == textBox3.Text)
                    {
                        userAccounts.UpdateUser(textBox1.Text, textBox2.Text, comboBox1.SelectedIndex, editedUser);
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                        comboBox1.SelectedIndex = 0;
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Error: Passwords do not match");
                    }
                }
                else
                {
                    MessageBox.Show("Error: No password entered");
                }
            }
            else
            {
                MessageBox.Show("Error: No username entered");
            }
        }
    }
}

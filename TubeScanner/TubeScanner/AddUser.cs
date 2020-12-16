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
    public partial class AddUser : Form
    {
        UserAccounts_SQLite userAccounts = new UserAccounts_SQLite();

        public AddUser()
        {
            InitializeComponent();
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
                        if (!userAccounts.isUserAlreadyExist(textBox1.Text))
                        {
                            userAccounts.AddNewUser(textBox1.Text, textBox2.Text, comboBox1.SelectedIndex);
                            textBox1.Text = "";
                            textBox2.Text = "";
                            textBox3.Text = "";
                            comboBox1.SelectedIndex = 0;
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("A user by this name already exists. Please choose another");
                        }
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

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
    public partial class Login : Form
    {
        private UserAccountsViewModel userAccountsViewModel = new UserAccountsViewModel();
        UserAccounts_SQLite userData = new UserAccounts_SQLite();

        public Login()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            this.Hide();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text;

            if (userData.isUserAlreadyExist(textBox1.Text))
            {
                string password = "";

                List<UserAccountsModel> userAccounts = userAccountsViewModel.UserAccounts;

                UserAccountsModel loggedUser = null;

                foreach (UserAccountsModel user in userAccounts)
                {
                    if (user.Name == userName)
                    {
                        password = user.Password;
                        loggedUser = user;
                        break;
                    }
                }

                if (textBox2.Text == Cryptography.Decrypt(password))
                {
                    Program.currentUser = userName;
                    DateTime date = DateTime.Now;
                    userData.UpdateDate(loggedUser.Name);
                    textBox1.Text = "";
                    textBox2.Text = "";
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Error: Incorrect password");
                }
            }
            else
            {
                MessageBox.Show("Error: User does not exist");
            }
        }
    }
}

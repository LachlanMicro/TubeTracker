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
    public partial class UsersPage : Form
    {
        private DataGridView usersDataGridView = new DataGridView();
        private UserAccountsViewModel userAccountsViewModel = new UserAccountsViewModel();
        UserAccounts_SQLite userData = new UserAccounts_SQLite();

        public string userAccess = "";
        public int userNum;

        public UsersPage()
        {
            InitializeComponent();
            SetupDataGridView();
            PopulateDataGridView(userAccountsViewModel.UserAccounts);
        }

        private void SetupDataGridView()
        {
            this.Controls.Add(usersDataGridView);

            usersDataGridView.ColumnCount = 4;

            //usersDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            //usersDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            usersDataGridView.ColumnHeadersDefaultCellStyle.Font =
                new Font(usersDataGridView.Font, FontStyle.Bold);

            usersDataGridView.Name = "usersDataGridView";
            usersDataGridView.Location = new Point(0, 100);
            usersDataGridView.Size = new Size(600, 450);
            usersDataGridView.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            usersDataGridView.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Single;
            usersDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            usersDataGridView.GridColor = Color.Black;
            usersDataGridView.RowHeadersVisible = false;
            usersDataGridView.AllowUserToAddRows = false;
            usersDataGridView.ReadOnly = true;

            usersDataGridView.Columns[0].Name = "Username";
            usersDataGridView.Columns[1].Name = "Password";
            usersDataGridView.Columns[2].Name = "Access level";
            usersDataGridView.Columns[3].Name = "Last login";

            usersDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            usersDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            usersDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            usersDataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            usersDataGridView.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            usersDataGridView.MultiSelect = false;
            usersDataGridView.Dock = DockStyle.Fill;

            /*usersDataGridView.CellFormatting += new
                DataGridViewCellFormattingEventHandler(
                songsDataGridView_CellFormatting);*/
        }

        private void PopulateDataGridView(List<UserAccountsModel> userAccounts)
        {
            foreach (UserAccountsModel user in userAccounts)
            {
                string accessLevel = user.Admin.ToString();

                if (user.Admin == 0)
                {
                    accessLevel = "Admin";
                }
                if (user.Admin == 1)
                {
                    accessLevel = "Staff";
                }

                if (user.Name == Program.currentUser)
                {
                    userAccess = accessLevel;
                    userNum = userAccounts.IndexOf(user);
                }

                string password = Cryptography.Decrypt(user.Password);
                string hiddenPassword = "";
                foreach (char c in password)
                {
                    hiddenPassword += "*";
                }

                string[] userAppend = { user.Name, hiddenPassword, accessLevel, user.LastLogin };
                usersDataGridView.Rows.Add(userAppend);
            }
        }

        // Add user
        private void AddButton_Click(object sender, EventArgs e)
        {
            // Need admin status
            if (Program.currentUser != "")
            {
                if (userAccess == "Admin")
                {
                    int numUsers = userAccountsViewModel.UserAccounts.Count;

                    AddUser form = new AddUser();
                    form.ShowDialog();

                    UserAccountsViewModel userAccountsViewModelUpdated = new UserAccountsViewModel();

                    // If a new user got added, update user list and display
                    if (userAccountsViewModelUpdated.UserAccounts.Count > numUsers)
                    {
                        userAccountsViewModel.UserAccounts = userAccountsViewModelUpdated.UserAccounts;
                        usersDataGridView.Rows.Clear();
                        PopulateDataGridView(userAccountsViewModel.UserAccounts);
                        usersDataGridView.Refresh();
                    }
                }
                else
                {
                    MessageBox.Show("You do not have permissions to add users");
                }
            }
            else
            {
                MessageBox.Show("Please log in to access this feature");
            }
        }

        // Edit user
        private void EditButton_Click(object sender, EventArgs e)
        {
            // If current user has admin status: open form with access level
            if (Program.currentUser != "")
            {
                foreach (DataGridViewRow row in usersDataGridView.SelectedRows)
                {
                    string usernameEdit = usersDataGridView.Rows[row.Index].Cells[0].Value.ToString();
                    string passwordEdit = ""; //usersDataGridView.Rows[row.Index].Cells[1].Value.ToString();
                    foreach (UserAccountsModel user in userAccountsViewModel.UserAccounts)
                    {
                        if (user.Name == usernameEdit)
                        {
                            passwordEdit = user.Password;
                            break;
                        }
                    }
                    string accessEdit = usersDataGridView.Rows[row.Index].Cells[2].Value.ToString();
                    int selectedIndex = 0;

                    if (accessEdit == "Staff")
                    {
                        selectedIndex = 1;
                    }

                    if (Program.currentUser == "Admin")
                    {
                        // Show edit page with access level and make sure existing stuff is loaded in
                        // all selectedUserInfo and true or false for disabling access edit

                        if (usernameEdit == "Admin")
                        {
                            EditUser form = new EditUser(usernameEdit, passwordEdit, selectedIndex, false, false);
                            form.ShowDialog();
                            UserAccountsViewModel userAccountsViewModelUpdated = new UserAccountsViewModel();
                            usersDataGridView.Rows.Clear();
                            PopulateDataGridView(userAccountsViewModelUpdated.UserAccounts);
                            usersDataGridView.Refresh();
                        }
                        else
                        {
                            EditUser form = new EditUser(usernameEdit, passwordEdit, selectedIndex, true, true);
                            form.ShowDialog();
                            UserAccountsViewModel userAccountsViewModelUpdated = new UserAccountsViewModel();
                            usersDataGridView.Rows.Clear();
                            PopulateDataGridView(userAccountsViewModelUpdated.UserAccounts);
                            usersDataGridView.Refresh();
                        }
                    }
                    else
                    {
                        if (Program.currentUser == usernameEdit && userAccess != "Admin")
                        {
                            EditUser form = new EditUser(usernameEdit, passwordEdit, selectedIndex, false, true);
                            form.ShowDialog();
                            UserAccountsViewModel userAccountsViewModelUpdated = new UserAccountsViewModel();
                            usersDataGridView.Rows.Clear();
                            PopulateDataGridView(userAccountsViewModelUpdated.UserAccounts);
                            usersDataGridView.Refresh();

                            Program.currentUser = userAccountsViewModelUpdated.UserAccounts[userNum].Name;
                        }
                        else if (userAccess == "Admin" && usernameEdit != "Admin")
                        {
                            EditUser form = new EditUser(usernameEdit, passwordEdit, selectedIndex, true, true);
                            form.ShowDialog();
                            UserAccountsViewModel userAccountsViewModelUpdated = new UserAccountsViewModel();
                            usersDataGridView.Rows.Clear();
                            PopulateDataGridView(userAccountsViewModelUpdated.UserAccounts);
                            usersDataGridView.Refresh();

                            if (Program.currentUser == usernameEdit)
                            {
                                Program.currentUser = userAccountsViewModelUpdated.UserAccounts[userNum].Name;
                            }
                        }
                        else
                        {
                            MessageBox.Show("You do not have permissions to edit this user");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please log in to access this feature");
            }
        }

        // Delete user
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            // Need admin status or need to be that user
            if (Program.currentUser != "")
            {
                foreach (DataGridViewRow row in usersDataGridView.SelectedRows)
                {
                    string usernameDelete = usersDataGridView.Rows[row.Index].Cells[0].Value.ToString();
                    // Cannot delete original admin                
                    if (usernameDelete != "Admin")
                    {
                        if (Program.currentUser != usernameDelete)
                        {
                            if (userAccess == "Admin")
                            {
                                DialogResult dialogResult = MessageBox.Show("Are you sure you wish to delete this user?", "Delete user", MessageBoxButtons.OKCancel);
                                if (dialogResult == DialogResult.OK)
                                {
                                    usersDataGridView.Rows.RemoveAt(row.Index);
                                    userData.DeleteUser(usernameDelete);
                                }
                            }
                            else
                            {
                                MessageBox.Show("You do not have permissions to delete this user");
                            }
                        }
                        else
                        {
                            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to delete your account?", "Delete user", MessageBoxButtons.OKCancel);
                            if (dialogResult == DialogResult.OK)
                            {
                                Program.currentUser = "";
                                usersDataGridView.Rows.RemoveAt(row.Index);
                                userData.DeleteUser(usernameDelete);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You do not have permissions to delete this user");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please log in to access this feature");
            }
        }

        // Go back
        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

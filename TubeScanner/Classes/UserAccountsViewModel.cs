using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubeScanner.Classes
{
    public class UserAccountsViewModel
    {
        public List<UserAccountsModel> _userAccounts = new List<UserAccountsModel>();

        public UserAccountsViewModel()
        {
            try
            {
                _userAccounts = new List<UserAccountsModel>();
                var userAccounts = new UserAccounts_SQLite();
                var ds = userAccounts.GetAllUsers();

                if (ds == null)
                {
                    return;
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 0)//if no users exist add a new default administrator
                {
                    var UserAccounts = new UserAccounts_SQLite();
                    userAccounts.AddNewUser("Admin", "admin", 0);
                    ds = userAccounts.GetAllUsers();
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        _userAccounts.Add(new UserAccountsModel
                        {
                            Name = dr["UserName"].ToString(),
                            Password = dr["Password"].ToString(),
                            Admin = Int32.Parse(dr["Admin"].ToString()),
                            LastLogin = dr["LastLogin"].ToString(),
                            LastLogOut = dr["LastLogout"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Source != null)
                    Console.WriteLine("Exception UserAccountsViewModel: {0}", ex.Source);
            }
        }

        public List<UserAccountsModel> UserAccounts
        {
            get
            {
                return _userAccounts;
            }
            set
            {
                _userAccounts = value;
            }
        }
    }
}

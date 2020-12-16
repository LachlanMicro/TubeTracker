using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubeScanner.Classes
{
    public class UserAccounts_SQLite
    {

        public DataSet GetAllUsers()
        {
            string query = "SELECT * FROM Users";
            var dbConnect = new DBConnectSQLite();
            return dbConnect.GetDataSet(query);
        }

        public void AddNewUser(string userName, string Password, int Admin)
        {
            var dbConnect = new DBConnectSQLite();

            //if (isUserAlreadyExist(userName))
            //    throw new Exception("User already exist");
            Password = Cryptography.Encrypt(Password);
            string query = "INSERT INTO Users (UserName, Password, Admin) VALUES('" + userName + "', '" + Password + "', " + Admin + ")";//////
            dbConnect.ExecuteNonQuery(query);
        }

        public void UpdateDate(string ActualUser) // DateTime Login
        {
            var dbConnect = new DBConnectSQLite();
            string query = "Update Users Set LastLogin= datetime() where UserName='" + ActualUser + "'"; // string query = "Update Users Set LastLogin='" + Login + "' where UserName='" + ActualUser + "'";
            dbConnect.ExecuteNonQuery(query);
        }

        public void UpdateUser(string userName, string Password, int Admin, string ActualUser)
        {
            var dbConnect = new DBConnectSQLite();

            Password = Cryptography.Encrypt(Password);
            string query = "Update Users Set UserName='" + userName + "', Password='" + Password + "', Admin=" + Admin + " where UserName='" + ActualUser + "'";
            dbConnect.ExecuteNonQuery(query);

            /*
            if (Login.ActiveLoggedInUser == ActualUser)
            {
                Login.ActiveLoggedInUser = userName;
            }
            */
        }

        public void DeleteUser(string ActualUser)
        {
            var dbConnect = new DBConnectSQLite();

            string query = "Delete from Users where UserName='" + ActualUser + "'";
            dbConnect.ExecuteNonQuery(query);
        }

        public bool isUserAlreadyExist(string userName)
        {

            string query = "SELECT Count(*) FROM Users where UserName='" + userName + "'";
            var dbConnect = new DBConnectSQLite();
            var result = dbConnect.ExecuteScalarQuery(query);
            var Count = int.Parse(result.ToString());
            return Count > 0;
        }

    }
}

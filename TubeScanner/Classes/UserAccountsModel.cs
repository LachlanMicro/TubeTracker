using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubeScanner.Classes
{
    public class UserAccountsModel
    {
        private string _name;
        private string _password;
        private int _admin;
        private string _accessLevel;
        private string _lastLogIn;
        private string _lastLogOut;


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

            }

        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;

            }
        }



        public int Admin
        {
            get
            {
                return _admin;
            }
            set
            {
                _admin = value;

                /*
                switch (_admin)
                {
                    case (int)SQLite.eAccessLevel.ADMIN:
                        AccessLevel = "Admin";
                        break;
                    case (int)SQLite.eAccessLevel.LEVEL1:
                        AccessLevel = "Level 1";
                        break;
                    case (int)SQLite.eAccessLevel.LEVEL2:
                        AccessLevel = "Level 2";
                        break;
                    case (int)SQLite.eAccessLevel.LEVEL3:
                        AccessLevel = "Level 3";
                        break;

                }
                */


            }
        }

        public string AccessLevel
        {

            get
            {
                return _accessLevel;
            }
            set
            {
                _accessLevel = value;

            }
        }

        public string LastLogin
        {
            get
            {
                return _lastLogIn;
            }
            set
            {
                if (DateTime.TryParse(value, out DateTime result))
                {
                    DateTime dateTime = result.ToLocalTime();
                    _lastLogIn = dateTime.ToString();
                }
                else
                {
                    _lastLogIn = "Never";
                }
            }
        }

        public string LastLogOut
        {

            get
            {
                return _lastLogOut;
            }
            set
            {
                _lastLogOut = value;

            }
        }


        public UserAccountsModel()
        {

        }

    }
}

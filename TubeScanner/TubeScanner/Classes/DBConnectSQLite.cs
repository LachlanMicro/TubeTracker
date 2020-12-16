using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TubeScanner.Classes
{
    public class DBConnectSQLite
    {
        private static string ProgramFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        private static string FolderPath { get; } = ProgramFolderPath + "\\Bsd\\Test\\db\\";
        private string _fileName = "system.bsdb";
        private SQLiteConnection _connection;

        //Constructor
        public DBConnectSQLite()
        {

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!System.IO.File.Exists(Path.Combine(FolderPath, _fileName)))
            {
                SQLiteConnection.CreateFile(Path.Combine(FolderPath, _fileName)); //currently saves file to the debug folder
                GrantAccess(Path.Combine(FolderPath, _fileName));

                SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Path.Combine(FolderPath, _fileName) + ";");
                m_dbConnection.Open();
                string query = "create table Users (UserName varchar(255), Password varchar(2000), Admin tinyint(1), LastLogin DateTime, LastLogout DateTime)";
                SQLiteCommand command = new SQLiteCommand(query, m_dbConnection);
                command.ExecuteNonQuery();

                string userName = "Admin";
                string Password = Cryptography.Encrypt("admin");
                int Admin = 0;
                query = "INSERT INTO Users (UserName, Password, Admin) VALUES('" + userName + "', '" + Password + "', " + Admin + ")";
                command = new SQLiteCommand(query, m_dbConnection);
                command.ExecuteNonQuery();

                m_dbConnection.Close();
            }
            Initialize();
        }

        private void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            bool modified;
            AccessRule rule;
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            rule = new FileSystemAccessRule(
                       securityIdentifier,
                       FileSystemRights.Write |
                       FileSystemRights.ReadAndExecute |
                       FileSystemRights.Modify,
                       AccessControlType.Allow);

            dSecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);

            //   dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }


        //Initialize values
        private void Initialize()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection("Data Source=" + Path.Combine(FolderPath, _fileName) + ";");
            }
        }

        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                _connection.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Cannot connect to server.", ex);
            }
        }


        //Close connection
        private void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }

        public object ExecuteScalarQuery(string query)
        {
            try
            {
                //Open Connection
                if (OpenConnection() == true)
                {
                    //Create SQLite Command
                    SQLiteCommand command = new SQLiteCommand(query, _connection);

                    //ExecuteScalar will return one value
                    return command.ExecuteScalar();
                }

            }
            catch (SQLiteException)
            {
                //
            }
            catch (Exception e)
            {
                if (e.InnerException.Source == "MySql.Data")
                {
                    //
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                //close Connection
                CloseConnection();
            }
            return null;
        }

        public int ExecuteNonQuery(string query)
        {
            try
            {
                //Open Connection
                if (OpenConnection() == true)
                {
                    //Create Mysql Command
                    SQLiteCommand command = new SQLiteCommand(query, _connection);
                    return command.ExecuteNonQuery();
                }

            }
            catch (Exception e)
            {
                if (e.InnerException.Source == "MySql.Data")
                {
                    //
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                //close Connection
                CloseConnection();
            }
            return 0;
        }

        public DataSet GetDataSet(string query)
        {
            try
            {
                var command = new SQLiteCommand
                {
                    Connection = _connection,
                    CommandText = query
                };

                var ds = new DataSet();
                var da = new SQLiteDataAdapter(command);
                da.Fill(ds);

                return ds;
            }
            catch (Exception e)
            {
                if (e.InnerException.Source == "MySql.Data")
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public void SaveToDataBase(DataTable dataTable)
        {
            try
            {
                StringBuilder sbColumns = new StringBuilder();
                IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().
                                                 Select(column => column.ColumnName);
                sbColumns.AppendLine(string.Join(",", columnNames));

                StringBuilder sCommand = new StringBuilder("INSERT INTO " + dataTable.TableName + " (" + sbColumns.ToString() + ") VALUES ");

                foreach (DataRow row in dataTable.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sCommand.Append("('");
                    sCommand.AppendLine(string.Join("','", fields));
                    sCommand.Append("'),");
                }
                sCommand.Remove(sCommand.Length - 1, 1);
                sCommand.Append(";");
                _connection.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sCommand.ToString().Replace(Environment.NewLine, ""), _connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                if (e.InnerException.Source == "MySql.Data")
                {
                    //
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                //close Connection
                CloseConnection();
            }
        }

    }
}

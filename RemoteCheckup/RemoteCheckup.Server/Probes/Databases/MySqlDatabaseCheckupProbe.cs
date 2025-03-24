using System;
using System.Data.Common;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using RemoteCheckup.DTOs;

namespace RemoteCheckup.Probes
{
    public class MySqlDatabaseCheckupProbe : DatabaseCheckupProbe
    {
        private MySqlConnection connection;
        private string connectionString;

        public MySqlDatabaseCheckupProbe(string connString) 
        {
            connectionString = connString;
            EnsureConnection();
        }

        private bool EnsureConnection() 
        {
            if (connection == null || connection.IsDisposed)
                connection = new MySqlConnection(connectionString);
            return connection != null;
        }

        public override void GetDatabaseInfo(DatabaseServerInfo info)
        {
            info.Type = "mysql";

            if (!EnsureConnection()) 
            {
                Console.Error.WriteLine("MySQL connection failed");
                info.Online = false;
                return;
            }
            info.Online = true;
            connection.Open();
            
            var query = new MySqlCommand("SHOW DATABASES", connection);
            var reader = query.ExecuteReader();
            reader.Read();
            
            while (reader.Read())
            {
                info.Databases.Add(new DatabaseInfo() {
                    Name = reader.GetString("Database")
                });
            }

            connection.Close();
        }
    }
}

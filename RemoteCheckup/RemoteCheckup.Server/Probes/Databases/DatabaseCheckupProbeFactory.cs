using System;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using RemoteCheckup.Models;

namespace RemoteCheckup.Probes
{
    public static class DatabaseCheckupProbeFactory
    {
        public static DatabaseCheckupProbe Create(DatabaseTarget target) 
        {
            string connString = target.ConnectionString.Replace("{{SECRET}}", target.ConnectionSecret);

            return target.DatabaseType switch
            {
                DatabaseType.MySQL => new MySqlDatabaseCheckupProbe(target.Name, connString),
                _ => throw new ArgumentException("Invalid database type " + target.DatabaseType),
            };
        }
    }
}

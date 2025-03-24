using System;
using RemoteCheckup.DTOs;

namespace RemoteCheckup.Probes
{
    public abstract class DatabaseCheckupProbe
    {
        public abstract void GetDatabaseInfo(DatabaseServerInfo info);
        public async Task GetDatabaseInfoAsync(DatabaseServerInfo info)
        {
            await Task.Run(() => GetDatabaseInfo(info));
        }
    }
}

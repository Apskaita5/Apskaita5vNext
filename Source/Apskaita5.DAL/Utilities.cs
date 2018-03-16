using System;
using System.Web;

namespace Apskaita5.DAL
{
    public static class Utilities
    {

        private const string appDataFolder = "App_Data";

        public static string AppDataPath()
        {
            return System.IO.Path.Combine(System.AppContext.BaseDirectory, appDataFolder);
        }

    }
}

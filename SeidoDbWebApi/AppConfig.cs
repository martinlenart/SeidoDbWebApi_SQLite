using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAppWebApi
{
    public sealed class AppConfig
    {
        public const string ThisConnection = "SQLite_seidowebservice_ws8";
        private static AppConfig _instance = null;
        private static readonly object instanceLock = new();
        private static IConfigurationRoot _configuration;

#if DEBUG
        private string _appsettingfile = "appsettings.Development.json";
#else
        private string _appsettingfile = "appsettings.json";
#endif
        private AppConfig()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(_appsettingfile, optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public static IConfigurationRoot ConfigurationRoot
        {
            get
            {
                lock (instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppConfig();
                    }
                    return _configuration;
                }
            }
        }
    }
}

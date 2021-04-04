using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace XVagas.Globals
{
    public static class Globals
    {
        public static ILoggerFactory loggerFactory { get; set; }
        private static IConfiguration Config;
        public static IConfiguration Configuration
        {
            get
            {
                if (Config == null)
                {
                    var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddEnvironmentVariables();

                    Config = builder.Build();
                }
                return Config;
            }

        }


    }
}
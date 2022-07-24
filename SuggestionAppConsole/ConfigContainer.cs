using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using SuggestionAppLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionAppConsole
{
    public static class ConfigContainer
    {
        private static string _appSettingsFile = "appsettings.json";

        public static IHostBuilder CreatеHostBuilder(string [] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile(_appSettingsFile);
                });

            return configureServices(hostBuilder);
        }

        private static IHostBuilder configureServices(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IDbConnection, DbConnection>();
                services.AddSingleton<IUserData, MongoUserData>();
                services.AddSingleton<IBusinessLogic, BusinessLogic>();
                services.AddSingleton<IApplication, Application>();
            });
            return hostBuilder;
        }

    }
}

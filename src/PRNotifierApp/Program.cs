using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PRNotifierApp.Services;
using PRNotifierApp.Clients;
using Microsoft.Extensions.Configuration;
using PRNotifierApp.Configuration;

namespace PRNotifierApp
{
    public class Program
    {
        public static void Main()
        {
            var host = CreateHostBuilder().Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) =>
                {
                    services.Configure<TeamsConfiguration>(context.Configuration.GetSection("Teams"));
                    services.AddSingleton<IAzureDevOpsClient, AzureDevOpsClient>();
                    services.AddSingleton<ITeamsClient, TeamsClient>();
                    services.AddSingleton<IAzureDevOpsService, AzureDevOpsService>();
                    services.AddSingleton<ITeamsService, TeamsService>();
                    services.AddSingleton<IPRNotifierService, PRNotifierService>();
                });
    }
}
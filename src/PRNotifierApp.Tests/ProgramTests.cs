using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PRNotifierApp.Clients;
using PRNotifierApp.Services;
using Xunit;

namespace PRNotifierApp.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void CreateHostBuilder_ConfiguresServices_Correctly()
        {
            // Arrange
            var host = Program.CreateHostBuilder().Build();

            // Act
            var serviceProvider = host.Services;

            // Assert
            Assert.NotNull(serviceProvider.GetService<IAzureDevOpsClient>());
            Assert.NotNull(serviceProvider.GetService<ITeamsClient>());
            Assert.NotNull(serviceProvider.GetService<IAzureDevOpsService>());
            Assert.NotNull(serviceProvider.GetService<ITeamsService>());
            Assert.NotNull(serviceProvider.GetService<IPRNotifierService>());

            Assert.IsType<AzureDevOpsClient>(serviceProvider.GetService<IAzureDevOpsClient>());
            Assert.IsType<TeamsClient>(serviceProvider.GetService<ITeamsClient>());
            Assert.IsType<AzureDevOpsService>(serviceProvider.GetService<IAzureDevOpsService>());
            Assert.IsType<TeamsService>(serviceProvider.GetService<ITeamsService>());
            Assert.IsType<PRNotifierService>(serviceProvider.GetService<IPRNotifierService>());
        }
    }
}
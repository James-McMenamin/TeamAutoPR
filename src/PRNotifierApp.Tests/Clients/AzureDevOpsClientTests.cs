using System;
using System.Threading.Tasks;
using PRNotifierApp.Clients;
using Xunit;

namespace PRNotifierApp.Tests.Clients
{
    public class AzureDevOpsClientTests
    {
        private readonly AzureDevOpsClient _client;

        public AzureDevOpsClientTests()
        {
            _client = new AzureDevOpsClient();
        }

        [Fact]
        public async Task GetPullRequestAsync_ThrowsNotImplementedException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => 
                _client.GetPullRequestAsync(1, "repo", "project"));
        }

        [Fact]
        public async Task GetWorkItemsAsync_ThrowsNotImplementedException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => 
                _client.GetWorkItemsAsync(1, "repo", "project"));
        }

        [Fact]
        public async Task GetReviewersAsync_ThrowsNotImplementedException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => 
                _client.GetReviewersAsync(1, "repo", "project"));
        }
    }
}
using System.Threading.Tasks;
using Moq;
using PRNotifierApp.Clients;
using PRNotifierApp.Models;
using PRNotifierApp.Services;
using Xunit;

namespace PRNotifierApp.Tests.Services
{
    public class AzureDevOpsServiceTests
    {
        private readonly Mock<IAzureDevOpsClient> _mockClient;
        private readonly AzureDevOpsService _service;

        public AzureDevOpsServiceTests()
        {
            _mockClient = new Mock<IAzureDevOpsClient>();
            _service = new AzureDevOpsService(_mockClient.Object);
        }

        [Fact]
        public async Task GetPullRequestDetailsAsync_ValidInput_ReturnsPullRequest()
        {
            // Arrange
            var pullRequestId = 123;
            var repositoryId = "repo-id";
            var projectId = "project-id";

            var expectedPR = new PullRequest
            {
                Id = pullRequestId,
                Title = "Test PR",
                Description = "Test Description",
                Url = "http://test.com",
                Status = "Active",
                HasComments = false,
                HasPendingComments = false,
                WorkItemIds = new List<string> { "1", "2" },
                Reviewers = new List<Reviewer>()
            };

            _mockClient.Setup(c => c.GetPullRequestAsync(pullRequestId, repositoryId, projectId))
                .ReturnsAsync(expectedPR);
            _mockClient.Setup(c => c.GetWorkItemsAsync(pullRequestId, repositoryId, projectId))
                .ReturnsAsync(new List<string> { "1", "2" });
            _mockClient.Setup(c => c.GetReviewersAsync(pullRequestId, repositoryId, projectId))
                .ReturnsAsync(new List<Reviewer>());

            // Act
            var result = await _service.GetPullRequestDetailsAsync(pullRequestId, repositoryId, projectId);

            // Assert
            Assert.Equal(expectedPR.Id, result.Id);
            Assert.Equal(expectedPR.Title, result.Title);
            Assert.Equal(expectedPR.Description, result.Description);
            Assert.Equal(expectedPR.Url, result.Url);
            Assert.Equal(expectedPR.Status, result.Status);
            Assert.Equal(expectedPR.HasComments, result.HasComments);
            Assert.Equal(expectedPR.HasPendingComments, result.HasPendingComments);
            Assert.Equal(expectedPR.WorkItemIds, result.WorkItemIds);
            Assert.Equal(expectedPR.Reviewers, result.Reviewers);
        }
    }
}
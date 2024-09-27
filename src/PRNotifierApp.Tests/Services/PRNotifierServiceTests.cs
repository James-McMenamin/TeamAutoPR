using System;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using PRNotifierApp.Configuration;
using PRNotifierApp.Models;
using PRNotifierApp.Services;
using Xunit;

namespace PRNotifierApp.Tests.Services
{
    public class PRNotifierServiceTests
    {
        private readonly Mock<IAzureDevOpsService> _mockAzureDevOpsService;
        private readonly Mock<ITeamsService> _mockTeamsService;
        private readonly Mock<IOptions<TeamsConfiguration>> _mockTeamsConfig;
        private readonly PRNotifierService _service;

        public PRNotifierServiceTests()
        {
            _mockAzureDevOpsService = new Mock<IAzureDevOpsService>();
            _mockTeamsService = new Mock<ITeamsService>();
            _mockTeamsConfig = new Mock<IOptions<TeamsConfiguration>>();
            _mockTeamsConfig.Setup(x => x.Value).Returns(new TeamsConfiguration 
            { 
                DefaultChannelId = "default-channel-id",
                RepositoryChannelMappings = new Dictionary<string, string>
                {
                    { "repo-id-1", "channel-id-1" },
                    { "repo-id-2", "channel-id-2" }
                }
            });

            _service = new PRNotifierService(
                _mockAzureDevOpsService.Object,
                _mockTeamsService.Object,
                _mockTeamsConfig.Object
            );
        }

        [Fact]
        public async Task ProcessPullRequestEventAsync_ValidEvent_ProcessesSuccessfully()
        {
            // Arrange
            var eventData = JObject.Parse(@"{
                'resource': {
                    'pullRequestId': 123,
                    'repository': {
                        'id': 'repo-id-1',
                        'project': {
                            'id': 'project-id'
                        }
                    }
                }
            }");

            var pullRequest = new PullRequest
            {
                Id = 123,
                Title = "Test PR",
                Description = "Test Description",
                Url = "http://test.com",
                Status = "Active",
                HasComments = false,
                HasPendingComments = false,
                WorkItemIds = new List<string> { "1", "2" },
                Reviewers = new List<Reviewer>()
            };

            _mockAzureDevOpsService.Setup(s => s.GetPullRequestDetailsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(pullRequest);

            _mockTeamsService.Setup(s => s.PostCardAsync(It.IsAny<TeamsCard>(), It.IsAny<string>()))
                .ReturnsAsync("message-id");

            // Act
            await _service.ProcessPullRequestEventAsync(eventData);

            // Assert
            _mockAzureDevOpsService.Verify(s => s.GetPullRequestDetailsAsync(123, "repo-id-1", "project-id"), Times.Once);
            _mockTeamsService.Verify(s => s.PostCardAsync(It.IsAny<TeamsCard>(), "channel-id-1"), Times.Once);
        }

        [Fact]
        public async Task ProcessPullRequestEventAsync_UnmappedRepository_UsesDefaultChannel()
        {
            // Arrange
            var eventData = JObject.Parse(@"{
                'resource': {
                    'pullRequestId': 123,
                    'repository': {
                        'id': 'unmapped-repo-id',
                        'project': {
                            'id': 'project-id'
                        }
                    }
                }
            }");

            var pullRequest = new PullRequest
            {
                Id = 123,
                Title = "Test PR",
                Description = "Test Description",
                Url = "http://test.com",
                Status = "Active",
                HasComments = false,
                HasPendingComments = false,
                WorkItemIds = new List<string> { "1", "2" },
                Reviewers = new List<Reviewer>()
            };

            _mockAzureDevOpsService.Setup(s => s.GetPullRequestDetailsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(pullRequest);

            _mockTeamsService.Setup(s => s.PostCardAsync(It.IsAny<TeamsCard>(), It.IsAny<string>()))
                .ReturnsAsync("message-id");

            // Act
            await _service.ProcessPullRequestEventAsync(eventData);

            // Assert
            _mockAzureDevOpsService.Verify(s => s.GetPullRequestDetailsAsync(123, "unmapped-repo-id", "project-id"), Times.Once);
            _mockTeamsService.Verify(s => s.PostCardAsync(It.IsAny<TeamsCard>(), "default-channel-id"), Times.Once);
        }

        [Fact]
        public void CreateTeamsCard_ValidPullRequest_ReturnsCorrectCard()
        {
            // Arrange
            var pullRequest = new PullRequest
            {
                Id = 123,
                Title = "Test PR",
                Description = "Test Description",
                Url = "http://test.com",
                Status = "Active",
                HasComments = true,
                HasPendingComments = false,
                WorkItemIds = new List<string> { "1", "2" },
                Reviewers = new List<Reviewer>()
            };

            // Act
            var card = _service.CreateTeamsCard(pullRequest);

            // Assert
            Assert.Equal(5, card.Body.Count);
            Assert.Equal(1, card.Actions.Count);
            Assert.Equal("Test PR", card.Body[0].Text);
            Assert.Contains("Work Items: 1, 2", card.Body[1].Text);
            Assert.Contains("Status: Active", card.Body[2].Text);
            Assert.Contains("Has Comments: True", card.Body[3].Text);
            Assert.Contains("Has Pending Comments: False", card.Body[4].Text);
            Assert.Equal("View PR", card.Actions[0].Title);
            Assert.Equal("http://test.com", card.Actions[0].Url);
        }

        [Fact]
        public async Task ProcessPullRequestEventAsync_NullEventData_ThrowsException()
        {
            // Arrange
            object eventData = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ProcessPullRequestEventAsync(eventData));
        }

        [Fact]
        public async Task ProcessPullRequestEventAsync_MultipleReposToSameChannel_ProcessesCorrectly()
        {
            // Arrange
            var mockConfig = new TeamsConfiguration
            {
                DefaultChannelId = "default-channel",
                RepositoryChannelMappings = new Dictionary<string, string>
                {
                    { "repo-1", "shared-channel" },
                    { "repo-2", "shared-channel" }
                }
            };
            _mockTeamsConfig.Setup(x => x.Value).Returns(mockConfig);

            var service = new PRNotifierService(
                _mockAzureDevOpsService.Object,
                _mockTeamsService.Object,
                _mockTeamsConfig.Object
            );

            var eventData1 = JObject.Parse(@"{
                'resource': {
                    'pullRequestId': 123,
                    'repository': {
                        'id': 'repo-1',
                        'project': { 'id': 'project-id' }
                    }
                }
            }");

            var eventData2 = JObject.Parse(@"{
                'resource': {
                    'pullRequestId': 456,
                    'repository': {
                        'id': 'repo-2',
                        'project': { 'id': 'project-id' }
                    }
                }
            }");

            var pullRequest = new PullRequest
            {
                Id = 123,
                Title = "Test PR",
                Description = "Test Description",
                Url = "http://test.com",
                Status = "Active",
                HasComments = false,
                HasPendingComments = false,
                WorkItemIds = new List<string> { "1", "2" },
                Reviewers = new List<Reviewer>()
            };

            _mockAzureDevOpsService.Setup(s => s.GetPullRequestDetailsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(pullRequest);

            // Act
            await service.ProcessPullRequestEventAsync(eventData1);
            await service.ProcessPullRequestEventAsync(eventData2);

            // Assert
            _mockTeamsService.Verify(s => s.PostCardAsync(It.IsAny<TeamsCard>(), "shared-channel"), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessPullRequestEventAsync_SingleRepoToMultipleChannels_ThrowsNotSupportedException()
        {
            // Arrange
            var mockConfig = new TeamsConfiguration
            {
                DefaultChannelId = "default-channel",
                RepositoryChannelMappings = new Dictionary<string, string>
                {
                    { "repo-1", "channel-1,channel-2" }
                }
            };
            _mockTeamsConfig.Setup(x => x.Value).Returns(mockConfig);

            var service = new PRNotifierService(
                _mockAzureDevOpsService.Object,
                _mockTeamsService.Object,
                _mockTeamsConfig.Object
            );

            var eventData = JObject.Parse(@"{
                'resource': {
                    'pullRequestId': 123,
                    'repository': {
                        'id': 'repo-1',
                        'project': { 'id': 'project-id' }
                    }
                }
            }");

            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() => service.ProcessPullRequestEventAsync(eventData));
        }
    }
}
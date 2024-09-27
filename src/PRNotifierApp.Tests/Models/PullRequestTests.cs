using PRNotifierApp.Models;
using System.Collections.Generic;
using Xunit;

namespace PRNotifierApp.Tests.Models
{
    public class PullRequestTests
    {
        [Fact]
        public void PullRequest_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var pullRequest = new PullRequest
            {
                Id = 1,
                Title = "Test PR",
                Description = "Test Description",
                Url = "http://test.com",
                Status = "Active",
                HasComments = true,
                HasPendingComments = false,
                WorkItemIds = new List<string> { "1", "2" },
                Reviewers = new List<Reviewer>
                {
                    new Reviewer
                    {
                        DisplayName = "John Doe",
                        AvatarUrl = "http://avatar.com",
                        VoteStatus = "Approved"
                    }
                }
            };

            // Act & Assert
            Assert.Equal(1, pullRequest.Id);
            Assert.Equal("Test PR", pullRequest.Title);
            Assert.Equal("Test Description", pullRequest.Description);
            Assert.Equal("http://test.com", pullRequest.Url);
            Assert.Equal("Active", pullRequest.Status);
            Assert.True(pullRequest.HasComments);
            Assert.False(pullRequest.HasPendingComments);
            Assert.Equal(new List<string> { "1", "2" }, pullRequest.WorkItemIds);
            Assert.Single(pullRequest.Reviewers);
            Assert.Equal("John Doe", pullRequest.Reviewers[0].DisplayName);
            Assert.Equal("http://avatar.com", pullRequest.Reviewers[0].AvatarUrl);
            Assert.Equal("Approved", pullRequest.Reviewers[0].VoteStatus);
        }
    }
}
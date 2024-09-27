using System.Threading.Tasks;
using PRNotifierApp.Models;
using PRNotifierApp.Clients;

namespace PRNotifierApp.Services
{
    public interface IAzureDevOpsService
    {
        Task<PullRequest> GetPullRequestDetailsAsync(int pullRequestId, string repositoryId, string projectId);
    }

    public class AzureDevOpsService : IAzureDevOpsService
    {
        private readonly IAzureDevOpsClient _azureDevOpsClient;

        public AzureDevOpsService(IAzureDevOpsClient azureDevOpsClient)
        {
            _azureDevOpsClient = azureDevOpsClient;
        }

        public async Task<PullRequest> GetPullRequestDetailsAsync(int pullRequestId, string repositoryId, string projectId)
        {
            // Implement the logic to fetch PR details using the Azure DevOps client
            // This is a simplified version, you'll need to add more details
            var prDetails = await _azureDevOpsClient.GetPullRequestAsync(pullRequestId, repositoryId, projectId);
            var workItems = await _azureDevOpsClient.GetWorkItemsAsync(pullRequestId, repositoryId, projectId);
            var reviewers = await _azureDevOpsClient.GetReviewersAsync(pullRequestId, repositoryId, projectId);

            return new PullRequest
            {
                Id = prDetails.Id,
                Title = prDetails.Title,
                Description = prDetails.Description,
                Url = prDetails.Url,
                Status = prDetails.Status,
                HasComments = prDetails.HasComments,
                HasPendingComments = prDetails.HasPendingComments,
                WorkItemIds = workItems,
                Reviewers = reviewers
            };
        }
    }
}
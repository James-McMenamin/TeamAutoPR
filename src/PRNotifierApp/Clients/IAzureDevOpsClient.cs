using System.Threading.Tasks;
using PRNotifierApp.Models;
using System.Collections.Generic;

namespace PRNotifierApp.Clients
{
    public interface IAzureDevOpsClient
    {
        Task<PullRequest> GetPullRequestAsync(int pullRequestId, string repositoryId, string projectId);
        Task<List<string>> GetWorkItemsAsync(int pullRequestId, string repositoryId, string projectId);
        Task<List<Reviewer>> GetReviewersAsync(int pullRequestId, string repositoryId, string projectId);
    }
}
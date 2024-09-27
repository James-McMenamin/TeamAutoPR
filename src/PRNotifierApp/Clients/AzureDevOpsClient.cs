using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRNotifierApp.Models;

namespace PRNotifierApp.Clients
{
    public class AzureDevOpsClient : IAzureDevOpsClient
    {
        public Task<PullRequest> GetPullRequestAsync(int pullRequestId, string repositoryId, string projectId)
        {
            // Implement the actual API call to Azure DevOps
            throw new NotImplementedException();
        }

        public Task<List<string>> GetWorkItemsAsync(int pullRequestId, string repositoryId, string projectId)
        {
            // Implement the actual API call to Azure DevOps
            throw new NotImplementedException();
        }

        public Task<List<Reviewer>> GetReviewersAsync(int pullRequestId, string repositoryId, string projectId)
        {
            // Implement the actual API call to Azure DevOps
            throw new NotImplementedException();
        }
    }
}
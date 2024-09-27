using System.Threading.Tasks;
using PRNotifierApp.Models;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using PRNotifierApp.Configuration;

[assembly: InternalsVisibleTo("PRNotifierApp.Tests")]

namespace PRNotifierApp.Services
{
    public interface IPRNotifierService
    {
        Task ProcessPullRequestEventAsync(dynamic eventData);
    }

    public class PRNotifierService : IPRNotifierService
    {
        private readonly IAzureDevOpsService _azureDevOpsService;
        private readonly ITeamsService _teamsService;
        private readonly TeamsConfiguration _teamsConfig;

        public PRNotifierService(
            IAzureDevOpsService azureDevOpsService, 
            ITeamsService teamsService,
            IOptions<TeamsConfiguration> teamsConfig)
        {
            _azureDevOpsService = azureDevOpsService;
            _teamsService = teamsService;
            _teamsConfig = teamsConfig.Value;
        }

        public async Task ProcessPullRequestEventAsync(dynamic eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            var pullRequestId = (int)eventData.resource.pullRequestId;
            var repositoryId = (string)eventData.resource.repository.id;
            var projectId = (string)eventData.resource.repository.project.id;

            var channelId = GetChannelIdForRepository(repositoryId);
            // Throw NotSupportedException here if multiple channels are detected
            if (channelId.Contains(","))
            {
                throw new NotSupportedException("Mapping a single repository to multiple channels is not supported.");
            }

            var pullRequest = await _azureDevOpsService.GetPullRequestDetailsAsync(pullRequestId, repositoryId, projectId);
            var card = CreateTeamsCard(pullRequest);
            await _teamsService.PostCardAsync(card, channelId);
        }

        private string GetChannelIdForRepository(string repositoryId)
        {
            if (_teamsConfig.RepositoryChannelMappings.TryGetValue(repositoryId, out var channelId))
            {
                return channelId;
            }
            return _teamsConfig.DefaultChannelId;
        }

        internal TeamsCard CreateTeamsCard(PullRequest pullRequest)
        {
            return new TeamsCard
            {
                Body = new List<CardElement>
                {
                    new CardElement
                    {
                        Type = "TextBlock",
                        Text = pullRequest.Title,
                        Size = "Large",
                        Weight = "Bolder"
                    },
                    new CardElement
                    {
                        Type = "TextBlock",
                        Text = $"Work Items: {string.Join(", ", pullRequest.WorkItemIds)}",
                        Size = "Medium",
                        Weight = "Normal",
                        Wrap = true
                    },
                    new CardElement
                    {
                        Type = "TextBlock",
                        Text = $"Status: {pullRequest.Status}",
                        Size = "Medium",
                        Weight = "Normal",
                        Wrap = true
                    },
                    new CardElement
                    {
                        Type = "TextBlock",
                        Text = $"Has Comments: {pullRequest.HasComments}",
                        Size = "Medium",
                        Weight = "Normal",
                        Wrap = true
                    },
                    new CardElement
                    {
                        Type = "TextBlock",
                        Text = $"Has Pending Comments: {pullRequest.HasPendingComments}",
                        Size = "Medium",
                        Weight = "Normal",
                        Wrap = true
                    }
                },
                Actions = new List<CardAction>
                {
                    new CardAction
                    {
                        Type = "Action.OpenUrl",
                        Title = "View PR",
                        Url = pullRequest.Url
                    }
                }
            };
        }
    }
}
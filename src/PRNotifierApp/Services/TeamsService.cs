using System.Threading.Tasks;
using PRNotifierApp.Models;
using PRNotifierApp.Clients;

namespace PRNotifierApp.Services
{
    public interface ITeamsService
    {
        Task<string> PostCardAsync(TeamsCard card, string channelId);
        Task UpdateCardAsync(TeamsCard card, string channelId, string messageId);
    }

    public class TeamsService : ITeamsService
    {
        private readonly ITeamsClient _teamsClient;

        public TeamsService(ITeamsClient teamsClient)
        {
            _teamsClient = teamsClient;
        }

        public async Task<string> PostCardAsync(TeamsCard card, string channelId)
        {
            return await _teamsClient.PostMessageAsync(card, channelId);
        }

        public async Task UpdateCardAsync(TeamsCard card, string channelId, string messageId)
        {
            await _teamsClient.UpdateMessageAsync(card, channelId, messageId);
        }
    }
}
using System;
using System.Threading.Tasks;
using PRNotifierApp.Models;

namespace PRNotifierApp.Clients
{
    public class TeamsClient : ITeamsClient
    {
        public Task<string> PostMessageAsync(TeamsCard card, string channelId)
        {
            // Implement the actual API call to Microsoft Teams
            throw new NotImplementedException();
        }

        public Task UpdateMessageAsync(TeamsCard card, string channelId, string messageId)
        {
            // Implement the actual API call to Microsoft Teams
            throw new NotImplementedException();
        }
    }
}
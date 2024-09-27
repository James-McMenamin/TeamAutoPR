using System.Threading.Tasks;
using PRNotifierApp.Models;

namespace PRNotifierApp.Clients
{
    public interface ITeamsClient
    {
        Task<string> PostMessageAsync(TeamsCard card, string channelId);
        Task UpdateMessageAsync(TeamsCard card, string channelId, string messageId);
    }
}
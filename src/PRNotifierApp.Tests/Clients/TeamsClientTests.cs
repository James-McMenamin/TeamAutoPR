using System;
using System.Threading.Tasks;
using PRNotifierApp.Clients;
using PRNotifierApp.Models;
using Xunit;

namespace PRNotifierApp.Tests.Clients
{
    public class TeamsClientTests
    {
        private readonly TeamsClient _client;

        public TeamsClientTests()
        {
            _client = new TeamsClient();
        }

        [Fact]
        public async Task PostMessageAsync_ThrowsNotImplementedException()
        {
            var card = new TeamsCard { Body = new List<CardElement>(), Actions = new List<CardAction>() };
            await Assert.ThrowsAsync<NotImplementedException>(() => 
                _client.PostMessageAsync(card, "channel"));
        }

        [Fact]
        public async Task UpdateMessageAsync_ThrowsNotImplementedException()
        {
            var card = new TeamsCard { Body = new List<CardElement>(), Actions = new List<CardAction>() };
            await Assert.ThrowsAsync<NotImplementedException>(() => 
                _client.UpdateMessageAsync(card, "channel", "message"));
        }
    }
}
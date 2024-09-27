using System.Threading.Tasks;
using Moq;
using PRNotifierApp.Clients;
using PRNotifierApp.Models;
using PRNotifierApp.Services;
using Xunit;

namespace PRNotifierApp.Tests.Services
{
    public class TeamsServiceTests
    {
        private readonly Mock<ITeamsClient> _mockClient;
        private readonly TeamsService _service;

        public TeamsServiceTests()
        {
            _mockClient = new Mock<ITeamsClient>();
            _service = new TeamsService(_mockClient.Object);
        }

        [Fact]
        public async Task PostCardAsync_ValidInput_ReturnsMessageId()
        {
            // Arrange
            var card = new TeamsCard
            {
                Body = new List<CardElement>(),
                Actions = new List<CardAction>()
            };
            var channelId = "channel-id";
            var expectedMessageId = "message-id";

            _mockClient.Setup(c => c.PostMessageAsync(card, channelId))
                .ReturnsAsync(expectedMessageId);

            // Act
            var result = await _service.PostCardAsync(card, channelId);

            // Assert
            Assert.Equal(expectedMessageId, result);
            _mockClient.Verify(c => c.PostMessageAsync(card, channelId), Times.Once);
        }

        [Fact]
        public async Task UpdateCardAsync_ValidInput_CallsUpdateMessage()
        {
            // Arrange
            var card = new TeamsCard
            {
                Body = new List<CardElement>(),
                Actions = new List<CardAction>()
            };
            var channelId = "channel-id";
            var messageId = "message-id";

            // Act
            await _service.UpdateCardAsync(card, channelId, messageId);

            // Assert
            _mockClient.Verify(c => c.UpdateMessageAsync(card, channelId, messageId), Times.Once);
        }
    }
}
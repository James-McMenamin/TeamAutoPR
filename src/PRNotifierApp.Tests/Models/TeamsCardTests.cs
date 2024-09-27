using PRNotifierApp.Models;
using System.Collections.Generic;
using Xunit;

namespace PRNotifierApp.Tests.Models
{
    public class TeamsCardTests
    {
        [Fact]
        public void TeamsCard_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var teamsCard = new TeamsCard
            {
                Body = new List<CardElement>
                {
                    new CardElement
                    {
                        Type = "TextBlock",
                        Text = "Test Text",
                        Size = "Medium",
                        Weight = "Normal",
                        Wrap = true
                    }
                },
                Actions = new List<CardAction>
                {
                    new CardAction
                    {
                        Type = "OpenUrl",
                        Title = "View",
                        Url = "http://test.com"
                    }
                }
            };

            // Act & Assert
            Assert.Equal("AdaptiveCard", teamsCard.Type);
            Assert.Equal("1.0", teamsCard.Version);
            Assert.Single(teamsCard.Body);
            Assert.Single(teamsCard.Actions);

            var cardElement = teamsCard.Body[0];
            Assert.Equal("TextBlock", cardElement.Type);
            Assert.Equal("Test Text", cardElement.Text);
            Assert.Equal("Medium", cardElement.Size);
            Assert.Equal("Normal", cardElement.Weight);
            Assert.True(cardElement.Wrap);

            var cardAction = teamsCard.Actions[0];
            Assert.Equal("OpenUrl", cardAction.Type);
            Assert.Equal("View", cardAction.Title);
            Assert.Equal("http://test.com", cardAction.Url);
        }
    }
}
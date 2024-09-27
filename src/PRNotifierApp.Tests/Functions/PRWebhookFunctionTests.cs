using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PRNotifierApp.Functions;
using PRNotifierApp.Services;
using Xunit;

namespace PRNotifierApp.Tests.Functions
{
    public class PRWebhookFunctionTests
    {
        private readonly Mock<IPRNotifierService> _mockPRNotifierService;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger> _mockLogger;
        private readonly PRWebhookFunction _function;

        public PRWebhookFunctionTests()
        {
            _mockPRNotifierService = new Mock<IPRNotifierService>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLogger = new Mock<ILogger>();
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);
            _function = new PRWebhookFunction(_mockPRNotifierService.Object, _mockLoggerFactory.Object);
        }

        [Theory]
        [InlineData("git.pullrequest.created")]
        [InlineData("git.pullrequest.updated")]
        public async Task Run_ValidPREvent_ReturnsOkResult(string eventType)
        {
            // Arrange
            var eventData = CreateEventData(eventType);
            var request = CreateMockRequest(eventData);

            // Act
            var result = await _function.Run(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            _mockPRNotifierService.Verify(s => s.ProcessPullRequestEventAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task Run_InvalidEventType_ReturnsBadRequestResult()
        {
            // Arrange
            var eventData = CreateEventData("git.pullrequest.invalid");
            var request = CreateMockRequest(eventData);

            // Act
            var result = await _function.Run(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            _mockPRNotifierService.Verify(s => s.ProcessPullRequestEventAsync(It.IsAny<object>()), Times.Never);

            // Add this assertion to check the response body content
            result.Body.Position = 0;
            using var reader = new StreamReader(result.Body);
            var content = await reader.ReadToEndAsync();
            Assert.Contains("Unsupported event type", content);
        }

        [Fact]
        public async Task Run_NullEventData_ReturnsBadRequestResult()
        {
            // Arrange
            var request = CreateMockRequest(null);

            // Act
            var result = await _function.Run(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            _mockPRNotifierService.Verify(s => s.ProcessPullRequestEventAsync(It.IsAny<object>()), Times.Never);

            // Add this assertion to check the response body content
            result.Body.Position = 0;
            using var reader = new StreamReader(result.Body);
            var content = await reader.ReadToEndAsync();
            Assert.Contains("Unsupported event type", content);
        }

        private object CreateEventData(string eventType)
        {
            return new
            {
                eventType = eventType,
                resource = new
                {
                    pullRequestId = 123,
                    repository = new
                    {
                        id = "repo-id",
                        project = new
                        {
                            id = "project-id"
                        }
                    }
                }
            };
        }

        private HttpRequestData CreateMockRequest(object eventData)
        {
            var json = eventData != null ? JsonConvert.SerializeObject(eventData) : "";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;

            var mockContext = new Mock<FunctionContext>();
            var mockRequest = new Mock<HttpRequestData>(mockContext.Object);
            var mockResponse = new Mock<HttpResponseData>(mockContext.Object);

            mockRequest.Setup(r => r.Body).Returns(stream);
            mockRequest.Setup(r => r.CreateResponse()).Returns(mockResponse.Object);
            mockResponse.SetupProperty(r => r.StatusCode);
            mockResponse.Setup(r => r.Body).Returns(new MemoryStream());

            return mockRequest.Object;
        }
    }
}
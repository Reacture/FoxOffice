namespace FoxOffice.api.commands
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Khala.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SendCreateMovieCommand : ApiTest
    {
        [TestMethod]
        public async Task when_send_CreateMovie_command_then_Movie_created_correctly()
        {
            // Arrange
            var command = new { Title = $"Movie-{Guid.NewGuid():n}" };

            // Act
            HttpResponseMessage commandResponse = await
                PostAsJson("api/commands/SendCreateMovieCommand", command);

            // Assert
            commandResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
            await Retry.Run(async () =>
            {
                Uri queryUri = ComposeUri(commandResponse.Headers.Location);
                HttpResponseMessage queryResponse = await Get(queryUri);
                queryResponse.EnsureSuccessStatusCode();
            });
        }
    }
}

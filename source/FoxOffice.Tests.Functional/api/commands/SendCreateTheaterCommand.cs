namespace FoxOffice.api.commands
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Khala.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SendCreateTheaterCommand : ApiTest
    {
        [TestMethod, AutoData]
        public async Task when_send_CreateTheater_command_then_Theater_created_correctly(
            [Range(10, 15)] int seatRowCount,
            [Range(20, 30)] int seatColumnCount)
        {
            // Arrange
            var command = new
            {
                Name = $"Theater-{Guid.NewGuid():n}",
                SeatRowCount = seatRowCount,
                SeatColumnCount = seatColumnCount,
            };

            // Act
            HttpResponseMessage commandResponse = await
                PostAsJson("api/commands/SendCreateTheaterCommand", command);

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

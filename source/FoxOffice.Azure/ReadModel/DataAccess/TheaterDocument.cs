namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using Newtonsoft.Json;

    public class TheaterDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }

        public string Discriminator => nameof(TheaterDocument);

        public string Name { get; set; }

        public int SeatRowCount { get; set; }

        public int SeatColumnCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

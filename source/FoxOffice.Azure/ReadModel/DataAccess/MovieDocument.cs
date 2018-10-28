namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MovieDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }

        public string Discriminator => nameof(MovieDocument);

        public string Title { get; set; }

        public List<ScreeningEntity> Screenings { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

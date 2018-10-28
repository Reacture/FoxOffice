namespace FoxOffice.api
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Khala.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class ApiTest
    {
        public static HttpClient TestClient { get; } = new HttpClient();

        public static RetryPolicy Retry { get; } = RetryPolicy.Linear(20, TimeSpan.FromMilliseconds(500));

        public TestContext TestContext { get; set; }

        protected Uri ComposeUri(string path)
        {
            dynamic endpoint = TestContext.Properties["ApiEndpoint"];
            return new Uri(new Uri(endpoint), path);
        }

        protected Uri ComposeUri(Uri path)
        {
            dynamic endpoint = TestContext.Properties["ApiEndpoint"];
            return new Uri(new Uri(endpoint), path);
        }

        protected Task<HttpResponseMessage> PostAsJson<T>(string path, T value)
        {
            return TestClient.PostAsJsonAsync(ComposeUri(path), value);
        }

        protected Task<HttpResponseMessage> Get(Uri uri)
        {
            return TestClient.GetAsync(uri);
        }
    }
}

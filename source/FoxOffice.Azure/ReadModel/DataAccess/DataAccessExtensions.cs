namespace FoxOffice.ReadModel.DataAccess
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    internal static class DataAccessExtensions
    {
        public static async Task<IEnumerable<T>> Execute<T>(
            this IDocumentQuery<T> query)
        {
            var documents = new List<T>();
            do
            {
                documents.AddRange(await query.ExecuteNextAsync<T>());
            }
            while (query.HasMoreResults);
            return documents;
        }

        public static async Task<T> SingleOrDefault<T>(
            this IDocumentQuery<T> query)
        {
            FeedResponse<T> response = await query.ExecuteNextAsync<T>();
            return response.SingleOrDefault();
        }
    }
}

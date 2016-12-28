using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace DdhpCore.ClubImporter.Runner.Extensions
{
    public static class TableQueryExtensions
    {
        public static async Task<IEnumerable<T>> BatchQuery<T>(this TableQuery<T> query, CloudTable table) where T: ITableEntity, new()
        {
            TableContinuationToken continuation = null;

            var results = new List<T>();

            do
            {
                var result = await table.ExecuteQuerySegmentedAsync(query, continuation);
                results.AddRange(result.Results as IEnumerable<T>);
                continuation = result.ContinuationToken;
            } while (continuation != null);

            return results;
        }
    }
}
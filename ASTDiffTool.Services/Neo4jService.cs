using ASTDiffTool.Services.Interfaces;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class Neo4jService : INeo4jService
    {
        private readonly IDriver _driver;

        public Neo4jService(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        }

        #region Queries
        public async Task<int> GetNodeCountAsync()
        {
            var query = "MATCH (n: Node) RETURN COUNT(n) AS nodeCount";
            return await ExecuteQuerySingleResultAsync<int>(query, "nodeCount");
        }
        #endregion

        private async Task<T> ExecuteQuerySingleResultAsync<T>(string query, string fieldName)
        {
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query);
                return (T)Convert.ChangeType(await result.SingleAsync(r => r[fieldName]), typeof(T));
            });
        }

        private async Task<Dictionary<TKey, TValue>> ExecuteQueryMultipleResultsAsync<TKey, TValue>(
            string query,
            string keyFieldName,
            string valueFieldName)
        {
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query);
                var dict = new Dictionary<TKey, TValue>();
                await foreach (var record in result)
                {
                    dict[(TKey)record[keyFieldName]] = (TValue)Convert.ChangeType(record[valueFieldName], typeof(TValue));
                }
                return dict;
            });
        }

        public async Task CloseAsync()
        {
            await _driver.DisposeAsync();
        }
    }
}

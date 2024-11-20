using ASTDiffTool.Services.Interfaces;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    using ASTDiffTool.Shared;
    using Neo4j.Driver;
    using System.Diagnostics;

    public class Neo4jService : INeo4jService
    {
        private readonly IDriver _driver;

        public Neo4jService(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        }

        public async Task<int> GetNodeCountAsync()
        {
            const string query = "MATCH (n:Node) RETURN count(n) AS count";

            using var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(query);
                var record = await result.SingleAsync();
                return record["count"].As<int>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying total node count: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetNodesByAstOriginAsync(ASTOrigins astOrigin)
        {
            const string query = @"
            MATCH (n:Node)
            WHERE n.astOrigin = $astOrigin
            RETURN count(n) AS count";

            var parameters = new { astOrigin = astOrigin.ToDatabaseString() };

            using var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(query, parameters);
                var record = await result.SingleAsync();
                return record["count"].As<int>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying nodes by AST origin: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetNodesByDifferenceTypeAsync(Differences differenceType)
        {
            const string query = @"
            MATCH (n:Node)
            WHERE n.diffType = $differenceType
            RETURN count(n) AS count";

            var parameters = new { differenceType = differenceType.ToDatabaseString() };

            using var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(query, parameters);
                var record = await result.SingleAsync();
                return record["count"].As<int>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error querying nodes by difference type: {ex.Message}");
                return 0;
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}

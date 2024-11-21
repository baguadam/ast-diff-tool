using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using ASTDiffTool.Shared;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class Neo4jService : INeo4jService, IDisposable
    {
        private readonly IDriver _driver;
        private readonly int _pageSize = 100;

        public Neo4jService(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        }

        #region Public Queries

        public async Task<int> GetNodeCountAsync()
        {
            const string query = "MATCH (n:Node) RETURN count(n) AS count";

            return await ExecuteSingleValueQueryAsync<int>(query, null, "count");
        }

        public async Task<int> GetNodesByAstOriginAsync(ASTOrigins astOrigin)
        {
            const string query = @"
            MATCH (n:Node)
            WHERE n.ast = $astOrigin
            RETURN count(n) AS count";

            return await ExecuteSingleValueQueryAsync<int>(query, new { astOrigin = astOrigin.ToDatabaseString() }, "count");
        }

        public async Task<int> GetNodesByDifferenceTypeAsync(Differences differenceType)
        {
            const string query = @"
            MATCH (n:Node)
            WHERE n.diffType = $differenceType
            RETURN count(n) AS count";

            return await ExecuteSingleValueQueryAsync<int>(query, new { differenceType = differenceType.ToDatabaseString() }, "count");
        }

        public async Task<List<Node>> GetHighestLevelSubtreesAsync(Differences differenceType, int page, int pageSize)
        {
            const string query = @"
        MATCH (root:Node {diffType: $diffType})
        WHERE root.isHighLevel = true
        WITH root
        ORDER BY root.enhancedKey
        SKIP $skip
        LIMIT $limit
        OPTIONAL MATCH (parent:Node)-[rel:HAS_CHILD]->(child:Node)
        WHERE parent.diffType = $diffType
        WITH root, collect({parent: parent, child: child}) AS relationships
        RETURN root, relationships";

            var parameters = new Dictionary<string, object>
    {
        { "diffType", differenceType.ToDatabaseString() },
        { "skip", Math.Max(0, page * pageSize) },
        { "limit", pageSize }
    };

            var nodesMap = new Dictionary<string, Node>();

            using var session = _driver.AsyncSession();
            await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query, parameters);

                while (await result.FetchAsync())
                {
                    var rootNodeRecord = result.Current["root"].As<INode>();
                    var rootNode = CreateNodeFromRecord(rootNodeRecord);
                    var rootKey = CreateCompositeKey(rootNode.EnhancedKey, rootNode.TopologicalOrder);

                    if (!nodesMap.ContainsKey(rootKey))
                    {
                        nodesMap[rootKey] = rootNode;
                    }

                    var relationships = result.Current["relationships"].As<IList<IDictionary<string, INode>>>();

                    foreach (var relationship in relationships)
                    {
                        var parentRecord = relationship["parent"];
                        var childRecord = relationship["child"];

                        var parentNode = CreateNodeFromRecord(parentRecord);
                        var childNode = CreateNodeFromRecord(childRecord);

                        var parentKey = CreateCompositeKey(parentNode.EnhancedKey, parentNode.TopologicalOrder);
                        var childKey = CreateCompositeKey(childNode.EnhancedKey, childNode.TopologicalOrder);

                        if (!nodesMap.ContainsKey(parentKey))
                        {
                            nodesMap[parentKey] = parentNode;
                        }

                        if (!nodesMap.ContainsKey(childKey))
                        {
                            nodesMap[childKey] = childNode;
                        }

                        // Link child to parent
                        if (!nodesMap[parentKey].Children.Contains(nodesMap[childKey]))
                        {
                            nodesMap[parentKey].Children.Add(nodesMap[childKey]);
                        }
                    }
                }
            });

            // Return only the root nodes
            return nodesMap.Values.Where(node => node.IsHighLevel).ToList();
        }

        public async Task<List<Node>> GetFlatNodesByDifferenceTypeAsync(Differences differenceType, int page, int pageSize = 100)
        {
            var query = @"
                MATCH (node:Node {diffType: $differenceType})
                RETURN node
                SKIP $skip LIMIT $limit";

            return await QueryFlatNodesAsync(query, new { differenceType = differenceType.ToDatabaseString(), skip = (page - 1) * pageSize, limit = pageSize });
        }

        #endregion

        #region Private Helper Methods

        private async Task<List<Node>> QueryFlatNodesAsync(string query, object parameters)
        {
            using var session = _driver.AsyncSession();
            var result = await session.RunAsync(query, parameters);

            var nodes = new List<Node>();
            await foreach (var record in result)
            {
                nodes.Add(CreateNodeFromRecord(record["node"].As<INode>()));
            }

            return nodes;
        }

        private async Task<T> ExecuteSingleValueQueryAsync<T>(string query, object parameters, string returnKey)
        {
            using var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(query, parameters);
                var record = await result.SingleAsync();
                return record[returnKey].As<T>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing query: {ex.Message}");
                throw;
            }
        }

        private Node CreateNodeFromRecord(INode record)
        {
            return new Node
            {
                EnhancedKey = record["enhancedKey"].As<string>(),
                TopologicalOrder = record["topologicalOrder"].As<int>(),
                Type = record["type"].As<string>(),
                Kind = record["kind"].As<string>(),
                Usr = record["usr"].As<string>(),
                Path = record["path"].As<string>(),
                LineNumber = record["lineNumber"].As<int>(),
                ColumnNumber = record["columnNumber"].As<int>(),
                IsHighLevel = record["isHighLevel"].As<bool>(),
                DifferenceType = record["diffType"].As<string>(),
                AstOrigin = record["ast"].As<string>(),
                Children = new List<Node>() // Initialize children list
            };
        }

        private string CreateCompositeKey(string enhancedKey, int topologicalOrder)
        {
            return $"{enhancedKey}_{topologicalOrder}";
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _driver?.Dispose();
        }

        #endregion
    }
}
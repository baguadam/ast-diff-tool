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

        public async Task<List<Node>> GetHighestLevelSubtreesAsync(Differences differenceType, int page, int pageSize = 20)
        {
            const string query = @"
                MATCH (root:Node {diffType: $differenceType})
                WHERE root.isHighLevel = true
                WITH root
                ORDER BY root.topologicalOrder
                SKIP $skip
                LIMIT $limit
                OPTIONAL MATCH path = (root)-[:HAS_CHILD*0..]->(descendant)
                RETURN root, collect(distinct path) AS paths";

            var parameters = new Dictionary<string, object>()
            {
                { "differenceType", differenceType.ToDatabaseString() },
                { "skip", (page - 1) * pageSize },
                { "limit", pageSize },
            };

            var treeBuilder = new TreeBuilder(); // local TreeBuilder

            using var session = _driver.AsyncSession();
            var result = await session.RunAsync(query, parameters);
            await result.ForEachAsync(record =>
            {
                // extract the root node from the record
                var rootNode = record["root"].As<INode>();
                Node root = CreateNodeFromRecord(rootNode);

                // cache to store already-created nodes by their IDs
                var nodeCache = new Dictionary<string, Node>();

                // check for root node existance
                if (!nodeCache.ContainsKey(rootNode.ElementId))
                {
                    nodeCache[rootNode.ElementId] = root;

                    if (!treeBuilder.RootNodes.Contains(root))
                    {
                        treeBuilder.RootNodes.Add(root);
                    }
                }

                // extract paths and add relationships
                var paths = record["paths"].As<List<IPath>>();
                foreach (var path in paths)
                {
                    foreach (var relationship in path.Relationships)
                    {
                        // get start and end node IDs
                        var startNodeId = relationship.StartNodeElementId;
                        var endNodeId = relationship.EndNodeElementId;

                        // get or create parent node from cache
                        if (!nodeCache.TryGetValue(startNodeId, out Node parentNode))
                        {
                            var parentNeo4jNode = path.Nodes.First(node => node.ElementId == startNodeId);
                            parentNode = CreateNodeFromRecord(parentNeo4jNode);
                            nodeCache[startNodeId] = parentNode;
                        }

                        // get or create child node from cache
                        if (!nodeCache.TryGetValue(endNodeId, out Node childNode))
                        {
                            var childNeo4jNode = path.Nodes.First(node => node.ElementId == endNodeId);
                            childNode = CreateNodeFromRecord(childNeo4jNode);
                            nodeCache[endNodeId] = childNode;
                        }

                        // add the relationship to TreeBuilder
                        treeBuilder.AddRelationship(parentNode, childNode);
                    }
                }
            });

            return treeBuilder.RootNodes;
        }

        public async Task<List<Node>> GetFlatNodesByDifferenceTypeAsync(Differences differenceType, int page, int pageSize = 20)
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
                Children = new List<Node>() // initialize children list
            };
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
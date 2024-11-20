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

        public async Task<List<Node>> GetSubtreesByDifferenceTypeAsync(Differences differenceType, int page, int pageSize = 100)
        {
            var query = @"
                MATCH (root:Node {diffType: $differenceType, isHighLevel: true})
                OPTIONAL MATCH (root)-[:HAS_CHILD*]->(child:Node)
                RETURN root, collect(child) AS children
                SKIP $skip LIMIT $limit";

            return await QueryHierarchicalNodesAsync(query, new { differenceType = differenceType.ToDatabaseString(), skip = (page - 1) * pageSize, limit = pageSize });
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
                nodes.Add(MapNode(record["node"].As<INode>()));
            }

            return nodes;
        }

        private async Task<List<Node>> QueryHierarchicalNodesAsync(string query, object parameters)
        {
            using var session = _driver.AsyncSession();
            var result = await session.RunAsync(query, parameters);

            var nodeDictionary = new Dictionary<string, Node>();
            await foreach (var record in result)
            {
                var root = MapNode(record["root"].As<INode>());

                if (!nodeDictionary.ContainsKey(root.EnhancedKey))
                {
                    nodeDictionary[root.EnhancedKey] = root;
                }

                var children = record["children"].As<List<INode>>();
                foreach (var child in children)
                {
                    var childNode = MapNode(child);

                    if (!nodeDictionary.ContainsKey(childNode.EnhancedKey))
                    {
                        nodeDictionary[childNode.EnhancedKey] = childNode;
                    }

                    if (nodeDictionary.TryGetValue(root.EnhancedKey, out var parentNode))
                    {
                        childNode.Parent = parentNode;
                        parentNode.Children.Add(childNode);
                    }
                }
            }

            return nodeDictionary.Values.Where(n => n.Parent == null).ToList();
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

        private static Node MapNode(INode node)
        {
            return new Node
            {
                EnhancedKey = node["enhancedKey"].As<string>(),
                Type = node["type"].As<string>(),
                DifferenceType = node["diffType"].As<string>(),
                TopologicalOrder = node["topologicalOrder"].As<int>(),
                Path = node["path"].As<string>(),
                LineNumber = node["lineNumber"].As<int>(),
                ColumnNumber = node["columnNumber"].As<int>(),
                IsHighLevel = node["isHighLevel"].As<bool>()
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
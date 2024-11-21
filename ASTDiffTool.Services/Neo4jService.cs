﻿using ASTDiffTool.Models;
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
                MATCH (root:Node {diffType: $differenceType})
                WHERE root.isHighLevel = true
                WITH root
                ORDER BY root.topologicalOrder
                SKIP $skip
                LIMIT $limit
                MATCH path = (root)-[:HAS_CHILD*0..]->(descendant)
                WITH root, descendant, path
                UNWIND relationships(path) AS rel
                WITH DISTINCT rel
                RETURN startNode(rel) AS parent, endNode(rel) AS child";

            var parameters = new Dictionary<string, object>()
            {
                { "differenceType", differenceType.ToDatabaseString() },
                { "skip", (page - 1) * pageSize },
                { "limit", pageSize },
            };

            var treeBuilder = new TreeBuilder(); // local treebuilder

            using var session = _driver.AsyncSession();
            var result = await session.RunAsync(query, parameters);
            await result.ForEachAsync(record =>
            {
                // extract parent and child for each record
                var parent = record["parent"].As<INode>();
                var child = record["child"].As<INode>();

                // create the nodes
                Node parentNode = CreateNodeFromRecord(parent);
                Node childNode = CreateNodeFromRecord(child);

                treeBuilder.AddRelationship(parentNode, childNode);
            });

            // return only the root nodes of the constructed tree
            return treeBuilder.RootNodes;
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
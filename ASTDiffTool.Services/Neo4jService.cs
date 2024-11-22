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
    /// <summary>
    /// Service to interact with the Neo4j database for managing and querying AST nodes.
    /// </summary>
    public class Neo4jService : INeo4jService, IDisposable
    {
        private readonly IDriver _driver;

        /// <summary>
        /// Gets the Neo4j driver instance. Primarily used for testing purposes.
        /// </summary>
        public IDriver Driver => _driver; // read-only for testing

        /// <summary>
        /// Initializes a new instance of the <see cref="Neo4jService"/> class with connection details.
        /// </summary>
        /// <param name="uri">The URI of the Neo4j server.</param>
        /// <param name="username">Username for authentication.</param>
        /// <param name="password">Password for authentication.</param>
        public Neo4jService(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Neo4jService"/> class with an existing Neo4j driver.
        /// </summary>
        /// <param name="driver">An instance of the Neo4j driver.</param>
        public Neo4jService(IDriver driver)
        {
            _driver = driver;
        }

        #region Public Queries
        /// <summary>
        /// Gets the total count of nodes in the database.
        /// </summary>
        /// <returns>The total number of nodes in the database.</returns>
        public async Task<int> GetNodeCountAsync()
        {
            const string query = "MATCH (n:Node) RETURN count(n) AS count";

            return await ExecuteSingleValueQueryAsync<int>(query, null, "count");
        }

        /// <summary>
        /// Gets the count of nodes based on the specified AST origin.
        /// </summary>
        /// <param name="astOrigin">The origin of the AST (FIRST_AST or SECOND_AST).</param>
        /// <returns>The number of nodes with the specified AST origin.</returns>
        public async Task<int> GetNodesByAstOriginAsync(ASTOrigins astOrigin)
        {
            const string query = @"
                MATCH (n:Node)
                WHERE n.ast = $astOrigin
                RETURN count(n) AS count";

            return await ExecuteSingleValueQueryAsync<int>(query, new { astOrigin = astOrigin.ToDatabaseString() }, "count");
        }

        /// <summary>
        /// Gets the count of nodes based on the specified difference type.
        /// </summary>
        /// <param name="differenceType">The type of difference (e.g., ONLY_IN_FIRST_AST, DIFFERENT_PARENTS).</param>
        /// <returns>The number of nodes with the specified difference type.</returns>
        public async Task<int> GetNodesByDifferenceTypeAsync(Differences differenceType)
        {
            const string query = @"
                MATCH (n:Node)
                WHERE n.diffType = $differenceType
                RETURN count(n) AS count";

            return await ExecuteSingleValueQueryAsync<int>(query, new { differenceType = differenceType.ToDatabaseString() }, "count");
        }

        /// <summary>
        /// Retrieves the highest level subtrees from the database based on the difference type.
        /// </summary>
        /// <param name="differenceType">The type of difference to filter the nodes.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of nodes to return per page.</param>
        /// <returns>A list of root nodes of the highest level subtrees.</returns>
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

        /// <summary>
        /// Retrieves flat nodes (non-hierarchical) from the database based on the difference type.
        /// </summary>
        /// <param name="differenceType">The type of difference to filter the nodes.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of nodes to return per page.</param>
        /// <returns>A list of nodes with the specified difference type.</returns>
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
        /// <summary>
        /// Queries the database for nodes using the specified query and parameters.
        /// </summary>
        /// <param name="query">The Cypher query string.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>A list of nodes matching the query.</returns>
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

                /// <summary>
        /// Executes a Cypher query that returns a single value, such as a count.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the query.</typeparam>
        /// <param name="query">The Cypher query string.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <param name="returnKey">The key to extract the result from the record.</param>
        /// <returns>The result value of type <typeparamref name="T"/>.</returns>
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

        /// <summary>
        /// Creates a Node object from a given Neo4j record.
        /// </summary>
        /// <param name="record">The Neo4j node record.</param>
        /// <returns>A <see cref="Node"/> object with its properties populated from the Neo4j record.</returns>
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
        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Neo4jService"/> class.
        /// </summary>
        public void Dispose()
        {
            _driver?.Dispose();
        }
        #endregion
    }
}
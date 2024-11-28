using ASTDiffTool.Services;
using ASTDiffTool.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ASTDiffTool.Tests
{
    [Collection("Neo4j collection")]
    public class Neo4jServiceIntegrationTests : IClassFixture<Neo4jTestFixture>
    {
        private readonly Neo4jService _neo4jService;
        private readonly Neo4jTestFixture _fixture;
        private readonly bool _isLocalEnvironment;

        public Neo4jServiceIntegrationTests(Neo4jTestFixture fixture)
        {
            _fixture = fixture;

            // local or CI
            _isLocalEnvironment = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));

            string uri, username, password;

            if (_isLocalEnvironment)
            {
                // local configs
                uri = "bolt://localhost:7688";
                username = "neo4j";
                password = "testpassword";
            }
            else
            {
                // CI env variables
                uri = Environment.GetEnvironmentVariable("NEO4J_URI")
                      ?? throw new InvalidOperationException("Environment variable 'NEO4J_URI' is not set.");
                username = Environment.GetEnvironmentVariable("NEO4J_USERNAME")
                           ?? throw new InvalidOperationException("Environment variable 'NEO4J_USERNAME' is not set.");
                password = Environment.GetEnvironmentVariable("NEO4J_PASSWORD")
                           ?? throw new InvalidOperationException("Environment variable 'NEO4J_PASSWORD' is not set.");
            }

            _neo4jService = new Neo4jService(uri, username, password);
        }

        /*****************************************
         * GetNodeCountAsync()
        *****************************************/
        [Fact]
        public async Task GetNodeCountAsync_ShouldReturnZeroInitially()
        {
            await ClearDatabase();
            var count = await _neo4jService.GetNodeCountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetNodeCountAsync_ShouldReturnOneAfterAddingOneNode()
        {
            await ClearDatabase();
            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: "None")
            });

            var count = await _neo4jService.GetNodeCountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetNodeCountAsync_ShouldReturnCorrectCountAfterAddingMultipleNodes()
        {
            await ClearDatabase();
            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: "None"),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: "None"),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: "None")
            });

            var count = await _neo4jService.GetNodeCountAsync();
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task GetNodeCountAsync_ShouldReturnZeroAfterAddingAndRemovingNodes()
        {
            await ClearDatabase();
            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: "None"),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: "None")
            });

            var countAfterAdding = await _neo4jService.GetNodeCountAsync();
            Assert.Equal(2, countAfterAdding);

            await ClearDatabase();
            var countAfterClearing = await _neo4jService.GetNodeCountAsync();
            Assert.Equal(0, countAfterClearing);
        }

        /*****************************************
         * GetNodesByAstOriginAsync()
        *****************************************/
        [Fact]
        public async Task GetNodesByAstOriginAsync_ShouldReturnCorrectCount()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ASTOrigins.FIRST_AST.ToDatabaseString(), "None"),
                CreateTestNode(ASTOrigins.FIRST_AST.ToDatabaseString(), "None")
            });

            var count = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.FIRST_AST);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetNodesByAstOriginAsync_ShouldReturnZero_WhenNoNodesExist()
        {
            await ClearDatabase();

            var countFirstAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.FIRST_AST);
            var countSecondAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.SECOND_AST);

            Assert.Equal(0, countFirstAst);
            Assert.Equal(0, countSecondAst);
        }

        [Fact]
        public async Task GetNodesByAstOriginAsync_ShouldReturnCorrectCount_WhenNodesExistFromFirstAstOnly()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ASTOrigins.FIRST_AST.ToDatabaseString(), "None"),
                CreateTestNode(ASTOrigins.FIRST_AST.ToDatabaseString(), "None")
            });

            var countFirstAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.FIRST_AST);
            var countSecondAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.SECOND_AST);

            Assert.Equal(2, countFirstAst);
            Assert.Equal(0, countSecondAst);
        }

        [Fact]
        public async Task GetNodesByAstOriginAsync_ShouldReturnCorrectCount_WhenNodesExistFromSecondAstOnly()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ASTOrigins.SECOND_AST.ToDatabaseString(), "None"),
                CreateTestNode(ASTOrigins.SECOND_AST.ToDatabaseString(), "None")
            });

            var countFirstAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.FIRST_AST);
            var countSecondAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.SECOND_AST);

            Assert.Equal(0, countFirstAst);
            Assert.Equal(2, countSecondAst);
        }

        [Fact]
        public async Task GetNodesByAstOriginAsync_ShouldReturnCorrectCount_WhenNodesExistFromBothAsts()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ASTOrigins.FIRST_AST.ToDatabaseString(), "None"),
                CreateTestNode(ASTOrigins.SECOND_AST.ToDatabaseString(), "None"),
                CreateTestNode(ASTOrigins.FIRST_AST.ToDatabaseString(), "None"),
                CreateTestNode(ASTOrigins.SECOND_AST.ToDatabaseString(), "None")
            });

            var countFirstAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.FIRST_AST);
            var countSecondAst = await _neo4jService.GetNodesByAstOriginAsync(ASTOrigins.SECOND_AST);

            Assert.Equal(2, countFirstAst);
            Assert.Equal(2, countSecondAst);
        }

        /*****************************************
         * GetNodesByDifferenceTypeAsync()
        *****************************************/
        [Fact]
        public async Task GetNodesByDifferenceTypeAsync_ShouldReturnZero_WhenNoNodesExist()
        {
            await ClearDatabase();

            foreach (Differences difference in Enum.GetValues(typeof(Differences)))
            {
                var count = await _neo4jService.GetNodesByDifferenceTypeAsync(difference);
                Assert.Equal(0, count);
            }
        }

        [Fact]
        public async Task GetNodesByDifferenceTypeAsync_ShouldReturnCorrectCount_WhenNodesExistFromOnlyInFirstAst()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString())
            });

            var count = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetNodesByDifferenceTypeAsync_ShouldReturnCorrectCount_WhenNodesExistFromOnlyInSecondAst()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_SECOND_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_SECOND_AST.ToDatabaseString())
            });

            var count = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.ONLY_IN_SECOND_AST);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetNodesByDifferenceTypeAsync_ShouldReturnCorrectCount_WhenNodesExistFromDifferentParents()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_PARENTS.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_PARENTS.ToDatabaseString())
            });

            var count = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.DIFFERENT_PARENTS);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetNodesByDifferenceTypeAsync_ShouldReturnCorrectCount_WhenNodesExistFromDifferentSourceLocations()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_SOURCE_LOCATIONS.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_SOURCE_LOCATIONS.ToDatabaseString())
            });

            var count = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.DIFFERENT_SOURCE_LOCATIONS);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetNodesByDifferenceTypeAsync_ShouldReturnCorrectCount_WhenNodesExistWithMixedDifferences()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_SECOND_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_PARENTS.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_SOURCE_LOCATIONS.ToDatabaseString())
            });

            var countOnlyInFirstAst = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST);
            var countOnlyInSecondAst = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.ONLY_IN_SECOND_AST);
            var countDifferentParents = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.DIFFERENT_PARENTS);
            var countDifferentSourceLocations = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.DIFFERENT_SOURCE_LOCATIONS);

            Assert.Equal(1, countOnlyInFirstAst);
            Assert.Equal(1, countOnlyInSecondAst);
            Assert.Equal(1, countDifferentParents);
            Assert.Equal(1, countDifferentSourceLocations);
        }

        /*****************************************
         * GetFlatNodesByDifferenceTypeAsync()
        *****************************************/
        [Fact]
        public async Task GetFlatNodesByDifferenceTypeAsync_ShouldReturnEmptyList_WhenNoNodesExist()
        {
            await ClearDatabase();

            var nodes = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Empty(nodes);
        }

        [Fact]
        public async Task GetFlatNodesByDifferenceTypeAsync_ShouldReturnEmptyList_WhenRequestingPageBeyondData()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString())
            });

            var nodes = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST, page: 2, pageSize: 10);
            Assert.Empty(nodes);
        }

        [Fact]
        public async Task GetFlatNodesByDifferenceTypeAsync_ShouldReturnCorrectNodes_WhenNodesExistWithoutPagination()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString())
            });

            var nodes = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Equal(2, nodes.Count);
            Assert.All(nodes, node => Assert.Equal(Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), node.DifferenceType));
        }

        [Fact]
        public async Task GetFlatNodesByDifferenceTypeAsync_ShouldReturnCorrectNodes_WhenUsingPagination()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString())
            });

            // page 1 with page size 2
            var nodesPage1 = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 2);
            Assert.Equal(2, nodesPage1.Count);

            // page 2 with page size 2
            var nodesPage2 = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST, page: 2, pageSize: 2);
            Assert.Single(nodesPage2);
        }

        [Fact]
        public async Task GetFlatNodesByDifferenceTypeAsync_ShouldReturnCorrectNodes_WhenNodesWithMixedDifferencesExist()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_SECOND_AST.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_PARENTS.ToDatabaseString()),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.DIFFERENT_SOURCE_LOCATIONS.ToDatabaseString())
            });

            var nodes = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Single(nodes);
            Assert.All(nodes, node => Assert.Equal(Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), node.DifferenceType));
        }

        /*****************************************
         * GetHighestLevelSubtreesAsync()
        *****************************************/
        [Fact]
        public async Task GetHighestLevelSubtreesAsync_ShouldReturnEmptyList_WhenNoNodesExist()
        {
            await ClearDatabase();

            var subtrees = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Empty(subtrees);
        }

        [Fact]
        public async Task GetHighestLevelSubtreesAsync_ShouldReturnEmptyList_WhenNoHighLevelNodesExist()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: false),
                CreateTestNode(ast: ASTOrigins.SECOND_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: false)
            });

            var subtrees = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Empty(subtrees);
        }

        [Fact]
        public async Task GetHighestLevelSubtreesAsync_ShouldReturnHighLevelNodes_WhenTheyExistWithoutRelationships()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: true),
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: true)
            });

            var subtrees = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Equal(2, subtrees.Count);
            Assert.All(subtrees, node => Assert.True(node.IsHighLevel));
        }

        [Fact]
        public async Task GetHighestLevelSubtreesAsync_ShouldReturnSubtree_WhenHighLevelNodesWithDescendantsExist()
        {
            await ClearDatabase();

            var rootNode = CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: true, enhancedKey: "root1", topologicalOrder: 1);
            var childNode1 = CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), enhancedKey: "child1", topologicalOrder: 2);
            var childNode2 = CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), enhancedKey: "child2", topologicalOrder: 3);

            await SeedDatabase(new[] { rootNode, childNode1, childNode2 });
            await CreateRelationship(rootNode, childNode1);
            await CreateRelationship(rootNode, childNode2);

            var subtrees = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Single(subtrees);

            var root = subtrees.First();
            Assert.True(root.IsHighLevel);
            Assert.Equal(2, root.Children.Count);
        }

        [Fact]
        public async Task GetHighestLevelSubtreesAsync_ShouldReturnOnlyRequestedDifferenceType_WhenMixedDifferencesExist()
        {
            await ClearDatabase();

            var rootNode = CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: true, enhancedKey: "root1", topologicalOrder: 1);
            var childNode = CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_SECOND_AST.ToDatabaseString(), isHighLevel: false, enhancedKey: "child1", topologicalOrder: 2);

            await SeedDatabase(new[] { rootNode, childNode });
            await CreateRelationship(rootNode, childNode);

            var subtrees = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 10);
            Assert.Single(subtrees);
            Assert.All(subtrees, node => Assert.Equal(Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), node.DifferenceType));
        }

        [Fact]
        public async Task GetHighestLevelSubtreesAsync_ShouldHandlePaginationCorrectly()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: true, enhancedKey: "root1", topologicalOrder: 1),
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: true, enhancedKey: "root2", topologicalOrder: 2),
                CreateTestNode(ast: ASTOrigins.FIRST_AST.ToDatabaseString(), diffType: Differences.ONLY_IN_FIRST_AST.ToDatabaseString(), isHighLevel: true, enhancedKey: "root3", topologicalOrder: 3)
            });

            var subtreesPage1 = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.ONLY_IN_FIRST_AST, page: 1, pageSize: 2);
            var subtreesPage2 = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.ONLY_IN_FIRST_AST, page: 2, pageSize: 2);

            Assert.Equal(2, subtreesPage1.Count);
            Assert.Single(subtreesPage2);
        }

        /*****************************************
         * HELPER METHODS
        *****************************************/
        private async Task ClearDatabase()
        {
            const string query = "MATCH (n) DETACH DELETE n";
            using var session = _neo4jService.Driver.AsyncSession();
            try
            {
                await session.RunAsync(query);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private async Task SeedDatabase(IEnumerable<(string Label, Dictionary<string, object> Properties)> nodes)
        {
            using var session = _neo4jService.Driver.AsyncSession();
            try
            {
                foreach (var (label, properties) in nodes)
                {
                    var props = string.Join(", ", properties.Keys.Select(key => $"{key}: ${key}"));
                    var query = $"CREATE (:{label} {{{props}}})";
                    await session.RunAsync(query, properties);
                }
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private (string Label, Dictionary<string, object> Properties) CreateTestNode(
            string ast,
            string diffType,
            string enhancedKey = "defaultEnhancedKey",
            int topologicalOrder = 0,
            string type = "defaultType",
            string kind = "defaultKind",
            string usr = "defaultUsr",
            string path = "defaultPath",
            int lineNumber = 1,
            int columnNumber = 1,
            bool isHighLevel = false)
        {
            return (
                "Node",
                new Dictionary<string, object>
                {
                    { "ast", ast },
                    { "diffType", diffType },
                    { "enhancedKey", enhancedKey },
                    { "topologicalOrder", topologicalOrder },
                    { "type", type },
                    { "kind", kind },
                    { "usr", usr },
                    { "path", path },
                    { "lineNumber", lineNumber },
                    { "columnNumber", columnNumber },
                    { "isHighLevel", isHighLevel }
                }
            );
        }

        private async Task CreateRelationship((string Label, Dictionary<string, object> Properties) parentNode, (string Label, Dictionary<string, object> Properties) childNode)
        {
            using var session = _neo4jService.Driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (a:Node {enhancedKey: $parentKey, topologicalOrder: $parentOrder})
                    MATCH (b:Node {enhancedKey: $childKey, topologicalOrder: $childOrder})
                    CREATE (a)-[:HAS_CHILD]->(b)";

                var parameters = new
                {
                    parentKey = parentNode.Properties["enhancedKey"],
                    parentOrder = parentNode.Properties["topologicalOrder"],
                    childKey = childNode.Properties["enhancedKey"],
                    childOrder = childNode.Properties["topologicalOrder"]
                };

                await session.RunAsync(query, parameters);
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
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
    public class Neo4jServiceTests : IClassFixture<Neo4jTestFixture>
    {
        private readonly Neo4jService _neo4jService;
        private readonly Neo4jTestFixture _fixture;

        public Neo4jServiceTests(Neo4jTestFixture fixture)
        {
            _fixture = fixture;
            _neo4jService = new Neo4jService("bolt://localhost:7688", "neo4j", "testpassword"); // neo4j is initialized after fixture
        }

        [Fact]
        public async Task GetNodeCountAsync_ShouldReturnZeroInitially()
        {
            await ClearDatabase();
            var count = await _neo4jService.GetNodeCountAsync();
            Assert.Equal(0, count);
        }

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
        public async Task GetNodesByDifferenceTypeAsync_ShouldReturnCorrectCount()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(ASTOrigins.FIRST_AST.ToDatabaseString(), Differences.ONLY_IN_FIRST_AST.ToDatabaseString()),
                CreateTestNode(ASTOrigins.SECOND_AST.ToDatabaseString(), Differences.ONLY_IN_FIRST_AST.ToDatabaseString())
            });

            var count = await _neo4jService.GetNodesByDifferenceTypeAsync(Differences.ONLY_IN_FIRST_AST);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetFlatNodesByDifferenceTypeAsync_ShouldReturnFlatNodes()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(
                    ast: ASTOrigins.FIRST_AST.ToDatabaseString(),
                    diffType: Differences.DIFFERENT_PARENTS.ToDatabaseString(),
                    enhancedKey: "someValue",
                    topologicalOrder: 1,
                    type: "defaultType",
                    kind: "defaultKind",
                    usr: "defaultUsr",
                    path: "defaultPath",
                    lineNumber: 10,
                    columnNumber: 5,
                    isHighLevel: false
                ),
                CreateTestNode(
                    ast: ASTOrigins.FIRST_AST.ToDatabaseString(),
                    diffType: Differences.DIFFERENT_PARENTS.ToDatabaseString(),
                    enhancedKey: "anotherValue",
                    topologicalOrder: 2,
                    type: "defaultType",
                    kind: "defaultKind",
                    usr: "anotherUsr",
                    path: "anotherPath",
                    lineNumber: 15,
                    columnNumber: 3,
                    isHighLevel: true
                )
            });

            var nodes = await _neo4jService.GetFlatNodesByDifferenceTypeAsync(Differences.DIFFERENT_PARENTS, 1, 10);
            Assert.Equal(2, nodes.Count);
        }

        [Fact]
        public async Task GetHighestLevelSubtreesAsync_ShouldReturnCorrectSubtrees()
        {
            await ClearDatabase();

            await SeedDatabase(new[]
            {
                CreateTestNode(
                    ast: ASTOrigins.FIRST_AST.ToDatabaseString(),
                    diffType: Differences.DIFFERENT_SOURCE_LOCATIONS.ToDatabaseString(),
                    enhancedKey: "someValue",
                    topologicalOrder: 1,
                    type: "defaultType",
                    kind: "defaultKind",
                    usr: "usr1",
                    path: "defaultPath",
                    lineNumber: 1,
                    columnNumber: 1,
                    isHighLevel: true
                ),
                CreateTestNode(
                    ast: ASTOrigins.SECOND_AST.ToDatabaseString(),
                    diffType: Differences.DIFFERENT_SOURCE_LOCATIONS.ToDatabaseString(),
                    enhancedKey: "anotherValue",
                    topologicalOrder: 2,
                    type: "defaultType",
                    kind: "defaultKind",
                    usr: "usr2",
                    path: "anotherPath",
                    lineNumber: 2,
                    columnNumber: 2,
                    isHighLevel: false
                )
            });

            var subtrees = await _neo4jService.GetHighestLevelSubtreesAsync(Differences.DIFFERENT_SOURCE_LOCATIONS, 1, 10);
            Assert.Single(subtrees);
        }

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
    }
}
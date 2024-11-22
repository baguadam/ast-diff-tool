using ASTDiffTool.Services;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Tests
{
    public class Neo4jServiceIntegrationTests : IClassFixture<Neo4jTestFixture>
    {
        private readonly IDriver _driver;

        public Neo4jServiceIntegrationTests()
        {
            _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "testpassword"));
        }

        [Fact]
        public async Task GetNodeCountAsync_ReturnsCorrectCount()
        {
            // seed the database
            var seedQuery = @"
            CREATE (n:Node {name: 'Test Node', diffType: 'SomeType'}),
                   (m:Node {name: 'Another Node', diffType: 'AnotherType'})";

            using var session = _driver.AsyncSession();
            await session.RunAsync(seedQuery);

            var service = new Neo4jService(_driver);

            // call service method
            var count = await service.GetNodeCountAsync();

            // assert
            Assert.Equal(2, count);
        }
    }
}

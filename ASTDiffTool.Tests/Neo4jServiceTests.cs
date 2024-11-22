using ASTDiffTool.Services;
using Moq;
using Neo4j.Driver;
using System.Threading.Tasks;
using Xunit;

namespace ASTDiffTool.Tests
{
    public class Neo4jServiceTests
    {
        private readonly Mock<IDriver> _mockDriver;
        private readonly Mock<IAsyncSession> _mockSession;
        private readonly Neo4jService _service;

        public Neo4jServiceTests()
        {
            _mockDriver = new Mock<IDriver>();
            _mockSession = new Mock<IAsyncSession>();

            // driver return a mock session
            _mockDriver.Setup(d => d.AsyncSession(It.IsAny<Action<SessionConfigBuilder>>()))
                       .Returns(_mockSession.Object);

            // service with the mock driver
            _service = new Neo4jService(_mockDriver.Object);
        }
    }
}

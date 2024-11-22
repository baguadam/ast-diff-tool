using Neo4j.Driver;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ASTDiffTool.Tests
{
    public class Neo4jTestFixture : IAsyncLifetime
    {
        private readonly IDriver _driver;
        private readonly bool _isLocalEnvironment;

        public IDriver Driver => _driver;

        public Neo4jTestFixture()
        {
            // local or CI
            _isLocalEnvironment = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));

            if (_isLocalEnvironment)
            {
                // local environment
                Console.WriteLine("Starting Neo4j Docker container locally...");
                RunDockerComposeCommand("up -d --remove-orphans");

                _driver = GraphDatabase.Driver("bolt://localhost:7688", AuthTokens.Basic("neo4j", "testpassword"));
            }
            else
            {
                // CI/CD, Neo4j is already running
                _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "testpassword"));
            }
        }

        public async Task InitializeAsync()
        {
            if (_isLocalEnvironment)
            {
                Console.WriteLine("Waiting for Neo4j to become healthy locally...");
                await WaitForNeo4jHealthAsync();
            }

            Console.WriteLine("Neo4j is ready.");
        }

        public async Task DisposeAsync()
        {
            if (_isLocalEnvironment)
            {
                Console.WriteLine("Stopping and removing Neo4j Docker container locally...");
                RunDockerComposeCommand("down --remove-orphans");
            }

            await _driver.DisposeAsync();
        }

        private static void RunDockerComposeCommand(string args)
        {
            var workingDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../"));
            Console.WriteLine($"Running Docker Compose command: {args} in {workingDirectory}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"compose {args}",
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Docker Compose command failed:\nError: {error}\nOutput: {output}");
            }

            Console.WriteLine($"Docker Compose command succeeded:\n{output}");
        }

        private static async Task WaitForNeo4jHealthAsync()
        {
            const int maxRetries = 20;
            const int delayMilliseconds = 3000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using var driver = GraphDatabase.Driver("bolt://localhost:7688", AuthTokens.Basic("neo4j", "testpassword"));
                    using var session = driver.AsyncSession();
                    await session.RunAsync("RETURN 1");
                    return; // success
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Neo4j is not ready yet. Retrying in {delayMilliseconds / 1000} seconds... Attempt {i + 1}/{maxRetries}. Error: {ex.Message}");
                    await Task.Delay(delayMilliseconds);
                }
            }

            throw new Exception("Neo4j did not become healthy in the expected time.");
        }
    }
}
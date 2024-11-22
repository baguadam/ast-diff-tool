﻿using Neo4j.Driver;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ASTDiffTool.Tests
{
    public class Neo4jTestFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            Console.WriteLine("Starting Neo4j Docker container...");
            RunDockerComposeCommand("up -d --remove-orphans");

            Console.WriteLine("Waiting for Neo4j to become healthy...");
            await WaitForNeo4jHealthAsync();
            Console.WriteLine("Neo4j is healthy and ready.");
        }

        public Task DisposeAsync()
        {
            Console.WriteLine("Stopping and removing Neo4j Docker container...");
            RunDockerComposeCommand("down --remove-orphans");
            Console.WriteLine("Neo4j container stopped.");
            return Task.CompletedTask;
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
            const int maxRetries = 30;
            const int delayMilliseconds = 5000; // delay for startup

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
                    Console.WriteLine($"Neo4j is not ready yet (attempt {i + 1}/{maxRetries}): {ex.Message}. Retrying in {delayMilliseconds / 1000} seconds...");
                    await Task.Delay(delayMilliseconds);
                }
            }

            throw new Exception("Neo4j did not become healthy in the expected time.");
        }
    }
}
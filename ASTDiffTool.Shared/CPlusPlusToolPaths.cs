using System;
using System.IO;

namespace ASTDiffTool.Shared
{
    /// <summary>
    /// Contains the path-related information for the C++ Tool.
    /// </summary>
    public static class CPlusPlusToolPaths
    {
        /// <summary>
        /// Gets the path to the dump tool.
        /// </summary>
        public static string DumpToolPath => NormalizePath(Path.Combine(GetToolPath(), "dump-tool\\build\\clang_ast_tool.exe"));

        /// <summary>
        /// Gets the path to the comparer tool.
        /// </summary>
        public static string ComparerToolPath => NormalizePath(Path.Combine(GetToolPath(), "comparer\\build\\ast-tree-comparer.exe"));

        /// <summary>
        /// Gets the base directory for AST projects.
        /// </summary>
        public static string BaseASTDirectoryPath => NormalizePath(Path.Combine(GetToolPath(), "asts"));

        /// <summary>
        /// Gets the path to the temporary AST directory.
        /// </summary>
        public static string TempASTPath => NormalizePath(Path.Combine(Path.GetTempPath(), "AST"));

        /// <summary>
        /// Resolves the base tool path from the environment variable.
        /// </summary>
        /// <returns>The resolved tool path.</returns>
        private static string GetToolPath()
        {
            string toolPath = Environment.GetEnvironmentVariable("TOOL_PATH");
            if (string.IsNullOrEmpty(toolPath))
            {
                throw new InvalidOperationException("The environment variable 'TOOL_PATH' is not set.");
            }
            return toolPath;
        }

        /// <summary>
        /// Normalizes a file or directory path.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        private static string NormalizePath(string path) => Path.GetFullPath(path);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    /// <summary>
    /// Contains the path related information for the C++ Tool.
    /// </summary>
    public static class CPlusPlusToolPaths
    {
        /// <summary>
        /// Path to the dump tool.
        /// </summary>
        public static readonly string DUMP_TOOL_PATH = "C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Adam\\Egyetem - 07\\SZAKDOLGOZAT\\ast-tree-comparer\\dump-tool\\build\\clang_ast_tool.exe";
        
        /// <summary>
        /// Path to the comparer tool.
        /// </summary>
        public static readonly string COMPARER_TOOL_PATH = "C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Adam\\Egyetem - 07\\SZAKDOLGOZAT\\ast-tree-comparer\\comparer\\build\\ast-tree-comparer.exe";
        
        /// <summary>
        /// Path to the directory that contains the projects.
        /// </summary>
        public static readonly string BASE_AST_DIRECTORY_PATH = "C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Adam\\Egyetem - 07\\SZAKDOLGOZAT\\ast-tree-comparer\\asts";
        
        /// <summary>
        /// Path to the temps for creating the temp compile_commands.json
        /// </summary>
        public static readonly string TEMP_AST_PATH = Path.Combine(Path.GetTempPath(), "AST");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    public static class CPlusPlusToolPaths
    {
        public static readonly string DUMP_TOOL_PATH = "C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Adam\\Egyetem - 07\\SZAKDOLGOZAT\\ast-tree-comparer\\dump-tool\\build\\clang_ast_tool.exe";
        public static readonly string COMPARER_TOOL_PATH = "C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Adam\\Egyetem - 07\\SZAKDOLGOZAT\\ast-tree-comparer\\comparer\\build\\ast-tree-comparer.exe";
        public static readonly string BASE_AST_DIRECTORY_PATH = "C:\\Users\\bagua\\OneDrive - Eotvos Lorand Tudomanyegyetem\\Adam\\Egyetem - 07\\SZAKDOLGOZAT\\ast-tree-comparer\\asts";
        public static readonly string TEMP_AST_PATH = Path.Combine(Path.GetTempPath(), "AST");
    }
}

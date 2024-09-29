using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class ProjectSettings
    {
        private const int INDEX_FOR_FIRST = 0;
        private const int INDEX_FOR_SECOND = 1;
        public List<string> AllStandards => ["C++98", "C++03", "C++11", "C++14", "C++17"];

        public int FirstSelectedStandard { get; set; } = INDEX_FOR_FIRST;
        public int SecondSelectedStandard { get; set; } = INDEX_FOR_SECOND;
        public bool IsStoreAssemblyChecked { get; set; } = false;
        public bool IsStorePreprocessedCodeChecked { get; set; } = false;
    }
}

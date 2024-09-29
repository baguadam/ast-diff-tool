using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class ProjectSettings
    {
        public List<string> AllStandards { get; } = ["C++98", "C++03", "C++11", "C++14", "C++17"];
    }
}

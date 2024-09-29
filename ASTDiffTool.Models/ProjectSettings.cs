using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class ProjectSettings
    {
        public List<string> AllStandards => ["C++98", "C++03", "C++11", "C++14", "C++17"];

        public int FirstSelectedStandard { get; private set; }
        public int SecondSelectedStandard { get; private set; }

        public ProjectSettings()
        {
            FirstSelectedStandard = 0;
            SecondSelectedStandard = 1;
        }

        public void ChangeFirstSelectedStandard(int standardIndex)
        {
            FirstSelectedStandard = standardIndex;
        }

        public void ChangeSecondSelectedStandard(int standardIndex) 
        { 
            SecondSelectedStandard = standardIndex;
        }
    }
}

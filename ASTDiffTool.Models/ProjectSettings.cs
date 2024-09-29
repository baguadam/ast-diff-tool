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
        public List<string> AllStandards => ["C++98", "C++03", "C++11", "C++14", "C++17"];

        public int FirstSelectedStandard { get; private set; }
        public int SecondSelectedStandard { get; private set; }

        public ProjectSettings()
        {
            FirstSelectedStandard = 0;
            SecondSelectedStandard = 0;
        }

        public void UpdateSelectedStandards(int firstSelectedIndex, int secondSelectedIndex)
        {
            Debug.WriteLine($"METHOD CALLED, BEFORE: {FirstSelectedStandard}, {SecondSelectedStandard}");
            FirstSelectedStandard = firstSelectedIndex;
            SecondSelectedStandard = secondSelectedIndex;
            Debug.WriteLine($"AFTER: {FirstSelectedStandard}, {SecondSelectedStandard}");
        }
    }
}

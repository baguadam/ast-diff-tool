using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class Project
    {
        public string Name { get; set; }
        public int NumberOfDifferences { get; set; } = 15; // for testing purpose
    }
}

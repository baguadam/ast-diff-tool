using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class ProjectDatabaseInfoModel
    {
        public int TotalNodeCount { get; set; }
        public int NodesInFirstAST { get; set; }
        public int NodesInSecondAST { get; set; }
        public int OnlyInFirstAST { get; set; }
        public int OnlyInSecondAST { get; set; }
        public int DifferentParents { get; set; }
        public int DifferentSourceLocations { get; set; }
    }
}

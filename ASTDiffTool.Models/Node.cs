using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class Node
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public string Kind { get; set; }
        public string Usr { get; set; }
        public string Path { get; set; }
        public int DifferenceType { get; set; }
        public int AST { get; set; }
        public bool IsHighestLevelNode { get; set; } = false;
        public string Comment { get; set; }

        // Navigation property for related Edge entities where this Node is a parent
        public ICollection<Edge> ParentEdges { get; set; }

        // Navigation property for related Edge entities where this Node is a child
        public ICollection<Edge> ChildEdges { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class Node
    {
        public string EnhancedKey { get; set; }
        public int TopologicalOrder { get; set; }
        public string Type { get; set; }
        public string Kind { get; set; }
        public string Usr { get; set; }
        public string Path { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public bool IsHighLevel { get; set; }
        public string DifferenceType { get; set; }
        public string AstOrigin { get; set; }
        
        public IList<Node> Children { get; set; } = new List<Node>();
        public bool HasItems => Children != null && Children.Count > 0;

        public override string ToString()
        {
            return $"{Kind} (Topological order: {TopologicalOrder})";
        }
    }
}

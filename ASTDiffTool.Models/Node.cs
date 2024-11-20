using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class Node
    {
        public string EnhancedKey { get; set; }
        public string Type { get; set; }
        public string DifferenceType { get; set; }
        public int TopologicalOrder {  get; set; }
        public string Path {  get; set; }
        public int LineNumber {  get; set; }
        public int ColumnNumber { get; set; }
        public bool IsHighLevel {  get; set; }
        public Node? Parent { get; set; }
        public List<Node> Children { get; set; } = [];

        public override string ToString()
        {
            return $"{Type} - {EnhancedKey} - {Path} {LineNumber}:{ColumnNumber}";
        }
    }
}

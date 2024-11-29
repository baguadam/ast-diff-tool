using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    /// <summary>
    /// Class that represents the structure of the nodes.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// The enhanced key of the node.
        /// </summary>
        public string EnhancedKey { get; set; }

        /// <summary>
        /// Topological order of the node, unique number.
        /// </summary>
        public int TopologicalOrder { get; set; }

        /// <summary>
        /// The type of the node whether it is a Declaration or a Statement.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The kind of the node.
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// The USR of the node, in most cases NOT unique.
        /// </summary>
        public string Usr { get; set; }

        /// <summary>
        /// The path of the node.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Line number of the node.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Column number of the node.
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Indicates whether the node is a highest level node.
        /// </summary>
        public bool IsHighLevel { get; set; }

        /// <summary>
        /// Indicates what is the detected difference of the node.
        /// </summary>
        public string DifferenceType { get; set; }

        /// <summary>
        /// The AST origin of the node whether it is FIRST_AST or SECOND_AST.
        /// </summary>
        public string AstOrigin { get; set; }
        
        /// <summary>
        /// The children of the node
        /// </summary>
        public IList<Node> Children { get; set; } = new List<Node>();

        /// <summary>
        /// Whether the node has any children.
        /// </summary>
        public bool HasItems => Children != null && Children.Count > 0;

        /// <summary>
        /// Retrieves the kind of the node combined with its topological order.
        /// </summary>
        public string KindWithOrder => $"{Kind} - {TopologicalOrder}";

        /// <summary>
        /// Overrides the ToString() method to be displayed in the view.
        /// </summary>
        /// <returns>Combined kind and topological order</returns>
        public override string ToString()
        {
            return $"{Kind} (Topological order: {TopologicalOrder})";
        }
    }
}
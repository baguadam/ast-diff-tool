using ASTDiffTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    /// <summary>
    /// Class responsible for creating the tree structure from the nodes, maintaining the hierarchical order.
    /// </summary>
    public class TreeBuilder
    {
        private readonly Dictionary<string, Node> nodeLookup = new();

        /// <summary>
        /// List of the root nodes.
        /// </summary>
        public List<Node> RootNodes { get; set; } = new();

        /// <summary>
        /// Handles the relationship between two nodes, sets the node as children if needed. 
        /// </summary>
        /// <param name="parentNode">The parent node</param>
        /// <param name="childNode">The child node</param>
        public void AddRelationship(Node parentNode, Node childNode)
        {
            string parentUniqueKey = parentNode.EnhancedKey + "-" + parentNode.TopologicalOrder;
            string childUniqueKey = childNode.EnhancedKey + "-" + childNode.TopologicalOrder;

            if (!nodeLookup.TryGetValue(parentUniqueKey, out Node existingParentNode))
            {
                existingParentNode = parentNode;
                nodeLookup[parentUniqueKey] = existingParentNode;

                // highest level node
                if (parentNode.IsHighLevel &&!RootNodes.Contains(existingParentNode))
                {
                    RootNodes.Add(existingParentNode);
                }
            }

            if (!nodeLookup.TryGetValue(childUniqueKey, out Node existingChildNode))
            {
                existingChildNode = childNode;
                nodeLookup[childUniqueKey] = existingChildNode;
            }

            // avoid duplave paths
            if (!existingParentNode.Children.Contains(existingChildNode))
            {
                existingParentNode.Children.Add(existingChildNode);
            }
        }
    }
}

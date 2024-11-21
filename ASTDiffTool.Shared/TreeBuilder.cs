using ASTDiffTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    public class TreeBuilder
    {
        private readonly Dictionary<string, Node> nodeLookup = new();

        public List<Node> RootNodes { get; set; } = new();

        public void AddRelationship(Node parentNode, Node childNode)
        {
            string parentUniqueKey = parentNode.EnhancedKey + "-" + parentNode.TopologicalOrder;
            string childUniqueKey = childNode.EnhancedKey + "-" + childNode.TopologicalOrder;

            if (!nodeLookup.TryGetValue(parentUniqueKey, out Node existingParentNode))
            {
                existingParentNode = parentNode;
                nodeLookup[parentUniqueKey] = existingParentNode;

                // highest level node
                if (parentNode.IsHighLevel)
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

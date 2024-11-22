using ASTDiffTool.Models;
using ASTDiffTool.Shared;
using Xunit;
using System.Collections.Generic;

namespace ASTDiffTool.Tests
{
    public class TreeBuilderTests
    {
        [Fact]
        public void AddRelationship_ShouldAddSingleRelationship()
        {
            var treeBuilder = new TreeBuilder();
            var parentNode = new Node { EnhancedKey = "parent1", TopologicalOrder = 1, IsHighLevel = true };
            var childNode = new Node { EnhancedKey = "child1", TopologicalOrder = 1 };

            treeBuilder.AddRelationship(parentNode, childNode);

            Assert.Single(treeBuilder.RootNodes);
            Assert.Equal(parentNode, treeBuilder.RootNodes[0]);
            Assert.Single(parentNode.Children);
            Assert.Equal(childNode, parentNode.Children[0]);
        }

        [Fact]
        public void AddRelationship_ShouldNotAddDuplicateRelationship()
        {
            var treeBuilder = new TreeBuilder();
            var parentNode = new Node { EnhancedKey = "parent1", TopologicalOrder = 1, IsHighLevel = true };
            var childNode = new Node { EnhancedKey = "child1", TopologicalOrder = 1 };

            treeBuilder.AddRelationship(parentNode, childNode);
            treeBuilder.AddRelationship(parentNode, childNode);

            Assert.Single(treeBuilder.RootNodes);
            Assert.Single(parentNode.Children);
        }

        [Fact]
        public void AddRelationship_ShouldAddMultipleChildrenToParent()
        {
            var treeBuilder = new TreeBuilder();
            var parentNode = new Node { EnhancedKey = "parent1", TopologicalOrder = 1, IsHighLevel = true };
            var childNode1 = new Node { EnhancedKey = "child1", TopologicalOrder = 1 };
            var childNode2 = new Node { EnhancedKey = "child2", TopologicalOrder = 1 };

            treeBuilder.AddRelationship(parentNode, childNode1);
            treeBuilder.AddRelationship(parentNode, childNode2);

            Assert.Single(treeBuilder.RootNodes);
            Assert.Equal(2, parentNode.Children.Count);
            Assert.Contains(childNode1, parentNode.Children);
            Assert.Contains(childNode2, parentNode.Children);
        }

        [Fact]
        public void AddRelationship_ShouldAddHighLevelNodeToRootNodes()
        {
            var treeBuilder = new TreeBuilder();
            var highLevelNode = new Node { EnhancedKey = "highLevelNode", TopologicalOrder = 1, IsHighLevel = true };

            treeBuilder.AddRelationship(highLevelNode, new Node { EnhancedKey = "child1", TopologicalOrder = 1 });

            Assert.Single(treeBuilder.RootNodes);
            Assert.Equal(highLevelNode, treeBuilder.RootNodes[0]);
        }

        [Fact]
        public void AddRelationship_ShouldNotAddNonHighLevelNodeToRootNodes()
        {
            var treeBuilder = new TreeBuilder();
            var nonHighLevelNode = new Node { EnhancedKey = "nonHighLevelNode", TopologicalOrder = 1, IsHighLevel = false };

            treeBuilder.AddRelationship(nonHighLevelNode, new Node { EnhancedKey = "child1", TopologicalOrder = 1 });

            Assert.Empty(treeBuilder.RootNodes);
        }

        [Fact]
        public void AddRelationship_ShouldPreventDuplicateChildren()
        {
            var treeBuilder = new TreeBuilder();
            var parentNode = new Node { EnhancedKey = "parent1", TopologicalOrder = 1 };
            var childNode = new Node { EnhancedKey = "child1", TopologicalOrder = 1 };

            treeBuilder.AddRelationship(parentNode, childNode);
            treeBuilder.AddRelationship(parentNode, childNode);

            Assert.Single(parentNode.Children);
        }

        [Fact]
        public void AddRelationship_ShouldBuildComplexTreeWithMultipleLevels()
        {
            var treeBuilder = new TreeBuilder();
            var rootNode = new Node { EnhancedKey = "root", TopologicalOrder = 1, IsHighLevel = true };
            var intermediateNode = new Node { EnhancedKey = "intermediate", TopologicalOrder = 2 };
            var leafNode = new Node { EnhancedKey = "leaf", TopologicalOrder = 3 };

            treeBuilder.AddRelationship(rootNode, intermediateNode);
            treeBuilder.AddRelationship(intermediateNode, leafNode);

            Assert.Single(treeBuilder.RootNodes);
            Assert.Single(rootNode.Children);
            Assert.Equal(intermediateNode, rootNode.Children[0]);
            Assert.Single(intermediateNode.Children);
            Assert.Equal(leafNode, intermediateNode.Children[0]);
        }
    }
}
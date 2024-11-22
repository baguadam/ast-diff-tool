using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    /// <summary>
    /// Class that represents the structure of the information returned
    /// from the Neo4j database for project information.
    /// </summary>
    public class ProjectDatabaseInfoModel
    {
        /// <summary>
        /// The total number of nodes in the database.
        /// </summary>
        public int TotalNodeCount { get; set; }

        /// <summary>
        /// Number of nodes in the first AST.
        /// </summary>
        public int NodesInFirstAST { get; set; }

        /// <summary>
        /// Number of nodes in the second AST.
        /// </summary>
        public int NodesInSecondAST { get; set; }

        /// <summary>
        /// Number of nodes that only exists in the first AST.
        /// </summary>
        public int OnlyInFirstAST { get; set; }

        /// <summary>
        /// Number of nodes that only exists in the second AST.
        /// </summary>
        public int OnlyInSecondAST { get; set; }

        /// <summary>
        /// Number of nodes that has different parents.
        /// </summary>
        public int DifferentParents { get; set; }

        /// <summary>
        /// Number of nodes that has different source locations.
        /// </summary>
        public int DifferentSourceLocations { get; set; }
    }
}

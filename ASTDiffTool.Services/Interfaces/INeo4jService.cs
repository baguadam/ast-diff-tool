using ASTDiffTool.Models;
using ASTDiffTool.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    /// <summary>
    /// Interface of the Neo4jService providing methods for querying the database.
    /// </summary>
    public interface INeo4jService
    {
        /// <summary>
        /// Retrieves the total number of nodes.
        /// </summary>
        /// <returns>A task's result contains the count of nodes</returns>
        Task<int> GetNodeCountAsync();

        /// <summary>
        /// Retrieves the number of nodes that are related to the given AST.
        /// </summary>
        /// <param name="astOrigin">FIRST_AST or SECOND_AST</param>
        /// <returns>Task's result contains the count of the related nodes</returns>
        Task<int> GetNodesByAstOriginAsync(ASTOrigins astOrigin);

        /// <summary>
        /// Retrieves the number of nodes that are related to the given difference type.
        /// </summary>
        /// <param name="differenceType">Difference type that is wished</param>
        /// <returns>Task's result contains the number of related nodes</returns>
        Task<int> GetNodesByDifferenceTypeAsync(Differences differenceType);

        /// <summary>
        /// Retrieves the highest level subtrees based on the specified difference type from the Neo4j database.
        /// </summary>
        /// <param name="differenceType">The difference type to filter nodes by.</param>
        /// <param name="page">The current page number for pagination.</param>
        /// <param name="pageSize">The size of each page (default is 20).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of root nodes for the highest level subtrees.</returns>
        Task<List<Node>> GetHighestLevelSubtreesAsync(Differences differenceType, int page, int pageSize = 20);

        /// <summary>
        /// Retrieves a flat list of nodes based on the specified difference type from the Neo4j database.
        /// </summary>
        /// <param name="differenceType">The difference type to filter nodes by.</param>
        /// <param name="page">The current page number for pagination.</param>
        /// <param name="pageSize">The size of each page (default is 20).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of nodes that match the specified difference type.</returns>
        Task<List<Node>> GetFlatNodesByDifferenceTypeAsync(Differences differenceType, int page, int pageSize = 20);
    }
}

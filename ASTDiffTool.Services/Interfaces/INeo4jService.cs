using ASTDiffTool.Models;
using ASTDiffTool.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    public interface INeo4jService
    {
        Task<int> GetNodeCountAsync();
        Task<int> GetNodesByAstOriginAsync(ASTOrigins astOrigin);
        Task<int> GetNodesByDifferenceTypeAsync(Differences differenceType);
        Task<List<Node>> GetSubtreesByDifferenceTypeAsync(Differences differenceType, int page,  int pageSize = 100);
        Task<List<Node>> GetFlatNodesByDifferenceTypeAsync(Differences differenceType, int page, int pageSize = 100);
    }
}

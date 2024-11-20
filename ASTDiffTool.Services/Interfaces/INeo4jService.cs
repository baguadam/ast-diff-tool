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
    }
}

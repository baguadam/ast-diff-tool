using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    public enum ASTOrigins
    {
        FIRST_AST,
        SECOND_AST
    }

    public enum Differences
    {
        ONLY_IN_FIRST_AST,
        ONLY_IN_SECOND_AST,
        DIFFERENT_PARENTS,
        DIFFERENT_SOURCE_LOCATIONS
    }
}

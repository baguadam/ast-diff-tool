using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    public static class EnumExtensions
    {
        public static string ToDatabaseString(this ASTOrigins origin)
        {
            return origin switch
            {
                ASTOrigins.FIRST_AST => "FIRST_AST",
                ASTOrigins.SECOND_AST => "SECOND_AST",
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
            };
        }

        public static string ToDatabaseString(this Differences difference)
        {
            return difference switch
            {
                Differences.ONLY_IN_FIRST_AST => "ONLY_IN_FIRST_AST",
                Differences.ONLY_IN_SECOND_AST => "ONLY_IN_SECOND_AST",
                Differences.DIFFERENT_PARENTS => "DIFFERENT_PARENTS",
                Differences.DIFFERENT_SOURCE_LOCATIONS => "DIFFERENT_SOURCE_LOCATIONS",
                _ => throw new ArgumentOutOfRangeException(nameof(difference), difference, null)
            };
        }
    }
}

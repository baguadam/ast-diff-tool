using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    /// <summary>
    /// Static class used for converting enums into strings.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts the AST into a string that can be used for database queries.
        /// </summary>
        /// <param name="origin">The AST value</param>
        /// <returns>The string of the AST value</returns>
        /// <exception cref="ArgumentOutOfRangeException">In case of not existing enum value</exception>
        public static string ToDatabaseString(this ASTOrigins origin)
        {
            return origin switch
            {
                ASTOrigins.FIRST_AST => "FIRST_AST",
                ASTOrigins.SECOND_AST => "SECOND_AST",
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
            };
        }

        /// <summary>
        /// Convert the DifferenceType into a string that can be used in database queries.
        /// </summary>
        /// <param name="difference">The type of the difference</param>
        /// <returns>The converted string</returns>
        /// <exception cref="ArgumentOutOfRangeException">In case of not existing enum value</exception>
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

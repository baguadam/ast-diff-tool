using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public enum LineState
    {
        NORMAL,
        HIGHLIGHTED
    }

    public class LineModel
    {
        public string Line { get; set; }
        public LineState State { get; set; }
    }
}

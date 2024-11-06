using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Models
{
    public class Edge
    {
        public string ChildId { get; set; }
        public string ParentId { get; set; }

        // Navigation properties to the related Node entities
        public Node ChildNode { get; set; }
        public Node ParentNode { get; set; }
    }
}

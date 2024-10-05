using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Events
{
    public class ProjectCompilationEvent(bool isSuccessful)
    {
        public bool IsSuccessful { get; set; } = isSuccessful;
    }
}

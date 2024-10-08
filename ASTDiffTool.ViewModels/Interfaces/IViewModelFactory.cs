﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Interfaces
{
    public interface IViewModelFactory
    {
        NewProjectPageViewModel CreateNewProjectPageViewModel();
        ASTPageViewModel CreateASTPageViewModel();
        PreprocessedCodePageViewModel CreatePreprocessedCodePageViewModel();
        ProjectPageViewModel CreateProjectPageViewModel();
    }
}

using ASTDiffTool.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Factories
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public NewProjectPageViewModel CreateNewProjectPageViewModel()
        {
            return _serviceProvider.GetRequiredService<NewProjectPageViewModel>();
        }

        public ASTPageViewModel CreateASTPageViewModel()
        {
            return _serviceProvider.GetRequiredService<ASTPageViewModel>();
        }

        public PreprocessedCodePageViewModel CreatePreprocessedCodePageViewModel()
        {
            return _serviceProvider.GetRequiredService<PreprocessedCodePageViewModel>();
        }

        public ProjectPageViewModel CreateProjectPageViewModel()
        {
            return _serviceProvider.GetRequiredService<ProjectPageViewModel>();
        }
    }
}

using ASTDiffTool.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool
{
    public class FileDialogService : IFileDialogService
    {
        public string? OpenFile(string filter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = filter
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }
    }
}

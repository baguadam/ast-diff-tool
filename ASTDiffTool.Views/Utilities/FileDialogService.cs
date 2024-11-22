using ASTDiffTool.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool
{
    /// <summary>
    /// Class that implement the interface, responsible for managing the jumping up service.
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        /// <summary>
        /// Method that opens the file dialog service.
        /// </summary>
        /// <param name="filter">Filter parameter to only make the wished formats available</param>
        /// <returns>Path to the selected file</returns>
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

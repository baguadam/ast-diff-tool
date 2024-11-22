using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ASTDiffTool.Services.Interfaces
{
    /// <summary>
    /// Interface that defines a service for opening file dialogs.
    /// Provides functionality for selecting files from the file system with specific filtering options.
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// Opens a file dialog with the specified filter, allowing the user to select a file.
        /// </summary>
        /// <param name="filter">
        /// A string that specifies the file types to filter in the dialog. 
        /// Example: "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        /// </param>
        /// <returns>
        /// The full path of the selected file, or null if no file was selected.
        /// </returns>
        string? OpenFile(string filter);
    }
}

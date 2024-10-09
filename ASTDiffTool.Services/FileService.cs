using ASTDiffTool.Models;
using ASTDiffTool.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class FileService : IFileService
    {
        public async Task<IList<LineModel>> ReadLinesFromFileAsync(string fileName)
        {
            var lines = new List<LineModel>();

            using (var reader = new StreamReader(fileName))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(new LineModel { Line = line, State = LineState.NORMAL });
                }
            }

            return lines;
        }
    }
}

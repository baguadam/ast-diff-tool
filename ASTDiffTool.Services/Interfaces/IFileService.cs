﻿using ASTDiffTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services.Interfaces
{
    public interface IFileService
    {
        Task<IList<LineModel>> ReadLinesFromFileAsync(string filePath);
    }
}

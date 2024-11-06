using ASTDiffTool.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class DatabaseConnectionService : IDatabaseConnectionService
    {
        private string _connectionString;

        public string GetConnectionString() => _connectionString;

        public void UpdateConnectionString(string filePath)
        {
            _connectionString = $"Data Source={filePath}";
        }
    }
}

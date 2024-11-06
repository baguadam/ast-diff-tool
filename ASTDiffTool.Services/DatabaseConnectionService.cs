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

        public DatabaseContext Create()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("The connection string has not been initialized.");
            }
            return new DatabaseContext(_connectionString);
        }
    }
}

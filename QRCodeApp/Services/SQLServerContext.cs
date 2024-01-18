using Microsoft.Data.SqlClient;
using System.Data;

namespace QRCodeApp.Services
{
    public class SQLServerContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public SQLServerContext(IConfiguration configuration)
        {
            _configuration = configuration;
            //_connectionString = _configuration.GetConnectionString("SqlConnection");
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}

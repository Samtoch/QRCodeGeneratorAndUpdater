using Azure.Identity;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using QRCodeApp.Models;
using System.Data;

namespace QRCodeApp.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        //private static string _connectionString;

        private readonly SQLServerContext _context; 
        public UserService(SQLServerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            //_connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        }

        public async Task<User> QueryUserByLogin(Login user)
        {
            string sqlSelect = "SELECT * FROM QRCodeUsers WHERE Username = '" + user.Username.Trim() + "' AND PASSWORD = '" + user.Password.Trim() + "' AND DelFlag = 'N'";
            var response = new User();

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    response = await connection.QueryFirstOrDefaultAsync<User>(sqlSelect);
                    return response;
                }
            }
            catch (Exception ex)
            {
                //log.Error("Error with QueryUserByLogin, \n\rScript: " + sqlSelect + "\n\r" + ex + "\n\r");
                response = null;
            }

            return response;
        }

        public async Task<int> CreateUserAction(string username, string actionId)
        {
            DateTime currDate = DateTime.Now;
            string sqlInsert = "INSERT INTO UserActions (Username, ActionId, DateCreated) VALUES ('" + username + "', '" + actionId + "', '" + currDate + "')";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(sqlInsert);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                //log.Error("Error with QueryUserByLogin, \n\rScript: " + sqlSelect + "\n\r" + ex + "\n\r");
                return 0;
            }
        }


        public async Task<int> UpdateUserAction(string actionId)
        {
            DateTime currDate = DateTime.Now;
            string sqlInsert = "update UserActions set Updated = 'Y', DateUpdated = '" + currDate + "'  where ActionId =  '" + actionId + "'";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(sqlInsert);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                //log.Error("Error with QueryUserByLogin, \n\rScript: " + sqlSelect + "\n\r" + ex + "\n\r");
                return 0;
            }
        }

        //public Task<User> QueryUserByLogin(Login user)
        //{
        //    string sqlSelect = "SELECT * FROM QRCodeUsers WHERE Username = '" + user.Username.Trim() + "' AND PASSWORD = '" + user.Password.Trim() + "' AND DelFlag = 'N'";
        //    var responses = new User();

        //    try
        //    {
        //        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        //        {
        //            conn.Open();
        //            responses = SqlMapper.Query<User>(conn, sqlSelect, commandType: CommandType.Text).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //log.Error("Error with QueryUserByLogin, \n\rScript: " + sqlSelect + "\n\r" + ex + "\n\r");
        //        responses = null;
        //    }

        //    return Task.FromResult(responses);
        //}

    }
}

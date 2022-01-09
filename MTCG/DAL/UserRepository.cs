using System;
using System.Data;
using Npgsql;

namespace MTCG.DAL
{
    public class UserRepository
    {
        private readonly DatabaseContext _ctx;

        public UserRepository(DatabaseContext context)
        {
            _ctx = context;
        }
        
        public bool CreateUser(string username, string password)
        {
            var cmd = new NpgsqlCommand("INSERT INTO users VALUES(@username, @password) ON CONFLICT DO NOTHING", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);
            
            var success = cmd.ExecuteNonQuery() == 1;
            _ctx.Commit();
            
            return success;
        }
        
        public bool AuthenticateUser(string username, string password)
        {
            var cmd = new NpgsqlCommand("SELECT username FROM users WHERE username=@username AND password=@password", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);
            
            var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
            var success = reader.Read();
            reader.Close();
            
            return success;
        }

        public UserDTO GetUser(string username)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username=@username", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("username", username);
            
            var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
            reader.Read();
            
            return new UserDTO
            {
                Username = reader.GetString(0),
                Password = reader.GetString(1),
            };
        }
    }
}
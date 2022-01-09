using System;
using System.Collections.Generic;
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
        
        public bool UpdateUser(UserDTO userDto)
        {
            var cmd = new NpgsqlCommand($"UPDATE users SET password=@password, name=@name, bio=@bio, image=@image WHERE username=@username", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("username", userDto.Username);
            cmd.Parameters.AddWithValue("password", userDto.Password);
            cmd.Parameters.AddWithValue("name", userDto.Name);
            cmd.Parameters.AddWithValue("bio", userDto.Bio);
            cmd.Parameters.AddWithValue("image", userDto.Image);
            
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
            
            var userDto = new UserDTO
            {
                Username = reader.GetString(0),
                Password = reader.GetString(1),
                Name = reader.GetValue(2) != DBNull.Value ? reader.GetString(2) : null,
                Bio = reader.GetValue(3) != DBNull.Value ? reader.GetString(3) : null,
                Image = reader.GetValue(4) != DBNull.Value ? reader.GetString(4) : null,
            };
            
            reader.Close();
            return userDto;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace MTCG.DAL.Repositories
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

            return cmd.ExecuteNonQuery() == 1;
        }
        
        public bool UpdateUser(UserDTO userDto)
        {
            var cmd = new NpgsqlCommand($"UPDATE users SET password=@password, name=@name, bio=@bio, image=@image, coins=@coins, elo=@elo WHERE username=@username", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("username", userDto.Username);
            cmd.Parameters.AddWithValue("password", userDto.Password);
            cmd.Parameters.AddWithValue("name", userDto.Name);
            cmd.Parameters.AddWithValue("bio", userDto.Bio);
            cmd.Parameters.AddWithValue("image", userDto.Image);
            cmd.Parameters.AddWithValue("coins", userDto.Coins);
            cmd.Parameters.AddWithValue("elo", userDto.Elo);
            
            return cmd.ExecuteNonQuery() == 1;
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
            var cmd = new NpgsqlCommand("SELECT username, password, name, bio, image, coins, elo FROM users WHERE username=@username", _ctx.Connection, _ctx.Transaction);
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
                Coins = reader.GetInt32(5),
                Elo = reader.GetInt32(6),
            };
            
            reader.Close();
            return userDto;
        }

        public List<ScoreDTO> GetScoreboard()
        {
            var cmd = new NpgsqlCommand("SELECT username, elo FROM users ORDER BY elo DESC", _ctx.Connection, _ctx.Transaction);

            var scores = new List<ScoreDTO>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                scores.Add(new ScoreDTO
                {
                    Username = reader.GetString(0),
                    Elo = reader.GetInt32(1),
                });
            }

            reader.Close();
            return scores;
        }
        
        public ScoreDTO GetStats(string username)
        {
            var cmd = new NpgsqlCommand("SELECT username, elo FROM users WHERE username=@username", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("username", username);
            
            var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
            var results = reader.Read();

            if (!results) return null;
            
            var score = new ScoreDTO
            {
                Username = reader.GetString(0),
                Elo = reader.GetInt32(1),
            };
            
            reader.Close();
            return score;
        }
    }
}
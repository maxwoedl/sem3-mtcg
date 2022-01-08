using System;
using Npgsql;

namespace MTCG.DAL
{
    public class UserRepository
    {
        private DatabaseContext ctx;

        public UserRepository(DatabaseContext context)
        {
            ctx = context;
        }

        public void GetUsers()
        {
            var cmd = new NpgsqlCommand("SELECT * FROM users", ctx.Connection, ctx.Transaction);
            var reader = cmd.ExecuteReader();

            reader.Read();
            
            Console.WriteLine(reader.GetString(0));
        }

        public UserDTO GetUser(string username)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username=@username", ctx.Connection, ctx.Transaction);
            cmd.Parameters.AddWithValue("username", username);
            
            var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
            reader.Read();
            
            return new UserDTO
            {
                Id = reader.GetString(0),
                Username = reader.GetString(1),
            };
        }
    }
}
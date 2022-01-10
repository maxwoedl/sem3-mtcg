using System;
using System.Data;
using System.Linq;
using Npgsql;

namespace MTCG.DAL.Repositories
{
    public class PackageRepository
    {
        private readonly DatabaseContext _ctx;

        public PackageRepository(DatabaseContext context)
        {
            _ctx = context;
        }
        
        public bool CreatePackage(PackageDTO packageDto)
        {
            var cmd = new NpgsqlCommand("INSERT INTO packages(id, price, cards) VALUES(@id, @price, @cards) ON CONFLICT DO NOTHING", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("id", packageDto.Id);
            cmd.Parameters.AddWithValue("price", packageDto.Price);
            cmd.Parameters.AddWithValue("cards", packageDto.Cards.ToArray());
            
            return cmd.ExecuteNonQuery() == 1;
        }

        public PackageDTO GetBuyablePackage()
        {
            var cmd = new NpgsqlCommand("SELECT id, price, cards FROM packages WHERE owner IS NULL", _ctx.Connection, _ctx.Transaction);
            
            var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
            var results = reader.Read();

            if (!results)
            {
                reader.Close();
                return null;
            }
            
            var packageDto = new PackageDTO
            {
                Id = reader.GetGuid(0),
                Price = reader.GetInt32(1),
                Owner = null,
            };

            var cards = reader.GetFieldValue<Guid[]>(2);
            
            foreach (var uuid in cards)
            {
                packageDto.Cards.Add(uuid);
            }
            
            reader.Close();
            return packageDto;
        }
        
        public bool ChangePackageOwner(Guid id, string username)
        {
            var cmd = new NpgsqlCommand("UPDATE packages SET owner=@owner WHERE id=@id", _ctx.Connection,
                _ctx.Transaction);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("owner", username);
            
            return cmd.ExecuteNonQuery() == 1;
        }
        
    }
}
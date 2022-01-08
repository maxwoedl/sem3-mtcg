using Npgsql;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MTCG.DAL
{
    public class DatabaseContext
    {
        public NpgsqlConnection Connection { get; }
        
        public NpgsqlTransaction Transaction { get; private set; }
        
        public DatabaseContext()
        {
            // string connectionString = configuration.GetConnectionString("Postgres");
            string connectionString = "Server=127.0.0.1;Port=5432;Database=mtcg;User Id=postgres;Password=W0oCWGm2";
            Connection = new NpgsqlConnection(connectionString);
        }

        public async Task InitAsync()
        {
            await Connection.OpenAsync();
            Transaction = Connection.BeginTransaction();
        }
        
        public Task CommitAsync()
        {
            return Transaction.CommitAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await Transaction.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }
}
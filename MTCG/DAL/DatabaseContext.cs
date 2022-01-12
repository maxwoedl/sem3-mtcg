using Npgsql;
using Microsoft.Extensions.Configuration;

namespace MTCG.DAL
{
    public sealed class DatabaseContext
    {
        private static DatabaseContext _instance; 
        
        public NpgsqlConnection Connection { get; }
        
        public NpgsqlTransaction Transaction { get; private set; }
        
        private DatabaseContext()
        {
            // string connectionString = configuration.GetConnectionString("Postgres");
            string connectionString = "Server=127.0.0.1;Port=5432;Database=mtcg;User Id=postgres;Password=postgres";
            Connection = new NpgsqlConnection(connectionString);
        }
        
        public static DatabaseContext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseContext();
            }
            return _instance;
        }

        public void Init()
        {
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }
        
        public void Commit()
        {
            Transaction.Commit();
            Transaction = Connection.BeginTransaction();
        }

        public void Rollback()
        {
            Transaction.Rollback();
            Transaction = Connection.BeginTransaction();
        }

        public void Dispose()
        {
            Transaction.Dispose();
            Connection.Dispose();
        }
    }
}
using RepoDb.TestProject.Models;
using System.Data.SqlClient;

namespace RepoDb.TestProject.Repositories
{
    public class AnimalRepository : BaseRepository<Animal, SqlConnection>
    {
        public AnimalRepository(string connectionString)
            : base(connectionString)
        {
        }
    }
}

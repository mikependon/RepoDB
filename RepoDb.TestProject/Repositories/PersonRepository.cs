using RepoDb.TestProject.Models;
using System.Data.SqlClient;

namespace RepoDb.TestProject.Repositories
{
    public class PersonRepository : BaseRepository<Person, SqlConnection>
    {
        public PersonRepository(string connectionString)
            : base(connectionString)
        {
        }
    }
}

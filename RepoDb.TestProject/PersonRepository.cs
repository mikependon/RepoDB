using System.Data.SqlClient;

namespace RepoDb.TestProject
{
    public class PersonRepository : BaseRepository<Person, SqlConnection>
    {
        public PersonRepository(string connectionString)
            : base(connectionString, null, null, new CustomTrace(), null)
        {
        }
    }
}

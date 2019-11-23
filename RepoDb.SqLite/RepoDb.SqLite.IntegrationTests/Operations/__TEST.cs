using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class __TEST
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        [TestMethod]
        public void ExtractDefinitions()
        {
            var definitions = string.Empty;
            using (var connection = new SQLiteConnection(Database.ConnectionString).EnsureOpen())
            {
                using (var reader = connection.ExecuteReader("SELECT * FROM [CompleteTable];"))
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var columName = reader.GetName(i);
                        var type = reader.GetFieldType(i);
                        definitions = string.Concat(definitions, $"public {type.FullName.ToString()} {columName} {{ get; set; }}\n");
                    }
                }
            }
        }
    }
}

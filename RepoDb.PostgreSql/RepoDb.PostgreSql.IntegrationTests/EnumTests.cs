using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Attributes;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.PostgreSql.IntegrationTests
{
    [TestClass]
    public class EnumTests
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

        #region Enumerations

        public enum Hands
        {
            Left,
            Right
        }

        #endregion

        #region SubClasses

        [Map("CompleteTable")]
        public class PersonText
        {
            public System.Int64 Id { get; set; }
            public Hands ColumnText { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonInteger
        {
            public System.Int64 Id { get; set; }
            public Hands ColumnInteger { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonTextAsInteger
        {
            public System.Int64 Id { get; set; }
            [TypeMap(System.Data.DbType.Int32)]
            public Hands ColumnText { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestInsertAndQueryEnumAsText()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = new PersonText { Id = 1, ColumnText = Hands.Right };

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonText>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsInteger()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = new PersonInteger { Id = 1, ColumnInteger = Hands.Right };

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonInteger>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnInteger, queryResult.ColumnInteger);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsInt()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = new PersonTextAsInteger { Id = 1, ColumnText = Hands.Right };

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonTextAsInteger>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
            }
        }
    }
}

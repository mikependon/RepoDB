using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System.Data.SQLite;

namespace RepoDb.SqLite.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Bootstrap.Initialize();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();
            var query = builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();
            var query = builder.CreateExists(new QueryBuilder(),
                "Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS [ExistsValue] FROM [Table] WHERE ([Id] = @Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion
    }
}

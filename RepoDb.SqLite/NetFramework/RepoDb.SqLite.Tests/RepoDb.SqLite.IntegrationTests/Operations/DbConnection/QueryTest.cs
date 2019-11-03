using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class DbConnectionOperationQueryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Bootstrap.Initialize();
            //Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Database.Cleanup();
            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                connection.DeleteAll<CompleteTable>();
            }
        }

        [TestMethod]
        public void TestExtractColumnType()
        {
            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                using (var reader = connection.ExecuteReader("select * from completetable where id = 5;"))
                {
                    var text = string.Empty;
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        text = string.Concat(text, $"{reader.GetName(i)} : {reader.GetFieldType(i)}\n");
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbConnectionAverage()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnInt,
                    e => e.Id > tables.First().Id && e.Id < tables.Last().Id);

                // Assert
                Assert.AreEqual(tables.Where(t => t.Id > tables.First().Id && t.Id < tables.Last().Id).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbConnectionBatchQuery()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<CompleteTable>(3,
                    1,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    (object)null);

                // Assert
                // TODO
                //Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbConnectionCount()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<CompleteTable>(e => e.Id == tables.Last().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbConnectionCountAll()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbConnectionDelete()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var deletedRows = connection.Delete<CompleteTable>(tables.Last().Id);

                // Act
                var result = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(1, deletedRows);
            }
        }

        [TestMethod]
        public void TestDbConnectionInsert()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                tables.ForEach(t => connection.Insert(t));

                // Assert
                Assert.IsTrue(tables.All(t => t.Id > 0));
            }
        }

        [TestMethod]
        public void TestDbConnectionQuery()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                tables.ForEach(t => connection.Insert(t));

                // Act
                var result = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.IsTrue(tables.All(t => t.Id > 0));
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(t => t.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestDbConnectionInsertAll()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.IsTrue(tables.All(t => t.Id > 0));
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(t => t.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestDbConnectionUpdate()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                tables.ForEach(t => connection.Insert(t));

                // Act
                var result = connection.QueryAll<CompleteTable>();

                // Setup
                tables.ForEach(t =>
                {
                    t.ColumnInt = int.MaxValue;
                });

                // Act
                tables.ForEach(t => connection.Update<CompleteTable>(t));

                // Act
                result = connection.QueryAll<CompleteTable>();
            }
        }

        [TestMethod]
        public void TestDbConnectionUpdateAll()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<CompleteTable>();

                // Setup
                tables.ForEach(t =>
                {
                    t.ColumnInt = int.MaxValue;
                });

                // Act
                var affected = connection.UpdateAll(tables);

                // Act
                result = connection.QueryAll<CompleteTable>();
            }
        }
    }
}

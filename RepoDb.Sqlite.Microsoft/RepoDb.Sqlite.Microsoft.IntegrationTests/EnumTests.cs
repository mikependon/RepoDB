using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.MDS
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
            Unidentified,
            Left,
            Right
        }

        #endregion

        #region SubClasses

        [Map("MdsCompleteTable")]
        public class PersonWithText
        {
            public System.Int64 Id { get; set; }
            public Hands? ColumnText { get; set; }
        }

        [Map("MdsCompleteTable")]
        public class PersonWithInteger
        {
            public System.Int64 Id { get; set; }
            public Hands? ColumnInteger { get; set; }
        }

        [Map("MdsCompleteTable")]
        public class PersonWithTextAsInteger
        {
            public System.Int64 Id { get; set; }
            [TypeMap(System.Data.DbType.Int32)]
            public Hands? ColumnText { get; set; }
        }

        #endregion

        #region Helpers

        public IEnumerable<PersonWithText> GetPersonWithText(int count)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                var hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
                yield return new PersonWithText
                {
                    Id = i,
                    ColumnText = hand
                };
            }
        }

        public IEnumerable<PersonWithInteger> GetPersonWithInteger(int count)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                var hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
                yield return new PersonWithInteger
                {
                    Id = i,
                    ColumnInteger = hand
                };
            }
        }

        public IEnumerable<PersonWithTextAsInteger> GetPersonWithTextAsInteger(int count)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                var hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
                yield return new PersonWithTextAsInteger
                {
                    Id = i,
                    ColumnText = hand
                };
            }
        }

        #endregion

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsNull()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var person = GetPersonWithText(1).First();
                person.ColumnText = null;

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithText>(person.Id).First();

                // Assert
                Assert.IsNull(queryResult.ColumnText);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsText()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var person = GetPersonWithText(1).First();

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithText>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextByBatch()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var people = GetPersonWithText(10).AsList();

                // Act
                connection.InsertAll(people);

                // Query
                var queryResult = connection.QueryAll<PersonWithText>().AsList();

                // Assert
                people.ForEach(p =>
                {
                    var item = queryResult.First(e => e.Id == p.Id);
                    Assert.AreEqual(p.ColumnText, item.ColumnText);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsNull()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var person = GetPersonWithInteger(1).First();
                person.ColumnInteger = null;

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithInteger>(person.Id).First();

                // Assert
                Assert.IsNull(queryResult.ColumnInteger);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsInteger()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var person = GetPersonWithInteger(1).First();

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithInteger>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnInteger, queryResult.ColumnInteger);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsBatch()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var people = GetPersonWithInteger(10).AsList();

                // Act
                connection.InsertAll(people);

                // Query
                var queryResult = connection.QueryAll<PersonWithInteger>().AsList();

                // Assert
                people.ForEach(p =>
                {
                    var item = queryResult.First(e => e.Id == p.Id);
                    Assert.AreEqual(p.ColumnInteger, item.ColumnInteger);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsInt()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var person = GetPersonWithTextAsInteger(1).First();

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithTextAsInteger>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsIntAsBatch()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                //  Create the table first
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var people = GetPersonWithTextAsInteger(10).AsList();

                // Act
                connection.InsertAll(people);

                // Query
                var queryResult = connection.QueryAll<PersonWithTextAsInteger>().AsList();

                // Assert
                people.ForEach(p =>
                {
                    var item = queryResult.First(e => e.Id == p.Id);
                    Assert.AreEqual(p.ColumnText, item.ColumnText);
                });
            }
        }
    }
}

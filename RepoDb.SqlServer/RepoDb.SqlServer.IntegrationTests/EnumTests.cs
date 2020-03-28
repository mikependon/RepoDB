using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.SqlServer.IntegrationTests
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

        [Map("CompleteTable")]
        public class PersonWithText
        {
            public System.Int64 Id { get; set; }
            public System.Guid SessionId { get; set; }
            public Hands? ColumnNVarChar { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonWithInteger
        {
            public System.Int64 Id { get; set; }
            public System.Guid SessionId { get; set; }
            public Hands? ColumnInt { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonWithTextAsInteger
        {
            public System.Int64 Id { get; set; }
            public System.Guid SessionId { get; set; }
            [TypeMap(System.Data.DbType.Int32)]
            public Hands? ColumnNVarChar { get; set; }
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
                    SessionId = Guid.NewGuid(),
                    ColumnNVarChar = hand
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
                    SessionId = Guid.NewGuid(),
                    ColumnInt = hand
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
                    SessionId = Guid.NewGuid(),
                    ColumnNVarChar = hand
                };
            }
        }

        #endregion

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsNull()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithText(1).First();
                person.ColumnNVarChar = null;

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithText>(person.Id).First();

                // Assert
                Assert.IsNull(queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithText(1).First();

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithText>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextByBatch()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
                    Assert.AreEqual(p.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsNull()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithInteger(1).First();
                person.ColumnInt = null;

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithInteger>(person.Id).First();

                // Assert
                Assert.IsNull(queryResult.ColumnInt);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsInteger()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithInteger(1).First();

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithInteger>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnInt, queryResult.ColumnInt);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsBatch()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
                    Assert.AreEqual(p.ColumnInt, item.ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsInt()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithTextAsInteger(1).First();

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.Query<PersonWithTextAsInteger>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsIntAsBatch()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
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
                    Assert.AreEqual(p.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }
    }
}

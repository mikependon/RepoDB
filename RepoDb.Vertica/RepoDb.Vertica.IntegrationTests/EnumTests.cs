using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Vertica.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Vertica.IntegrationTests
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
            public Hands? ColumnText { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonWithInteger
        {
            public System.Int64 Id { get; set; }
            public Hands? ColumnInt { get; set; }
        }

        [Map("CompleteTable")]
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
                    ColumnText = hand
                };
            }
        }

        #endregion

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsNull()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithText(1).First();
                person.ColumnText = null;

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.QueryAll<PersonWithText>().First();

                // Assert
                Assert.IsNull(queryResult.ColumnText);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsText()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithText(1).First();

                // Act
                connection.Insert(person);

                // Query - see the remarks in TestInsertAndQueryEnumAsTextAsNull on why not Query<T>(person.Id).
                var queryResult = connection.QueryAll<PersonWithText>().First();

                // Assert
                Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextByBatch()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var people = GetPersonWithText(10).AsList();

                // Act
                connection.InsertAll(people);

                // Query
                var queryResult = connection.QueryAll<PersonWithText>().AsList();

                // Assert
                Assert.AreEqual(people.Count, queryResult.Count);
                CollectionAssert.AreEqual(
                    people.Select(p => p.ColumnText).OrderBy(v => v).ToList(),
                    queryResult.Select(e => e.ColumnText).OrderBy(v => v).ToList());
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsNull()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithInteger(1).First();
                person.ColumnInt = null;

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.QueryAll<PersonWithInteger>().First();

                // Assert
                Assert.IsNull(queryResult.ColumnInt);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsInteger()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithInteger(1).First();

                // Act
                connection.Insert(person);

                // Query
                var queryResult = connection.QueryAll<PersonWithInteger>().First();

                // Assert
                Assert.AreEqual(person.ColumnInt, queryResult.ColumnInt);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsBatch()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var people = GetPersonWithInteger(10).AsList();

                // Act
                connection.InsertAll(people);

                // Query
                var queryResult = connection.QueryAll<PersonWithInteger>().AsList();

                // Assert
                Assert.AreEqual(people.Count, queryResult.Count);
                CollectionAssert.AreEqual(
                    people.Select(p => p.ColumnInt).OrderBy(v => v).ToList(),
                    queryResult.Select(e => e.ColumnInt).OrderBy(v => v).ToList());
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsIntThrows()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithTextAsInteger(1).First();

                // Act & Assert
                connection.Insert(person);
                Assert.AreEqual(1, connection.CountAll<PersonWithTextAsInteger>());
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsIntAsBatchThrows()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var people = GetPersonWithTextAsInteger(10).AsList();

                // Act & Assert
                connection.InsertAll(people);
                Assert.AreEqual(people.Count, connection.CountAll<PersonWithTextAsInteger>());
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
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
            public Hands? ColumnInteger { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonWithTextAsInteger
        {
            public System.Int64 Id { get; set; }
            [TypeMap(System.Data.DbType.Int32)]
            public Hands? ColumnText { get; set; }
        }

        [Map("EnumTable")]
        public class PersonWithEnum
        {
            public System.Int64 Id { get; set; }
            [NpgsqlDbType(NpgsqlTypes.NpgsqlDbType.Unknown)]
            public Hands ColumnEnum { get; set; }
        }

        [Map("EnumTable")]
        public class PersonWithNullableEnum
        {
            public System.Int64 Id { get; set; }
            [NpgsqlDbType(NpgsqlTypes.NpgsqlDbType.Unknown)]
            public Hands? ColumnEnum { get; set; }
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

        public IEnumerable<PersonWithEnum> GetPersonWithEnum(int count)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                var hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
                yield return new PersonWithEnum
                {
                    Id = i,
                    ColumnEnum = hand
                };
            }
        }

        public IEnumerable<PersonWithNullableEnum> GetPersonWithNullableEnum(int count)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                var hand = random.Next(100) > 50 ? Hands.Right : Hands.Left;
                yield return new PersonWithNullableEnum
                {
                    Id = i,
                    ColumnEnum = hand
                };
            }
        }

        #endregion

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
                    Assert.AreEqual(p.ColumnText, item.ColumnText);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
                    Assert.AreEqual(p.ColumnInteger, item.ColumnInteger);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsInt()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
                    Assert.AreEqual(p.ColumnText, item.ColumnText);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsEnum()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithEnum>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnEnum, queryResult.ColumnEnum);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsEnumAsBatch()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var people = GetPersonWithEnum(10).AsList();

                // Act
                connection.InsertAll(people);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.QueryAll<PersonWithEnum>().AsList();

                // Assert
                people.ForEach(p =>
                {
                    var item = queryResult.First(e => e.Id == p.Id);
                    Assert.AreEqual(p.ColumnEnum, item.ColumnEnum);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsEnumViaEnum()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithEnum>(where: p => p.ColumnEnum == person.ColumnEnum).First();

                // Assert
                Assert.AreEqual(person.ColumnEnum, queryResult.ColumnEnum);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsEnumViaDynamicEnum()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithEnum>(new { ColumnEnum = person.ColumnEnum }).First();

                // Assert
                Assert.AreEqual(person.ColumnEnum, queryResult.ColumnEnum);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnumAsNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();
                person.ColumnEnum = null;

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithNullableEnum>(person.Id).First();

                // Assert
                Assert.IsNull(queryResult.ColumnEnum);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnum()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithNullableEnum>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnEnum, queryResult.ColumnEnum);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnumAsBatch()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var people = GetPersonWithNullableEnum(10).AsList();

                // Act
                connection.InsertAll(people);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.QueryAll<PersonWithNullableEnum>().AsList();

                // Assert
                people.ForEach(p =>
                {
                    var item = queryResult.First(e => e.Id == p.Id);
                    Assert.AreEqual(p.ColumnEnum, item.ColumnEnum);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnumByEnum()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithNullableEnum>(where: p => p.ColumnEnum == person.ColumnEnum).First();

                // Assert
                Assert.AreEqual(person.ColumnEnum, queryResult.ColumnEnum);
            }
        }
    }
}

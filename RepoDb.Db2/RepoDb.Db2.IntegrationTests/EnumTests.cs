using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Db2.IntegrationTests.Setup;
using RepoDb.Db2.PropertyHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests
{
    [TestClass]
    public class EnumTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();

            // "SessionId" is stored as RAW(16); the property-level Guid<->byte[] handler registered
            // in Database.Initialize() is keyed specifically to CompleteTable.SessionId (registrations
            // are keyed by the exact declaring CLR type + property, not by column name), so each of the
            // [Map("CompleteTable")] subclasses below - being distinct CLR types - needs its own
            // registration even though they all resolve to the same physical column.
            PropertyHandlerMapper.Add<PersonWithText, Db2GuidToByteArrayPropertyHandler>(
                e => e.SessionId, new Db2GuidToByteArrayPropertyHandler(), true);
            PropertyHandlerMapper.Add<PersonWithInteger, Db2GuidToByteArrayPropertyHandler>(
                e => e.SessionId, new Db2GuidToByteArrayPropertyHandler(), true);
            PropertyHandlerMapper.Add<PersonWithTextAsInteger, Db2GuidToByteArrayPropertyHandler>(
                e => e.SessionId, new Db2GuidToByteArrayPropertyHandler(), true);
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

        // These subclasses re-map onto the real "CompleteTable" physical table (see Setup/Database.cs)
        // so no extra schema is needed - they just expose a narrower, enum-typed view of its existing
        // "ColumnVarchar" (NVARCHAR2(256)) and "ColumnNumber" (NUMBER(18,12)) columns.

        [Map("CompleteTable")]
        public class PersonWithText
        {
            public System.Int64 Id { get; set; }
            public System.Guid SessionId { get; set; }
            public Hands? ColumnVarchar { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonWithInteger
        {
            public System.Int64 Id { get; set; }
            public System.Guid SessionId { get; set; }
            public Hands? ColumnNumber { get; set; }
        }

        [Map("CompleteTable")]
        public class PersonWithTextAsInteger
        {
            public System.Int64 Id { get; set; }
            public System.Guid SessionId { get; set; }
            // Forces the enum's underlying Int32 ordinal to be bound instead of its string name, even
            // though the target column ("ColumnVarchar") is textual. Db2 implicitly converts a bound
            // NUMBER value into VARCHAR2 on INSERT/UPDATE, so this round-trips as "0"/"1"/"2" text.
            [TypeMap(System.Data.DbType.Int32)]
            public Hands? ColumnVarchar { get; set; }
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
                    SessionId = Guid.NewGuid(),
                    ColumnVarchar = hand
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
                    SessionId = Guid.NewGuid(),
                    ColumnNumber = hand
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
                    SessionId = Guid.NewGuid(),
                    ColumnVarchar = hand
                };
            }
        }

        #endregion

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsNull()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithText(1).First();
            person.ColumnVarchar = null;

            // Act
            var id = connection.Insert(person);

            // Query
            var queryResult = connection.Query<PersonWithText>(id).First();

            // Assert
            Assert.IsNull(queryResult.ColumnVarchar);
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsText()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithText(1).First();

            // Act
            var id = connection.Insert(person);

            // Query
            var queryResult = connection.Query<PersonWithText>(id).First();

            // Assert
            Assert.AreEqual(person.ColumnVarchar, queryResult.ColumnVarchar);
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextByBatch()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

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
                Assert.AreEqual(p.ColumnVarchar, item.ColumnVarchar);
            });
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsNull()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithInteger(1).First();
            person.ColumnNumber = null;

            // Act
            var id = connection.Insert(person);

            // Query
            var queryResult = connection.Query<PersonWithInteger>(id).First();

            // Assert
            Assert.IsNull(queryResult.ColumnNumber);
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsInteger()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithInteger(1).First();

            // Act
            var id = connection.Insert(person);

            // Query
            var queryResult = connection.Query<PersonWithInteger>(id).First();

            // Assert
            Assert.AreEqual(person.ColumnNumber, queryResult.ColumnNumber);
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsIntegerAsBatch()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

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
                Assert.AreEqual(p.ColumnNumber, item.ColumnNumber);
            });
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsInt()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithTextAsInteger(1).First();

            // Act
            var id = connection.Insert(person);

            // Query
            var queryResult = connection.Query<PersonWithTextAsInteger>(id).First();

            // Assert
            Assert.AreEqual(person.ColumnVarchar, queryResult.ColumnVarchar);
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsIntAsBatch()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

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
                Assert.AreEqual(p.ColumnVarchar, item.ColumnVarchar);
            });
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextAsNull()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithText(1).First();
            person.ColumnVarchar = null;

            // Act
            var id = await connection.InsertAsync(person);

            // Query
            var queryResult = (await connection.QueryAsync<PersonWithText>(id)).First();

            // Assert
            Assert.IsNull(queryResult.ColumnVarchar);
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsText()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithText(1).First();

            // Act
            var id = await connection.InsertAsync(person);

            // Query
            var queryResult = (await connection.QueryAsync<PersonWithText>(id)).First();

            // Assert
            Assert.AreEqual(person.ColumnVarchar, queryResult.ColumnVarchar);
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextByBatch()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var people = GetPersonWithText(10).AsList();

            // Act
            await connection.InsertAllAsync(people);

            // Query
            var queryResult = (await connection.QueryAllAsync<PersonWithText>()).AsList();

            // Assert
            people.ForEach(p =>
            {
                var item = queryResult.First(e => e.Id == p.Id);
                Assert.AreEqual(p.ColumnVarchar, item.ColumnVarchar);
            });
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsIntegerAsNull()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithInteger(1).First();
            person.ColumnNumber = null;

            // Act
            var id = await connection.InsertAsync(person);

            // Query
            var queryResult = (await connection.QueryAsync<PersonWithInteger>(id)).First();

            // Assert
            Assert.IsNull(queryResult.ColumnNumber);
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsInteger()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithInteger(1).First();

            // Act
            var id = await connection.InsertAsync(person);

            // Query
            var queryResult = (await connection.QueryAsync<PersonWithInteger>(id)).First();

            // Assert
            Assert.AreEqual(person.ColumnNumber, queryResult.ColumnNumber);
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsIntegerAsBatch()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var people = GetPersonWithInteger(10).AsList();

            // Act
            await connection.InsertAllAsync(people);

            // Query
            var queryResult = (await connection.QueryAllAsync<PersonWithInteger>()).AsList();

            // Assert
            people.ForEach(p =>
            {
                var item = queryResult.First(e => e.Id == p.Id);
                Assert.AreEqual(p.ColumnNumber, item.ColumnNumber);
            });
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextAsInt()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var person = GetPersonWithTextAsInteger(1).First();

            // Act
            var id = await connection.InsertAsync(person);

            // Query
            var queryResult = (await connection.QueryAsync<PersonWithTextAsInteger>(id)).First();

            // Assert
            Assert.AreEqual(person.ColumnVarchar, queryResult.ColumnVarchar);
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextAsIntAsBatch()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Setup
            var people = GetPersonWithTextAsInteger(10).AsList();

            // Act
            await connection.InsertAllAsync(people);

            // Query
            var queryResult = (await connection.QueryAllAsync<PersonWithTextAsInteger>()).AsList();

            // Assert
            people.ForEach(p =>
            {
                var item = queryResult.First(e => e.Id == p.Id);
                Assert.AreEqual(p.ColumnVarchar, item.ColumnVarchar);
            });
        }
    }
}

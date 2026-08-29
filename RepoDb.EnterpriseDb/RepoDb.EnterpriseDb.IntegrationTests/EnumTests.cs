#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using EnterpriseDB.EDBClient.NameTranslation;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.EnterpriseDb;
using RepoDb.Extensions;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests
{
    [TestClass]
    public class EnumTests
    {
        private EDBDataSource _enumDataSource;

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            _enumDataSource = new EDBDataSourceBuilder(Database.ConnectionString)
                .MapEnum<Hands>("hand", new EDBNullNameTranslator())
                .Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
            _enumDataSource?.Dispose();
            _enumDataSource = null;
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
            [EnterpriseDbType(EDBTypes.EDBDbType.Integer)]
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
            public Hands ColumnEnumHand { get; set; }
        }

        [Map("EnumTable")]
        public class PersonWithNullableEnum
        {
            public System.Int64 Id { get; set; }
            public Hands? ColumnEnumHand { get; set; }
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
                    ColumnEnumHand = hand
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
                    ColumnEnumHand = hand
                };
            }
        }

        #endregion

        [TestMethod]
        public void TestInsertAndQueryEnumAsTextAsNull()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = _enumDataSource.OpenConnection())
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithEnum>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsEnumAsBatch()
        {
            using (var connection = _enumDataSource.OpenConnection())
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
                    Assert.AreEqual(p.ColumnEnumHand, item.ColumnEnumHand);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsEnumViaEnum()
        {
            using (var connection = _enumDataSource.OpenConnection())
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithEnum>(where: p => p.ColumnEnumHand == person.ColumnEnumHand).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsEnumViaDynamicEnum()
        {
            using (var connection = _enumDataSource.OpenConnection())
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithEnum>(new { ColumnEnumHand = person.ColumnEnumHand }).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnumAsNull()
        {
            using (var connection = _enumDataSource.OpenConnection())
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();
                person.ColumnEnumHand = null;

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithNullableEnum>(person.Id).First();

                // Assert
                Assert.IsNull(queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnum()
        {
            using (var connection = _enumDataSource.OpenConnection())
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithNullableEnum>(person.Id).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnumAsBatch()
        {
            using (var connection = _enumDataSource.OpenConnection())
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
                    Assert.AreEqual(p.ColumnEnumHand, item.ColumnEnumHand);
                });
            }
        }

        [TestMethod]
        public void TestInsertAndQueryEnumAsNullableEnumByEnum()
        {
            using (var connection = _enumDataSource.OpenConnection())
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();

                // Act
                connection.Insert(person);

                // Query
                connection.ReloadTypes();
                var queryResult = connection.Query<PersonWithNullableEnum>(where: p => p.ColumnEnumHand == person.ColumnEnumHand).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextAsNull()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithText(1).First();
                person.ColumnText = null;

                // Act
                await connection.InsertAsync(person);

                // Query
                var queryResult = (await connection.QueryAsync<PersonWithText>(person.Id)).First();

                // Assert
                Assert.IsNull(queryResult.ColumnText);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsText()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithText(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                var queryResult = (await connection.QueryAsync<PersonWithText>(person.Id)).First();

                // Assert
                Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextByBatch()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
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
                    Assert.AreEqual(p.ColumnText, item.ColumnText);
                });
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsIntegerAsNull()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithInteger(1).First();
                person.ColumnInteger = null;

                // Act
                await connection.InsertAsync(person);

                // Query
                var queryResult = (await connection.QueryAsync<PersonWithInteger>(person.Id)).First();

                // Assert
                Assert.IsNull(queryResult.ColumnInteger);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsInteger()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithInteger(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                var queryResult = (await connection.QueryAsync<PersonWithInteger>(person.Id)).First();

                // Assert
                Assert.AreEqual(person.ColumnInteger, queryResult.ColumnInteger);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsIntegerAsBatch()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
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
                    Assert.AreEqual(p.ColumnInteger, item.ColumnInteger);
                });
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextAsInt()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var person = GetPersonWithTextAsInteger(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                var queryResult = (await connection.QueryAsync<PersonWithTextAsInteger>(person.Id)).First();

                // Assert
                Assert.AreEqual(person.ColumnText, queryResult.ColumnText);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsTextAsIntAsBatch()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
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
                    Assert.AreEqual(p.ColumnText, item.ColumnText);
                });
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsEnum()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAsync<PersonWithEnum>(person.Id)).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsEnumAsBatch()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var people = GetPersonWithEnum(10).AsList();

                // Act
                await connection.InsertAllAsync(people);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAllAsync<PersonWithEnum>()).AsList();

                // Assert
                people.ForEach(p =>
                {
                    var item = queryResult.First(e => e.Id == p.Id);
                    Assert.AreEqual(p.ColumnEnumHand, item.ColumnEnumHand);
                });
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsEnumViaEnum()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAsync<PersonWithEnum>(where: p => p.ColumnEnumHand == person.ColumnEnumHand)).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsEnumViaDynamicEnum()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var person = GetPersonWithEnum(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAsync<PersonWithEnum>(new { ColumnEnumHand = person.ColumnEnumHand })).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsNullableEnumAsNull()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();
                person.ColumnEnumHand = null;

                // Act
                await connection.InsertAsync(person);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAsync<PersonWithNullableEnum>(person.Id)).First();

                // Assert
                Assert.IsNull(queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsNullableEnum()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAsync<PersonWithNullableEnum>(person.Id)).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsNullableEnumAsBatch()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var people = GetPersonWithNullableEnum(10).AsList();

                // Act
                await connection.InsertAllAsync(people);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAllAsync<PersonWithNullableEnum>()).AsList();

                // Assert
                people.ForEach(p =>
                {
                    var item = queryResult.First(e => e.Id == p.Id);
                    Assert.AreEqual(p.ColumnEnumHand, item.ColumnEnumHand);
                });
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncEnumAsNullableEnumByEnum()
        {
            using (var connection = await _enumDataSource.OpenConnectionAsync())
            {
                // Setup
                var person = GetPersonWithNullableEnum(1).First();

                // Act
                await connection.InsertAsync(person);

                // Query
                await connection.ReloadTypesAsync();
                var queryResult = (await connection.QueryAsync<PersonWithNullableEnum>(where: p => p.ColumnEnumHand == person.ColumnEnumHand)).First();

                // Assert
                Assert.AreEqual(person.ColumnEnumHand, queryResult.ColumnEnumHand);
            }
        }
    }
}

#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    /// <summary>
    /// Regression coverage for #1385: Merge/MergeAll (and Insert/InsertAll) fail on SQL Server with
    /// "String or binary data would be truncated" (error 8152) when the key that is read back is a
    /// string longer than the length that the statement declares for it. The OUTPUT table variable
    /// was declared as a bare NVARCHAR, which SQL Server treats as NVARCHAR(1), and the CONVERT of
    /// the key to a bare NVARCHAR truncates it to 30 characters.
    /// </summary>
    [TestClass]
    public class StringKeyOutputTest
    {
        private const string TableName = "StringKeyOutputTable";

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.ExecuteNonQuery($@"IF OBJECT_ID(N'[dbo].[{TableName}]', N'U') IS NOT NULL DROP TABLE [dbo].[{TableName}]");
            connection.ExecuteNonQuery($@"CREATE TABLE [dbo].[{TableName}]
                (
                    [Code] NVARCHAR(500) NOT NULL PRIMARY KEY,
                    [GroupId] INT NOT NULL,
                    [Name] NVARCHAR(500) NULL
                )");
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.ExecuteNonQuery($@"IF OBJECT_ID(N'[dbo].[{TableName}]', N'U') IS NOT NULL DROP TABLE [dbo].[{TableName}]");
        }

        private static StringKeyOutputTable CreateEntity(int index, int length = 12)
        {
            return new StringKeyOutputTable
            {
                Code = string.Concat("KEY-", index.ToString("D4", System.Globalization.CultureInfo.InvariantCulture)).PadRight(length, 'X'),
                GroupId = 1,
                Name = $"Name_{index}"
            };
        }

        private static List<StringKeyOutputTable> CreateEntities(int count, int length = 12)
        {
            return Enumerable.Range(1, count).Select(index => CreateEntity(index, length)).ToList();
        }

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeWithStringKeyLongerThanOneCharacter()
        {
            // Setup
            var entity = CreateEntity(1);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.Merge(entity, qualifiers: e => new { e.GroupId, e.Code });

            // Assert
            Assert.AreEqual(entity.Code, result);
            Assert.AreEqual(1, connection.CountAll<StringKeyOutputTable>());
            Assert.AreEqual(entity.Name, connection.QueryAll<StringKeyOutputTable>().Single().Name);
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncWithStringKeyLongerThanOneCharacter()
        {
            // Setup
            var entity = CreateEntity(1);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = await connection.MergeAsync(entity, qualifiers: e => new { e.GroupId, e.Code }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(entity.Code, result);
            Assert.AreEqual(1, await connection.CountAllAsync<StringKeyOutputTable>().ConfigureAwait(false));
        }

        [TestMethod]
        public void TestSqlConnectionMergeWithStringKeyLongerThanThirtyCharacters()
        {
            // Setup
            var entity = CreateEntity(1, 200);
            var code = entity.Code;

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.Merge(entity, qualifiers: e => new { e.GroupId, e.Code });

            // Assert
            Assert.AreEqual(code, result);
            Assert.AreEqual(code, entity.Code);
            Assert.AreEqual(code, connection.QueryAll<StringKeyOutputTable>().Single().Code);
        }

        [TestMethod]
        public void TestSqlConnectionMergeWithStringKeyForExistingRow()
        {
            // Setup
            var entity = CreateEntity(1);

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Insert(entity);
            entity.Name = "Updated";

            // Act
            var result = connection.Merge(entity, qualifiers: e => new { e.GroupId, e.Code });

            // Assert
            Assert.AreEqual(entity.Code, result);
            Assert.AreEqual(1, connection.CountAll<StringKeyOutputTable>());
            Assert.AreEqual("Updated", connection.QueryAll<StringKeyOutputTable>().Single().Name);
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllWithStringKeyLongerThanOneCharacter()
        {
            // Setup
            var entities = CreateEntities(10);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.MergeAll(entities, qualifiers: e => new { e.GroupId, e.Code });

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, connection.CountAll<StringKeyOutputTable>());
            CollectionAssert.AreEquivalent(entities.Select(e => e.Code).ToList(), connection.QueryAll<StringKeyOutputTable>().Select(e => e.Code).ToList());
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncWithStringKeyLongerThanOneCharacter()
        {
            // Setup
            var entities = CreateEntities(10);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = await connection.MergeAllAsync(entities, qualifiers: e => new { e.GroupId, e.Code }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, await connection.CountAllAsync<StringKeyOutputTable>().ConfigureAwait(false));
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllWithStringKeyLongerThanThirtyCharacters()
        {
            // Setup
            var entities = CreateEntities(10, 200);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.MergeAll(entities, qualifiers: e => new { e.GroupId, e.Code });

            // Assert
            Assert.AreEqual(entities.Count, result);
            CollectionAssert.AreEquivalent(entities.Select(e => e.Code).ToList(), connection.QueryAll<StringKeyOutputTable>().Select(e => e.Code).ToList());
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllWithStringKeyForExistingRows()
        {
            // Setup
            var entities = CreateEntities(10);

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.InsertAll(entities);
            entities.ForEach(e => e.Name = "Updated");

            // Act
            var result = connection.MergeAll(entities, qualifiers: e => new { e.GroupId, e.Code });

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, connection.CountAll<StringKeyOutputTable>());
            Assert.IsTrue(connection.QueryAll<StringKeyOutputTable>().All(e => string.Equals(e.Name, "Updated", System.StringComparison.Ordinal)));
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertWithStringKeyLongerThanThirtyCharacters()
        {
            // Setup
            var entity = CreateEntity(1, 200);
            var code = entity.Code;

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.Insert(entity);

            // Assert
            Assert.AreEqual(code, result);
            Assert.AreEqual(code, entity.Code);
            Assert.AreEqual(code, connection.QueryAll<StringKeyOutputTable>().Single().Code);
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllWithStringKeyLongerThanThirtyCharacters()
        {
            // Setup
            var entities = CreateEntities(10, 200);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.InsertAll(entities);

            // Assert
            Assert.AreEqual(entities.Count, result);
            CollectionAssert.AreEquivalent(entities.Select(e => e.Code).ToList(), connection.QueryAll<StringKeyOutputTable>().Select(e => e.Code).ToList());
        }

        #endregion
    }
}

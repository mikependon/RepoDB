#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Operations
{
    [TestClass]
    [DoNotParallelize]
    public class BatchOperationBehaviorTest : RepositoryOperationTestBase
    {
        [TestMethod]
        public void TestEntityInsertAllBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new CoverageEntity { Name = "Input" };
            var second = new CoverageEntity { Name = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = connection.InsertAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second.Id);
        }

        [TestMethod]
        public void TestEntityInsertAllEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<CoverageEntity>();

            // Act
            var actual = connection.InsertAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestEntityInsertAllNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<CoverageEntity> entities = null;

            // Act
            var actual = connection.InsertAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestEntityMergeAllBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new CoverageEntity { Name = "Input" };
            var second = new CoverageEntity { Name = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = connection.MergeAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second.Id);
        }

        [TestMethod]
        public void TestEntityMergeAllEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<CoverageEntity>();

            // Act
            var actual = connection.MergeAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestEntityMergeAllNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<CoverageEntity> entities = null;

            // Act
            var actual = connection.MergeAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestEntityUpdateAllBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new CoverageEntity { Name = "Input" };
            var second = new CoverageEntity { Name = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = connection.UpdateAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public void TestEntityUpdateAllEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<CoverageEntity>();

            // Act
            var actual = connection.UpdateAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestEntityUpdateAllNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<CoverageEntity> entities = null;

            // Act
            var actual = connection.UpdateAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public async Task TestEntityInsertAllAsyncBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new CoverageEntity { Name = "Input" };
            var second = new CoverageEntity { Name = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = await connection.InsertAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second.Id);
        }

        [TestMethod]
        public async Task TestEntityInsertAllAsyncEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<CoverageEntity>();

            // Act
            var actual = await connection.InsertAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestEntityInsertAllAsyncNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<CoverageEntity> entities = null;

            // Act
            var actual = await connection.InsertAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public async Task TestEntityMergeAllAsyncBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new CoverageEntity { Name = "Input" };
            var second = new CoverageEntity { Name = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = await connection.MergeAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second.Id);
        }

        [TestMethod]
        public async Task TestEntityMergeAllAsyncEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<CoverageEntity>();

            // Act
            var actual = await connection.MergeAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestEntityMergeAllAsyncNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<CoverageEntity> entities = null;

            // Act
            var actual = await connection.MergeAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public async Task TestEntityUpdateAllAsyncBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new CoverageEntity { Name = "Input" };
            var second = new CoverageEntity { Name = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = await connection.UpdateAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public async Task TestEntityUpdateAllAsyncEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<CoverageEntity>();

            // Act
            var actual = await connection.UpdateAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestEntityUpdateAllAsyncNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<CoverageEntity> entities = null;

            // Act
            var actual = await connection.UpdateAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestDictionaryInsertAllBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var second = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = connection.InsertAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second["Id"]);
        }

        [TestMethod]
        public void TestDictionaryInsertAllEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<Dictionary<string, object>>();

            // Act
            var actual = connection.InsertAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestDictionaryInsertAllNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<Dictionary<string, object>> entities = null;

            // Act
            var actual = connection.InsertAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestDictionaryMergeAllBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var second = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = connection.MergeAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second["Id"]);
        }

        [TestMethod]
        public void TestDictionaryMergeAllEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<Dictionary<string, object>>();

            // Act
            var actual = connection.MergeAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestDictionaryMergeAllNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<Dictionary<string, object>> entities = null;

            // Act
            var actual = connection.MergeAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestDictionaryUpdateAllBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var second = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = connection.UpdateAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public void TestDictionaryUpdateAllEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<Dictionary<string, object>>();

            // Act
            var actual = connection.UpdateAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestDictionaryUpdateAllNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<Dictionary<string, object>> entities = null;

            // Act
            var actual = connection.UpdateAll("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public async Task TestDictionaryInsertAllAsyncBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var second = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = await connection.InsertAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second["Id"]);
        }

        [TestMethod]
        public async Task TestDictionaryInsertAllAsyncEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<Dictionary<string, object>>();

            // Act
            var actual = await connection.InsertAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDictionaryInsertAllAsyncNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<Dictionary<string, object>> entities = null;

            // Act
            var actual = await connection.InsertAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public async Task TestDictionaryMergeAllAsyncBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var second = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = await connection.MergeAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(2, second["Id"]);
        }

        [TestMethod]
        public async Task TestDictionaryMergeAllAsyncEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<Dictionary<string, object>>();

            // Act
            var actual = await connection.MergeAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDictionaryMergeAllAsyncNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<Dictionary<string, object>> entities = null;

            // Act
            var actual = await connection.MergeAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public async Task TestDictionaryUpdateAllAsyncBatchesAndAssignsIdentities()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            connection.UseBatchResults();
            var first = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var second = new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 0, ["Name"] = "Input" };
            var entities = new[] { first, second };

            // Act
            var actual = await connection.UpdateAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public async Task TestDictionaryUpdateAllAsyncEmptyInput()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var entities = Array.Empty<Dictionary<string, object>>();

            // Act
            var actual = await connection.UpdateAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDictionaryUpdateAllAsyncNullInputReturnsZero()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            IEnumerable<Dictionary<string, object>> entities = null;

            // Act
            var actual = await connection.UpdateAllAsync("CoverageEntity", entities, batchSize: 2, fields: new[] { new Field("Id"), new Field("Name") });

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestMergeUpsertUpdatesExistingRow()
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(new UpsertDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = connection.Merge<CoverageEntity, int>(entity);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 1);
        }

        [TestMethod]
        public async Task TestMergeAsyncUpsertUpdatesExistingRow()
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(new UpsertDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await connection.MergeAsync<CoverageEntity, int>(entity);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 1);
        }

        private sealed class UpsertDbSetting : CustomDbSetting
        {
            public UpsertDbSetting()
            {
                IsUseUpsert = true;
            }
        }
    }
}

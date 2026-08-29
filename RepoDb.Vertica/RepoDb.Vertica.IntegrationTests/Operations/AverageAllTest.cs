using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Vertica.IntegrationTests.Operations
{
    [TestClass]
    public class AverageAllTest
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestVerticaConnectionAverageAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionAverageAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.AverageAll<CompleteTable>(e => e.ColumnInt,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionAverageAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnVerticaConnectionAverageAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.AverageAllAsync<CompleteTable>(e => e.ColumnInt,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestVerticaConnectionAverageAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First());

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionAverageAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionAverageAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First());

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnVerticaConnectionAverageAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}

#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Extensions;
using RepoDb.Reflection;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteReaderTest
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

        #region Sync

        [TestMethod]
        public void TestHanaConnectionExecuteReader()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\";"))
                {
                    while (reader.Read())
                    {
                        // Act
                        var id = reader.GetInt64(0);
                        var columnInt = reader.GetInt32(1);
                        var columnDateTime = reader.GetDateTime(2);
                        var table = tables.FirstOrDefault(e => e.Id == id);

                        // Assert
                        Assert.IsNotNull(table);
                        Assert.AreEqual(columnInt, table.ColumnInt);
                        Assert.AreEqual(columnDateTime, table.ColumnDateTime);
                    }
                }
            }
        }

        [TestMethod]
        [Ignore("HANA's ADO.NET client rejects a command text containing more than one SQL statement.")]
        public void TestHanaConnectionExecuteReaderWithMultipleStatements()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\"; SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\";"))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            // Act
                            var id = reader.GetInt64(0);
                            var columnInt = reader.GetInt32(1);
                            var columnDateTime = reader.GetDateTime(2);
                            var table = tables.FirstOrDefault(e => e.Id == id);

                            // Assert
                            Assert.IsNotNull(table);
                            Assert.AreEqual(columnInt, table.ColumnInt);
                            Assert.AreEqual(columnDateTime, table.ColumnDateTime);
                        }
                    } while (reader.NextResult());
                }
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteReaderAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\";"))
                {
                    // Act
                    var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteReaderAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\";"))
                {
                    // Act
                    var result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\";"))
                {
                    while (reader.Read())
                    {
                        // Act
                        var id = reader.GetInt64(0);
                        var columnInt = reader.GetInt32(1);
                        var columnDateTime = reader.GetDateTime(2);
                        var table = tables.FirstOrDefault(e => e.Id == id);

                        // Assert
                        Assert.IsNotNull(table);
                        Assert.AreEqual(columnInt, table.ColumnInt);
                        Assert.AreEqual(columnDateTime, table.ColumnDateTime);
                    }
                }
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionExecuteReaderWithMultipleStatements.")]
        public async Task TestHanaConnectionExecuteReaderAsyncWithMultipleStatements()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\"; SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\";"))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            // Act
                            var id = reader.GetInt64(0);
                            var columnInt = reader.GetInt32(1);
                            var columnDateTime = reader.GetDateTime(2);
                            var table = tables.FirstOrDefault(e => e.Id == id);

                            // Assert
                            Assert.IsNotNull(table);
                            Assert.AreEqual(columnInt, table.ColumnInt);
                            Assert.AreEqual(columnDateTime, table.ColumnDateTime);
                        }
                    } while (reader.NextResult());
                }
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteReaderAsyncAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\";"))
                {
                    // Act
                    var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteReaderAsyncAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\";"))
                {
                    // Act
                    var result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        #endregion
    }
}

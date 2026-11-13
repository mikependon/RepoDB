#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.Db2.IntegrationTests
{
    [TestClass]
    public class JsonColumnTest
    {
        #region Private Methods

        private static DB2Connection CreateConnection()
        {
            var connection = new DB2Connection(Database.ConnectionString);
            connection.EnsureOpen();
            DropTable(connection);
            connection.ExecuteNonQuery(@"CREATE TABLE ""JsonSample"" (""Id"" INTEGER NOT NULL PRIMARY KEY, ""Name"" VARCHAR(100), ""Document"" CLOB, ""Tags"" CLOB)");
            return connection;
        }

        private static void DropTable(DB2Connection connection)
        {
            try
            {
                connection.ExecuteNonQuery(@"DROP TABLE ""JsonSample""");
            }
            catch (Exception)
            {
                // The table does not exist yet
            }
        }

        private static JsonSampleEntity CreateCopenhagen() =>
            new JsonSampleEntity
            {
                Id = 1,
                Name = "First",
                Document = new JsonSampleDocument { Id = 5, Address = new JsonSampleAddress { City = "Copenhagen" } },
                Tags = new[] { "repo", "json" }
            };

        private static JsonSampleEntity CreateGentofte() =>
            new JsonSampleEntity
            {
                Id = 2,
                Name = "Second",
                Document = new JsonSampleDocument { Id = 6, Address = new JsonSampleAddress { City = "Gentofte" } },
                Tags = new[] { "other" }
            };

        private static void InsertSamples(DB2Connection connection)
        {
            connection.Insert(CreateCopenhagen());
            connection.Insert(CreateGentofte());
        }

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
        }

        #endregion

        #region Insert and Query

        [TestMethod]
        public void TestJsonColumnInsertAndQueryDocument()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                var row = connection.Query<JsonSampleEntity>(e => e.Id == 1).Single();

                // Assert
                Assert.AreEqual("Copenhagen", row.Document.Value.Address.City);
            }
        }

        [TestMethod]
        public void TestJsonColumnInsertAndQueryTags()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                var row = connection.Query<JsonSampleEntity>(e => e.Id == 1).Single();

                // Assert
                Assert.AreEqual("json", row.Tags.Value[1]);
            }
        }

        [TestMethod]
        public void TestJsonColumnInsertAndQueryEmptyDocument()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                connection.Insert(new JsonSampleEntity { Id = 3, Name = "Empty" });

                // Act
                var row = connection.Query<JsonSampleEntity>(e => e.Name == "Empty").Single();

                // Assert
                Assert.IsTrue(row.Document.IsEmpty);
            }
        }

        #endregion

        #region Filter

        [TestMethod]
        public void TestJsonColumnQueryByValueNumber()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                var rows = connection.Query<JsonSampleEntity>(e => e.Document.Value.Id == 5);

                // Assert
                Assert.AreEqual(1, rows.Count());
                Assert.AreEqual(1, rows.First().Id);
            }
        }

        [TestMethod]
        public void TestJsonColumnQueryByValueText()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                var rows = connection.Query<JsonSampleEntity>(e => e.Document.Value.Address.City == "Gentofte");

                // Assert
                Assert.AreEqual(2, rows.Single().Id);
            }
        }

        [TestMethod]
        public void TestJsonColumnQueryByExtractValue()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                var rows = connection.Query<JsonSampleEntity>(e => e.Document.ExtractValue(d => d.Address.City) == "Copenhagen");

                // Assert
                Assert.AreEqual(1, rows.Single().Id);
            }
        }

        [TestMethod]
        public void TestJsonColumnQueryByValueWithoutMatch()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                var rows = connection.Query<JsonSampleEntity>(e => e.Document.Value.Id == 99);

                // Assert
                Assert.AreEqual(0, rows.Count());
            }
        }

        #endregion

        #region Update and Delete

        [TestMethod]
        public void TestJsonColumnUpdateDocument()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                var row = connection.Query<JsonSampleEntity>(e => e.Id == 1).Single();
                row.Document = new JsonSampleDocument { Id = 7, Address = new JsonSampleAddress { City = "Aarhus" } };

                // Act
                connection.Update(row);

                // Assert
                Assert.AreEqual(1, connection.Query<JsonSampleEntity>(e => e.Document.Value.Id == 7).Count());
            }
        }

        [TestMethod]
        public void TestJsonColumnDeleteByValue()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                connection.Delete<JsonSampleEntity>(e => e.Document.Value.Id == 5);

                // Assert
                Assert.AreEqual(1, connection.CountAll<JsonSampleEntity>());
            }
        }

        #endregion
    }
}

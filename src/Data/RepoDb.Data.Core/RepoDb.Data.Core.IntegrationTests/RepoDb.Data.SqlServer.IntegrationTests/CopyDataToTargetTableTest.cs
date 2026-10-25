#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.SqlServer.IntegrationTests.Setup;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.SqlServer.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table into a target table of another name: the schema is created by the name of the target table.
    /// </summary>
    [TestClass]
    public class CopyDataToTargetTableTest
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
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        [TestMethod]
        public void TestCopyDataToCreatesTheTableByTheNameOfTheTargetTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "Money", target, tableExistenceBehavior: TableExistsBehavior.Throw).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "[dbo].[Money]" }, Helper.TargetTables().ToArray());
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Money]"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCreatesTheTableByTheNameOfTheTargetTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", "Money", target, tableExistenceBehavior: TableExistsBehavior.Throw)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "[dbo].[Money]" }, Helper.TargetTables().ToArray());
            }
        }
    }
}

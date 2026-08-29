using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.IntegrationTests.Conversions
{
    [TestClass]
    public class MergeAllConversionTest
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

        #region MergeAll<TEntity> (String To Integer Conversion)

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTEntityAutomaticConversionFromStringToInt()
        {
            // Setup
            var tables = Enumerable.Range(1, 5).Select(i => new IdentityTableWithColumnIntAsString
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = (i * 100).ToString(),
                ColumnNVarChar = Guid.NewGuid().ToString()
            }).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var affectedRows = connection.MergeAll<IdentityTableWithColumnIntAsString>(tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
                var results = connection.QueryAll<IdentityTable>().ToList();
                foreach (var table in tables)
                {
                    var match = results.First(r => r.RowGuid == table.RowGuid);
                    Assert.AreEqual(int.Parse(table.ColumnInt), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTEntityAutomaticConversionFromNullStringToInt()
        {
            // Setup
            var tables = Enumerable.Range(1, 5).Select(i => new IdentityTableWithColumnIntAsString
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = null,
                ColumnNVarChar = Guid.NewGuid().ToString()
            }).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var affectedRows = connection.MergeAll<IdentityTableWithColumnIntAsString>(tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
                var results = connection.QueryAll<IdentityTable>().ToList();
                foreach (var table in tables)
                {
                    var match = results.First(r => r.RowGuid == table.RowGuid);
                    Assert.IsNull(match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTEntityAutomaticConversionFromStringToIntUsingTableName()
        {
            // Setup
            var tables = Enumerable.Range(1, 5).Select(i => new IdentityTableWithColumnIntAsString
            {
                RowGuid = Guid.NewGuid(),
                ColumnInt = (i * 100).ToString(),
                ColumnNVarChar = Guid.NewGuid().ToString()
            }).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var affectedRows = connection.MergeAll<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, affectedRows);
                var results = connection.QueryAll<IdentityTable>().ToList();
                foreach (var table in tables)
                {
                    var match = results.First(r => r.RowGuid == table.RowGuid);
                    Assert.AreEqual(int.Parse(table.ColumnInt), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMergeAllViaTEntityWithAutomaticConversionOnNonNumericString()
        {
            // Setup
            var tables = new List<IdentityTableWithColumnIntAsString>
            {
                new IdentityTableWithColumnIntAsString
                {
                    RowGuid = Guid.NewGuid(),
                    ColumnInt = "100",
                    ColumnNVarChar = Guid.NewGuid().ToString()
                },
                new IdentityTableWithColumnIntAsString
                {
                    RowGuid = Guid.NewGuid(),
                    ColumnInt = "not-a-number",
                    ColumnNVarChar = Guid.NewGuid().ToString()
                }
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Assert
                Assert.Throws<FormatException>(() =>
                    connection.MergeAll<IdentityTableWithColumnIntAsString>(tables));

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}

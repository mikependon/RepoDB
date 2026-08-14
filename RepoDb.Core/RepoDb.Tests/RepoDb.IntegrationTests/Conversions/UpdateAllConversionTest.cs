using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
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
    public class UpdateAllConversionTest
    {
        /// <summary>
        /// Mapped to the same table as <see cref="IdentityTable"/>, except the <see cref="ColumnInt"/> column
        /// (a SQL <c>int</c>) is bound to a <see cref="string"/> property instead of <see cref="int"/>?.
        /// This is used to exercise the parameter-value conversion that <see cref="ConversionType.Automatic"/>
        /// performs before the value is sent to the database.
        /// </summary>
        [Map("[sc].[IdentityTable]")]
        public class IdentityTableWithColumnIntAsString
        {
            public long Id { get; set; }
            public Guid RowGuid { get; set; }
            public bool? ColumnBit { get; set; }
            public DateTime? ColumnDateTime { get; set; }
            public DateTime? ColumnDateTime2 { get; set; }
            public decimal? ColumnDecimal { get; set; }
            public double? ColumnFloat { get; set; }
            public string ColumnInt { get; set; }
            public string ColumnNVarChar { get; set; }
        }

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

        #region UpdateAll<TEntity> (String To Integer Conversion)

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaTEntityAutomaticConversionFromStringToInt()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = tables.Select((table, index) => new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = ((index + 1) * 100).ToString()
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll<IdentityTableWithColumnIntAsString>(updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(updates.Count, affectedRows);
                var results = connection.QueryAll<IdentityTable>().ToList();
                foreach (var update in updates)
                {
                    var match = results.First(r => r.Id == update.Id);
                    Assert.AreEqual(int.Parse(update.ColumnInt), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaTEntityAutomaticConversionFromNullStringToInt()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = tables.Select(table => new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = null
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll<IdentityTableWithColumnIntAsString>(updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(updates.Count, affectedRows);
                var results = connection.QueryAll<IdentityTable>().ToList();
                foreach (var update in updates)
                {
                    var match = results.First(r => r.Id == update.Id);
                    Assert.IsNull(match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAllViaTEntityAutomaticConversionFromStringToIntUsingTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = tables.Select((table, index) => new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = ((index + 1) * 100).ToString()
                }).AsList();

                // Act
                var affectedRows = connection.UpdateAll<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>(),
                    updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(updates.Count, affectedRows);
                var results = connection.QueryAll<IdentityTable>().ToList();
                foreach (var update in updates)
                {
                    var match = results.First(r => r.Id == update.Id);
                    Assert.AreEqual(int.Parse(update.ColumnInt), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionUpdateAllViaTEntityWithAutomaticConversionOnNonNumericString()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(2);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new List<IdentityTableWithColumnIntAsString>
                {
                    new IdentityTableWithColumnIntAsString
                    {
                        Id = tables[0].Id,
                        ColumnInt = "100"
                    },
                    new IdentityTableWithColumnIntAsString
                    {
                        Id = tables[1].Id,
                        ColumnInt = "not-a-number"
                    }
                };

                // Assert
                Assert.Throws<FormatException>(() =>
                    connection.UpdateAll<IdentityTableWithColumnIntAsString>(updates,
                        fields: Field.From(nameof(IdentityTable.ColumnInt))));

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}

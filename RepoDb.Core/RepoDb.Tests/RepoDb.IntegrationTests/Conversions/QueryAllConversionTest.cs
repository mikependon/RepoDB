using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Conversions
{
    [TestClass]
    public class QueryAllConversionTest
    {
        /// <summary>
        /// Mapped to the same table as <see cref="IdentityTable"/>, except the <see cref="ColumnInt"/> column
        /// (a SQL <c>int</c>) is bound to a <see cref="string"/> property instead of <see cref="int"/>?.
        /// This is a conversion that <see cref="ConversionType.Automatic"/> is expected to resolve.
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

        /// <summary>
        /// Mapped to the same table as <see cref="IdentityTable"/>, except the <see cref="RowGuid"/> column
        /// (a SQL <c>uniqueidentifier</c>) is bound to an <see cref="int"/> property. This is a conversion
        /// that is expected to fail regardless of <see cref="ConversionType"/>.
        /// </summary>
        [Map("[sc].[IdentityTable]")]
        public class IdentityTableWithRowGuidAsInt
        {
            public long Id { get; set; }
            public int RowGuid { get; set; }
            public bool? ColumnBit { get; set; }
            public DateTime? ColumnDateTime { get; set; }
            public DateTime? ColumnDateTime2 { get; set; }
            public decimal? ColumnDecimal { get; set; }
            public double? ColumnFloat { get; set; }
            public int? ColumnInt { get; set; }
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

        #region QueryAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTEntityAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.QueryAll<IdentityTableWithColumnIntAsString>().ToList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                foreach (var table in tables)
                {
                    var match = result.First(r => r.Id == table.Id);
                    Assert.AreEqual(table.ColumnInt.ToString(), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.QueryAll<IdentityTableWithColumnIntAsString>();

                // Assert
                Assert.IsFalse(result.Any());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTEntityAutomaticConversionUsingExplicitTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.QueryAll<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>()).ToList();

                // Assert
                Assert.AreEqual(tables.Count, result.Count);
                foreach (var table in tables)
                {
                    var match = result.First(r => r.Id == table.Id);
                    Assert.AreEqual(table.ColumnInt.ToString(), match.ColumnInt);
                }

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionQueryAllViaTEntityWithAutomaticConversionOnIncompatibleType()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Assert
                Assert.Throws<InvalidOperationException>(() =>
                    connection.QueryAll<IdentityTableWithRowGuidAsInt>().ToList());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}

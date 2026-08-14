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
    public class QueryConversionTest
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

        #region Query<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryViaTEntityAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Query<IdentityTableWithColumnIntAsString>(table.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(table.ColumnInt.ToString(), result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Query<IdentityTableWithColumnIntAsString>((object)null);

                // Assert
                Assert.IsFalse(result.Any());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryViaTEntityAutomaticConversionUsingExplicitTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Query<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>(),
                    table.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(table.ColumnInt.ToString(), result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionQueryViaTEntityWithAutomaticConversionOnIncompatibleType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Assert
                Assert.Throws<InvalidOperationException>(() =>
                    connection.Query<IdentityTableWithRowGuidAsInt>(table.Id).FirstOrDefault());

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}

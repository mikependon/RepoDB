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
    public class UpdateConversionTest
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

        #region Update<TEntity> (String To Integer Conversion)

        [TestMethod]
        public void TestSqlConnectionUpdateViaTEntityAutomaticConversionFromStringToInt()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = "123"
                };

                // Act
                var affectedRows = connection.Update<IdentityTableWithColumnIntAsString>(updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, affectedRows);
                var result = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.AreEqual(123, result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTEntityAutomaticConversionFromNullStringToInt()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = null
                };

                // Act
                var affectedRows = connection.Update<IdentityTableWithColumnIntAsString>(updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, affectedRows);
                var result = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.IsNull(result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTEntityAutomaticConversionFromStringToIntUsingTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = "456"
                };

                // Act
                var affectedRows = connection.Update<IdentityTableWithColumnIntAsString>(ClassMappedNameCache.Get<IdentityTable>(),
                    updates,
                    fields: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(1, affectedRows);
                var result = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();
                Assert.IsNotNull(result);
                Assert.AreEqual(456, result.ColumnInt);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionUpdateViaTEntityWithAutomaticConversionOnNonNumericString()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(table);

                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                var updates = new IdentityTableWithColumnIntAsString
                {
                    Id = table.Id,
                    ColumnInt = "not-a-number"
                };

                // Assert
                Assert.Throws<FormatException>(() =>
                    connection.Update<IdentityTableWithColumnIntAsString>(updates,
                        fields: Field.From(nameof(IdentityTable.ColumnInt))));

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}

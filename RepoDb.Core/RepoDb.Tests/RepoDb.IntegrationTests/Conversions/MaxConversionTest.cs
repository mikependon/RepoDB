using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.IO;
using System.Linq;

namespace RepoDb.IntegrationTests.Conversions
{
    [TestClass]
    public class MaxConversionTest
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

        #region Max<TEntity>

        [TestMethod]
        public void TestSqlConnectionMaxViaTEntityAutomaticConversion()
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
                var result = connection.Max<IdentityTable>(e => e.ColumnInt, (object)null);

                // Assert
                Assert.AreEqual(tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTEntityAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt, (object)null);

                // Assert
                Assert.IsNull(result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTEntityAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Max<IdentityTable, double>(new Field("ColumnInt"), (object)null);

                // Assert
                Assert.AreEqual((double)tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMaxViaTEntityWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Max<IdentityTable>(e => e.ColumnInt, (object)null));
            }
        }

        #endregion

        #region Max<TEntity, TResult>

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultAutomaticConversion()
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
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt, (object)null);

                // Assert
                Assert.AreEqual(tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt, (object)null);

                // Assert
                Assert.AreEqual(default(int?), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Max<IdentityTable, double>(new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual((double)tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMaxTypedResultWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Max<IdentityTable, int>(new Field("ColumnInt"), (object)null));
            }
        }

        #endregion

        #region Max (TableName)

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameAutomaticConversion()
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
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.IsNull(result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Max<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual((double)tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMaxViaTableNameWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                        new Field("ColumnInt"),
                        (object)null));
            }
        }

        #endregion

        #region Max<TResult> (TableName)

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultAutomaticConversion()
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
                var result = connection.Max<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultAutomaticConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Max<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(default(int?), result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultAutomaticConversionOnDifferentReturnType()
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
                var result = connection.Max<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual((double)tables.Last().ColumnInt, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionMaxViaTableNameTypedResultWithStrictConversionOnNoRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidCastException>(() =>
                    connection.Max<int>(ClassMappedNameCache.Get<IdentityTable>(),
                        new Field("ColumnInt"),
                        (object)null));
            }
        }

        #endregion
    }
}

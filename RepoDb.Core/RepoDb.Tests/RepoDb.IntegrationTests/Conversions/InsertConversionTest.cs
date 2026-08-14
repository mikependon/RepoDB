using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.IO;

namespace RepoDb.IntegrationTests.Conversions
{
    [TestClass]
    public class InsertConversionTest
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

        #region Insert<TEntity, TResult>

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<IdentityTable, long>(table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversionUsingTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<IdentityTable, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTEntityAutomaticConversionOnDifferentReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<IdentityTable, double>(table);

                // Assert
                Assert.AreEqual((double)table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionInsertViaTEntityWithStrictConversionOnIncompatibleReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidDataException>(() =>
                    connection.Insert<IdentityTable, Guid>(table));
            }
        }

        #endregion

        #region Insert (TableName)<TResult>

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameTypedResultAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameAutomaticConversionOnDifferentReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

                // Act
                var result = connection.Insert<double>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual((double)table.Id, result);

                // Reset
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionInsertViaTableNameWithStrictConversionOnIncompatibleReturnType()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Assert
                Assert.Throws<InvalidDataException>(() =>
                    connection.Insert<Guid>(ClassMappedNameCache.Get<IdentityTable>(),
                        table));
            }
        }

        #endregion
    }
}

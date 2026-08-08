using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class UpdateTest
    {
        private static readonly Random m_random = new();

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

        // Local, self-contained mutator - this project's shared Helper (which this workstream does not
        // own) has no "update the properties of an existing entity" equivalent. Deliberately sticks to
        // plain numeric/string columns so the post-update Helper.AssertPropertiesEquality round-trip can't
        // trip over the CHAR/NCHAR blank-padding or CLOB/NCLOB/BLOB/XMLTYPE edge cases already documented
        // in Helper.cs.
        private static void UpdateCompleteTableProperties(CompleteTable table)
        {
            table.ColumnVarchar = $"Updated-{m_random.Next(int.MaxValue)}";
            table.ColumnInt = m_random.Next(int.MinValue, int.MaxValue);
            table.ColumnNumber = Math.Round(Convert.ToDecimal(m_random.NextDouble() * 1000), 12);
        }

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionUpdateViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaDataEntityWithAutomaticConversion()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.Update<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update<CompleteTable>(table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaDataEntityWithAutomaticConversion()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync<CompleteTable>(table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #endregion

        #region TableName

        // NOTE: RepoDb.SqlServer.IntegrationTests.Operations.UpdateTest also has an "AsExpandoObject"
        // variant here. Skipped - this project's shared Helper (which this workstream does not own) has
        // no ExpandoObject/dynamic entity source.

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionUpdateViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

            // Assert
            Assert.AreEqual(1, result);

            // Act
            var queryResult = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        #endregion

        #endregion

        #region Hints

        [TestMethod]
        public void TestDb2ConnectionUpdateWithHintsThrows()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Update<CompleteTable>(table, hints: "NOLOCK"));
        }

        [TestMethod]
        public async Task TestDb2ConnectionUpdateAsyncWithHintsThrows()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            await Assert.ThrowsAsync<System.NotSupportedException>(() =>
                connection.UpdateAsync<CompleteTable>(table, hints: "NOLOCK"));
        }

        #endregion
    }
}

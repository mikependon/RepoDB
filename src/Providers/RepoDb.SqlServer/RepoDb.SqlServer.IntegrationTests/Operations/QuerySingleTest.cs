#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class QuerySingleTest
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<IdentityCompleteTable>(table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaExpression()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<IdentityCompleteTable>(e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<IdentityCompleteTable>(new { table.Id });

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<IdentityCompleteTable>(new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<IdentityCompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<IdentityCompleteTable>(new QueryGroup(queryFields));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleWithTop()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<IdentityCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQuerySingleIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<IdentityCompleteTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<IdentityCompleteTable>(new QueryField("Id", -1)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle<IdentityCompleteTable>((object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<IdentityCompleteTable>(table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<IdentityCompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<IdentityCompleteTable>(new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<IdentityCompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<IdentityCompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<IdentityCompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncWithTop()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<IdentityCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<IdentityCompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<IdentityCompleteTable>(new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync<IdentityCompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), table.Id);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), new { table.Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", table.Id));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryFields);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryGroup(queryFields));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQuerySingleViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQuerySingleViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQuerySingleViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", -1)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQuerySingleViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), table.Id).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new { table.Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryFields).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQuerySingleAsyncViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQuerySingleAsyncViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQuerySingleAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQuerySingleAsyncViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}

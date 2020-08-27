using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using RepoDb.IntegrationTests.Models;
using System.Data.Common;
using RepoDb.Enumerations;
using System.Reflection;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ClassHandlerTest
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

        #region Handlers

        private class ClassHandlerIdentityTableClassHandler : IClassHandler<ClassHandlerIdentityTable>
        {
            public int GetCallCount { get; set; }

            public int SetCallCount { get; set; }

            public ClassHandlerIdentityTable Get(ClassHandlerIdentityTable entity,
                DbDataReader reader)
            {
                ++GetCallCount;
                return entity;
            }

            public ClassHandlerIdentityTable Set(ClassHandlerIdentityTable entity)
            {
                ++SetCallCount;
                return entity;
            }

            public void Reset()
            {
                GetCallCount = 0;
                SetCallCount = 0;
            }
        }

        #endregion

        #region Classes

        [Map("[sc].[IdentityTable]"), ClassHandler(typeof(ClassHandlerIdentityTableClassHandler))]
        private class ClassHandlerIdentityTable : IdentityTable { }

        #endregion

        #region Helpers

        private IEnumerable<ClassHandlerIdentityTable> CreateClassHandlerIdentityTables(int count)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new ClassHandlerIdentityTable
                {
                    ColumnBit = true,
                    ColumnDateTime = DateTime.UtcNow.Date,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = random.Next(100),
                    ColumnFloat = random.Next(100),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"ColumnNvarChar-{Guid.NewGuid()}",
                    RowGuid = Guid.NewGuid()
                };
            }
        }

        #endregion

        #region Methods

        #region BatchQuery

        [TestMethod]
        public void TestBatchQueryWithClassHandler()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                var result = connection.BatchQuery<ClassHandlerIdentityTable>(page: 0,
                    rowsPerBatch: 10,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null);

                // Assert
                Assert.AreEqual(tables.Count, handler.GetCallCount);
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region ExecuteQuery

        [TestMethod]
        public void TestExecuteQueryWithClassHandler()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                var result = connection.ExecuteQuery<ClassHandlerIdentityTable>("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, handler.GetCallCount);
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestQueryWithClassHandler()
        {
            // Setup
            var table = CreateClassHandlerIdentityTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert(table);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                var result = connection.Query<ClassHandlerIdentityTable>(id).First();

                // Assert
                Assert.AreEqual(1, handler.GetCallCount);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestQueryAllWithClassHandler()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                var result = connection.QueryAll<ClassHandlerIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, handler.GetCallCount);
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #endregion
    }
}

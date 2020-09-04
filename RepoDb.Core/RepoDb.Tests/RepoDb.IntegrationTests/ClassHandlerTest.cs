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
using RepoDb.Exceptions;

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
            public int GetMethodCallCount { get; set; }

            public int SetMethodCallCount { get; set; }

            public ClassHandlerIdentityTable Get(ClassHandlerIdentityTable entity,
                DbDataReader reader)
            {
                ++GetMethodCallCount;
                return entity;
            }

            public ClassHandlerIdentityTable Set(ClassHandlerIdentityTable entity)
            {
                ++SetMethodCallCount;
                return entity;
            }

            public void Reset()
            {
                GetMethodCallCount = 0;
                SetMethodCallCount = 0;
            }
        }

        private class ClassHandlerTestModelClassHandler : IClassHandler<TestModel>
        {
            public TestModel Get(TestModel entity,
                DbDataReader reader)
            {
                return entity;
            }

            public TestModel Set(TestModel entity)
            {
                return entity;
            }
        }

        #endregion

        #region Classes

        private class TestModel { }

        [Map("[sc].[IdentityTable]"), ClassHandler(typeof(ClassHandlerIdentityTableClassHandler))]
        private class ClassHandlerIdentityTable : IdentityTable { }

        [Map("[sc].[IdentityTable]"), ClassHandler(typeof(ClassHandlerTestModelClassHandler))]
        private class ClassHandlerIdentityTableWithTestModel : IdentityTable { }

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

        private IEnumerable<ClassHandlerIdentityTableWithTestModel> CreateClassHandlerIdentityTableWithTestModels(int count)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new ClassHandlerIdentityTableWithTestModel
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
                Assert.AreEqual(tables.Count, handler.GetMethodCallCount);
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnBatchQueryWithClassHandlerWithDifferentModel()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.BatchQuery<ClassHandlerIdentityTableWithTestModel>(page: 0,
                    rowsPerBatch: 10,
                    orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                    where: (object)null);
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
                Assert.AreEqual(tables.Count, handler.GetMethodCallCount);
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnExecuteQueryWithClassHandlerWithDifferentModel()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.ExecuteQuery<ClassHandlerIdentityTableWithTestModel>("SELECT * FROM [sc].[IdentityTable];");
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestMergeWithClassHandler()
        {
            // Setup
            var table = CreateClassHandlerIdentityTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                var id = connection.Merge(table);

                // Assert
                Assert.AreEqual(1, handler.SetMethodCallCount);
            }
        }

        [TestMethod]
        public void TestMergeManyWithClassHandler()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                tables.ForEach(table => connection.Merge(table));

                // Assert
                Assert.AreEqual(tables.Count, handler.SetMethodCallCount);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnMergeWithClassHandlerWithDifferentModel()
        {
            // Setup
            var table = CreateClassHandlerIdentityTableWithTestModels(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.Merge(table);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestMergeAllWithClassHandler()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                connection.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, handler.SetMethodCallCount);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnMergeAllWithClassHandlerWithDifferentModel()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTableWithTestModels(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.MergeAll(tables);
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestInsertWithClassHandler()
        {
            // Setup
            var table = CreateClassHandlerIdentityTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                var id = connection.Insert(table);

                // Assert
                Assert.AreEqual(1, handler.SetMethodCallCount);
            }
        }

        [TestMethod]
        public void TestInsertManyWithClassHandler()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                tables.ForEach(table => connection.Insert(table));

                // Assert
                Assert.AreEqual(tables.Count, handler.SetMethodCallCount);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnInsertWithClassHandlerWithDifferentModel()
        {
            // Setup
            var table = CreateClassHandlerIdentityTableWithTestModels(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.Insert(table);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestInsertAllWithClassHandler()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                connection.InsertAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, handler.SetMethodCallCount);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnInsertAllWithClassHandlerWithDifferentModel()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTableWithTestModels(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.InsertAll(tables);
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
                Assert.AreEqual(1, handler.GetMethodCallCount);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnQueryWithClassHandlerWithDifferentModel()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.Query<ClassHandlerIdentityTableWithTestModel>(e => e.Id > 0);
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
                Assert.AreEqual(tables.Count, handler.GetMethodCallCount);
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnQueryAllWithClassHandlerWithDifferentModel()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.QueryAll<ClassHandlerIdentityTableWithTestModel>();
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestUpdateWithClassHandler()
        {
            // Setup
            var table = CreateClassHandlerIdentityTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerIdentityTableClassHandler>(typeof(ClassHandlerIdentityTable));
                handler.Reset();

                // Act
                connection.Update(table);

                // Assert
                Assert.AreEqual(1, handler.SetMethodCallCount);
            }
        }

        [TestMethod]
        public void TestUpdateManyWithClassHandler()
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
                tables.ForEach(table => connection.Update(table));

                // Assert
                Assert.AreEqual(tables.Count, handler.SetMethodCallCount);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnUpdateWithClassHandlerWithDifferentModel()
        {
            // Setup
            var table = CreateClassHandlerIdentityTableWithTestModels(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.Update(table);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestUpdateAllWithClassHandler()
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
                connection.UpdateAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, handler.SetMethodCallCount);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnUpdateAllWithClassHandlerWithDifferentModel()
        {
            // Setup
            var tables = CreateClassHandlerIdentityTableWithTestModels(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var handler = ClassHandlerCache.Get<ClassHandlerTestModelClassHandler>(typeof(ClassHandlerIdentityTableWithTestModel));

                // Act
                connection.UpdateAll(tables);
            }
        }

        #endregion

        #endregion
    }
}

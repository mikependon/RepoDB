using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Numbers
{
    [TestClass]
    public class DbRepositoryNumbersTest
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

        #region <TEntity>

        [TestMethod]
        public void TestDbRepositoryNumbersCrud()
        {
            // Setup
            var entity = new NumbersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345,
                ColumnBit = true,
                ColumnDecimal = 12345,
                ColumnFloat = (float)12345.12,
                ColumnInt = 12345,
                ColumnMoney = (decimal)12345.12,
                ColumnNumeric = 12345,
                ColumnReal = (float)12345.12,
                ColumnSmallInt = 12345,
                ColumnSmallMoney = 12345
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<NumbersClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigInt, data.ColumnBigInt);
                Assert.AreEqual(entity.ColumnBit, data.ColumnBit);
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
                Assert.AreEqual(entity.ColumnInt, data.ColumnInt);
                Assert.AreEqual(entity.ColumnMoney, data.ColumnMoney);
                Assert.AreEqual(entity.ColumnNumeric, data.ColumnNumeric);
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
                Assert.AreEqual(entity.ColumnSmallInt, data.ColumnSmallInt);
                Assert.AreEqual(entity.ColumnSmallMoney, data.ColumnSmallMoney);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersNullCrud()
        {
            // Setup
            var entity = new NumbersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = null,
                ColumnBit = null,
                ColumnDecimal = null,
                ColumnFloat = null,
                ColumnInt = null,
                ColumnMoney = null,
                ColumnNumeric = null,
                ColumnReal = null,
                ColumnSmallInt = null,
                ColumnSmallMoney = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<NumbersClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigInt);
                Assert.IsNull(data.ColumnBit);
                Assert.IsNull(data.ColumnDecimal);
                Assert.IsNull(data.ColumnFloat);
                Assert.IsNull(data.ColumnInt);
                Assert.IsNull(data.ColumnMoney);
                Assert.IsNull(data.ColumnNumeric);
                Assert.IsNull(data.ColumnReal);
                Assert.IsNull(data.ColumnSmallInt);
                Assert.IsNull(data.ColumnSmallInt);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersMappedCrud()
        {
            // Setup
            var entity = new NumbersMappedClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = 12345,
                ColumnBitMapped = true,
                ColumnDecimalMapped = 12345,
                ColumnFloatMapped = (float)12345.12,
                ColumnIntMapped = 12345,
                ColumnMoneyMapped = (decimal)12345.12,
                ColumnNumericMapped = 12345,
                ColumnRealMapped = (float)12345.12,
                ColumnSmallIntMapped = 12345,
                ColumnSmallMoneyMapped = 13456
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<NumbersMappedClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDecimalMapped, data.ColumnDecimalMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnMoneyMapped, data.ColumnMoneyMapped);
                Assert.AreEqual(entity.ColumnNumericMapped, data.ColumnNumericMapped);
                Assert.AreEqual(entity.ColumnRealMapped, data.ColumnRealMapped);
                Assert.AreEqual(entity.ColumnSmallIntMapped, data.ColumnSmallIntMapped);
                Assert.AreEqual(entity.ColumnSmallMoneyMapped, data.ColumnSmallMoneyMapped);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersMappedNullCrud()
        {
            // Setup
            var entity = new NumbersMappedClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = null,
                ColumnBitMapped = null,
                ColumnDecimalMapped = null,
                ColumnFloatMapped = null,
                ColumnIntMapped = null,
                ColumnMoneyMapped = null,
                ColumnNumericMapped = null,
                ColumnRealMapped = null,
                ColumnSmallIntMapped = null,
                ColumnSmallMoneyMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<NumbersMappedClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigIntMapped);
                Assert.IsNull(data.ColumnBitMapped);
                Assert.IsNull(data.ColumnDecimalMapped);
                Assert.IsNull(data.ColumnFloatMapped);
                Assert.IsNull(data.ColumnIntMapped);
                Assert.IsNull(data.ColumnMoneyMapped);
                Assert.IsNull(data.ColumnNumericMapped);
                Assert.IsNull(data.ColumnRealMapped);
                Assert.IsNull(data.ColumnSmallIntMapped);
                Assert.IsNull(data.ColumnSmallMoneyMapped);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersCrudAsync()
        {
            // Setup
            var entity = new NumbersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345,
                ColumnBit = true,
                ColumnDecimal = 12345,
                ColumnFloat = (float)12345.12,
                ColumnInt = 12345,
                ColumnMoney = (decimal)12345.12,
                ColumnNumeric = 12345,
                ColumnReal = (float)12345.12,
                ColumnSmallInt = 12345,
                ColumnSmallMoney = 12345
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<NumbersClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigInt, data.ColumnBigInt);
                Assert.AreEqual(entity.ColumnBit, data.ColumnBit);
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
                Assert.AreEqual(entity.ColumnInt, data.ColumnInt);
                Assert.AreEqual(entity.ColumnMoney, data.ColumnMoney);
                Assert.AreEqual(entity.ColumnNumeric, data.ColumnNumeric);
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
                Assert.AreEqual(entity.ColumnSmallInt, data.ColumnSmallInt);
                Assert.AreEqual(entity.ColumnSmallMoney, data.ColumnSmallMoney);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersNullCrudAsync()
        {
            // Setup
            var entity = new NumbersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = null,
                ColumnBit = null,
                ColumnDecimal = null,
                ColumnFloat = null,
                ColumnInt = null,
                ColumnMoney = null,
                ColumnNumeric = null,
                ColumnReal = null,
                ColumnSmallInt = null,
                ColumnSmallMoney = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<NumbersClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigInt);
                Assert.IsNull(data.ColumnBit);
                Assert.IsNull(data.ColumnDecimal);
                Assert.IsNull(data.ColumnFloat);
                Assert.IsNull(data.ColumnInt);
                Assert.IsNull(data.ColumnMoney);
                Assert.IsNull(data.ColumnNumeric);
                Assert.IsNull(data.ColumnReal);
                Assert.IsNull(data.ColumnSmallInt);
                Assert.IsNull(data.ColumnSmallMoney);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersMappedCrudAsync()
        {
            // Setup
            var entity = new NumbersMappedClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = 12345,
                ColumnBitMapped = true,
                ColumnDecimalMapped = 12345,
                ColumnFloatMapped = (float)12345.12,
                ColumnIntMapped = 12345,
                ColumnMoneyMapped = (decimal)12345.12,
                ColumnNumericMapped = 12345,
                ColumnRealMapped = (float)12345.12,
                ColumnSmallIntMapped = 12345,
                ColumnSmallMoneyMapped = 13456
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<NumbersMappedClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDecimalMapped, data.ColumnDecimalMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnMoneyMapped, data.ColumnMoneyMapped);
                Assert.AreEqual(entity.ColumnNumericMapped, data.ColumnNumericMapped);
                Assert.AreEqual(entity.ColumnRealMapped, data.ColumnRealMapped);
                Assert.AreEqual(entity.ColumnSmallIntMapped, data.ColumnSmallIntMapped);
                Assert.AreEqual(entity.ColumnSmallMoneyMapped, data.ColumnSmallMoneyMapped);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersMappedNullCrudAsync()
        {
            // Setup
            var entity = new NumbersMappedClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = null,
                ColumnBitMapped = null,
                ColumnDecimalMapped = null,
                ColumnFloatMapped = null,
                ColumnIntMapped = null,
                ColumnMoneyMapped = null,
                ColumnNumericMapped = null,
                ColumnRealMapped = null,
                ColumnSmallIntMapped = null,
                ColumnSmallMoneyMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<NumbersMappedClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigIntMapped);
                Assert.IsNull(data.ColumnBitMapped);
                Assert.IsNull(data.ColumnDecimalMapped);
                Assert.IsNull(data.ColumnFloatMapped);
                Assert.IsNull(data.ColumnIntMapped);
                Assert.IsNull(data.ColumnMoneyMapped);
                Assert.IsNull(data.ColumnNumericMapped);
                Assert.IsNull(data.ColumnRealMapped);
                Assert.IsNull(data.ColumnSmallIntMapped);
                Assert.IsNull(data.ColumnSmallMoneyMapped);
            }
        }

        #endregion

        #region (TableName)

        [TestMethod]
        public void TestDbRepositoryNumbersCrudViaTableName()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345,
                ColumnBit = true,
                ColumnDecimal = 12345,
                ColumnFloat = (float)12345.12,
                ColumnInt = 12345,
                ColumnMoney = (decimal)12345.12,
                ColumnNumeric = 12345,
                ColumnReal = (float)12345.12,
                ColumnSmallInt = 12345,
                ColumnSmallMoney = 12345
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(ClassMappedNameCache.Get<NumbersClass>(), entity);

                // Act Query
                var data = repository.Query(ClassMappedNameCache.Get<NumbersClass>(), new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigInt, data.ColumnBigInt);
                Assert.AreEqual(entity.ColumnBit, data.ColumnBit);
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
                Assert.AreEqual(entity.ColumnInt, data.ColumnInt);
                Assert.AreEqual(entity.ColumnMoney, data.ColumnMoney);
                Assert.AreEqual(entity.ColumnNumeric, data.ColumnNumeric);
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
                Assert.AreEqual(entity.ColumnSmallInt, data.ColumnSmallInt);
                Assert.AreEqual(entity.ColumnSmallMoney, data.ColumnSmallMoney);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersNullViaTableNameCrud()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = (long?)null,
                ColumnBit = (bool?)null,
                ColumnDecimal = (decimal?)null,
                ColumnFloat = (double?)null,
                ColumnInt = (int?)null,
                ColumnMoney = (decimal?)null,
                ColumnNumeric = (int?)null,
                ColumnReal = (float?)null,
                ColumnSmallInt = (short?)null,
                ColumnSmallMoney = (decimal?)null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(ClassMappedNameCache.Get<NumbersClass>(), entity);

                // Act Query
                var data = repository.Query(ClassMappedNameCache.Get<NumbersClass>(), new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigInt);
                Assert.IsNull(data.ColumnBit);
                Assert.IsNull(data.ColumnDecimal);
                Assert.IsNull(data.ColumnFloat);
                Assert.IsNull(data.ColumnInt);
                Assert.IsNull(data.ColumnMoney);
                Assert.IsNull(data.ColumnNumeric);
                Assert.IsNull(data.ColumnReal);
                Assert.IsNull(data.ColumnSmallInt);
                Assert.IsNull(data.ColumnSmallMoney);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersCrudViaTableNameAsync()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345,
                ColumnBit = true,
                ColumnDecimal = 12345,
                ColumnFloat = (float)12345.12,
                ColumnInt = 12345,
                ColumnMoney = (decimal)12345.12,
                ColumnNumeric = 12345,
                ColumnReal = (float)12345.12,
                ColumnSmallInt = 12345,
                ColumnSmallMoney = 12345
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(ClassMappedNameCache.Get<NumbersClass>(), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(ClassMappedNameCache.Get<NumbersClass>(), new { SessionId = (Guid)id });
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigInt, data.ColumnBigInt);
                Assert.AreEqual(entity.ColumnBit, data.ColumnBit);
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
                Assert.AreEqual(entity.ColumnInt, data.ColumnInt);
                Assert.AreEqual(entity.ColumnMoney, data.ColumnMoney);
                Assert.AreEqual(entity.ColumnNumeric, data.ColumnNumeric);
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
                Assert.AreEqual(entity.ColumnSmallInt, data.ColumnSmallInt);
                Assert.AreEqual(entity.ColumnSmallMoney, data.ColumnSmallMoney);
            }
        }

        [TestMethod]
        public void TestDbRepositoryNumbersNullCrudViaTableNameAsync()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = (long?)null,
                ColumnBit = (bool?)null,
                ColumnDecimal = (decimal?)null,
                ColumnFloat = (double?)null,
                ColumnInt = (int?)null,
                ColumnMoney = (decimal?)null,
                ColumnNumeric = (int?)null,
                ColumnReal = (float?)null,
                ColumnSmallInt = (short?)null,
                ColumnSmallMoney = (decimal?)null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(ClassMappedNameCache.Get<NumbersClass>(), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(ClassMappedNameCache.Get<NumbersClass>(), new { SessionId = (Guid)id });
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigInt);
                Assert.IsNull(data.ColumnBit);
                Assert.IsNull(data.ColumnDecimal);
                Assert.IsNull(data.ColumnFloat);
                Assert.IsNull(data.ColumnInt);
                Assert.IsNull(data.ColumnMoney);
                Assert.IsNull(data.ColumnNumeric);
                Assert.IsNull(data.ColumnReal);
                Assert.IsNull(data.ColumnSmallInt);
                Assert.IsNull(data.ColumnSmallMoney);
            }
        }

        #endregion
    }
}

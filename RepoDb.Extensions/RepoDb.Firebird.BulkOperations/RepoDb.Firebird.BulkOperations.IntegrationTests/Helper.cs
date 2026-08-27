using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Firebird.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests
{
    /// <summary>
    /// A helper class for the integration testing.
    /// </summary>
    public static class Helper
    {
        static Helper()
        {
            EpocDate = new DateTime(1970, 1, 1, 0, 0, 0);
        }

        #region Properties

        /// <summary>
        /// Gets the value of the Epoc date.
        /// </summary>
        public static DateTime EpocDate { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DateTime UtcNowFirebirdPrecision() =>
            new(DateTime.UtcNow.Ticks / 1000 * 1000, DateTimeKind.Utc);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public static void AssertPropertiesEquality<T1, T2>(T1 t1, T2 t2)
        {
            var propertiesOfType1 = typeof(T1).GetProperties();
            var propertiesOfType2 = typeof(T2).GetProperties();
            propertiesOfType1.AsList().ForEach(propertyOfType1 =>
            {
                if (propertyOfType1.Name == "Id" || propertyOfType1.Name == "IdMapped")
                {
                    return;
                }
                var propertyOfType2 = propertiesOfType2.FirstOrDefault(p => p.Name == propertyOfType1.Name);
                if (propertyOfType2 == null)
                {
                    return;
                }
                var value1 = propertyOfType1.GetValue(t1);
                var value2 = propertyOfType2.GetValue(t2);
                Assert.AreEqual(value1, value2,
                    $"Assert failed for '{propertyOfType1.Name}'. The values are '{value1}' and '{value2}'.");
            });
        }

        #endregion

        #region BulkOperationIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationIdentityTable> CreateBulkOperationIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<BulkOperationIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationIdentityTable
                {
                    Id = hasId ? index : 0,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowFirebirdPrecision(),
                    ColumnDecimal = random.Next(100),
                    ColumnFloat = random.Next(100),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationIdentityTables(List<BulkOperationIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                table.ColumnBit = false;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = UtcNowFirebirdPrecision();
                table.ColumnDecimal = Convert.ToDecimal(random.Next(100));
                table.ColumnFloat = Convert.ToDouble(random.Next(100));
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
            }
        }

        #endregion

        #region BulkOperationNonIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationNonIdentityTable> CreateBulkOperationNonIdentityTables(int count,
            bool hasId = true)
        {
            var random = new Random();
            var tables = new List<BulkOperationNonIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationNonIdentityTable
                {
                    Id = hasId ? index : 0,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowFirebirdPrecision(),
                    ColumnDecimal = random.Next(100),
                    ColumnFloat = random.Next(100),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationNonIdentityTables(List<BulkOperationNonIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                table.ColumnBit = false;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = UtcNowFirebirdPrecision();
                table.ColumnDecimal = Convert.ToDecimal(random.Next(100));
                table.ColumnFloat = Convert.ToDouble(random.Next(100));
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
            }
        }

        #endregion

        #region BulkOperationMappedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationMappedIdentityTable> CreateBulkOperationMappedIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<BulkOperationMappedIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationMappedIdentityTable
                {
                    IdMapped = hasId ? index : 0,
                    RowGuidMapped = Guid.NewGuid(),
                    ColumnBitMapped = true,
                    ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2Mapped = UtcNowFirebirdPrecision(),
                    ColumnDecimalMapped = random.Next(100),
                    ColumnFloatMapped = random.Next(100),
                    ColumnIntMapped = random.Next(100),
                    ColumnNVarCharMapped = $"NVARCHAR{random.Next(100)}"
                });
            }
            return tables;
        }

        #endregion

        #region BulkOperationMappedNonIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationMappedNonIdentityTable> CreateBulkOperationMappedNonIdentityTables(int count,
            bool hasId = true)
        {
            var random = new Random();
            var tables = new List<BulkOperationMappedNonIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationMappedNonIdentityTable
                {
                    IdMapped = hasId ? index : 0,
                    RowGuidMapped = Guid.NewGuid(),
                    ColumnBitMapped = true,
                    ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2Mapped = UtcNowFirebirdPrecision(),
                    ColumnDecimalMapped = random.Next(100),
                    ColumnFloatMapped = random.Next(100),
                    ColumnIntMapped = random.Next(100),
                    ColumnNVarCharMapped = $"NVARCHAR{random.Next(100)}"
                });
            }
            return tables;
        }

        #endregion
    }
}

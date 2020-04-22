using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RepoDb.SqlServer.IntegrationTests
{
    [TestClass]
    public class MaxSizesTest
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

        #region SubClasses

        [Map("[dbo].[CompleteTable]")]
        private class MaxNVarCharClass
        {
            public Guid SessionId { get; set; }
            public string ColumnNVarChar { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class MaxVarBinaryClass
        {
            public Guid SessionId { get; set; }
            public byte[] ColumnVarBinary { get; set; }
        }

        private IEnumerable<MaxNVarCharClass> GetMaxNVarCharClasses(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new MaxNVarCharClass
                {
                    SessionId = Guid.NewGuid(),
                    ColumnNVarChar = GetRandomString(10000)
                };
            }
        }

        private IEnumerable<MaxVarBinaryClass> GetMaxVarBinaryClasses(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new MaxVarBinaryClass
                {
                    SessionId = Guid.NewGuid(),
                    ColumnVarBinary = Encoding.UTF8.GetBytes(GetRandomString(10000))
                };
            }
        }

        private string GetRandomString(int count = 8000)
        {
            var random = new Random();
            var result = string.Empty;
            for (var i = 0; i < count; i++)
            {
                result = string.Concat(result, (char)random.Next(61, 122));
            }
            return result;
        }

        #endregion


        [TestMethod]
        public void TestNVarCharMax()
        {
            // Setup
            var entities = GetMaxNVarCharClasses(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(entities);

                // Act
                var result = connection.QueryAll<MaxNVarCharClass>();

                // Assert
                entities.ForEach(entity =>
                {
                    var item = result.First(e => e.SessionId == entity.SessionId);
                    Helper.AssertPropertiesEquality(entity, item);
                });
            }
        }

        [TestMethod]
        public void TestVarBinaryMax()
        {
            // Setup
            var entities = GetMaxVarBinaryClasses(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(entities);

                // Act
                var result = connection.QueryAll<MaxVarBinaryClass>();

                // Assert
                entities.ForEach(entity =>
                {
                    var item = result.First(e => e.SessionId == entity.SessionId);
                    Helper.AssertPropertiesEquality(entity, item);
                });
            }
        }
    }
}

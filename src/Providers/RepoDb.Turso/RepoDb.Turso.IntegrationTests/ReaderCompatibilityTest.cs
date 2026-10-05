using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Reflection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Turso.Data.Sqlite;

namespace RepoDb.Turso.IntegrationTests
{
    [TestClass]
    public sealed class ReaderCompatibilityTest
    {
        [TestMethod]
        public async Task TestTursoTypedValuesAndIdentityRoundTripAsync()
        {
            GlobalConfiguration.Setup(new()
            {
                ConversionType = Enumerations.ConversionType.Automatic
            }).UseTurso();
            using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync().ConfigureAwait(false);
            await connection.ExecuteNonQueryAsync(
                "CREATE TABLE TypedRow (Id INTEGER PRIMARY KEY, Amount DECIMAL, Created DATETIME, Enabled BOOLEAN, Bytes BLOB, Optional TEXT);").ConfigureAwait(false);
            var entity = new TypedRow
            {
                Amount = 12.125m,
                Created = new DateTime(2026, 1, 2, 3, 4, 5).AddTicks(1234567),
                Enabled = true,
                Bytes = new byte[] { 0, 1, 255 }
            };

            var id = await connection.InsertAsync<TypedRow, long>(entity).ConfigureAwait(false);
            var result = (await connection.QueryAsync<TypedRow>(id).ConfigureAwait(false)).Single();
            Assert.AreEqual(1L, id);
            Assert.AreEqual(id, entity.Id);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(entity.Amount, result.Amount);
            Assert.AreEqual(entity.Created, result.Created);
            Assert.AreEqual(entity.Enabled, result.Enabled);
            CollectionAssert.AreEqual(entity.Bytes, result.Bytes);
            Assert.IsNull(result.Optional);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(3)]
        public void TestLateBoundReaderTypesPreserveAllTypedAndDynamicRows(int count)
        {
            GlobalConfiguration.Setup().UseTurso();
            using var connection = CreateConnection(count);
            using var reader = (DbDataReader)connection.ExecuteReader("SELECT Id, Name FROM ReaderRow ORDER BY Id;");
            var rows = DataReader.ToEnumerable<ReaderRow>(reader).ToList();
            Assert.HasCount(count, rows);
            for (var index = 0; index < count; index++)
            {
                Assert.AreEqual(index + 1L, rows[index].Id);
                Assert.AreEqual($"Name{index}", rows[index].Name);
            }

            using var dynamicReader = (DbDataReader)connection.ExecuteReader("SELECT Id, 'literal' AS Marker FROM ReaderRow ORDER BY Id;");
            var dynamicRows = DataReader.ToEnumerable(dynamicReader).ToList();
            Assert.HasCount(count, dynamicRows);
            for (var index = 0; index < count; index++)
            {
                var row = (IDictionary<string, object>)dynamicRows[index];
                Assert.AreEqual(index + 1L, row["Id"]);
                Assert.AreEqual("literal", row["Marker"]);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(3)]
        public async Task TestLateBoundReaderTypesPreserveAllTypedAndDynamicRowsAsync(int count)
        {
            GlobalConfiguration.Setup().UseTurso();
            using var connection = CreateConnection(count);
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync("SELECT Id, Name FROM ReaderRow ORDER BY Id;").ConfigureAwait(false);
            var rows = new List<ReaderRow>();
            await foreach (var row in DataReader.ToEnumerableAsync<ReaderRow>(reader).ConfigureAwait(false))
            {
                rows.Add(row);
            }
            Assert.HasCount(count, rows);
            for (var index = 0; index < count; index++)
            {
                Assert.AreEqual(index + 1L, rows[index].Id);
                Assert.AreEqual($"Name{index}", rows[index].Name);
            }

            using var dynamicReader = (DbDataReader)await connection.ExecuteReaderAsync("SELECT Id, 'literal' AS Marker FROM ReaderRow ORDER BY Id;").ConfigureAwait(false);
            var dynamicRows = new List<object>();
            await foreach (var row in DataReader.ToEnumerableAsync(dynamicReader).ConfigureAwait(false))
            {
                dynamicRows.Add(row);
            }
            Assert.HasCount(count, dynamicRows);
            for (var index = 0; index < count; index++)
            {
                var row = Assert.IsInstanceOfType<IDictionary<string, object>>(dynamicRows[index]);
                Assert.AreEqual(index + 1L, row["Id"]);
                Assert.AreEqual("literal", row["Marker"]);
            }
        }

        private static SqliteConnection CreateConnection(int count)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            connection.ExecuteNonQuery("CREATE TABLE ReaderRow (Id INTEGER PRIMARY KEY, Name TEXT);");
            for (var index = 0; index < count; index++)
            {
                connection.ExecuteNonQuery("INSERT INTO ReaderRow (Name) VALUES (@Name);", new { Name = $"Name{index}" });
            }
            return connection;
        }

        public sealed class ReaderRow
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        public sealed class TypedRow
        {
            public long Id { get; set; }
            public decimal Amount { get; set; }
            public DateTime Created { get; set; }
            public bool Enabled { get; set; }
            public byte[] Bytes { get; set; }
            public string Optional { get; set; }
        }
    }
}

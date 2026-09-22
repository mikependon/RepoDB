#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PropertyHandlers;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="StringToIPAddressPropertyHandler"/>. The <c>NVARCHAR(MAX)</c> column of the <c>[dbo].[CompleteTable]</c>
    /// is used to store the <see cref="IPAddress"/> as a text. The <c>VARBINARY(MAX)</c> column is used to read the address as its raw bytes.
    /// </summary>
    [TestClass]
    public class StringToIPAddressPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(IPAddress));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class IPAddressAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(StringToIPAddressPropertyHandler))]
            public IPAddress Address { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class IPAddressBinaryAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnVarBinary"), PropertyHandler(typeof(StringToIPAddressPropertyHandler))]
            public IPAddress Address { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class IPAddressFluentModel
        {
            public Guid SessionId { get; set; }

            public IPAddress ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="IPAddress"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class IPAddressTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public IPAddress ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Negative: the handler supports a <see cref="string"/>, <see cref="T:byte[]"/> or <see cref="IPAddress"/>, but the column is returned as an <see cref="int"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class IPAddressOnIntColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnInt"), PropertyHandler(typeof(StringToIPAddressPropertyHandler))]
            public IPAddress Address { get; set; }
        }

        #endregion

        #region Helpers

        private static IPAddressAttributeModel CreateModel(string address) =>
            new IPAddressAttributeModel { SessionId = Guid.NewGuid(), Address = address == null ? null : IPAddress.Parse(address) };

        private static string GetRawValue(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar(
                "SELECT [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as string;

        private static bool IsRawValueNull(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [ColumnNVarChar] IS NULL;",
                new { SessionId = sessionId }) == 1;

        private static Guid InsertRawValue(SqlConnection connection,
            string value)
        {
            var sessionId = Guid.NewGuid();
            connection.ExecuteNonQuery(
                "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, @Value);",
                new { SessionId = sessionId, Value = value });
            return sessionId;
        }

        private static Guid InsertRawBinaryValue(SqlConnection connection,
            string hexLiteral)
        {
            var sessionId = Guid.NewGuid();
            connection.ExecuteNonQuery(
                $"INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnVarBinary]) VALUES (@SessionId, {hexLiteral});",
                new { SessionId = sessionId });
            return sessionId;
        }

        #endregion

        #region Positive

        [TestMethod]
        public void TestStringToIPAddressPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("192.168.1.10");

                // Act
                connection.Insert(model);
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.Address, result.Address);
            }
        }

        [TestMethod]
        public async Task TestStringToIPAddressPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("192.168.1.10");

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<IPAddressAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual(model.Address, result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWritesTheAddressText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("10.0.0.1");

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("10.0.0.1", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerReadsTheAddressText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "172.16.254.1");

                // Act
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(IPAddress.Parse("172.16.254.1"), result.Address);
                Assert.AreEqual(System.Net.Sockets.AddressFamily.InterNetwork, result.Address.AddressFamily);
            }
        }

        [TestMethod]
        [DataRow("0.0.0.0")]
        [DataRow("127.0.0.1")]
        [DataRow("255.255.255.255")]
        [DataRow("::1")]
        [DataRow("2001:db8::1")]
        [DataRow("fe80::1ff:fe23:4567:890a")]
        [DataRow("::")]
        public void TestStringToIPAddressPropertyHandlerWithAddresses(string address)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(address);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.Address.ToString(), raw, StringComparer.Ordinal);
                Assert.AreEqual(model.Address, result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWithIPv6AddressFamily()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("2001:db8::1");

                // Act
                connection.Insert(model);
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(System.Net.Sockets.AddressFamily.InterNetworkV6, result.Address.AddressFamily);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerReadsTheIPv4AddressFromBytes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawBinaryValue(connection, "0xC0A8010A");

                // Act
                var result = connection.Query<IPAddressBinaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(IPAddress.Parse("192.168.1.10"), result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerReadsTheIPv6AddressFromBytes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawBinaryValue(connection, "0x20010DB8000000000000000000000001");

                // Act
                var result = connection.Query<IPAddressBinaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(IPAddress.Parse("2001:db8::1"), result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(i => CreateModel($"10.0.0.{i}")).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<IPAddressAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(model.Address, result.First(e => e.SessionId == model.SessionId).Address);
                }
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("10.0.0.1");
                connection.Insert(model);

                // Act
                model.Address = IPAddress.Parse("10.0.0.2");
                var affectedRows = connection.Update(model);
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(IPAddress.Parse("10.0.0.2"), result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("10.0.0.1");
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<IPAddressAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual(model.Address, result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<IPAddressFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new StringToIPAddressPropertyHandler());
                var model = new IPAddressFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = IPAddress.Parse("10.1.2.3") };

                // Act
                connection.Insert(model);
                var result = connection.Query<IPAddressFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<IPAddress, StringToIPAddressPropertyHandler>(true);
                var model = new IPAddressTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = IPAddress.Parse("10.1.2.3") };

                // Act
                connection.Insert(model);
                var result = connection.Query<IPAddressTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWithNullAddress()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWithEmptyBinaryColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawBinaryValue(connection, "0x");

                // Act
                var result = connection.Query<IPAddressBinaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWithNullBinaryColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawBinaryValue(connection, "NULL");

                // Act
                var result = connection.Query<IPAddressBinaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Address);
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerUpdateToNullAddress()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("10.0.0.1");
                connection.Insert(model);

                // Act
                model.Address = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<IPAddressAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Address);
            }
        }

        [TestMethod]
        [DataRow("999.1.1.1")]
        [DataRow("not an address")]
        [DataRow("1.2.3.4.5")]
        [DataRow("2001:db8:::1")]
        public void TestStringToIPAddressPropertyHandlerWithInvalidText(string text)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, text);

                // Act / Assert
                Assert.Throws<FormatException>(() =>
                    connection.Query<IPAddressAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        [DataRow("0x010203")]
        [DataRow("0x0102030405")]
        public void TestStringToIPAddressPropertyHandlerWithInvalidBytesLength(string hexLiteral)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // Only 4 (IPv4) or 16 (IPv6) bytes are valid addresses
                var sessionId = InsertRawBinaryValue(connection, hexLiteral);

                // Act / Assert
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<IPAddressBinaryAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToIPAddressPropertyHandlerWithUnsupportedColumnType()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnInt]) VALUES (@SessionId, 1);",
                    new { SessionId = sessionId });

                // Act / Assert
                // The driver returns 'int', which is not a supported network address value
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<IPAddressOnIntColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}

#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Resolvers;

namespace RepoDb.MySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class MySqlDbTypeToMySqlStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector();
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForBinary()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Binary);

            // Assert
            Assert.AreEqual("BINARY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForBit()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Bit);

            // Assert
            Assert.AreEqual("BIT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Blob);

            // Assert
            Assert.AreEqual("BLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForByte()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Byte);

            // Assert
            Assert.AreEqual("TINYINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUByte()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UByte);

            // Assert
            Assert.AreEqual("TINYINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDate()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Date);

            // Assert
            Assert.AreEqual("DATE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.DateTime);

            // Assert
            Assert.AreEqual("DATETIME", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDouble()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForEnum()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Enum);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForGuid()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Guid);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForSet()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Set);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Text);

            // Assert
            Assert.AreEqual("TEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForFloat()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Float);

            // Assert
            Assert.AreEqual("FLOAT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForGeometry()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Geometry);

            // Assert
            Assert.AreEqual("GEOMETRY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt16()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int16);

            // Assert
            Assert.AreEqual("SMALLINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt24()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int24);

            // Assert
            Assert.AreEqual("SMALLINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt24()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt24);

            // Assert
            Assert.AreEqual("SMALLINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt16()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt16);

            // Assert
            Assert.AreEqual("SMALLINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt32()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int32);

            // Assert
            Assert.AreEqual("INT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt32()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt32);

            // Assert
            Assert.AreEqual("INT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt64()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int64);

            // Assert
            Assert.AreEqual("BIGINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt64()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt64);

            // Assert
            Assert.AreEqual("BIGINT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForJson()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.JSON);

            // Assert
            Assert.AreEqual("JSON", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForLongBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.LongBlob);

            // Assert
            Assert.AreEqual("LONGBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForLongText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.LongText);

            // Assert
            Assert.AreEqual("LONGTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForMediumBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.MediumBlob);

            // Assert
            Assert.AreEqual("MEDIUMBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForMediumText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.MediumText);

            // Assert
            Assert.AreEqual("MEDIUMTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForNewdate()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Newdate);

            // Assert
            Assert.AreEqual("DATE", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForNewDecimal()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.NewDecimal);

            // Assert
            Assert.AreEqual("DECIMAL", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForString()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.String);

            // Assert
            Assert.AreEqual("STRING", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTime()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Time);

            // Assert
            Assert.AreEqual("TIME", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTimestamp()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Timestamp);

            // Assert
            Assert.AreEqual("TIMESTAMP", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTinyBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.TinyBlob);

            // Assert
            Assert.AreEqual("TINYBLOB", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTinyText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.TinyText);

            // Assert
            Assert.AreEqual("TINYTEXT", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForVarBinary()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarBinary);

            // Assert
            Assert.AreEqual("VARBINARY", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForVarChar()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarChar);

            // Assert
            Assert.AreEqual("VARCHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForVarString()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarString);

            // Assert
            Assert.AreEqual("VARCHAR", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForYear()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Year);

            // Assert
            Assert.AreEqual("YEAR", result, StringComparer.Ordinal);
        }
    }
}

#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Resolvers;

namespace RepoDb.SapHana.UnitTests.Resolvers
{
    [TestClass]
    public class SapHanaDbTypeToStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSapHana();
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForTinyInt()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.TinyInt);

            // Assert
            Assert.AreEqual("TINYINT", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForSmallInt()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.SmallInt);

            // Assert
            Assert.AreEqual("SMALLINT", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForInteger()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Integer);

            // Assert
            Assert.AreEqual("INTEGER", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForBigInt()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.BigInt);

            // Assert
            Assert.AreEqual("BIGINT", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForSmallDecimal()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.SmallDecimal);

            // Assert
            Assert.AreEqual("SMALLDECIMAL", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForReal()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Real);

            // Assert
            Assert.AreEqual("REAL", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForDouble()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForBoolean()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Boolean);

            // Assert
            Assert.AreEqual("BOOLEAN", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForVarChar()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.VarChar);

            // Assert
            Assert.AreEqual("VARCHAR", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForNVarChar()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.NVarChar);

            // Assert
            Assert.AreEqual("NVARCHAR", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForText()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Text);

            // Assert
            Assert.AreEqual("TEXT", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForClob()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Clob);

            // Assert
            Assert.AreEqual("CLOB", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForNClob()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.NClob);

            // Assert
            Assert.AreEqual("NCLOB", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForDate()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Date);

            // Assert
            Assert.AreEqual("DATE", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForTime()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Time);

            // Assert
            Assert.AreEqual("TIME", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForTimestamp()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.TimeStamp);

            // Assert
            Assert.AreEqual("TIMESTAMP", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForSecondDate()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.SecondDate);

            // Assert
            Assert.AreEqual("SECONDDATE", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForBlob()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.Blob);

            // Assert
            Assert.AreEqual("BLOB", result, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbTypeToStringNameResolverForVarBinary()
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(HanaDbType.VarBinary);

            // Assert
            Assert.AreEqual("VARBINARY", result, System.StringComparer.Ordinal);
        }
    }
}

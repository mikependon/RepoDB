#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToOracleStringNameResolverTest
    {
        private readonly DbTypeToOracleStringNameResolver m_resolver = new();

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForInt64()
        {
            Assert.AreEqual("NUMBER(19)", m_resolver.Resolve(DbType.Int64), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForInt32()
        {
            Assert.AreEqual("NUMBER(10)", m_resolver.Resolve(DbType.Int32), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForInt16()
        {
            Assert.AreEqual("NUMBER(5)", m_resolver.Resolve(DbType.Int16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForByte()
        {
            Assert.AreEqual("NUMBER(3)", m_resolver.Resolve(DbType.Byte), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForBoolean()
        {
            Assert.AreEqual("NUMBER(1)", m_resolver.Resolve(DbType.Boolean), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForString()
        {
            Assert.AreEqual("NVARCHAR2(2000)", m_resolver.Resolve(DbType.String), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForAnsiString()
        {
            Assert.AreEqual("VARCHAR2(2000)", m_resolver.Resolve(DbType.AnsiString), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDate()
        {
            Assert.AreEqual("DATE", m_resolver.Resolve(DbType.Date), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDateTime()
        {
            Assert.AreEqual("DATE", m_resolver.Resolve(DbType.DateTime), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDateTime2()
        {
            Assert.AreEqual("TIMESTAMP", m_resolver.Resolve(DbType.DateTime2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDateTimeOffset()
        {
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", m_resolver.Resolve(DbType.DateTimeOffset), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDecimal()
        {
            Assert.AreEqual("NUMBER(18,2)", m_resolver.Resolve(DbType.Decimal), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForDouble()
        {
            Assert.AreEqual("BINARY_DOUBLE", m_resolver.Resolve(DbType.Double), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForSingle()
        {
            Assert.AreEqual("BINARY_FLOAT", m_resolver.Resolve(DbType.Single), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForGuid()
        {
            Assert.AreEqual("RAW(16)", m_resolver.Resolve(DbType.Guid), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForBinary()
        {
            Assert.AreEqual("BLOB", m_resolver.Resolve(DbType.Binary), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForXml()
        {
            Assert.AreEqual("XMLTYPE", m_resolver.Resolve(DbType.Xml), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForAnsiStringFixedLength()
        {
            Assert.AreEqual("CHAR(2000)", m_resolver.Resolve(DbType.AnsiStringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForStringFixedLength()
        {
            Assert.AreEqual("NCHAR(2000)", m_resolver.Resolve(DbType.StringFixedLength), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForObject()
        {
            Assert.AreEqual("BLOB", m_resolver.Resolve(DbType.Object), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverForTime()
        {
            Assert.AreEqual("INTERVAL DAY(0) TO SECOND(7)", m_resolver.Resolve(DbType.Time), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbTypeToOracleStringNameResolverFallsBackToNVarchar2ForUnmappedDbTypes()
        {
            // DbType.Currency has no explicit case in the switch, so it should hit the default arm.
            Assert.AreEqual("NVARCHAR2(2000)", m_resolver.Resolve(DbType.Currency), StringComparer.Ordinal);
        }
    }
}

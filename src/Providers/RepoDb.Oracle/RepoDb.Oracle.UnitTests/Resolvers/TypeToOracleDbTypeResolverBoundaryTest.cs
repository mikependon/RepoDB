using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Resolvers;
using System.Collections.Generic;
using System;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class TypeToOracleDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="expected">The expected provider type.</param>
        [DataRow(typeof(string),
            OracleDbType.NVarchar2)]
        [DataRow(typeof(char),
            OracleDbType.NChar)]
        [DataRow(typeof(char?),
            OracleDbType.NChar)]
        [DataRow(typeof(bool),
            OracleDbType.Int32)]
        [DataRow(typeof(bool?),
            OracleDbType.Int32)]
        [DataRow(typeof(byte),
            OracleDbType.Byte)]
        [DataRow(typeof(byte?),
            OracleDbType.Byte)]
        [DataRow(typeof(sbyte),
            OracleDbType.Int16)]
        [DataRow(typeof(sbyte?),
            OracleDbType.Int16)]
        [DataRow(typeof(short),
            OracleDbType.Int16)]
        [DataRow(typeof(short?),
            OracleDbType.Int16)]
        [DataRow(typeof(ushort),
            OracleDbType.Int32)]
        [DataRow(typeof(ushort?),
            OracleDbType.Int32)]
        [DataRow(typeof(int),
            OracleDbType.Int32)]
        [DataRow(typeof(int?),
            OracleDbType.Int32)]
        [DataRow(typeof(uint),
            OracleDbType.Int64)]
        [DataRow(typeof(uint?),
            OracleDbType.Int64)]
        [DataRow(typeof(long),
            OracleDbType.Int64)]
        [DataRow(typeof(long?),
            OracleDbType.Int64)]
        [DataRow(typeof(ulong),
            OracleDbType.Decimal)]
        [DataRow(typeof(ulong?),
            OracleDbType.Decimal)]
        [DataRow(typeof(float),
            OracleDbType.BinaryFloat)]
        [DataRow(typeof(float?),
            OracleDbType.BinaryFloat)]
        [DataRow(typeof(double),
            OracleDbType.BinaryDouble)]
        [DataRow(typeof(double?),
            OracleDbType.BinaryDouble)]
        [DataRow(typeof(decimal),
            OracleDbType.Decimal)]
        [DataRow(typeof(decimal?),
            OracleDbType.Decimal)]
        [DataRow(typeof(DateTime),
            OracleDbType.TimeStamp)]
        [DataRow(typeof(DateTime?),
            OracleDbType.TimeStamp)]
        [DataRow(typeof(DateTimeOffset),
            OracleDbType.TimeStampTZ)]
        [DataRow(typeof(DateTimeOffset?),
            OracleDbType.TimeStampTZ)]
        [DataRow(typeof(TimeSpan),
            OracleDbType.IntervalDS)]
        [DataRow(typeof(TimeSpan?),
            OracleDbType.IntervalDS)]
        [DataRow(typeof(Guid),
            OracleDbType.Raw)]
        [DataRow(typeof(Guid?),
            OracleDbType.Raw)]
        [DataRow(typeof(byte[]),
            OracleDbType.Blob)]
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderType(Type type,
            OracleDbType expected)
        {
            // Setup
            var resolver = new TypeToOracleDbTypeResolver();


            // Act
            var actual = resolver.Resolve(type);


            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Identifies the missing CLR type.
        /// </summary>
        [TestMethod]
        public void ResolveNullReportsArgumentName()
        {
            // Setup
            var resolver = new TypeToOracleDbTypeResolver();


            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));


            // Assert
            Assert.AreEqual("nullableType",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Uses the default representation for unknown types.
        /// </summary>
        [TestMethod]
        public void ResolveUnsupportedTypeUsesFallback()
        {
            // Setup
            var resolver = new TypeToOracleDbTypeResolver();


            // Act
            var actual = resolver.Resolve(typeof(Uri));


            // Assert
            Assert.AreEqual(OracleDbType.NVarchar2,
                actual);
        }

        #endregion
    }
}
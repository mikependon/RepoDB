using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Resolvers;
using System.Collections.Generic;
using System;

namespace RepoDb.Db2.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class TypeToDb2TypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="expected">The expected provider type.</param>
        [DataRow(typeof(string),
            DB2Type.VarChar)]
        [DataRow(typeof(char),
            DB2Type.Char)]
        [DataRow(typeof(char?),
            DB2Type.Char)]
        [DataRow(typeof(bool),
            DB2Type.SmallInt)]
        [DataRow(typeof(bool?),
            DB2Type.SmallInt)]
        [DataRow(typeof(byte),
            DB2Type.Byte)]
        [DataRow(typeof(byte?),
            DB2Type.Byte)]
        [DataRow(typeof(sbyte),
            DB2Type.SmallInt)]
        [DataRow(typeof(sbyte?),
            DB2Type.SmallInt)]
        [DataRow(typeof(short),
            DB2Type.SmallInt)]
        [DataRow(typeof(short?),
            DB2Type.SmallInt)]
        [DataRow(typeof(ushort),
            DB2Type.Integer)]
        [DataRow(typeof(ushort?),
            DB2Type.Integer)]
        [DataRow(typeof(int),
            DB2Type.Integer)]
        [DataRow(typeof(int?),
            DB2Type.Integer)]
        [DataRow(typeof(uint),
            DB2Type.BigInt)]
        [DataRow(typeof(uint?),
            DB2Type.BigInt)]
        [DataRow(typeof(long),
            DB2Type.BigInt)]
        [DataRow(typeof(long?),
            DB2Type.BigInt)]
        [DataRow(typeof(ulong),
            DB2Type.Decimal)]
        [DataRow(typeof(ulong?),
            DB2Type.Decimal)]
        [DataRow(typeof(float),
            DB2Type.Real)]
        [DataRow(typeof(float?),
            DB2Type.Real)]
        [DataRow(typeof(double),
            DB2Type.Double)]
        [DataRow(typeof(double?),
            DB2Type.Double)]
        [DataRow(typeof(decimal),
            DB2Type.Decimal)]
        [DataRow(typeof(decimal?),
            DB2Type.Decimal)]
        [DataRow(typeof(DateTime),
            DB2Type.Timestamp)]
        [DataRow(typeof(DateTime?),
            DB2Type.Timestamp)]
        [DataRow(typeof(DateTimeOffset),
            DB2Type.TimeStampWithTimeZone)]
        [DataRow(typeof(DateTimeOffset?),
            DB2Type.TimeStampWithTimeZone)]
        [DataRow(typeof(TimeSpan),
            DB2Type.Time)]
        [DataRow(typeof(TimeSpan?),
            DB2Type.Time)]
        [DataRow(typeof(Guid),
            DB2Type.Binary)]
        [DataRow(typeof(Guid?),
            DB2Type.Binary)]
        [DataRow(typeof(byte[]),
            DB2Type.Blob)]
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderType(Type type,
            DB2Type expected)
        {
            // Setup
            var resolver = new TypeToDb2TypeResolver();


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
            var resolver = new TypeToDb2TypeResolver();


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
            var resolver = new TypeToDb2TypeResolver();


            // Act
            var actual = resolver.Resolve(typeof(Uri));


            // Assert
            Assert.AreEqual(DB2Type.VarChar,
                actual);
        }

        #endregion
    }
}
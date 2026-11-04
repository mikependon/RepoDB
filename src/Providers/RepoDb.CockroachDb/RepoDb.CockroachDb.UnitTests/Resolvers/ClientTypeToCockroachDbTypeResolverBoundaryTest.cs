using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Resolvers;
using System;

namespace RepoDb.CockroachDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToCockroachDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="expected">The expected provider type.</param>
        [DataRow(typeof(Boolean),
            CockroachDbType.Boolean)]
        [DataRow(typeof(Byte[]),
            CockroachDbType.Bytea)]
        [DataRow(typeof(Char),
            CockroachDbType.Char)]
        [DataRow(typeof(System.Collections.BitArray),
            CockroachDbType.Bit)]
        [DataRow(typeof(DateTime),
            CockroachDbType.Timestamp)]
        [DataRow(typeof(DateTimeOffset),
            CockroachDbType.TimestampTz)]
        [DataRow(typeof(DateOnly),
            CockroachDbType.Date)]
        [DataRow(typeof(TimeOnly),
            CockroachDbType.Time)]
        [DataRow(typeof(Decimal),
            CockroachDbType.Decimal)]
        [DataRow(typeof(Double),
            CockroachDbType.Double)]
        [DataRow(typeof(Guid),
            CockroachDbType.Uuid)]
        [DataRow(typeof(Int16),
            CockroachDbType.SmallInt)]
        [DataRow(typeof(Int32),
            CockroachDbType.Integer)]
        [DataRow(typeof(Int64),
            CockroachDbType.BigInt)]
        [DataRow(typeof(System.Net.IPAddress),
            CockroachDbType.Inet)]
        [DataRow(typeof(Single),
            CockroachDbType.Real)]
        [DataRow(typeof(String),
            CockroachDbType.Text)]
        [DataRow(typeof(TimeSpan),
            CockroachDbType.Interval)]
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderType(Type type,
            CockroachDbType expected)
        {
            // Setup
            var resolver = new ClientTypeToCockroachDbTypeResolver();


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
            var resolver = new ClientTypeToCockroachDbTypeResolver();


            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));


            // Assert
            Assert.AreEqual("type",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects types without a provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveUnsupportedTypeThrows()
        {
            // Setup
            var resolver = new ClientTypeToCockroachDbTypeResolver();


            // Act
            Action action = () => resolver.Resolve(typeof(Uri));


            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}
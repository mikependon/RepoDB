using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Resolvers;
using System;

namespace RepoDb.EnterpriseDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToEDBDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="expected">The expected provider type.</param>
        [DataRow(typeof(Boolean),
            EDBType.Boolean)]
        [DataRow(typeof(Byte[]),
            EDBType.Bytea)]
        [DataRow(typeof(Char),
            EDBType.Char)]
        [DataRow(typeof(System.Collections.BitArray),
            EDBType.Bit)]
        [DataRow(typeof(DateTime),
            EDBType.Timestamp)]
        [DataRow(typeof(DateTimeOffset),
            EDBType.TimestampTz)]
        [DataRow(typeof(DateOnly),
            EDBType.Date)]
        [DataRow(typeof(TimeOnly),
            EDBType.Time)]
        [DataRow(typeof(Decimal),
            EDBType.Money)]
        [DataRow(typeof(Double),
            EDBType.Double)]
        [DataRow(typeof(Guid),
            EDBType.Uuid)]
        [DataRow(typeof(Int16),
            EDBType.SmallInt)]
        [DataRow(typeof(Int32),
            EDBType.Integer)]
        [DataRow(typeof(Int64),
            EDBType.BigInt)]
        [DataRow(typeof(System.Net.IPAddress),
            EDBType.Inet)]
        [DataRow(typeof(System.Net.NetworkInformation.PhysicalAddress),
            EDBType.MacAddr)]
        [DataRow(typeof(Single),
            EDBType.Real)]
        [DataRow(typeof(String),
            EDBType.Char)]
        [DataRow(typeof(TimeSpan),
            EDBType.Interval)]
        [DataRow(typeof(ValueTuple<System.Net.IPAddress, Int32>),
            EDBType.Cidr)]
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderType(Type type,
            EDBType expected)
        {
            // Setup
            var resolver = new ClientTypeToEDBDbTypeResolver();


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
            var resolver = new ClientTypeToEDBDbTypeResolver();


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
            var resolver = new ClientTypeToEDBDbTypeResolver();


            // Act
            Action action = () => resolver.Resolve(typeof(Uri));


            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}
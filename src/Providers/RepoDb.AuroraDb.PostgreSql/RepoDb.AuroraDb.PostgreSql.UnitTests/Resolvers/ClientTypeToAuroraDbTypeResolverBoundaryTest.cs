using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Resolvers;
using System.Collections.Generic;
using System;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToAuroraDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="expected">The expected provider type.</param>
        [DataRow(typeof(NpgsqlBox),
            AuroraDbType.Box)]
        [DataRow(typeof(NpgsqlCircle),
            AuroraDbType.Circle)]
        [DataRow(typeof(NpgsqlLine),
            AuroraDbType.Line)]
        [DataRow(typeof(NpgsqlLSeg),
            AuroraDbType.LSeg)]
        [DataRow(typeof(NpgsqlPath),
            AuroraDbType.Path)]
        [DataRow(typeof(NpgsqlPoint),
            AuroraDbType.Point)]
        [DataRow(typeof(NpgsqlPolygon),
            AuroraDbType.Polygon)]
        [DataRow(typeof(NpgsqlCidr),
            AuroraDbType.Cidr)]
        [DataRow(typeof(NpgsqlRange<Int32>),
            AuroraDbType.IntegerRange)]
        [DataRow(typeof(NpgsqlRange<Int64>),
            AuroraDbType.BigIntRange)]
        [DataRow(typeof(NpgsqlRange<Decimal>),
            AuroraDbType.NumericRange)]
        [DataRow(typeof(NpgsqlRange<DateOnly>),
            AuroraDbType.DateRange)]
        [DataRow(typeof(NpgsqlTsQuery),
            AuroraDbType.TsQuery)]
        [DataRow(typeof(NpgsqlTsVector),
            AuroraDbType.TsVector)]
        [DataRow(typeof(System.Net.NetworkInformation.PhysicalAddress),
            AuroraDbType.MacAddr)]
        [DataRow(typeof(Dictionary<String, String>),
            AuroraDbType.Hstore)]
        [DataRow(typeof(Boolean),
            AuroraDbType.Boolean)]
        [DataRow(typeof(Byte[]),
            AuroraDbType.Bytea)]
        [DataRow(typeof(Char),
            AuroraDbType.Char)]
        [DataRow(typeof(System.Collections.BitArray),
            AuroraDbType.Bit)]
        [DataRow(typeof(DateTime),
            AuroraDbType.Timestamp)]
        [DataRow(typeof(DateTimeOffset),
            AuroraDbType.TimestampTz)]
        [DataRow(typeof(DateOnly),
            AuroraDbType.Date)]
        [DataRow(typeof(TimeOnly),
            AuroraDbType.Time)]
        [DataRow(typeof(Decimal),
            AuroraDbType.Decimal)]
        [DataRow(typeof(Double),
            AuroraDbType.Double)]
        [DataRow(typeof(Guid),
            AuroraDbType.Uuid)]
        [DataRow(typeof(Int16),
            AuroraDbType.SmallInt)]
        [DataRow(typeof(Int32),
            AuroraDbType.Integer)]
        [DataRow(typeof(Int64),
            AuroraDbType.BigInt)]
        [DataRow(typeof(System.Net.IPAddress),
            AuroraDbType.Inet)]
        [DataRow(typeof(Single),
            AuroraDbType.Real)]
        [DataRow(typeof(String),
            AuroraDbType.Text)]
        [DataRow(typeof(TimeSpan),
            AuroraDbType.Interval)]
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderType(Type type,
            AuroraDbType expected)
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbTypeResolver();


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
            var resolver = new ClientTypeToAuroraDbTypeResolver();


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
            var resolver = new ClientTypeToAuroraDbTypeResolver();


            // Act
            Action action = () => resolver.Resolve(typeof(Uri));


            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}
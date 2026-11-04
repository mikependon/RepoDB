using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToNpgsqlDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="expected">The expected provider type.</param>
        [DataRow(typeof(NpgsqlBox),
            NpgsqlDbType.Box)]
        [DataRow(typeof(NpgsqlCircle),
            NpgsqlDbType.Circle)]
        [DataRow(typeof(NpgsqlLine),
            NpgsqlDbType.Line)]
        [DataRow(typeof(NpgsqlLogSequenceNumber),
            NpgsqlDbType.PgLsn)]
        [DataRow(typeof(NpgsqlLSeg),
            NpgsqlDbType.LSeg)]
        [DataRow(typeof(NpgsqlPath),
            NpgsqlDbType.Path)]
        [DataRow(typeof(NpgsqlPoint),
            NpgsqlDbType.Point)]
        [DataRow(typeof(NpgsqlPolygon),
            NpgsqlDbType.Polygon)]
        [DataRow(typeof(NpgsqlRange<DateTime>),
            NpgsqlDbType.Unknown)]
        [DataRow(typeof(NpgsqlRange<Decimal>),
            NpgsqlDbType.Unknown)]
        [DataRow(typeof(NpgsqlRange<Int32>),
            NpgsqlDbType.Unknown)]
        [DataRow(typeof(NpgsqlRange<Int64>),
            NpgsqlDbType.Unknown)]
        [DataRow(typeof(NpgsqlTid),
            NpgsqlDbType.Tid)]
        [DataRow(typeof(NpgsqlTsQuery),
            NpgsqlDbType.TsQuery)]
        [DataRow(typeof(NpgsqlTsVector),
            NpgsqlDbType.TsVector)]
        [DataRow(typeof(Array),
            NpgsqlDbType.Unknown)]
        [DataRow(typeof(Boolean),
            NpgsqlDbType.Boolean)]
        [DataRow(typeof(Byte[]),
            NpgsqlDbType.Bytea)]
        [DataRow(typeof(Char),
            NpgsqlDbType.InternalChar)]
        [DataRow(typeof(System.Collections.BitArray),
            NpgsqlDbType.Bit)]
        [DataRow(typeof(DateTime),
            NpgsqlDbType.Timestamp)]
        [DataRow(typeof(DateTimeOffset),
            NpgsqlDbType.TimestampTz)]
        [DataRow(typeof(DateOnly),
            NpgsqlDbType.Date)]
        [DataRow(typeof(TimeOnly),
            NpgsqlDbType.Time)]
        [DataRow(typeof(Decimal),
            NpgsqlDbType.Money)]
        [DataRow(typeof(Double),
            NpgsqlDbType.Double)]
        [DataRow(typeof(Guid),
            NpgsqlDbType.Uuid)]
        [DataRow(typeof(Int16),
            NpgsqlDbType.Smallint)]
        [DataRow(typeof(Int32),
            NpgsqlDbType.Integer)]
        [DataRow(typeof(Int64),
            NpgsqlDbType.Bigint)]
        [DataRow(typeof(System.Net.IPAddress),
            NpgsqlDbType.Inet)]
        [DataRow(typeof(System.Net.NetworkInformation.PhysicalAddress),
            NpgsqlDbType.MacAddr)]
        [DataRow(typeof(Single),
            NpgsqlDbType.Real)]
        [DataRow(typeof(String),
            NpgsqlDbType.Char)]
        [DataRow(typeof(TimeSpan),
            NpgsqlDbType.Interval)]
        [DataRow(typeof(UInt32),
            NpgsqlDbType.Cid)]
        [DataRow(typeof(ValueTuple<System.Net.IPAddress, Int32>),
            NpgsqlDbType.Cidr)]
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderType(Type type,
            NpgsqlDbType expected)
        {
            // Setup
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();


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
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();


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
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();


            // Act
            Action action = () => resolver.Resolve(typeof(Uri));


            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}
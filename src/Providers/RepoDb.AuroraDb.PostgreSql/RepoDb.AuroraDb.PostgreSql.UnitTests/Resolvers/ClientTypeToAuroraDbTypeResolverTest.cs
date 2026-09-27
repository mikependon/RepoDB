#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Resolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToAuroraDbTypeResolverTest
    {
        #region Positive

        [TestMethod]
        [DataRow(typeof(bool), AuroraDbType.Boolean)]
        [DataRow(typeof(byte[]), AuroraDbType.Bytea)]
        [DataRow(typeof(char), AuroraDbType.Char)]
        [DataRow(typeof(BitArray), AuroraDbType.Bit)]
        [DataRow(typeof(DateTime), AuroraDbType.Timestamp)]
        [DataRow(typeof(DateTimeOffset), AuroraDbType.TimestampTz)]
        [DataRow(typeof(DateOnly), AuroraDbType.Date)]
        [DataRow(typeof(TimeOnly), AuroraDbType.Time)]
        [DataRow(typeof(decimal), AuroraDbType.Decimal)]
        [DataRow(typeof(double), AuroraDbType.Double)]
        [DataRow(typeof(Guid), AuroraDbType.Uuid)]
        [DataRow(typeof(short), AuroraDbType.SmallInt)]
        [DataRow(typeof(int), AuroraDbType.Integer)]
        [DataRow(typeof(long), AuroraDbType.BigInt)]
        [DataRow(typeof(IPAddress), AuroraDbType.Inet)]
        [DataRow(typeof(float), AuroraDbType.Real)]
        [DataRow(typeof(string), AuroraDbType.Text)]
        [DataRow(typeof(TimeSpan), AuroraDbType.Interval)]
        public void TestClientTypeToAuroraDbTypeResolverForScalarTypes(Type type,
            AuroraDbType expected)
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbTypeResolver();

            // Act
            var result = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(typeof(NpgsqlBox), AuroraDbType.Box)]
        [DataRow(typeof(NpgsqlCircle), AuroraDbType.Circle)]
        [DataRow(typeof(NpgsqlLine), AuroraDbType.Line)]
        [DataRow(typeof(NpgsqlLSeg), AuroraDbType.LSeg)]
        [DataRow(typeof(NpgsqlPath), AuroraDbType.Path)]
        [DataRow(typeof(NpgsqlPoint), AuroraDbType.Point)]
        [DataRow(typeof(NpgsqlPolygon), AuroraDbType.Polygon)]
        [DataRow(typeof(NpgsqlCidr), AuroraDbType.Cidr)]
        [DataRow(typeof(NpgsqlRange<int>), AuroraDbType.IntegerRange)]
        [DataRow(typeof(NpgsqlRange<long>), AuroraDbType.BigIntRange)]
        [DataRow(typeof(NpgsqlRange<decimal>), AuroraDbType.NumericRange)]
        [DataRow(typeof(NpgsqlRange<DateOnly>), AuroraDbType.DateRange)]
        [DataRow(typeof(NpgsqlTsQuery), AuroraDbType.TsQuery)]
        [DataRow(typeof(NpgsqlTsVector), AuroraDbType.TsVector)]
        [DataRow(typeof(PhysicalAddress), AuroraDbType.MacAddr)]
        [DataRow(typeof(Dictionary<string, string>), AuroraDbType.Hstore)]
        public void TestClientTypeToAuroraDbTypeResolverForPostgreSqlTypes(Type type,
            AuroraDbType expected)
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbTypeResolver();

            // Act
            var result = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Negative

        [TestMethod]
        public void ThrowExceptionOnClientTypeToAuroraDbTypeResolverIfTheTypeIsNull()
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbTypeResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }

        [TestMethod]
        [DataRow(typeof(object))]
        [DataRow(typeof(int[]))]
        [DataRow(typeof(NpgsqlRange<DateTime>))]
        [DataRow(typeof(Uri))]
        public void ThrowExceptionOnClientTypeToAuroraDbTypeResolverIfTheTypeIsNotResolvable(Type type)
        {
            // Setup
            var resolver = new ClientTypeToAuroraDbTypeResolver();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => resolver.Resolve(type));
        }

        #endregion
    }
}

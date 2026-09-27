#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Resolvers;
using System;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class AuroraDbDbTypeNameToAuroraDbTypeResolverTest
    {
        #region Positive

        [TestMethod]
        [DataRow("bigint", AuroraDbType.BigInt)]
        [DataRow("integer", AuroraDbType.Integer)]
        [DataRow("smallint", AuroraDbType.SmallInt)]
        [DataRow("boolean", AuroraDbType.Boolean)]
        [DataRow("bytea", AuroraDbType.Bytea)]
        [DataRow("text", AuroraDbType.Text)]
        [DataRow("uuid", AuroraDbType.Uuid)]
        [DataRow("jsonb", AuroraDbType.Jsonb)]
        [DataRow("json", AuroraDbType.Json)]
        [DataRow("xml", AuroraDbType.Xml)]
        [DataRow("money", AuroraDbType.Money)]
        [DataRow("inet", AuroraDbType.Inet)]
        [DataRow("cidr", AuroraDbType.Cidr)]
        [DataRow("macaddr", AuroraDbType.MacAddr)]
        [DataRow("point", AuroraDbType.Point)]
        [DataRow("polygon", AuroraDbType.Polygon)]
        [DataRow("int4range", AuroraDbType.IntegerRange)]
        [DataRow("int8range", AuroraDbType.BigIntRange)]
        [DataRow("numrange", AuroraDbType.NumericRange)]
        [DataRow("daterange", AuroraDbType.DateRange)]
        [DataRow("tsrange", AuroraDbType.TimestampRange)]
        [DataRow("tstzrange", AuroraDbType.TimestampTzRange)]
        [DataRow("tsvector", AuroraDbType.TsVector)]
        [DataRow("tsquery", AuroraDbType.TsQuery)]
        [DataRow("geometry", AuroraDbType.Geometry)]
        [DataRow("geography", AuroraDbType.Geography)]
        [DataRow("character varying", AuroraDbType.Text)]
        [DataRow("double precision", AuroraDbType.Double)]
        [DataRow("timestamp without time zone", AuroraDbType.Timestamp)]
        [DataRow("timestamp with time zone", AuroraDbType.TimestampTz)]
        [DataRow("date", AuroraDbType.Date)]
        [DataRow("interval", AuroraDbType.Interval)]
        public void TestAuroraDbDbTypeNameToAuroraDbTypeResolver(string dbTypeName,
            AuroraDbType expected)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToAuroraDbTypeResolver();

            // Act
            var result = resolver.Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("USER-DEFINED")]
        [DataRow("ARRAY")]
        [DataRow("unknown_type")]
        public void TestAuroraDbDbTypeNameToAuroraDbTypeResolverForUnresolvableTypes(string dbTypeName)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToAuroraDbTypeResolver();

            // Act
            var result = resolver.Resolve(dbTypeName);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Negative

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void ThrowExceptionOnAuroraDbDbTypeNameToAuroraDbTypeResolverIfTheNameIsNullOrWhitespace(string dbTypeName)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToAuroraDbTypeResolver();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(dbTypeName));
        }

        #endregion
    }
}

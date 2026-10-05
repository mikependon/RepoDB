#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Schema.CockroachDb.UnitTests.Resolvers
{
    [TestClass]
    public class CockroachDbTypeNameResolversTest
    {
        #region DbTypeNameToCockroachDbTypeNameResolver

        [TestMethod]
        [DataRow("nvarchar", "varchar")]
        [DataRow("character varying", "varchar")]
        [DataRow("nchar", "char")]
        [DataRow("character", "char")]
        [DataRow("ntext", "text")]
        [DataRow("bit", "boolean")]
        [DataRow("int", "integer")]
        [DataRow("int8", "bigint")]
        [DataRow("tinyint", "smallint")]
        [DataRow("float", "double precision")]
        [DataRow("float4", "real")]
        [DataRow("money", "numeric")]
        [DataRow("uniqueidentifier", "uuid")]
        [DataRow("varbinary", "bytea")]
        [DataRow("datetime2", "timestamp")]
        [DataRow("timestamp without time zone", "timestamp")]
        [DataRow("datetimeoffset", "timestamptz")]
        [DataRow("timestamp with time zone", "timestamptz")]
        [DataRow("time with time zone", "timetz")]
        [DataRow("sql_variant", "text")]
        [DataRow("  INTEGER ", "integer")]
        [DataRow("jsonb", "jsonb")]
        public void TestDbTypeNameToCockroachDbTypeNameResolver(string dbTypeName, string expected)
        {
            // Act
            var actual = new DbTypeNameToCockroachDbTypeNameResolver().Resolve(dbTypeName);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDbTypeNameToCockroachDbTypeNameResolverWithBlankName()
        {
            // Assert
            Assert.IsNull(new DbTypeNameToCockroachDbTypeNameResolver().Resolve(null));
            Assert.IsNull(new DbTypeNameToCockroachDbTypeNameResolver().Resolve(" "));
        }

        #endregion

        #region ClientTypeToCockroachDbTypeNameResolver

        [TestMethod]
        [DataRow(typeof(string), "varchar")]
        [DataRow(typeof(int), "integer")]
        [DataRow(typeof(int?), "integer")]
        [DataRow(typeof(long), "bigint")]
        [DataRow(typeof(short), "smallint")]
        [DataRow(typeof(byte), "smallint")]
        [DataRow(typeof(bool), "boolean")]
        [DataRow(typeof(decimal), "numeric")]
        [DataRow(typeof(double), "double precision")]
        [DataRow(typeof(float), "real")]
        [DataRow(typeof(DateTime), "timestamp")]
        [DataRow(typeof(DateTimeOffset), "timestamptz")]
        [DataRow(typeof(TimeSpan), "time")]
        [DataRow(typeof(Guid), "uuid")]
        [DataRow(typeof(byte[]), "bytea")]
        [DataRow(typeof(object), "text")]
        public void TestClientTypeToCockroachDbTypeNameResolver(Type type, string expected)
        {
            // Act
            var actual = new ClientTypeToCockroachDbTypeNameResolver().Resolve(type);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnClientTypeToCockroachDbTypeNameResolverIfTheTypeIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new ClientTypeToCockroachDbTypeNameResolver().Resolve(null));
        }

        #endregion
    }
}

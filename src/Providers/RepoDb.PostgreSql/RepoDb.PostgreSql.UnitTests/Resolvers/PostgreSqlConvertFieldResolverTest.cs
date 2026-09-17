#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class PostgreSqlConvertFieldResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForInt32()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(int));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS INTEGER)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForInt64()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(long));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS BIGINT)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForInt16()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(short));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS SMALLINT)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForDateTime()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(DateTime));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS TIMESTAMP)", result, StringComparer.Ordinal);
        }
        
        [TestMethod]
        public void TestSqLiteConvertFieldResolverForDateTimeOffset()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(DateTimeOffset));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS TIMESTAMPTZ)", result, StringComparer.Ordinal);
        }


        [TestMethod]
        public void TestSqLiteConvertFieldResolverForString()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(string));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS TEXT)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForByte()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(byte));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS BYTEA)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForDecimal()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(decimal));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS NUMERIC)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForFloat()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(float));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS REAL)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForTimeSpan()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(TimeSpan));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS INTERVAL)", result, StringComparer.Ordinal);
        }
#if NET6_0_OR_GREATER
        [TestMethod]
        public void TestSqLiteConvertFieldResolverForDate()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(DateOnly));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS DATE)", result, StringComparer.Ordinal);
        }
        [TestMethod]
        public void TestSqLiteConvertFieldResolverForTime()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();
            var resolver = new PostgreSqlConvertFieldResolver();
            var field = new Field("Field", typeof(TimeOnly));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST(\"Field\" AS INTERVAL)", result, StringComparer.Ordinal);
        }
#endif

    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Resolvers;

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
            Assert.AreEqual("CAST(\"Field\" AS INTEGER)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS BIGINT)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS SMALLINT)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS TIMESTAMP)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS TIMESTAMPTZ)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS TEXT)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS BYTEA)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS NUMERIC)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS REAL)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS INTERVAL)", result);
        }
#if NET
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
            Assert.AreEqual("CAST(\"Field\" AS DATE)", result);
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
            Assert.AreEqual("CAST(\"Field\" AS INTERVAL)", result);
        }
#endif

    }
}

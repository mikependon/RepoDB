using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.SqLite.UnitTests.Resolvers
{
    [TestClass]
    public class SqLiteConvertFieldResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForInt32()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(int));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [INT])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForInt64()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(long));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [BIGINT])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForInt16()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(short));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [INT])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForDateTime()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(DateTime));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [DATETIME])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForString()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(string));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [TEXT])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForByte()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(byte));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [BLOB])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForDecimal()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(decimal));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [DECIMAL])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForFloat()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(float));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [REAL])", result);
        }

        [TestMethod]
        public void TestSqLiteConvertFieldResolverForTimeSpan()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new SqLiteConvertFieldResolver();
            var field = new Field("Field", typeof(TimeSpan));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [TIME])", result);
        }
    }
}

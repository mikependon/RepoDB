using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;
using System.Data;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToClientTypeResolverTest
    {
        private readonly DbTypeToClientTypeResolver m_resolver = new DbTypeToClientTypeResolver();

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeInt64()
        {
            // Setup
            var dbType = DbType.Int64;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(long);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeBinary()
        {
            // Setup
            var dbType = DbType.Binary;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(byte[]);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeByte()
        {
            // Setup
            var dbType = DbType.Byte;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(byte[]);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeBoolean()
        {
            // Setup
            var dbType = DbType.Boolean;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(bool);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeString()
        {
            // Setup
            var dbType = DbType.String;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(string);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeAnsiString()
        {
            // Setup
            var dbType = DbType.AnsiString;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(string);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeAnsiStringFixedLength()
        {
            // Setup
            var dbType = DbType.AnsiStringFixedLength;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(string);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeStringFixedLength()
        {
            // Setup
            var dbType = DbType.StringFixedLength;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(string);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeDate()
        {
            // Setup
            var dbType = DbType.Date;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(DateTime);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeDateTime()
        {
            // Setup
            var dbType = DbType.DateTime;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(DateTime);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeDateTime2()
        {
            // Setup
            var dbType = DbType.DateTime2;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(DateTime);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeDateTimeOffset()
        {
            // Setup
            var dbType = DbType.DateTimeOffset;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(DateTimeOffset);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeDecimal()
        {
            // Setup
            var dbType = DbType.Decimal;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(decimal);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeSingle()
        {
            // Setup
            var dbType = DbType.Single;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(Single);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeDouble()
        {
            // Setup
            var dbType = DbType.Double;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(double);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeInt32()
        {
            // Setup
            var dbType = DbType.Int32;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(int);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeInt16()
        {
            // Setup
            var dbType = DbType.Int16;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(short);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeTime()
        {
            // Setup
            var dbType = DbType.Time;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(TimeSpan);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeGuid()
        {
            // Setup
            var dbType = DbType.Guid;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(Guid);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbTypeToClientTypeResolverTestForDbTypeObject()
        {
            // Setup
            var dbType = DbType.Object;

            // Act
            var result = m_resolver.Resolve(dbType);
            var expected = typeof(object);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Resolvers;

namespace RepoDb.MariaDb.UnitTests.Resolvers
{
    [TestClass]
    public class MariaDbDbTypeToMySqlStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Binary);

            // Assert
            Assert.AreEqual("BINARY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForBit()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Bit);

            // Assert
            Assert.AreEqual("BIT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Blob);

            // Assert
            Assert.AreEqual("BLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForByte()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Byte);

            // Assert
            Assert.AreEqual("TINYINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForUByte()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UByte);

            // Assert
            Assert.AreEqual("TINYINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForDate()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Date);

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.DateTime);

            // Assert
            Assert.AreEqual("DATETIME", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForDouble()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForEnum()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Enum);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForGuid()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Guid);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForSet()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Set);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Text);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForFloat()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Float);

            // Assert
            Assert.AreEqual("FLOAT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForGeometry()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Geometry);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForInt16()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int16);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForInt24()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int24);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForUInt24()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt24);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForUInt16()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt16);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForInt32()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int32);

            // Assert
            Assert.AreEqual("INT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForUInt32()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt32);

            // Assert
            Assert.AreEqual("INT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForInt64()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int64);

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForUInt64()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt64);

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForJson()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.JSON);

            // Assert
            Assert.AreEqual("JSON", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForLongBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.LongBlob);

            // Assert
            Assert.AreEqual("LONGBLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForLongText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.LongText);

            // Assert
            Assert.AreEqual("LONGTEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForMediumBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.MediumBlob);

            // Assert
            Assert.AreEqual("MEDIUMBLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForMediumText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.MediumText);

            // Assert
            Assert.AreEqual("MEDIUMTEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForNewdate()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Newdate);

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForNewDecimal()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.NewDecimal);

            // Assert
            Assert.AreEqual("DECIMAL", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForString()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.String);

            // Assert
            Assert.AreEqual("STRING", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Time);

            // Assert
            Assert.AreEqual("TIME", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForTimestamp()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Timestamp);

            // Assert
            Assert.AreEqual("TIMESTAMP", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForTinyBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.TinyBlob);

            // Assert
            Assert.AreEqual("TINYBLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForTinyText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.TinyText);

            // Assert
            Assert.AreEqual("TINYTEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForVarBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarBinary);

            // Assert
            Assert.AreEqual("VARBINARY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForVarChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarChar);

            // Assert
            Assert.AreEqual("VARCHAR", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForVarString()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarString);

            // Assert
            Assert.AreEqual("VARCHAR", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToMySqlStringNameResolverForYear()
        {
            // Setup
            var resolver = new MariaDbDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Year);

            // Assert
            Assert.AreEqual("YEAR", result);
        }
    }
}

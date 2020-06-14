using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Resolvers;

namespace RepoDb.MySqlConnector.UnitTests.Resolvers
{
    [TestClass]
    public class MySqlDbTypeToMySqlStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            MySqlConnectorBootstrap.Initialize();
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForBinary()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Binary);

            // Assert
            Assert.AreEqual("BINARY", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForBit()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Bit);

            // Assert
            Assert.AreEqual("BIT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Blob);

            // Assert
            Assert.AreEqual("BLOB", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForByte()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Byte);

            // Assert
            Assert.AreEqual("TINYINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUByte()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UByte);

            // Assert
            Assert.AreEqual("TINYINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDate()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Date);

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.DateTime);

            // Assert
            Assert.AreEqual("DATETIME", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForDouble()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForEnum()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Enum);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForGuid()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Guid);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForSet()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Set);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Text);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForFloat()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Float);

            // Assert
            Assert.AreEqual("FLOAT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForGeometry()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Geometry);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt16()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int16);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt24()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int24);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt24()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt24);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt16()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt16);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt32()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int32);

            // Assert
            Assert.AreEqual("INT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt32()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt32);

            // Assert
            Assert.AreEqual("INT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForInt64()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Int64);

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForUInt64()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.UInt64);

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForJson()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.JSON);

            // Assert
            Assert.AreEqual("JSON", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForLongBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.LongBlob);

            // Assert
            Assert.AreEqual("LONGBLOB", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForLongText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.LongText);

            // Assert
            Assert.AreEqual("LONGTEXT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForMediumBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.MediumBlob);

            // Assert
            Assert.AreEqual("MEDIUMBLOB", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForMediumText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.MediumText);

            // Assert
            Assert.AreEqual("MEDIUMTEXT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForNewdate()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Newdate);

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForNewDecimal()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.NewDecimal);

            // Assert
            Assert.AreEqual("DECIMAL", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForString()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.String);

            // Assert
            Assert.AreEqual("STRING", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTime()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Time);

            // Assert
            Assert.AreEqual("TIME", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTimestamp()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Timestamp);

            // Assert
            Assert.AreEqual("TIMESTAMP", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTinyBlob()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.TinyBlob);

            // Assert
            Assert.AreEqual("TINYBLOB", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForTinyText()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.TinyText);

            // Assert
            Assert.AreEqual("TINYTEXT", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForVarBinary()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarBinary);

            // Assert
            Assert.AreEqual("VARBINARY", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForVarChar()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarChar);

            // Assert
            Assert.AreEqual("VARCHAR", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForVarString()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.VarString);

            // Assert
            Assert.AreEqual("VARCHAR", result);
        }

        [TestMethod]
        public void TestMySqlDbTypeToMySqlStringNameResolverForYear()
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();

            // Act
            var result = resolver.Resolve(MySqlDbType.Year);

            // Assert
            Assert.AreEqual("YEAR", result);
        }
    }
}

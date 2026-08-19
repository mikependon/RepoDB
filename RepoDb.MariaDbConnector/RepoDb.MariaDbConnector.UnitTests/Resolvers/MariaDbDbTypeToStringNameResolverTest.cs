using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDbConnector;
using RepoDb.Resolvers;

namespace RepoDb.MariaDb.UnitTests.Resolvers
{
    [TestClass]
    public class MariaDbDbTypeToStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTinyInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.TinyInt);

            // Assert
            Assert.AreEqual("TINYINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForSmallInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.SmallInt);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMediumInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MediumInt);

            // Assert
            Assert.AreEqual("MEDIUMINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Int);

            // Assert
            Assert.AreEqual("INT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBigInt()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.BigInt);

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForFloat()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Float);

            // Assert
            Assert.AreEqual("FLOAT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDouble()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBit()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Bit);

            // Assert
            Assert.AreEqual("BIT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Char);

            // Assert
            Assert.AreEqual("CHAR", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForVarChar()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.VarChar);

            // Assert
            Assert.AreEqual("VARCHAR", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTinyText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.TinyText);

            // Assert
            Assert.AreEqual("TINYTEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Text);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForEnum()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Enum);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForSet()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Set);

            // Assert
            Assert.AreEqual("TEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMediumText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MediumText);

            // Assert
            Assert.AreEqual("MEDIUMTEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForLongText()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.LongText);

            // Assert
            Assert.AreEqual("LONGTEXT", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Binary);

            // Assert
            Assert.AreEqual("BINARY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForVarBinary()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.VarBinary);

            // Assert
            Assert.AreEqual("VARBINARY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTinyBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.TinyBlob);

            // Assert
            Assert.AreEqual("TINYBLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Blob);

            // Assert
            Assert.AreEqual("BLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMediumBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MediumBlob);

            // Assert
            Assert.AreEqual("MEDIUMBLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForLongBlob()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.LongBlob);

            // Assert
            Assert.AreEqual("LONGBLOB", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDate()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Date);

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Time);

            // Assert
            Assert.AreEqual("TIME", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.DateTime);

            // Assert
            Assert.AreEqual("DATETIME", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForTimestamp()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Timestamp);

            // Assert
            Assert.AreEqual("TIMESTAMP", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForYear()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Year);

            // Assert
            Assert.AreEqual("YEAR", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForJson()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Json);

            // Assert
            Assert.AreEqual("JSON", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForGeometry()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Geometry);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForPoint()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Point);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForLineString()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.LineString);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForPolygon()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.Polygon);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMultiPoint()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MultiPoint);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMultiLineString()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MultiLineString);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForMultiPolygon()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.MultiPolygon);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }

        [TestMethod]
        public void TestMariaDbDbTypeToStringNameResolverForGeometryCollection()
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();

            // Act
            var result = resolver.Resolve(MariaDbType.GeometryCollection);

            // Assert
            Assert.AreEqual("GEOMETRY", result);
        }
    }
}

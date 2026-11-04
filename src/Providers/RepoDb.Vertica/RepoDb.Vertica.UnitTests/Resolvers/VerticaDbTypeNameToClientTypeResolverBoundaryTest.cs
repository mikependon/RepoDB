using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Vertica.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class VerticaDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt()
        {
            // Setup
            string name = "int";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIntInUppercase()
        {
            // Setup
            string name = "INT";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInteger()
        {
            // Setup
            string name = "integer";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIntegerInUppercase()
        {
            // Setup
            string name = "INTEGER";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSmallint()
        {
            // Setup
            string name = "smallint";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSmallintInUppercase()
        {
            // Setup
            string name = "SMALLINT";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBigint()
        {
            // Setup
            string name = "bigint";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBigintInUppercase()
        {
            // Setup
            string name = "BIGINT";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt8()
        {
            // Setup
            string name = "int8";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt8InUppercase()
        {
            // Setup
            string name = "INT8";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTinyint()
        {
            // Setup
            string name = "tinyint";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTinyintInUppercase()
        {
            // Setup
            string name = "TINYINT";
            Type expected = typeof(long);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat()
        {
            // Setup
            string name = "float";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloatInUppercase()
        {
            // Setup
            string name = "FLOAT";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat8()
        {
            // Setup
            string name = "float8";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat8InUppercase()
        {
            // Setup
            string name = "FLOAT8";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDoublePrecision()
        {
            // Setup
            string name = "double precision";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDoublePrecisionInUppercase()
        {
            // Setup
            string name = "DOUBLE PRECISION";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForReal()
        {
            // Setup
            string name = "real";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForRealInUppercase()
        {
            // Setup
            string name = "REAL";
            Type expected = typeof(double);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNumeric()
        {
            // Setup
            string name = "numeric";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNumericInUppercase()
        {
            // Setup
            string name = "NUMERIC";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal()
        {
            // Setup
            string name = "decimal";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimalInUppercase()
        {
            // Setup
            string name = "DECIMAL";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNumber()
        {
            // Setup
            string name = "number";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNumberInUppercase()
        {
            // Setup
            string name = "NUMBER";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForMoney()
        {
            // Setup
            string name = "money";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForMoneyInUppercase()
        {
            // Setup
            string name = "MONEY";
            Type expected = typeof(decimal);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForChar()
        {
            // Setup
            string name = "char";
            Type expected = typeof(string);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForCharInUppercase()
        {
            // Setup
            string name = "CHAR";
            Type expected = typeof(string);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarchar()
        {
            // Setup
            string name = "varchar";
            Type expected = typeof(string);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarcharInUppercase()
        {
            // Setup
            string name = "VARCHAR";
            Type expected = typeof(string);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLongVarchar()
        {
            // Setup
            string name = "long varchar";
            Type expected = typeof(string);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLongVarcharInUppercase()
        {
            // Setup
            string name = "LONG VARCHAR";
            Type expected = typeof(string);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBinary()
        {
            // Setup
            string name = "binary";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBinaryInUppercase()
        {
            // Setup
            string name = "BINARY";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarbinary()
        {
            // Setup
            string name = "varbinary";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarbinaryInUppercase()
        {
            // Setup
            string name = "VARBINARY";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLongVarbinary()
        {
            // Setup
            string name = "long varbinary";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLongVarbinaryInUppercase()
        {
            // Setup
            string name = "LONG VARBINARY";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBytea()
        {
            // Setup
            string name = "bytea";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForByteaInUppercase()
        {
            // Setup
            string name = "BYTEA";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForRaw()
        {
            // Setup
            string name = "raw";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForRawInUppercase()
        {
            // Setup
            string name = "RAW";
            Type expected = typeof(byte[]);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBoolean()
        {
            // Setup
            string name = "boolean";
            Type expected = typeof(bool);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBooleanInUppercase()
        {
            // Setup
            string name = "BOOLEAN";
            Type expected = typeof(bool);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDate()
        {
            // Setup
            string name = "date";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDateInUppercase()
        {
            // Setup
            string name = "DATE";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTime()
        {
            // Setup
            string name = "time";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimeInUppercase()
        {
            // Setup
            string name = "TIME";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimeWithTimezone()
        {
            // Setup
            string name = "time with timezone";
            Type expected = typeof(DateTimeOffset);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimeWithTimezoneInUppercase()
        {
            // Setup
            string name = "TIME WITH TIMEZONE";
            Type expected = typeof(DateTimeOffset);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestamp()
        {
            // Setup
            string name = "timestamp";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampInUppercase()
        {
            // Setup
            string name = "TIMESTAMP";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetime()
        {
            // Setup
            string name = "datetime";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetimeInUppercase()
        {
            // Setup
            string name = "DATETIME";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSmalldatetime()
        {
            // Setup
            string name = "smalldatetime";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSmalldatetimeInUppercase()
        {
            // Setup
            string name = "SMALLDATETIME";
            Type expected = typeof(DateTime);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampWithTimezone()
        {
            // Setup
            string name = "timestamp with timezone";
            Type expected = typeof(DateTimeOffset);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampWithTimezoneInUppercase()
        {
            // Setup
            string name = "TIMESTAMP WITH TIMEZONE";
            Type expected = typeof(DateTimeOffset);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUuid()
        {
            // Setup
            string name = "uuid";
            Type expected = typeof(Guid);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUuidInUppercase()
        {
            // Setup
            string name = "UUID";
            Type expected = typeof(Guid);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNone()
        {
            // Setup
            string name = "none";
            Type expected = typeof(object);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNoneInUppercase()
        {
            // Setup
            string name = "NONE";
            Type expected = typeof(object);
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForEmptyString()
        {
            // Setup
            string name = "";
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForUnsupportedType()
        {
            // Setup
            string name = "__unsupported_type__";
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForWhitespace()
        {
            // Setup
            string name = "   ";
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Identifies the missing database type name.
        /// </summary>
        [TestMethod]
        public void ResolveNullReportsArgumentName()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));

            // Assert
            Assert.AreEqual("dbTypeName",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        #endregion
    }
}
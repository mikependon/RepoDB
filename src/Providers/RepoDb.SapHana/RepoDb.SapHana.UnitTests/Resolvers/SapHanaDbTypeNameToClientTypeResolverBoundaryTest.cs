using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.SapHana.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class SapHanaDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTinyint()
        {
            // Setup
            string name = "tinyint";
            Type expected = typeof(byte);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(byte);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(short);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(short);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(int);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(int);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForInt()
        {
            // Setup
            string name = "int";
            Type expected = typeof(int);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(int);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForSmalldecimal()
        {
            // Setup
            string name = "smalldecimal";
            Type expected = typeof(decimal);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForSmalldecimalInUppercase()
        {
            // Setup
            string name = "SMALLDECIMAL";
            Type expected = typeof(decimal);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(float);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(float);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(float);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(float);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForDouble()
        {
            // Setup
            string name = "double";
            Type expected = typeof(double);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForDoubleInUppercase()
        {
            // Setup
            string name = "DOUBLE";
            Type expected = typeof(double);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForNvarchar()
        {
            // Setup
            string name = "nvarchar";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForNvarcharInUppercase()
        {
            // Setup
            string name = "NVARCHAR";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForAlphanum()
        {
            // Setup
            string name = "alphanum";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForAlphanumInUppercase()
        {
            // Setup
            string name = "ALPHANUM";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForShorttext()
        {
            // Setup
            string name = "shorttext";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForShorttextInUppercase()
        {
            // Setup
            string name = "SHORTTEXT";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForNchar()
        {
            // Setup
            string name = "nchar";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForNcharInUppercase()
        {
            // Setup
            string name = "NCHAR";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForText()
        {
            // Setup
            string name = "text";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForTextInUppercase()
        {
            // Setup
            string name = "TEXT";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForClob()
        {
            // Setup
            string name = "clob";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForClobInUppercase()
        {
            // Setup
            string name = "CLOB";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForNclob()
        {
            // Setup
            string name = "nclob";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForNclobInUppercase()
        {
            // Setup
            string name = "NCLOB";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForString()
        {
            // Setup
            string name = "string";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForStringInUppercase()
        {
            // Setup
            string name = "STRING";
            Type expected = typeof(string);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(TimeSpan);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(TimeSpan);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForSeconddate()
        {
            // Setup
            string name = "seconddate";
            Type expected = typeof(DateTime);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForSeconddateInUppercase()
        {
            // Setup
            string name = "SECONDDATE";
            Type expected = typeof(DateTime);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForBlob()
        {
            // Setup
            string name = "blob";
            Type expected = typeof(byte[]);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForBlobInUppercase()
        {
            // Setup
            string name = "BLOB";
            Type expected = typeof(byte[]);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForBintext()
        {
            // Setup
            string name = "bintext";
            Type expected = typeof(byte[]);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
        public void ResolveSupportedNameReturnsExpectedTypeForBintextInUppercase()
        {
            // Setup
            string name = "BINTEXT";
            Type expected = typeof(byte[]);
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

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
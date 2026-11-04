using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Schema.ClickHouse.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class DbTypeNameToClickHouseTypeNameResolverCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForVarchar()
        {
            // Setup
            string name = "varchar";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForVarcharInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  VARCHAR  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForVarchar2()
        {
            // Setup
            string name = "varchar2";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForVarchar2InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  VARCHAR2  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNvarchar()
        {
            // Setup
            string name = "nvarchar";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNvarcharInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  NVARCHAR  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNvarchar2()
        {
            // Setup
            string name = "nvarchar2";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNvarchar2InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  NVARCHAR2  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForCharacterVarying()
        {
            // Setup
            string name = "character varying";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForCharacterVaryingInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  CHARACTER VARYING  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForString()
        {
            // Setup
            string name = "string";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForStringInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  STRING  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForText()
        {
            // Setup
            string name = "text";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTextInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TEXT  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTinytext()
        {
            // Setup
            string name = "tinytext";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTinytextInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TINYTEXT  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMediumtext()
        {
            // Setup
            string name = "mediumtext";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMediumtextInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  MEDIUMTEXT  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForLongtext()
        {
            // Setup
            string name = "longtext";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForLongtextInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  LONGTEXT  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNtext()
        {
            // Setup
            string name = "ntext";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNtextInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  NTEXT  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForChar()
        {
            // Setup
            string name = "char";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForCharInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  CHAR  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForCharacter()
        {
            // Setup
            string name = "character";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForCharacterInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  CHARACTER  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBpchar()
        {
            // Setup
            string name = "bpchar";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBpcharInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BPCHAR  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNchar()
        {
            // Setup
            string name = "nchar";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNcharInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  NCHAR  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForXml()
        {
            // Setup
            string name = "xml";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForXmlInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  XML  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForJson()
        {
            // Setup
            string name = "json";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForJsonInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  JSON  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForJsonb()
        {
            // Setup
            string name = "jsonb";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForJsonbInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  JSONB  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForClob()
        {
            // Setup
            string name = "clob";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForClobInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  CLOB  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSqlVariant()
        {
            // Setup
            string name = "sql_variant";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSqlVariantInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  SQL_VARIANT  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTime()
        {
            // Setup
            string name = "time";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimeInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIME  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimeWithoutTimeZone()
        {
            // Setup
            string name = "time without time zone";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimeWithoutTimeZoneInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIME WITHOUT TIME ZONE  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimeWithTimeZone()
        {
            // Setup
            string name = "time with time zone";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimeWithTimeZoneInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIME WITH TIME ZONE  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimetz()
        {
            // Setup
            string name = "timetz";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimetzInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIMETZ  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBytea()
        {
            // Setup
            string name = "bytea";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForByteaInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BYTEA  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForRaw()
        {
            // Setup
            string name = "raw";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForRawInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  RAW  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForImage()
        {
            // Setup
            string name = "image";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForImageInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  IMAGE  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBlob()
        {
            // Setup
            string name = "blob";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBlobInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BLOB  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTinyblob()
        {
            // Setup
            string name = "tinyblob";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTinyblobInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TINYBLOB  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMediumblob()
        {
            // Setup
            string name = "mediumblob";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMediumblobInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  MEDIUMBLOB  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForLongblob()
        {
            // Setup
            string name = "longblob";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForLongblobInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  LONGBLOB  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBinary()
        {
            // Setup
            string name = "binary";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBinaryInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BINARY  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForVarbinary()
        {
            // Setup
            string name = "varbinary";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForVarbinaryInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  VARBINARY  ";
            string expected = "String";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBool()
        {
            // Setup
            string name = "bool";
            string expected = "Bool";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBoolInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BOOL  ";
            string expected = "Bool";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBoolean()
        {
            // Setup
            string name = "boolean";
            string expected = "Bool";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBooleanInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BOOLEAN  ";
            string expected = "Bool";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBit()
        {
            // Setup
            string name = "bit";
            string expected = "Bool";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBitInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BIT  ";
            string expected = "Bool";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInt()
        {
            // Setup
            string name = "int";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForIntInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  INT  ";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInteger()
        {
            // Setup
            string name = "integer";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForIntegerInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  INTEGER  ";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInt4()
        {
            // Setup
            string name = "int4";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInt4InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  INT4  ";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMediumint()
        {
            // Setup
            string name = "mediumint";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMediumintInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  MEDIUMINT  ";
            string expected = "Int32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBigint()
        {
            // Setup
            string name = "bigint";
            string expected = "Int64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBigintInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BIGINT  ";
            string expected = "Int64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInt8()
        {
            // Setup
            string name = "int8";
            string expected = "Int64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInt8InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  INT8  ";
            string expected = "Int64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSmallint()
        {
            // Setup
            string name = "smallint";
            string expected = "Int16";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSmallintInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  SMALLINT  ";
            string expected = "Int16";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInt2()
        {
            // Setup
            string name = "int2";
            string expected = "Int16";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForInt2InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  INT2  ";
            string expected = "Int16";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTinyint()
        {
            // Setup
            string name = "tinyint";
            string expected = "Int8";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTinyintInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TINYINT  ";
            string expected = "Int8";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDecimal()
        {
            // Setup
            string name = "decimal";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDecimalInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  DECIMAL  ";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNumeric()
        {
            // Setup
            string name = "numeric";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNumericInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  NUMERIC  ";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNumber()
        {
            // Setup
            string name = "number";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNumberInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  NUMBER  ";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMoney()
        {
            // Setup
            string name = "money";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForMoneyInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  MONEY  ";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSmallmoney()
        {
            // Setup
            string name = "smallmoney";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSmallmoneyInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  SMALLMONEY  ";
            string expected = "Decimal";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForFloat()
        {
            // Setup
            string name = "float";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForFloatInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  FLOAT  ";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForFloat8()
        {
            // Setup
            string name = "float8";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForFloat8InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  FLOAT8  ";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDouble()
        {
            // Setup
            string name = "double";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDoubleInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  DOUBLE  ";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDoublePrecision()
        {
            // Setup
            string name = "double precision";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDoublePrecisionInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  DOUBLE PRECISION  ";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBinaryDouble()
        {
            // Setup
            string name = "binary_double";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBinaryDoubleInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BINARY_DOUBLE  ";
            string expected = "Float64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForFloat4()
        {
            // Setup
            string name = "float4";
            string expected = "Float32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForFloat4InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  FLOAT4  ";
            string expected = "Float32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForReal()
        {
            // Setup
            string name = "real";
            string expected = "Float32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForRealInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  REAL  ";
            string expected = "Float32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBinaryFloat()
        {
            // Setup
            string name = "binary_float";
            string expected = "Float32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForBinaryFloatInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  BINARY_FLOAT  ";
            string expected = "Float32";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForUuid()
        {
            // Setup
            string name = "uuid";
            string expected = "UUID";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForUuidInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  UUID  ";
            string expected = "UUID";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForUniqueidentifier()
        {
            // Setup
            string name = "uniqueidentifier";
            string expected = "UUID";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForUniqueidentifierInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  UNIQUEIDENTIFIER  ";
            string expected = "UUID";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDatetime()
        {
            // Setup
            string name = "datetime";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDatetimeInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  DATETIME  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDatetime2()
        {
            // Setup
            string name = "datetime2";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDatetime2InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  DATETIME2  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSmalldatetime()
        {
            // Setup
            string name = "smalldatetime";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForSmalldatetimeInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  SMALLDATETIME  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestamp()
        {
            // Setup
            string name = "timestamp";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestampInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIMESTAMP  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestampWithoutTimeZone()
        {
            // Setup
            string name = "timestamp without time zone";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestampWithoutTimeZoneInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIMESTAMP WITHOUT TIME ZONE  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDatetimeoffset()
        {
            // Setup
            string name = "datetimeoffset";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDatetimeoffsetInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  DATETIMEOFFSET  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestampWithTimeZone()
        {
            // Setup
            string name = "timestamp with time zone";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestampWithTimeZoneInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIMESTAMP WITH TIME ZONE  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestamptz()
        {
            // Setup
            string name = "timestamptz";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForTimestamptzInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  TIMESTAMPTZ  ";
            string expected = "DateTime64";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDate()
        {
            // Setup
            string name = "date";
            string expected = "Date";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForDateInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  DATE  ";
            string expected = "Date";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Preserves absent and blank database types.
        /// </summary>
        [TestMethod]
        public void ResolveBlankNameReturnsNullForNull()
        {
            // Setup
            string name = null;
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Preserves absent and blank database types.
        /// </summary>
        [TestMethod]
        public void ResolveBlankNameReturnsNullForEmptyString()
        {
            // Setup
            string name = "";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Preserves absent and blank database types.
        /// </summary>
        [TestMethod]
        public void ResolveBlankNameReturnsNullForTabWhitespace()
        {
            // Setup
            string name = " \t ";
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Allows an unmapped native database type.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNamePreservesNativeType()
        {
            // Setup
            var resolver = new DbTypeNameToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(" Custom_Type ");

            // Assert
            Assert.AreEqual("Custom_Type",
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}

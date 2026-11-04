using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Schema.Oracle.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class DbTypeNameToOracleTypeNameResolverCoverageTest
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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "varchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "nvarchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "nvarchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "nvarchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "nvarchar2";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "char";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "char";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "char";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "char";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "clob";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "clob";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "clob";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "clob";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(1)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(1)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(1)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(1)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(1)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(1)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(10)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(19)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(19)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(19)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(19)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(5)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(5)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(5)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(5)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(3)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number(3)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "number";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_double";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_float";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_float";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_float";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "binary_float";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw(16)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw(16)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw(16)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw(16)";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "raw";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp with time zone";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp with time zone";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp with time zone";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp with time zone";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp with time zone";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "timestamp with time zone";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "interval day to second";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "json";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            string expected = "json";
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            var resolver = new DbTypeNameToOracleTypeNameResolver();

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
            var resolver = new DbTypeNameToOracleTypeNameResolver();

            // Act
            var actual = resolver.Resolve(" Custom_Type ");

            // Assert
            Assert.AreEqual("custom_type",
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}

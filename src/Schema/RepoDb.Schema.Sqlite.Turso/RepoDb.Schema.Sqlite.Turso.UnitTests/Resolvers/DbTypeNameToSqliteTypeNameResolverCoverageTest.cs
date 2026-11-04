using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Schema.Sqlite.Turso.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class DbTypeNameToSqliteTypeNameResolverCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Normalizes database type aliases and surrounding whitespace.
        /// </summary>
        [TestMethod]
        public void ResolveAliasesIgnoresCaseAndWhitespaceForNvarchar()
        {
            // Setup
            string name = "nvarchar";
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "varchar";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "char";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "char";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "text";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "boolean";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "boolean";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "boolean";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "boolean";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "integer";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "integer";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "integer";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "integer";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "bigint";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "bigint";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "smallint";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "smallint";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "double";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "double";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "double";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "double";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "real";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "real";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "uuid";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "uuid";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "blob";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "decimal";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "decimal";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "decimal";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "decimal";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "datetime";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "time";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "time";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "time";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "time";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "time";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            string expected = "time";
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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
            var resolver = new DbTypeNameToSqliteTypeNameResolver();

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

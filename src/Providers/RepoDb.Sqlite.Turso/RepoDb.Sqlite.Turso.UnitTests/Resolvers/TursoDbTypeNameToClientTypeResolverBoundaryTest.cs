using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Sqlite.Turso.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class TursoDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBigint()
        {
            // Setup
            string name = "bigint";
            Type expected = typeof(long);
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(long);
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(string);
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(string);
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(string);
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            Type expected = typeof(string);
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
            var resolver = new TursoDbTypeNameToClientTypeResolver();

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
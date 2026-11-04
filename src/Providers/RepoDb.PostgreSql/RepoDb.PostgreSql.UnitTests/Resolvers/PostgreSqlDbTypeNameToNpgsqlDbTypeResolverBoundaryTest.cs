using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class PostgreSqlDbTypeNameToNpgsqlDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForIntegerInMixedCase()
        {
            // Setup
            string name = "Integer";
            NpgsqlDbType expected = NpgsqlDbType.Integer;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForInteger()
        {
            // Setup
            string name = "integer";
            NpgsqlDbType expected = NpgsqlDbType.Integer;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForUserDefinedInUppercase()
        {
            // Setup
            string name = "USER-DEFINED";
            NpgsqlDbType expected = NpgsqlDbType.Unknown;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForUserDefined()
        {
            // Setup
            string name = "user-defined";
            NpgsqlDbType expected = NpgsqlDbType.Unknown;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForDoublePrecision()
        {
            // Setup
            string name = "double precision";
            NpgsqlDbType expected = NpgsqlDbType.Double;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForCharacterVarying()
        {
            // Setup
            string name = "character varying";
            NpgsqlDbType expected = NpgsqlDbType.Char;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForTimestampWithoutTimeZone()
        {
            // Setup
            string name = "timestamp without time zone";
            NpgsqlDbType expected = NpgsqlDbType.Timestamp;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles enum names, custom types and SQL aliases.
        /// </summary>
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderTypeForTimestampWithTimeZone()
        {
            // Setup
            string name = "timestamp with time zone";
            NpgsqlDbType expected = NpgsqlDbType.TimestampTz;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Rejects missing and blank type names.
        /// </summary>
        [TestMethod]
        public void ResolveBlankNameReportsArgumentForNull()
        {
            // Setup
            string name = null;
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(name));

            // Assert
            Assert.AreEqual("dbTypeName",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects missing and blank type names.
        /// </summary>
        [TestMethod]
        public void ResolveBlankNameReportsArgumentForEmptyString()
        {
            // Setup
            string name = "";
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(name));

            // Assert
            Assert.AreEqual("dbTypeName",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects missing and blank type names.
        /// </summary>
        [TestMethod]
        public void ResolveBlankNameReportsArgumentForWhitespace()
        {
            // Setup
            string name = " ";
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(name));

            // Assert
            Assert.AreEqual("dbTypeName",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects an unsupported SQL type.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameThrows()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToNpgsqlDbTypeResolver();

            // Act
            Action action = () => resolver.Resolve("__unsupported_type__");

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}
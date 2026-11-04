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
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected provider type.</param>
        [DataRow("Integer",
            NpgsqlDbType.Integer)]
        [DataRow("integer",
            NpgsqlDbType.Integer)]
        [DataRow("USER-DEFINED",
            NpgsqlDbType.Unknown)]
        [DataRow("user-defined",
            NpgsqlDbType.Unknown)]
        [DataRow("double precision",
            NpgsqlDbType.Double)]
        [DataRow("character varying",
            NpgsqlDbType.Char)]
        [DataRow("timestamp without time zone",
            NpgsqlDbType.Timestamp)]
        [DataRow("timestamp with time zone",
            NpgsqlDbType.TimestampTz)]
        [TestMethod]
        public void ResolveNameReturnsExpectedProviderType(string name,
            NpgsqlDbType expected)
        {
            // Setup
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
        /// <param name="name">The invalid type name.</param>
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [TestMethod]
        public void ResolveBlankNameReportsArgument(string name)
        {
            // Setup
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
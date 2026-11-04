using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.MySqlConnector.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class MySqlConnectorDbTypeToMySqlStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        /// <param name="dbType">The provider type.</param>
        /// <param name="expected">The expected SQL type name.</param>
        [DataRow(MySqlDbType.Binary,
            "BINARY")]
        [DataRow(MySqlDbType.Bit,
            "BIT")]
        [DataRow(MySqlDbType.Blob,
            "BLOB")]
        [DataRow(MySqlDbType.Byte,
            "TINYINT")]
        [DataRow(MySqlDbType.UByte,
            "TINYINT")]
        [DataRow(MySqlDbType.Date,
            "DATE")]
        [DataRow(MySqlDbType.DateTime,
            "DATETIME")]
        [DataRow(MySqlDbType.Decimal,
            "DECIMAL")]
        [DataRow(MySqlDbType.Double,
            "DOUBLE")]
        [DataRow(MySqlDbType.Enum,
            "TEXT")]
        [DataRow(MySqlDbType.Guid,
            "TEXT")]
        [DataRow(MySqlDbType.Set,
            "TEXT")]
        [DataRow(MySqlDbType.Text,
            "TEXT")]
        [DataRow(MySqlDbType.Float,
            "FLOAT")]
        [DataRow(MySqlDbType.Geometry,
            "GEOMETRY")]
        [DataRow(MySqlDbType.Int16,
            "SMALLINT")]
        [DataRow(MySqlDbType.Int24,
            "SMALLINT")]
        [DataRow(MySqlDbType.UInt24,
            "SMALLINT")]
        [DataRow(MySqlDbType.UInt16,
            "SMALLINT")]
        [DataRow(MySqlDbType.Int32,
            "INT")]
        [DataRow(MySqlDbType.UInt32,
            "INT")]
        [DataRow(MySqlDbType.Int64,
            "BIGINT")]
        [DataRow(MySqlDbType.UInt64,
            "BIGINT")]
        [DataRow(MySqlDbType.JSON,
            "JSON")]
        [DataRow(MySqlDbType.LongBlob,
            "LONGBLOB")]
        [DataRow(MySqlDbType.LongText,
            "LONGTEXT")]
        [DataRow(MySqlDbType.MediumBlob,
            "MEDIUMBLOB")]
        [DataRow(MySqlDbType.MediumText,
            "MEDIUMTEXT")]
        [DataRow(MySqlDbType.Newdate,
            "DATE")]
        [DataRow(MySqlDbType.NewDecimal,
            "DECIMAL")]
        [DataRow(MySqlDbType.String,
            "STRING")]
        [DataRow(MySqlDbType.Time,
            "TIME")]
        [DataRow(MySqlDbType.Timestamp,
            "TIMESTAMP")]
        [DataRow(MySqlDbType.TinyBlob,
            "TINYBLOB")]
        [DataRow(MySqlDbType.TinyText,
            "TINYTEXT")]
        [DataRow(MySqlDbType.VarBinary,
            "VARBINARY")]
        [DataRow(MySqlDbType.VarChar,
            "VARCHAR")]
        [DataRow(MySqlDbType.VarString,
            "VARCHAR")]
        [DataRow(MySqlDbType.Year,
            "YEAR")]
        [DataRow((MySqlDbType)(-1),
            "TEXT")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(MySqlDbType dbType,
            string expected)
        {
            // Setup
            var resolver = new MySqlConnectorDbTypeToMySqlStringNameResolver();


            // Act
            var actual = resolver.Resolve(dbType);


            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}
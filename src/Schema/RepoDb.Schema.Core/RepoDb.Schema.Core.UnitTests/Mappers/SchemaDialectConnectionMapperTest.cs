using System;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests.Mappers
{
    /// <summary>
    /// Tests connection-type dialect registration.
    /// </summary>
    [TestClass]
    public class SchemaDialectConnectionMapperTest
    {
        #region Public Methods

        /// <summary>
        /// Removes the settings and dialect used by these tests.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Remove<IDbConnection>();
            DbSettingMapper.Remove<DbConnection>();
            SchemaDialectMapper.Remove("DialectConnectionTest");
        }

        /// <summary>
        /// Resolves registration through the connection setting.
        /// </summary>
        [TestMethod]
        public void RegistersTheConnectionProviderDialect()
        {
            MapSetting<IDbConnection>("DialectConnectionTest");
            SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance);
            Assert.AreSame(CustomSchemaDialect.Instance,
                SchemaDialectMapper.Get("DialectConnectionTest"));
        }

        /// <summary>
        /// Replaces an earlier dialect for the connection provider.
        /// </summary>
        [TestMethod]
        public void ReplacesTheExistingDialect()
        {
            MapSetting<IDbConnection>("DialectConnectionTest");
            SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance);
            SchemaDialectMapper.Add<IDbConnection>(OtherSchemaDialect.Instance);
            Assert.AreSame(OtherSchemaDialect.Instance,
                SchemaDialectMapper.Get("DialectConnectionTest"));
        }

        /// <summary>
        /// Shares the dialect between connection types for one provider.
        /// </summary>
        [TestMethod]
        public void SharesTheDialectAcrossConnectionTypes()
        {
            MapSetting<IDbConnection>("DialectConnectionTest");
            MapSetting<DbConnection>("DialectConnectionTest");
            SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance);
            SchemaDialectMapper.Add<DbConnection>(OtherSchemaDialect.Instance);
            Assert.AreSame(OtherSchemaDialect.Instance,
                SchemaDialectMapper.Get("DialectConnectionTest"));
        }

        /// <summary>
        /// Keeps saved-definition lookup independent of connection settings.
        /// </summary>
        [TestMethod]
        public void KeepsProviderLookupAfterRemovingTheConnectionSetting()
        {
            MapSetting<IDbConnection>("DialectConnectionTest");
            SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance);
            DbSettingMapper.Remove<IDbConnection>();
            Assert.AreSame(CustomSchemaDialect.Instance,
                SchemaDialectMapper.Get("DialectConnectionTest"));
        }

        /// <summary>
        /// Rejects a missing dialect.
        /// </summary>
        [TestMethod]
        public void RejectsANullDialect()
        {
            MapSetting<IDbConnection>("DialectConnectionTest");
            Assert.ThrowsExactly<ArgumentNullException>(() => SchemaDialectMapper.Add<IDbConnection>(null));
        }

        /// <summary>
        /// Rejects an unconfigured connection type.
        /// </summary>
        [TestMethod]
        public void RejectsAMissingConnectionSetting()
        {
            DbSettingMapper.Remove<IDbConnection>();
            Assert.ThrowsExactly<MissingMappingException>(() => SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance));
        }

        /// <summary>
        /// Rejects a setting without a provider name.
        /// </summary>
        [TestMethod]
        public void RejectsANullProvider()
        {
            MapSetting<IDbConnection>(null);
            Assert.ThrowsExactly<MissingMappingException>(() => SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance));
        }

        /// <summary>
        /// Rejects an empty provider name.
        /// </summary>
        [TestMethod]
        public void RejectsAnEmptyProvider()
        {
            MapSetting<IDbConnection>(string.Empty);
            Assert.ThrowsExactly<MissingMappingException>(() => SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance));
        }

        /// <summary>
        /// Rejects a whitespace provider name.
        /// </summary>
        [TestMethod]
        public void RejectsAWhitespaceProvider()
        {
            MapSetting<IDbConnection>(" ");
            Assert.ThrowsExactly<MissingMappingException>(() => SchemaDialectMapper.Add<IDbConnection>(CustomSchemaDialect.Instance));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Registers a provider setting without creating a connection.
        /// </summary>
        /// <typeparam name="TDbConnection">The connection type.</typeparam>
        /// <param name="provider">The provider name.</param>
        private static void MapSetting<TDbConnection>(string provider)
            where TDbConnection : IDbConnection
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(value => value.Provider).Returns(provider);
            DbSettingMapper.Add<TDbConnection>(setting.Object,
                true);
        }

        #endregion
    }
}

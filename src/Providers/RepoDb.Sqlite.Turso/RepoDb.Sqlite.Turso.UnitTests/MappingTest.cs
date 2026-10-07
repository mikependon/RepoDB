#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb.Turso.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseTurso();
        }

        #region MDS

        [TestMethod]
        public void TestUseTursoPreservesSqliteRegistrationsAndConversionOptions()
        {
            var configuration = GlobalConfiguration.Setup().UseSqlite();
            var options = GlobalConfiguration.Options;
            var sqliteSetting = DbSettingMapper.Get<Microsoft.Data.Sqlite.SqliteConnection>();
            var sqliteHelper = DbHelperMapper.Get<Microsoft.Data.Sqlite.SqliteConnection>();
            var sqliteBuilder = StatementBuilderMapper.Get<Microsoft.Data.Sqlite.SqliteConnection>();

            Assert.AreSame(configuration, configuration.UseTurso());
            Assert.AreSame(options, GlobalConfiguration.Options);
            Assert.AreEqual(Enumerations.ConversionType.Default, options.ConversionType);
            Assert.AreSame(sqliteSetting, DbSettingMapper.Get<Microsoft.Data.Sqlite.SqliteConnection>());
            Assert.AreSame(sqliteHelper, DbHelperMapper.Get<Microsoft.Data.Sqlite.SqliteConnection>());
            Assert.AreSame(sqliteBuilder, StatementBuilderMapper.Get<Microsoft.Data.Sqlite.SqliteConnection>());
            Assert.IsInstanceOfType<TursoDbSetting>(DbSettingMapper.Get<SqliteConnection>());
            Assert.IsInstanceOfType<TursoDbHelper>(DbHelperMapper.Get<SqliteConnection>());
            Assert.IsInstanceOfType<TursoStatementBuilder>(StatementBuilderMapper.Get<SqliteConnection>());
        }

        [TestMethod]
        public void TestUseTursoIsIdempotent()
        {
            var configuration = GlobalConfiguration.Setup().UseTurso();
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var helper = DbHelperMapper.Get<SqliteConnection>();
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            Assert.AreSame(configuration, configuration.UseTurso());
            Assert.AreSame(setting, DbSettingMapper.Get<SqliteConnection>());
            Assert.AreSame(helper, DbHelperMapper.Get<SqliteConnection>());
            Assert.AreSame(builder, StatementBuilderMapper.Get<SqliteConnection>());
            Assert.IsTrue(TursoBootstrap.IsInitialized);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestMdsTursoDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestMdsTursoDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }

        #endregion
    }
}

#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Sqlite.Microsoft.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSqlite();
        }

        #region MDS

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestMdsSqLiteDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestMdsSqLiteDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }

        #endregion
    }
}

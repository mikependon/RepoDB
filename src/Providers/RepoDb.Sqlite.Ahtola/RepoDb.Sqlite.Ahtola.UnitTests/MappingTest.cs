#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Ahtola.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAhtola();
        }

        #region MDS

        [TestMethod]
        public void TestMdsAhtolaStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestMdsAhtolaDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }

        #endregion
    }
}

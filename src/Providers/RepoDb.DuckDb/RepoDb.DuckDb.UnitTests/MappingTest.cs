#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;

namespace RepoDb.DuckDb.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDuckDb();
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestDuckDbDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestDuckDbDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}

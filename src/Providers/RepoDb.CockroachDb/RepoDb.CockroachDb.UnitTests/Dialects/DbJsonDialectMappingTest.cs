#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Dialects;

namespace RepoDb.CockroachDb.UnitTests.Dialects
{
    [TestClass]
    public class DbJsonDialectMappingTest
    {
        #region Mapping

        [TestMethod]
        public void TestCockroachDbDbJsonDialectIsMappedByTheBootstrap()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();

            // Act
            var dialect = DbJsonDialectMapper.Get(new CockroachDbDbSetting());

            // Assert
            Assert.IsInstanceOfType<CockroachDbDbJsonDialect>(dialect);
        }

        #endregion
    }
}

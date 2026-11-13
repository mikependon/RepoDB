#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Dialects;

namespace RepoDb.SapHana.UnitTests.Dialects
{
    [TestClass]
    public class DbJsonDialectMappingTest
    {
        #region Mapping

        [TestMethod]
        public void TestSapHanaDbJsonDialectIsMappedByTheBootstrap()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseSapHana();

            // Act
            var dialect = DbJsonDialectMapper.Get(new SapHanaDbSetting());

            // Assert
            Assert.IsInstanceOfType<SapHanaDbJsonDialect>(dialect);
        }

        #endregion
    }
}

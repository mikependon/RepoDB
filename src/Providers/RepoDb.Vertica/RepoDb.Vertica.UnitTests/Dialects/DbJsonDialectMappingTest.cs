#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Dialects;

namespace RepoDb.Vertica.UnitTests.Dialects
{
    [TestClass]
    public class DbJsonDialectMappingTest
    {
        #region Mapping

        [TestMethod]
        public void TestVerticaDbJsonDialectIsMappedByTheBootstrap()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseVertica();

            // Act
            var dialect = DbJsonDialectMapper.Get(new VerticaDbSetting());

            // Assert
            Assert.IsInstanceOfType<VerticaDbJsonDialect>(dialect);
        }

        #endregion
    }
}

#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class SchemaDefaultPatternTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefaultPatternKind()
        {
            // Act
            var pattern = new SchemaDefaultPattern(ColumnDefaultValue.NewGuid,
                "^uuid$");

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, pattern.Kind);
        }

        [TestMethod]
        public void TestSchemaDefaultPatternPattern()
        {
            // Act
            var pattern = new SchemaDefaultPattern(ColumnDefaultValue.NewGuid,
                "^uuid$");

            // Assert
            Assert.AreEqual("^uuid$", pattern.Pattern);
        }

        #endregion
    }
}

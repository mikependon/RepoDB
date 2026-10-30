#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionFormat</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionFormatTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionFormatName()
        {
            // Act
            var actual = SchemaDefinitionFormat.Name;

            // Assert
            Assert.AreEqual("RepoDb.Schema.Definition", actual, StringComparer.Ordinal);
        }

        #endregion
    }
}

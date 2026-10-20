#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Enumerations
{
    [TestClass]
    public class CopySchemaForeignKeyRuleTest
    {
        #region Methods

        [TestMethod]
        public void TestForeignKeyRuleDefaultValue()
        {
            // Act
            var actual = default(CopySchemaForeignKeyRule);
            var expected = CopySchemaForeignKeyRule.NoAction;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestForeignKeyRuleValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopySchemaForeignKeyRule));
            var expected = new[] { "NoAction", "Restrict", "Cascade", "SetNull", "SetDefault" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}

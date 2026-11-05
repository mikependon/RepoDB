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
    public class SchemaExpressionTokenTest
    {
        #region Public Methods

        [TestMethod]
        public void TestConstructorSetsTheKind()
        {
            // Act
            var token = new SchemaExpressionToken(SchemaExpressionTokenKind.Identifier,
                "[A]",
                "A");

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Identifier, token.Kind);
        }

        [TestMethod]
        public void TestConstructorSetsTheText()
        {
            // Act
            var token = new SchemaExpressionToken(SchemaExpressionTokenKind.Identifier,
                "[A]",
                "A");

            // Assert
            Assert.AreEqual("[A]", token.Text);
        }

        [TestMethod]
        public void TestConstructorSetsTheName()
        {
            // Act
            var token = new SchemaExpressionToken(SchemaExpressionTokenKind.Identifier,
                "[A]",
                "A");

            // Assert
            Assert.AreEqual("A", token.Name);
        }

        [TestMethod]
        public void TestConstructorSetsNoNameByDefault()
        {
            // Act
            var token = new SchemaExpressionToken(SchemaExpressionTokenKind.Word,
                "abc");

            // Assert
            Assert.IsNull(token.Name);
        }

        #endregion
    }
}

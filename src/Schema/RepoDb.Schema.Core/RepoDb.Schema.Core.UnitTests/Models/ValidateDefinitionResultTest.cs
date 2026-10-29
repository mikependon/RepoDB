#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    /// <summary>
    /// Contains the tests of the <see cref="ValidateDefinitionResult"/> class.
    /// </summary>
    [TestClass]
    public class ValidateDefinitionResultTest
    {
        #region Public Methods

        [TestMethod]
        public void TestValidateDefinitionResultDefaults()
        {
            // Act
            var result = new ValidateDefinitionResult();

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsNull(result.Definition);
            Assert.IsNotNull(result.Warnings);
            Assert.AreEqual(0, result.Warnings.Count);
        }

        [TestMethod]
        public void TestValidateDefinitionResultIsValidWithWarnings()
        {
            // Act
            var result = new ValidateDefinitionResult { Warnings = new List<string> { "The primary key column 'Code' is nullable." } };

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.Warnings.Count);
        }

        [TestMethod]
        public void TestValidateDefinitionResultIsNotValidWithErrors()
        {
            // Act
            var result = new ValidateDefinitionResult { Errors = new List<string> { "The file is malformed." } };

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public void TestValidateDefinitionResultIsValidWithDefinition()
        {
            // Setup
            var definition = new SchemaDefinitionResult();

            // Act
            var result = new ValidateDefinitionResult { Definition = definition };

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreSame(definition, result.Definition);
        }

        #endregion
    }
}

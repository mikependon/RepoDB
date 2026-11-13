#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.Vertica.UnitTests.Dialects
{
    [TestClass]
    public class VerticaDbJsonDialectTest
    {
        #region FormatPath

        [TestMethod]
        public void TestVerticaDbJsonDialectFormatPath()
        {
            // Act
            var result = new VerticaDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'Address.City'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new VerticaDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'Tags.0'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaDbJsonDialectFormatPathWithLeadingArrayIndex()
        {
            // Act
            var result = new VerticaDbJsonDialect().FormatPath(new[] { "[1]", "Name" });

            // Assert
            Assert.AreEqual("'1.Name'", result, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestVerticaDbJsonDialectGetExtractFormat()
        {
            // Act
            var result = new VerticaDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("MAPLOOKUP(MAPJSONEXTRACTOR({0}), @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion
    }
}

#region Copyright Attributions

// Copyright (c) 2022 ngardon and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions.QueryFields;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class LiteralQuerFieldTest
	{
        [TestMethod]
        public void TestLiteralQueryFieldConstructor()
        {
            // Prepare
            var literalQueryField = new LiteralQueryField("[Id] BETWEEN 10 AND 100");

            // Assert
            Assert.AreEqual("fake", literalQueryField.Field.Name, System.StringComparer.Ordinal);
            Assert.AreEqual("[Id] BETWEEN 10 AND 100", literalQueryField.Literal, System.StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLiteralQueryFieldGetString()
        {
            // Prepare
            var literalQueryField = new LiteralQueryField("[Id] BETWEEN 10 AND 100");

            // Assert
            Assert.AreEqual("[Id] BETWEEN 10 AND 100", literalQueryField.GetString(0, null), System.StringComparer.Ordinal);
        }
    }
}


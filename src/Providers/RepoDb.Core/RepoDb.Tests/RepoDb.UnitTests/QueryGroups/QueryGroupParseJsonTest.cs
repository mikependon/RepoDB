#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Linq;

namespace RepoDb.UnitTests.QueryGroups
{
    [TestClass]
    public class QueryGroupParseJsonTest
    {
        #region Value chain

        [TestMethod]
        public void TestQueryGroupParseJsonValueChainField()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.Value.Id == 5);

            // Assert
            Assert.IsInstanceOfType<JsonExtractQueryField>(queryGroup.QueryFields.Single());
        }

        [TestMethod]
        public void TestQueryGroupParseJsonValueChainPath()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.Value.Address.City == "Gentofte");

            // Assert
            Assert.AreEqual("Address.City", ((JsonExtractQueryField)queryGroup.QueryFields.Single()).Path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseJsonValueChainColumnAndValue()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.Value.Id == 5);
            var field = queryGroup.QueryFields.Single();

            // Assert
            Assert.AreEqual("Document", field.Field.Name, StringComparer.Ordinal);
            Assert.AreEqual(5, field.Parameter.Value);
        }

        [TestMethod]
        public void TestQueryGroupParseJsonValueChainOperation()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.Value.Id != 5);

            // Assert
            Assert.AreEqual(Operation.NotEqual, queryGroup.QueryFields.Single().Operation);
        }

        #endregion

        #region ExtractValue

        [TestMethod]
        public void TestQueryGroupParseJsonExtractValueExpressionPath()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.ExtractValue(d => d.Address.City) == "Gentofte");

            // Assert
            Assert.AreEqual("Address.City", ((JsonExtractQueryField)queryGroup.QueryFields.Single()).Path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseJsonExtractValueTextPath()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.ExtractValue<JsonSampleDocument, int>("Id") == 5);

            // Assert
            Assert.AreEqual("Id", ((JsonExtractQueryField)queryGroup.QueryFields.Single()).Path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseJsonExtractValueArrayIndex()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Tags.ExtractValue(t => t[0]) == "repo");

            // Assert
            Assert.AreEqual("[0]", ((JsonExtractQueryField)queryGroup.QueryFields.Single()).Path, StringComparer.Ordinal);
        }

        #endregion

        #region Combined

        [TestMethod]
        public void TestQueryGroupParseJsonCombinedWithPlainField()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Id == 1 && x.Document.Value.Id == 5);

            // Assert
            Assert.AreEqual(2, queryGroup.GetFields(true).Count());
        }

        [TestMethod]
        public void TestQueryGroupParseJsonCombinedKeepsJsonField()
        {
            // Act
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Id == 1 && x.Document.Value.Id == 5);

            // Assert
            Assert.IsTrue(queryGroup.GetFields(true).Any(f => f is JsonExtractQueryField));
        }

        #endregion
    }
}

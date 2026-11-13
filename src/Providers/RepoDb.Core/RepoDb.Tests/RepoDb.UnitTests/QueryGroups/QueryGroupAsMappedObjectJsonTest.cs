#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.QueryGroups
{
    [TestClass]
    public class QueryGroupAsMappedObjectJsonTest
    {
        #region AsMappedObject

        [TestMethod]
        public void TestQueryGroupAsMappedObjectWithJsonExtractValue()
        {
            // Setup
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.Value.Id == 5);

            // Act
            var mapped = (IDictionary<string, object>)QueryGroup.AsMappedObject(new[] { queryGroup.MapTo<JsonSampleEntity>() });

            // Assert
            Assert.AreEqual(5, mapped["Document_Id"]);
        }

        [TestMethod]
        public void TestQueryGroupAsMappedObjectWithJsonExtractValueName()
        {
            // Setup
            var queryGroup = QueryGroup.Parse<JsonSampleEntity>(x => x.Document.Value.Address.City == "Gentofte");

            // Act
            var mapped = (IDictionary<string, object>)QueryGroup.AsMappedObject(new[] { queryGroup.MapTo<JsonSampleEntity>() });

            // Assert
            Assert.IsTrue(mapped.ContainsKey("Document_Address_City"));
        }

        #endregion
    }
}

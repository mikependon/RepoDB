#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Helpers;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RepoDb.UnitTests.Helpers
{
    [TestClass]
    public class DbJsonConverterTest
    {
        #region FromNode

        [TestMethod]
        public void TestDbJsonConverterFromNode()
        {
            // Setup
            var node = JsonNode.Parse("{\"Id\":7}");

            // Act
            var document = DbJsonConverter.FromNode<JsonSampleDocument>(node);

            // Assert
            Assert.AreEqual(7, document.Id);
        }

        [TestMethod]
        public void TestDbJsonConverterFromNodeWithNull()
        {
            // Act
            var document = DbJsonConverter.FromNode<JsonSampleDocument>(null);

            // Assert
            Assert.IsNull(document);
        }

        #endregion

        #region GetOptions

        [TestMethod]
        public void TestDbJsonConverterGetOptionsWithoutConfiguration()
        {
            // Act
            var options = DbJsonConverter.GetOptions();

            // Assert
            Assert.AreSame(JsonSerializerOptions.Default, options);
        }

        [TestMethod]
        public void TestDbJsonConverterGetOptionsWithConfiguration()
        {
            // Setup
            var configured = new JsonSerializerOptions();
            GlobalConfiguration.Options.JsonSerializerOptions = configured;

            try
            {
                // Act
                var options = DbJsonConverter.GetOptions();

                // Assert
                Assert.AreSame(configured, options);
            }
            finally
            {
                GlobalConfiguration.Options.JsonSerializerOptions = null;
            }
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestDbJsonConverterParse()
        {
            // Act
            var node = DbJsonConverter.Parse("{\"Id\":5}");

            // Assert
            Assert.AreEqual(5, node["Id"].GetValue<int>());
        }

        [TestMethod]
        public void TestDbJsonConverterParseWithWhitespace()
        {
            // Act
            var node = DbJsonConverter.Parse("  ");

            // Assert
            Assert.IsNull(node);
        }

        #endregion

        #region ToNode

        [TestMethod]
        public void TestDbJsonConverterToNode()
        {
            // Act
            var node = DbJsonConverter.ToNode(new JsonSampleDocument { Id = 5 });

            // Assert
            Assert.AreEqual(5, node["Id"].GetValue<int>());
        }

        [TestMethod]
        public void TestDbJsonConverterToNodeWithNull()
        {
            // Act
            var node = DbJsonConverter.ToNode<JsonSampleDocument>(null);

            // Assert
            Assert.IsNull(node);
        }

        #endregion

        #region ToJsonString

        [TestMethod]
        public void TestDbJsonConverterToJsonString()
        {
            // Setup
            var node = JsonNode.Parse("{\"Id\":5}");

            // Act
            var json = DbJsonConverter.ToJsonString(node);

            // Assert
            Assert.AreEqual("{\"Id\":5}", json, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbJsonConverterToJsonStringWithNull()
        {
            // Act
            var json = DbJsonConverter.ToJsonString(null);

            // Assert
            Assert.IsNull(json);
        }

        #endregion
    }
}

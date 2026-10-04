#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class TableInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestTableInfoConstructorWithNameAndSchema()
        {
            // Act
            var actual = new TableInfo("Person", "dbo");

            // Assert
            Assert.AreEqual("Person", actual.Name, StringComparer.Ordinal);
            Assert.AreEqual("dbo", actual.Schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableInfoNamePropertyDefaultValue()
        {
            // Act
            var actual = new TableInfo(null, null).Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableInfoNameProperty()
        {
            // Act
            var actual = new TableInfo("Person", null).Name;
            var expected = "Person";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableInfoSchemaPropertyDefaultValue()
        {
            // Act
            var actual = new TableInfo(null, null).Schema;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableInfoSchemaProperty()
        {
            // Act
            var actual = new TableInfo(null, "dbo").Schema;
            var expected = "dbo";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        #endregion
    }
}

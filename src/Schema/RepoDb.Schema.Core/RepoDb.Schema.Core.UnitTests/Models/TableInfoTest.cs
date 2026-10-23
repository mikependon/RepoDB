#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class TableInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestTableInfoEqualsWithSameValues()
        {
            // Act
            var a = Create();
            var b = Create();

            // Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void TestTableInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestTableInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((TableInfo)null) == null);
        }

        [TestMethod]
        public void TestTableInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestTableInfoNotEqualsWithDifferentName()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Name = "Other";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableInfoNotEqualsWithDifferentSchema()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Schema = "sales";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

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

        [TestMethod]
        public void TestTableInfoToStringReturnsTheName()
        {
            // Act
            var actual = Create().ToString();

            // Assert
            Assert.AreEqual("Person", actual);
        }

        [TestMethod]
        public void TestTableInfoToStringWithoutANameReturnsNull()
        {
            // Act
            var actual = new TableInfo(null, "dbo").ToString();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion

        #region Helpers

        private static TableInfo Create() =>
            new TableInfo("Person", "dbo");

        #endregion

        #region Database And Provider Type

        [TestMethod]
        public void TestTableInfoConstructorWithTheDatabaseAndTheProvider()
        {
            // Act
            var actual = new TableInfo("Person", "dbo", "Northwind", "SqlServer");

            // Assert
            Assert.AreEqual("Person", actual.Name, StringComparer.Ordinal);
            Assert.AreEqual("dbo", actual.Schema, StringComparer.Ordinal);
            Assert.AreEqual("Northwind", actual.Database, StringComparer.Ordinal);
            Assert.AreEqual("SqlServer", actual.Provider, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableInfoDatabaseAndProviderPropertiesDefaultValue()
        {
            // Act
            var actual = new TableInfo("Person", "dbo");

            // Assert
            Assert.IsNull(actual.Database);
            Assert.IsNull(actual.Provider);
        }

        [TestMethod]
        public void TestTableInfoDatabaseAndProviderProperties()
        {
            // Act
            var actual = new TableInfo("Person", "dbo")
            {
                Database = "Northwind",
                Provider = "MySql"
            };

            // Assert
            Assert.AreEqual("Northwind", actual.Database, StringComparer.Ordinal);
            Assert.AreEqual("MySql", actual.Provider, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableInfoEqualsIgnoresTheDatabaseAndTheProvider()
        {
            // Act
            var a = new TableInfo("Person", "dbo", "Northwind", "SqlServer");
            var b = new TableInfo("Person", "dbo", "Other", "MySql");
            var c = new TableInfo("Person", "dbo");

            // Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a == c);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), c.GetHashCode());
        }

        [TestMethod]
        public void TestTableInfoCanBeAKeyWhateverItsDatabaseAndProvider()
        {
            // Act
            var dictionary = new Dictionary<TableInfo, int> { [new TableInfo("Person", "dbo", "Northwind", "SqlServer")] = 1 };

            // Assert
            Assert.IsTrue(dictionary.ContainsKey(new TableInfo("Person", "dbo")));
            Assert.IsFalse(dictionary.ContainsKey(new TableInfo("Person", "sales", "Northwind", "SqlServer")));
        }

        #endregion
    }
}

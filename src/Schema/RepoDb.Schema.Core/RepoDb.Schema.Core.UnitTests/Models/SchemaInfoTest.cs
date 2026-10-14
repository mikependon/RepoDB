#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class SchemaInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestSchemaInfoEqualsWithSameValues()
        {
            // Act
            var a = new SchemaInfo("sales");
            var b = new SchemaInfo("sales");

            // Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void TestSchemaInfoEqualsWithDifferentValues()
        {
            // Act
            var a = new SchemaInfo("sales");
            var b = new SchemaInfo("public");

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestSchemaInfoEqualsWithNull()
        {
            // Act
            var a = new SchemaInfo("sales");

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a == null);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((SchemaInfo)null) == null);
        }

        [TestMethod]
        public void TestSchemaInfoToStringReturnsTheName()
        {
            // Act
            var actual = new SchemaInfo("sales");

            // Assert
            Assert.AreEqual("sales", actual.Name);
            Assert.AreEqual("sales", actual.ToString());
        }

        #endregion
    }
}

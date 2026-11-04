#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class GuidPropertyHandlerTest
    {
        [TestMethod]
        public void TestStringToGuidNull()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.AreEqual(Guid.Empty, actual);
        }

        [TestMethod]
        public void TestStringToGuidDatabaseNull()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.AreEqual(Guid.Empty, actual);
        }

        [TestMethod]
        public void TestStringToGuidEmptyText()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.AreEqual(Guid.Empty, actual);
        }

        [TestMethod]
        public void TestStringToGuidGuid()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();
            var expected = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff");

            // Act
            var actual = handler.Get(expected, null);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestStringToGuidText()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();
            var expected = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff");

            // Act
            var actual = handler.Get(expected.ToString("D"), null);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestStringToGuidInvalidText()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();

            // Act
            Action act = () => handler.Get("invalid", null);

            // Assert
            Assert.ThrowsExactly<FormatException>(act);
        }

        [TestMethod]
        public void TestStringToGuidUnsupportedType()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();

            // Act
            Action act = () => handler.Get(42, null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestStringToGuidWritesCanonicalText()
        {
            // Setup
            var handler = new StringToGuidPropertyHandler();
            var input = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("6f9619ff-8b86-d011-b42d-00c04fc964ff", actual);
        }

        [TestMethod]
        public void TestStringToNullableGuidNull()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestStringToNullableGuidDatabaseNull()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestStringToNullableGuidEmptyText()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestStringToNullableGuidGuid()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();
            var expected = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff");

            // Act
            var actual = handler.Get(expected, null);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestStringToNullableGuidText()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();
            var expected = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff");

            // Act
            var actual = handler.Get(expected.ToString("D"), null);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestStringToNullableGuidInvalidText()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();

            // Act
            Action act = () => handler.Get("invalid", null);

            // Assert
            Assert.ThrowsExactly<FormatException>(act);
        }

        [TestMethod]
        public void TestStringToNullableGuidUnsupportedType()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();

            // Act
            Action act = () => handler.Get(42, null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestStringToNullableGuidWritesCanonicalText()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();
            var input = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("6f9619ff-8b86-d011-b42d-00c04fc964ff", actual);
        }

        [TestMethod]
        public void TestStringToNullableGuidWritesNull()
        {
            // Setup
            var handler = new StringToNullableGuidPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

    }
}

#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class CollectionPropertyHandlerTest
    {
        [TestMethod]
        public void TestFloatArrayGetNull()
        {
            // Setup
            var handler = new ByteArrayToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestFloatArrayGetEmpty()
        {
            // Setup
            var handler = new ByteArrayToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Get(Array.Empty<byte>(), null);

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestFloatArraySetNull()
        {
            // Setup
            var handler = new ByteArrayToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestFloatArraySetEmpty()
        {
            // Setup
            var handler = new ByteArrayToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Set(Array.Empty<float>(), null);

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestFloatArrayReadsLittleEndianValues()
        {
            // Setup
            var handler = new ByteArrayToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Get(new byte[] { 0, 0, 128, 63, 0, 0, 32, 192 }, null);

            // Assert
            Assert.AreSequenceEqual(new[] { 1F, -2.5F }, actual);
        }

        [TestMethod]
        public void TestFloatArrayWritesLittleEndianValues()
        {
            // Setup
            var handler = new ByteArrayToFloatArrayPropertyHandler();

            // Act
            var actual = handler.Set(new[] { 1F, -2.5F }, null);

            // Assert
            Assert.AreSequenceEqual(new byte[] { 0, 0, 128, 63, 0, 0, 32, 192 }, actual);
        }

        [TestMethod]
        public void TestFloatArrayRejectsIncompleteValue()
        {
            // Setup
            var handler = new ByteArrayToFloatArrayPropertyHandler();

            // Act
            Action act = () => handler.Get(new byte[3], null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestSeparatedStringGetNull()
        {
            // Setup
            var handler = new CommaSeparatedStringToStringArrayPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestSeparatedStringSetNull()
        {
            // Setup
            var handler = new CommaSeparatedStringToStringArrayPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestSeparatedStringReadsEmpty()
        {
            // Setup
            var handler = new CommaSeparatedStringToStringArrayPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestSeparatedStringPreservesEmptyMembers()
        {
            // Setup
            var handler = new CommaSeparatedStringToStringArrayPropertyHandler();

            // Act
            var actual = handler.Get("one,,three", null);

            // Assert
            Assert.AreSequenceEqual(new[] { "one", "", "three" }, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSeparatedStringWritesMembers()
        {
            // Setup
            var handler = new CommaSeparatedStringToStringArrayPropertyHandler();

            // Act
            var actual = handler.Set(new[] { "one", null, "three" }, null);

            // Assert
            Assert.AreEqual("one,,three", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSeparatedStringWritesEmptyArray()
        {
            // Setup
            var handler = new CommaSeparatedStringToStringArrayPropertyHandler();

            // Act
            var actual = handler.Set(Array.Empty<string>(), null);

            // Assert
            Assert.AreEqual(string.Empty, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSeparatedStringRejectsEmbeddedComma()
        {
            // Setup
            var handler = new CommaSeparatedStringToStringArrayPropertyHandler();

            // Act
            Action act = () => handler.Set(new[] { "one,two" }, null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestArrayToListGetNull()
        {
            // Setup
            var handler = new ArrayToListPropertyHandler<int>();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestArrayToListSetNull()
        {
            // Setup
            var handler = new ArrayToListPropertyHandler<int>();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestArrayToListCopiesValues()
        {
            // Setup
            var handler = new ArrayToListPropertyHandler<int>();
            var input = new[] { 1, 2 };

            // Act
            var actual = handler.Get(input, null);
            input[0] = 99;

            // Assert
            Assert.AreSequenceEqual(new[] { 1, 2 }, actual);
        }

        [TestMethod]
        public void TestArrayToListWritesValues()
        {
            // Setup
            var handler = new ArrayToListPropertyHandler<int>();
            var input = new List<int> { 1, 2 };

            // Act
            var actual = handler.Set(input, null);
            input[0] = 99;

            // Assert
            Assert.AreSequenceEqual(new[] { 1, 2 }, actual);
        }

        [TestMethod]
        public void TestEnumReadsNull()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<DayOfWeek>();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.AreEqual(default(DayOfWeek), actual);
        }

        [TestMethod]
        public void TestEnumReadsEmpty()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<DayOfWeek>();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.AreEqual(default(DayOfWeek), actual);
        }

        [TestMethod]
        public void TestEnumReadsWhitespace()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<DayOfWeek>();

            // Act
            var actual = handler.Get("   ", null);

            // Assert
            Assert.AreEqual(default(DayOfWeek), actual);
        }

        [TestMethod]
        public void TestEnumReadsIgnoringCase()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<DayOfWeek>();

            // Act
            var actual = handler.Get("monday", null);

            // Assert
            Assert.AreEqual(DayOfWeek.Monday, actual);
        }

        [TestMethod]
        public void TestEnumRejectsUnknownMember()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<DayOfWeek>();

            // Act
            Action act = () => handler.Get("Unknown", null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestEnumWritesName()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<DayOfWeek>();

            // Act
            var actual = handler.Set(DayOfWeek.Monday, null);

            // Assert
            Assert.AreEqual("Monday", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFlagsEnumReadsMembers()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<Permissions>();

            // Act
            var actual = handler.Get("Read,Write", null);

            // Assert
            Assert.AreEqual(Permissions.Read | Permissions.Write, actual);
        }

        [TestMethod]
        public void TestFlagsEnumWritesWithoutSpaces()
        {
            // Setup
            var handler = new StringToEnumPropertyHandler<Permissions>();

            // Act
            var actual = handler.Set(Permissions.Read | Permissions.Write, null);

            // Assert
            Assert.AreEqual("Read,Write", actual, StringComparer.Ordinal);
        }

        [Flags]
        public enum Permissions { None = 0, Read = 1, Write = 2 }
    }
}

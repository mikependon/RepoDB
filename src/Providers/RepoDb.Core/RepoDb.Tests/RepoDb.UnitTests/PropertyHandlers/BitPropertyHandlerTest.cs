#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections;
using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class BitPropertyHandlerTest
    {
        [TestMethod]
        public void TestBitArrayNull()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBitArrayDatabaseNull()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBitArrayReadsUInt64()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(258UL, null);

            // Assert
            Assert.AreEqual(64, actual.Length);
            Assert.AreSequenceEqual(new[] { 1, 8 }, actual.Cast<bool>().Select((bit, index) => (bit, index)).Where(item => item.bit).Select(item => item.index));
        }

        [TestMethod]
        public void TestBitArrayReadsBigEndianBytes()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(new byte[] { 1, 2 }, null);

            // Assert
            Assert.AreEqual(64, actual.Length);
            Assert.AreSequenceEqual(new[] { 1, 8 }, actual.Cast<bool>().Select((bit, index) => (bit, index)).Where(item => item.bit).Select(item => item.index));
        }

        [TestMethod]
        public void TestBitArrayReadsTrue()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(true, null);

            // Assert
            Assert.AreEqual(true, actual[0]);
            Assert.AreEqual(1, actual.Cast<bool>().Count(bit => bit));
        }

        [TestMethod]
        public void TestBitArrayReadsFalse()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(false, null);

            // Assert
            Assert.AreEqual(false, actual[0]);
            Assert.AreEqual(0, actual.Cast<bool>().Count(bit => bit));
        }

        [TestMethod]
        public void TestBitArrayReadsConvertibleInt()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(258, null);

            // Assert
            Assert.AreEqual(64, actual.Length);
            Assert.AreSequenceEqual(new[] { 1, 8 }, actual.Cast<bool>().Select((bit, index) => (bit, index)).Where(item => item.bit).Select(item => item.index));
        }

        [TestMethod]
        public void TestBitArrayReadsEmptyBytes()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Get(Array.Empty<byte>(), null);

            // Assert
            Assert.AreEqual(false, actual[0]);
            Assert.AreEqual(0, actual.Cast<bool>().Count(bit => bit));
        }

        [TestMethod]
        public void TestBitArrayWritesNull()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBitArrayRejectsOversizedBytes()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            Action act = () => handler.Get(new byte[9], null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestBitArrayWritesBits()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();
            var input = new BitArray(new[] { true, false, true });

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual(5UL, actual);
        }

        [TestMethod]
        public void TestBitArrayRejectsOversizedBits()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();

            // Act
            Action act = () => handler.Set(new BitArray(65), null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestBitArrayWritesHighestBit()
        {
            // Setup
            var handler = new BitToBitArrayPropertyHandler();
            var input = new BitArray(64);
            input[63] = true;

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual(1UL << 63, actual);
        }

        [TestMethod]
        public void TestByteArrayNull()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestByteArrayDatabaseNull()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestByteArrayReadsUInt64()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(258UL, null);

            // Assert
            Assert.AreSequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 }, actual);
        }

        [TestMethod]
        public void TestByteArrayReadsBigEndianBytes()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(new byte[] { 1, 2 }, null);

            // Assert
            Assert.AreSequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 }, actual);
        }

        [TestMethod]
        public void TestByteArrayReadsTrue()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(true, null);

            // Assert
            Assert.AreSequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }, actual);
        }

        [TestMethod]
        public void TestByteArrayReadsFalse()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(false, null);

            // Assert
            Assert.AreSequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, actual);
        }

        [TestMethod]
        public void TestByteArrayReadsConvertibleInt()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(258, null);

            // Assert
            Assert.AreSequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 }, actual);
        }

        [TestMethod]
        public void TestByteArrayReadsEmptyBytes()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Get(Array.Empty<byte>(), null);

            // Assert
            Assert.AreSequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, actual);
        }

        [TestMethod]
        public void TestByteArrayWritesNull()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestByteArrayRejectsOversizedBytes()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            Action act = () => handler.Get(new byte[9], null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestByteArrayWritesBigEndianBytes()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            var actual = handler.Set(new byte[] { 1, 2 }, null);

            // Assert
            Assert.AreEqual(258UL, actual);
        }

        [TestMethod]
        public void TestByteArrayRejectsOversizedWrite()
        {
            // Setup
            var handler = new BitToByteArrayPropertyHandler();

            // Act
            Action act = () => handler.Set(new byte[9], null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

    }
}

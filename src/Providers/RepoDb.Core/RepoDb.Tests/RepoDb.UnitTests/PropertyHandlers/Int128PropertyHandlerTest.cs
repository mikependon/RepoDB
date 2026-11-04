#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Numerics;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class Int128PropertyHandlerTest
    {
        [TestMethod]
        public void TestBigIntegerToInt128Null()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.AreEqual(Int128.Zero, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128DatabaseNull()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.AreEqual(Int128.Zero, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128Int32()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Get(42, null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128Int64()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Get(42L, null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128Native()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Get((Int128)42, null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128BigInteger()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Get(new BigInteger(42), null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128MinValue()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();
            var input = (BigInteger)Int128.MinValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(Int128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128WritesMinValue()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Set(Int128.MinValue, null);

            // Assert
            Assert.AreEqual((BigInteger)Int128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128MaxValue()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();
            var input = (BigInteger)Int128.MaxValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(Int128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128WritesMaxValue()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            var actual = handler.Set(Int128.MaxValue, null);

            // Assert
            Assert.AreEqual((BigInteger)Int128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToInt128AboveMaximum()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)Int128.MaxValue + 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToInt128BelowMinimum()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)Int128.MinValue - 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToInt128UnsupportedType()
        {
            // Setup
            var handler = new BigIntegerToInt128PropertyHandler();

            // Act
            Action act = () => handler.Get("42", null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128Null()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128DatabaseNull()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128Int32()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Get(42, null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128Int64()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Get(42L, null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128Native()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Get((Int128)42, null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128BigInteger()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Get(new BigInteger(42), null);

            // Assert
            Assert.AreEqual((Int128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128MinValue()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();
            var input = (BigInteger)Int128.MinValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(Int128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128WritesMinValue()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Set(Int128.MinValue, null);

            // Assert
            Assert.AreEqual((BigInteger)Int128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128MaxValue()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();
            var input = (BigInteger)Int128.MaxValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(Int128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128WritesMaxValue()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Set(Int128.MaxValue, null);

            // Assert
            Assert.AreEqual((BigInteger)Int128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128AboveMaximum()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)Int128.MaxValue + 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128BelowMinimum()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)Int128.MinValue - 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128UnsupportedType()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            Action act = () => handler.Get("42", null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableInt128WritesNull()
        {
            // Setup
            var handler = new BigIntegerToNullableInt128PropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128Null()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.AreEqual(UInt128.Zero, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128DatabaseNull()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.AreEqual(UInt128.Zero, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128Int32()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128Int64()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42L, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128Native()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get((UInt128)42, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128BigInteger()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get(new BigInteger(42), null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128UInt32()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42U, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128UInt64()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42UL, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128MinValue()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();
            var input = (BigInteger)UInt128.MinValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(UInt128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128WritesMinValue()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Set(UInt128.MinValue, null);

            // Assert
            Assert.AreEqual((BigInteger)UInt128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128MaxValue()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();
            var input = (BigInteger)UInt128.MaxValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(UInt128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128WritesMaxValue()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            var actual = handler.Set(UInt128.MaxValue, null);

            // Assert
            Assert.AreEqual((BigInteger)UInt128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128AboveMaximum()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)UInt128.MaxValue + 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128BelowMinimum()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)UInt128.MinValue - 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128UnsupportedType()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get("42", null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128NegativeInt32()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get(-1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToUInt128NegativeInt64()
        {
            // Setup
            var handler = new BigIntegerToUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get(-1L, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128Null()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128DatabaseNull()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128Int32()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128Int64()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42L, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128Native()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get((UInt128)42, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128BigInteger()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get(new BigInteger(42), null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128UInt32()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42U, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128UInt64()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Get(42UL, null);

            // Assert
            Assert.AreEqual((UInt128)42, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128MinValue()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();
            var input = (BigInteger)UInt128.MinValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(UInt128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128WritesMinValue()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Set(UInt128.MinValue, null);

            // Assert
            Assert.AreEqual((BigInteger)UInt128.MinValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128MaxValue()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();
            var input = (BigInteger)UInt128.MaxValue;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreEqual(UInt128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128WritesMaxValue()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Set(UInt128.MaxValue, null);

            // Assert
            Assert.AreEqual((BigInteger)UInt128.MaxValue, actual);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128AboveMaximum()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)UInt128.MaxValue + 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128BelowMinimum()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get((BigInteger)UInt128.MinValue - 1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128UnsupportedType()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get("42", null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128NegativeInt32()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get(-1, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128NegativeInt64()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            Action act = () => handler.Get(-1L, null);

            // Assert
            Assert.ThrowsExactly<OverflowException>(act);
        }

        [TestMethod]
        public void TestBigIntegerToNullableUInt128WritesNull()
        {
            // Setup
            var handler = new BigIntegerToNullableUInt128PropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

    }
}

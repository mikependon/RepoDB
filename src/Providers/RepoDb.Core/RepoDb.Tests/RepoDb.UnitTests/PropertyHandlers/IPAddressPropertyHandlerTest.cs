#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Net;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class IPAddressPropertyHandlerTest
    {
        [TestMethod]
        public void TestIPAddressNull()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestIPAddressDatabaseNull()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get(DBNull.Value, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestIPAddressEmptyText()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestIPAddressEmptyBytes()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get(Array.Empty<byte>(), null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestIPAddressIPv4Text()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get("192.168.1.2", null);

            // Assert
            Assert.AreEqual(IPAddress.Parse("192.168.1.2"), actual);
        }

        [TestMethod]
        public void TestIPAddressIPv6Text()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get("2001:db8::1", null);

            // Assert
            Assert.AreEqual(IPAddress.Parse("2001:db8::1"), actual);
        }

        [TestMethod]
        public void TestIPAddressIPv4Bytes()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get(new byte[] { 192, 168, 1, 2 }, null);

            // Assert
            Assert.AreEqual(IPAddress.Parse("192.168.1.2"), actual);
        }

        [TestMethod]
        public void TestIPAddressIPv6Bytes()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Get(IPAddress.IPv6Loopback.GetAddressBytes(), null);

            // Assert
            Assert.AreEqual(IPAddress.Parse("::1"), actual);
        }

        [TestMethod]
        public void TestIPAddressPreservesInstance()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();
            var input = IPAddress.Loopback;

            // Act
            var actual = handler.Get(input, null);

            // Assert
            Assert.AreSame(input, actual);
        }

        [TestMethod]
        public void TestIPAddressInvalidText()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            Action act = () => handler.Get("invalid", null);

            // Assert
            Assert.ThrowsExactly<FormatException>(act);
        }

        [TestMethod]
        public void TestIPAddressInvalidByteCount()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            Action act = () => handler.Get(new byte[] { 1, 2 }, null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestIPAddressUnsupportedType()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            Action act = () => handler.Get(42, null);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(act);
        }

        [TestMethod]
        public void TestIPAddressWritesNull()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestIPAddressWritesText()
        {
            // Setup
            var handler = new StringToIPAddressPropertyHandler();
            var input = IPAddress.Parse("2001:db8::1");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("2001:db8::1", actual);
        }

    }
}

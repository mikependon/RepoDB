#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Sockets;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Contains the tests of the <c>TraceEnvironment</c> class.
    /// </summary>
    [TestClass]
    public class TraceEnvironmentTest
    {
        #region Public Methods

        [TestMethod]
        public void TestTraceEnvironmentPcNameIsTheNameOfTheMachine()
        {
            // Act
            var actual = TraceEnvironment.PcName;

            // Assert
            Assert.AreEqual(Environment.MachineName, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTraceEnvironmentIpAddressIsAnIPv4AddressThatIsNotALoopbackOrIsNotResolved()
        {
            // Act
            var actual = TraceEnvironment.IpAddress;

            // Assert
            if (actual != null)
            {
                Assert.IsTrue(IPAddress.TryParse(actual, out var address), $"'{actual}' is not an IP address.");
                Assert.AreEqual(AddressFamily.InterNetwork, address.AddressFamily);
                Assert.IsFalse(IPAddress.IsLoopback(address));
            }
        }

        [TestMethod]
        public void TestTraceEnvironmentIpAddressIsResolvedOnce()
        {
            // Act
            var first = TraceEnvironment.IpAddress;
            var second = TraceEnvironment.IpAddress;

            // Assert
            Assert.AreEqual(first, second, StringComparer.Ordinal);
        }

        #endregion
    }
}

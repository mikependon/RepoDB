#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies ExtendedTelemetryGlobalConfiguration behavior.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies registration is idempotent and preserves the original options.
        /// </summary>
        [TestMethod]
        public void ConfigurationRegistersOneSingleton()
        {
            var configuration = GlobalConfiguration.Setup();
            var option = new ExtendedTelemetryOption("PayrollSystem")
            {
                Frequency = Timeout.InfiniteTimeSpan
            };
            Assert.AreSame(configuration,
                configuration.UseExtendedTelemetry(option));
            var trace = ExtendedTelemetryTrace.Instance;
            configuration.UseExtendedTelemetry("http://localhost:5001",
                null,
                "Ignored");
            Assert.AreSame(trace,
                ExtendedTelemetryTrace.Instance);
            Assert.AreSame(option,
                trace.Option);
            Assert.AreEqual(1,
                GlobalTraceRegistration.GetTracers().Count(value => ReferenceEquals(value, trace)));
        }

        /// <summary>
        /// Verifies invalid settings are rejected before registration.
        /// </summary>
        [TestMethod]
        public void NullOptionsAreRejected()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                GlobalConfiguration.Setup().UseExtendedTelemetry((ExtendedTelemetryOption)null));
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                new ExtendedTelemetryTrace(null));
        }

        #endregion
    }
}


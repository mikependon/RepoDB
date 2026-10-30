#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Default;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies ExtendedTelemetryOption behavior.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryOptionTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies inherited defaults and optional enrichment.
        /// </summary>
        [TestMethod]
        public void DefaultsMatchDefaultTelemetry()
        {
            var option = new ExtendedTelemetryOption("PayrollSystem");
            Assert.IsInstanceOfType<DefaultTelemetryOption>(option);
            Assert.AreEqual("PayrollSystem",
                option.Application);
            Assert.AreEqual("Default",
                option.Group);
            Assert.AreEqual("http://localhost:5000",
                option.Host);
            Assert.AreEqual(TimeSpan.FromSeconds(5),
                option.Frequency);
            Assert.IsNull(option.Enrich);
        }

        #endregion
    }
}


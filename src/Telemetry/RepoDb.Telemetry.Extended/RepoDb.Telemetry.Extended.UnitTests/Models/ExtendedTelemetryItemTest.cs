#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Default;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies ExtendedTelemetryItem behavior.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryItemTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies inherited identity and independently allocated custom properties.
        /// </summary>
        [TestMethod]
        public void DefaultsPreserveInheritanceAndUnknownMetadata()
        {
            var first = new ExtendedTelemetryItem();
            var second = new ExtendedTelemetryItem();
            Assert.IsInstanceOfType<DefaultTelemetryItem>(first);
            Assert.AreEqual(LogEventLevel.Information,
                first.Level);
            Assert.IsNull(first.CommandType);
            Assert.IsNull(first.RowsReturned);
            Assert.IsNull(first.RowsAffected);
            Assert.IsNull(first.ThreadId);
            first.Properties["Operation"] = "Delete";
            Assert.IsEmpty(second.Properties);
        }

        #endregion
    }
}


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
        /// Verifies defaults application.
        /// </summary>
        [TestMethod]
        public void DefaultsApplication()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.Application;

            // Assert
            Assert.AreEqual("PayrollSystem",
                actual);
        }

        /// <summary>
        /// Verifies defaults group.
        /// </summary>
        [TestMethod]
        public void DefaultsGroup()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.Group;

            // Assert
            Assert.AreEqual("Default",
                actual);
        }

        /// <summary>
        /// Verifies defaults host.
        /// </summary>
        [TestMethod]
        public void DefaultsHost()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.Host;

            // Assert
            Assert.AreEqual("http://localhost:5000",
                actual);
        }

        /// <summary>
        /// Verifies defaults frequency.
        /// </summary>
        [TestMethod]
        public void DefaultsFrequency()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.Frequency;

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(5),
                actual);
        }

        /// <summary>
        /// Verifies defaults parameter value capture.
        /// </summary>
        [TestMethod]
        public void DefaultsParameterValueCapture()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.CaptureParameterValues;

            // Assert
            Assert.AreEqual(true,
                actual);
        }

        /// <summary>
        /// Verifies defaults result value capture.
        /// </summary>
        [TestMethod]
        public void DefaultsResultValueCapture()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.CaptureResultValues;

            // Assert
            Assert.AreEqual(true,
                actual);
        }

        /// <summary>
        /// Verifies defaults enrichment.
        /// </summary>
        [TestMethod]
        public void DefaultsEnrichment()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.Enrich;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults capture callback.
        /// </summary>
        [TestMethod]
        public void DefaultsCaptureCallback()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.CaptureValue;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults value serializer context.
        /// </summary>
        [TestMethod]
        public void DefaultsValueSerializerContext()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option.ValueSerializerContext;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies inherits default options.
        /// </summary>
        [TestMethod]
        public void InheritsDefaultOptions()
        {
            // Setup
            var option = new ExtendedTelemetryOption("PayrollSystem");

            // Act
            var actual = option is DefaultTelemetryOption;

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}

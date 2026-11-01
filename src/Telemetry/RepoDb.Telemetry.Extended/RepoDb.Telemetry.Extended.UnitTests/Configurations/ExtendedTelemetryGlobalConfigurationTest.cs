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
    /// Verifies singleton registration.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryGlobalConfigurationTest
    {
        #region Private Variables

        private static readonly ExtendedTelemetryOption _option = new ExtendedTelemetryOption("PayrollSystem")
        {
            Frequency = Timeout.InfiniteTimeSpan
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies registration returns configuration.
        /// </summary>
        [TestMethod]
        public void RegistrationReturnsConfiguration()
        {
            // Setup
            var configuration = GlobalConfiguration.Setup();

            // Act
            var actual = configuration.UseExtendedTelemetry(_option);

            // Assert
            Assert.AreSame(configuration,
                actual);
        }

        /// <summary>
        /// Verifies registration preserves singleton.
        /// </summary>
        [TestMethod]
        public void RegistrationPreservesSingleton()
        {
            // Setup
            var configuration = GlobalConfiguration.Setup();
            configuration.UseExtendedTelemetry(_option);
            var original = ExtendedTelemetryTrace.Instance;

            // Act
            configuration.UseExtendedTelemetry("http://localhost:5001",
                null,
                "Ignored");

            // Assert
            Assert.AreSame(original,
                ExtendedTelemetryTrace.Instance);
        }

        /// <summary>
        /// Verifies registration preserves original options.
        /// </summary>
        [TestMethod]
        public void RegistrationPreservesOriginalOptions()
        {
            // Setup
            var configuration = GlobalConfiguration.Setup();

            // Act
            configuration.UseExtendedTelemetry(_option);

            // Assert
            Assert.AreSame(_option,
                ExtendedTelemetryTrace.Instance.Option);
        }

        /// <summary>
        /// Verifies registration adds only one trace.
        /// </summary>
        [TestMethod]
        public void RegistrationAddsOnlyOneTrace()
        {
            // Setup
            var configuration = GlobalConfiguration.Setup();
            configuration.UseExtendedTelemetry(_option);

            // Act
            configuration.UseExtendedTelemetry(_option);
            var actual = GlobalTraceRegistration.GetTracers().Count(value => ReferenceEquals(value,
                ExtendedTelemetryTrace.Instance));

            // Assert
            Assert.AreEqual(1,
                actual);
        }

        /// <summary>
        /// Verifies null options are rejected.
        /// </summary>
        [TestMethod]
        public void NullOptionsAreRejected()
        {
            // Setup
            var configuration = GlobalConfiguration.Setup();

            // Act
            Action action = () => configuration.UseExtendedTelemetry((ExtendedTelemetryOption)null);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        /// <summary>
        /// Verifies null trace options are rejected.
        /// </summary>
        [TestMethod]
        public void NullTraceOptionsAreRejected()
        {
            // Setup
            ExtendedTelemetryOption option = null;

            // Act
            Action action = () => new ExtendedTelemetryTrace(option);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        /// <summary>
        /// Verifies null configuration is rejected.
        /// </summary>
        [TestMethod]
        public void NullConfigurationIsRejected()
        {
            // Setup
            GlobalConfiguration configuration = null;

            // Act
            Action action = () => configuration.UseExtendedTelemetry(_option);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        #endregion
    }
}

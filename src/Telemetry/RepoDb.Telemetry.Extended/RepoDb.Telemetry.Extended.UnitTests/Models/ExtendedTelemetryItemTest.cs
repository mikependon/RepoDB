#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics;
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
        /// Verifies defaults severity.
        /// </summary>
        [TestMethod]
        public void DefaultsSeverity()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.Level;

            // Assert
            Assert.AreEqual(LogEventLevel.Information,
                actual);
        }

        /// <summary>
        /// Verifies defaults command type.
        /// </summary>
        [TestMethod]
        public void DefaultsCommandType()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.CommandType;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults rows returned.
        /// </summary>
        [TestMethod]
        public void DefaultsRowsReturned()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.RowsReturned;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults rows affected.
        /// </summary>
        [TestMethod]
        public void DefaultsRowsAffected()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.RowsAffected;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults thread id.
        /// </summary>
        [TestMethod]
        public void DefaultsThreadId()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.ThreadId;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults connection type.
        /// </summary>
        [TestMethod]
        public void DefaultsConnectionType()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.ConnectionType;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults error message.
        /// </summary>
        [TestMethod]
        public void DefaultsErrorMessage()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.ErrorMessage;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults error stack trace.
        /// </summary>
        [TestMethod]
        public void DefaultsErrorStackTrace()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.ErrorStackTrace;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults error type.
        /// </summary>
        [TestMethod]
        public void DefaultsErrorType()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.ErrorType;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults parameters.
        /// </summary>
        [TestMethod]
        public void DefaultsParameters()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.Parameters;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults result.
        /// </summary>
        [TestMethod]
        public void DefaultsResult()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.Result;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults result type.
        /// </summary>
        [TestMethod]
        public void DefaultsResultType()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.ResultType;

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Verifies defaults trace level.
        /// </summary>
        [TestMethod]
        public void DefaultsTraceLevel()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item.TraceLevel;

            // Assert
            Assert.AreEqual(TraceLevel.Off,
                actual);
        }

        /// <summary>
        /// Verifies assigns connection type.
        /// </summary>
        [TestMethod]
        public void AssignsConnectionType()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            item.ConnectionType = "SqlConnection";

            // Assert
            Assert.AreEqual("SqlConnection",
                item.ConnectionType);
        }

        /// <summary>
        /// Verifies assigns error message.
        /// </summary>
        [TestMethod]
        public void AssignsErrorMessage()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            item.ErrorMessage = "Failure";

            // Assert
            Assert.AreEqual("Failure",
                item.ErrorMessage);
        }

        /// <summary>
        /// Verifies assigns error stack trace.
        /// </summary>
        [TestMethod]
        public void AssignsErrorStackTrace()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            item.ErrorStackTrace = "at Method()";

            // Assert
            Assert.AreEqual("at Method()",
                item.ErrorStackTrace);
        }

        /// <summary>
        /// Verifies assigns error type.
        /// </summary>
        [TestMethod]
        public void AssignsErrorType()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            item.ErrorType = "System.InvalidOperationException";

            // Assert
            Assert.AreEqual("System.InvalidOperationException",
                item.ErrorType);
        }

        /// <summary>
        /// Verifies assigns result type.
        /// </summary>
        [TestMethod]
        public void AssignsResultType()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            item.ResultType = "System.Int32";

            // Assert
            Assert.AreEqual("System.Int32",
                item.ResultType);
        }

        /// <summary>
        /// Verifies allocates independent properties.
        /// </summary>
        [TestMethod]
        public void AllocatesIndependentProperties()
        {
            // Setup
            var first = new ExtendedTelemetryItem();
            var second = new ExtendedTelemetryItem();

            // Act
            first.Properties["Operation"] = "Delete";

            // Assert
            Assert.IsEmpty(second.Properties);
        }

        /// <summary>
        /// Verifies defaults memory and process I/O.
        /// </summary>
        [TestMethod]
        public void DefaultsMemoryAndIo()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Assert
            Assert.IsNull(item.MemoryBytes);
            Assert.IsNull(item.ReadBytes);
            Assert.IsNull(item.WriteBytes);
        }

        /// <summary>
        /// Verifies keeps memory and process I/O.
        /// </summary>
        [TestMethod]
        public void KeepsMemoryAndIo()
        {
            // Act
            var item = new ExtendedTelemetryItem
            {
                MemoryBytes = 1,
                ReadBytes = 2,
                WriteBytes = 3
            };

            // Assert
            Assert.AreEqual(1L, item.MemoryBytes);
            Assert.AreEqual(2L, item.ReadBytes);
            Assert.AreEqual(3L, item.WriteBytes);
        }

        /// <summary>
        /// Verifies inherits default item.
        /// </summary>
        [TestMethod]
        public void InheritsDefaultItem()
        {
            // Setup
            var item = new ExtendedTelemetryItem();

            // Act
            var actual = item is DefaultTelemetryItem;

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}

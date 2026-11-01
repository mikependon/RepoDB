#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies TelemetryParameter behavior.
    /// </summary>
    [TestClass]
    public class TelemetryParameterTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies assigns name.
        /// </summary>
        [TestMethod]
        public void AssignsName()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            string expected = "@Id";
            parameter.Name = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.Name);
        }

        /// <summary>
        /// Verifies assigns db type.
        /// </summary>
        [TestMethod]
        public void AssignsDbType()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            DbType expected = DbType.Int32;
            parameter.DbType = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.DbType);
        }

        /// <summary>
        /// Verifies assigns direction.
        /// </summary>
        [TestMethod]
        public void AssignsDirection()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            ParameterDirection expected = ParameterDirection.Output;
            parameter.Direction = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.Direction);
        }

        /// <summary>
        /// Verifies assigns is nullable.
        /// </summary>
        [TestMethod]
        public void AssignsIsNullable()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            bool expected = true;
            parameter.IsNullable = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.IsNullable);
        }

        /// <summary>
        /// Verifies assigns size.
        /// </summary>
        [TestMethod]
        public void AssignsSize()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            int expected = 4;
            parameter.Size = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.Size);
        }

        /// <summary>
        /// Verifies assigns precision.
        /// </summary>
        [TestMethod]
        public void AssignsPrecision()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            byte expected = (byte)8;
            parameter.Precision = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.Precision);
        }

        /// <summary>
        /// Verifies assigns scale.
        /// </summary>
        [TestMethod]
        public void AssignsScale()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            byte expected = (byte)2;
            parameter.Scale = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.Scale);
        }

        /// <summary>
        /// Verifies assigns source column.
        /// </summary>
        [TestMethod]
        public void AssignsSourceColumn()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            string expected = "Id";
            parameter.SourceColumn = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.SourceColumn);
        }

        /// <summary>
        /// Verifies assigns source version.
        /// </summary>
        [TestMethod]
        public void AssignsSourceVersion()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            DataRowVersion expected = DataRowVersion.Current;
            parameter.SourceVersion = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.SourceVersion);
        }

        /// <summary>
        /// Verifies assigns value type.
        /// </summary>
        [TestMethod]
        public void AssignsValueType()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            string expected = "System.Int32";
            parameter.ValueType = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.ValueType);
        }

        /// <summary>
        /// Verifies assigns value.
        /// </summary>
        [TestMethod]
        public void AssignsValue()
        {
            // Setup
            var parameter = new TelemetryParameter();

            // Act
            JsonElement? expected = JsonSerializer.SerializeToElement(42,
                TestValueJsonSerializerContext.Default.Int32);
            parameter.Value = expected;

            // Assert
            Assert.AreEqual(expected,
                parameter.Value);
        }

        #endregion
    }
}

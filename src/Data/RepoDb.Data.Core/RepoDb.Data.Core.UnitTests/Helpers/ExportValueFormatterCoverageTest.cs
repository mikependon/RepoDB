using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Helpers;
using System;
using System.IO;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class ExportValueFormatterCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableFalseRepresentation()
        {
            // Setup
            object value = false;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("false",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableTrueRepresentation()
        {
            // Setup
            object value = true;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("true",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableGuidRepresentation()
        {
            // Setup
            object value = Guid.Empty;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("00000000-0000-0000-0000-000000000000",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableDateTimeRepresentation()
        {
            // Setup
            object value = DateTime.MinValue;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("0001-01-01T00:00:00.0000000",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableDateTimeOffsetRepresentation()
        {
            // Setup
            object value = DateTimeOffset.MinValue;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("0001-01-01T00:00:00.0000000+00:00",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableTimeSpanRepresentation()
        {
            // Setup
            object value = TimeSpan.FromSeconds(1);

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("00:00:01",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableSingleRepresentation()
        {
            // Setup
            object value = 12.25f;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("12.25",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableDoubleInfinityRepresentation()
        {
            // Setup
            object value = double.NegativeInfinity;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("-Infinity",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableByteArrayRepresentation()
        {
            // Setup
            object value = new byte[] { 1, 2, 3 };

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("AQID",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableEnumRepresentation()
        {
            // Setup
            object value = DayOfWeek.Friday;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual("5",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Formats an export value without culture-dependent output.
        /// </summary>
        [TestMethod]
        public void ToTextUsesPortableDatabaseNullRepresentation()
        {
            // Setup
            object value = DBNull.Value;

            // Act
            var actual = ExportValueFormatter.ToText(value);

            // Assert
            Assert.AreEqual(null,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Encodes binary content from the current stream position.
        /// </summary>
        [TestMethod]
        public void ToTextReadsRemainingBinaryStream()
        {
            // Setup
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            stream.Position = 1;

            // Act
            var actual = ExportValueFormatter.ToText(stream);

            // Assert
            Assert.AreEqual("AgM=",
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}

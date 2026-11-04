using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Helpers;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class JsonRowWriterCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesByteRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = byte.MaxValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("255",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesSByteRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = sbyte.MinValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("-128",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesInt16Representation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = short.MinValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("-32768",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesUInt16Representation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = ushort.MaxValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("65535",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesInt32Representation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = int.MinValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("-2147483648",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesUInt32Representation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = uint.MaxValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("4294967295",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesInt64Representation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = long.MinValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("-9223372036854775808",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesUInt64Representation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = ulong.MaxValue;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("18446744073709551615",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesDecimalRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = 12.25m;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("12.25",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesDoubleRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = 12.25d;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("12.25",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesSingleRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = 12.25f;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("12.25",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesDoubleNaNRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = double.NaN;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("\"NaN\"",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesDoubleInfinityRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = double.PositiveInfinity;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("\"Infinity\"",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesSingleInfinityRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = float.NegativeInfinity;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("\"-Infinity\"",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesSingleNaNRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = float.NaN;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("\"NaN\"",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesCharRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = 'a';

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("\"a\"",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesEnumRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = DayOfWeek.Friday;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("5",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesGuidRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = Guid.Empty;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("\"00000000-0000-0000-0000-000000000000\"",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the exact JSON representation of a scalar.
        /// </summary>
        [TestMethod]
        public void WriteValuePreservesTimeSpanRepresentation()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            object value = TimeSpan.Zero;

            // Act
            JsonRowWriter.WriteValue(writer,
                value);
            writer.Flush();
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.AreEqual("\"00:00:00\"",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Writes the complete contents of a seekable binary stream.
        /// </summary>
        [TestMethod]
        public void WriteValueRewindsBinaryStream()
        {
            // Setup
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            using var binary = new MemoryStream(new byte[] { 1, 2, 3 });
            binary.Position = 2;

            // Act
            JsonRowWriter.WriteValue(writer,
                binary);
            writer.Flush();
            using var actual = JsonDocument.Parse(stream.ToArray());

            // Assert
            Assert.AreEqual("AQID",
                actual.RootElement.GetString(),
                StringComparer.Ordinal);
        }

        #endregion
    }
}

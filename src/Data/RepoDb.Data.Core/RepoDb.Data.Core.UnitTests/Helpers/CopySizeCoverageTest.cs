using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Helpers;
using System;
using System.Collections.Generic;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class CopySizeCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsNullPayload()
        {
            // Setup
            object value = null;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(0L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsDatabaseNullPayload()
        {
            // Setup
            object value = DBNull.Value;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(0L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsEmptyStringPayload()
        {
            // Setup
            object value = string.Empty;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(0L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsUnicodeTextPayload()
        {
            // Setup
            object value = "text";

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(8L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsByteArrayPayload()
        {
            // Setup
            object value = new byte[] { 1, 2, 3 };

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(3L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsCharArrayPayload()
        {
            // Setup
            object value = new char[] { 'a', 'b' };

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(4L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsGuidPayload()
        {
            // Setup
            object value = Guid.Empty;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(16L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsDecimalPayload()
        {
            // Setup
            object value = decimal.MaxValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(16L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsDateTimeOffsetPayload()
        {
            // Setup
            object value = DateTimeOffset.MinValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(16L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsDateTimePayload()
        {
            // Setup
            object value = DateTime.MinValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(8L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsTimeSpanPayload()
        {
            // Setup
            object value = TimeSpan.Zero;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(8L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsInt64Payload()
        {
            // Setup
            object value = long.MaxValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(8L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsUInt64Payload()
        {
            // Setup
            object value = ulong.MaxValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(8L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsDoublePayload()
        {
            // Setup
            object value = double.NaN;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(8L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsUInt32Payload()
        {
            // Setup
            object value = uint.MaxValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(4L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsInt32Payload()
        {
            // Setup
            object value = int.MinValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(4L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsSinglePayload()
        {
            // Setup
            object value = float.NaN;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(4L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsInt16Payload()
        {
            // Setup
            object value = short.MinValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(2L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsUInt16Payload()
        {
            // Setup
            object value = ushort.MaxValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(2L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsCharPayload()
        {
            // Setup
            object value = 'a';

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(2L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsBytePayload()
        {
            // Setup
            object value = byte.MaxValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(1L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsSBytePayload()
        {
            // Setup
            object value = sbyte.MinValue;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(1L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsBooleanPayload()
        {
            // Setup
            object value = true;

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(1L,
                actual);
        }

        /// <summary>
        /// Estimates a supported payload representation.
        /// </summary>
        [TestMethod]
        public void OfValueCountsUnknownObjectPayload()
        {
            // Setup
            object value = new object();

            // Act
            var actual = CopySize.OfValue(value);

            // Assert
            Assert.AreEqual(0L,
                actual);
        }

        /// <summary>
        /// Sums all row values without retaining completed batches.
        /// </summary>
        [TestMethod]
        public void OfRowsCombinesValuesAcrossRows()
        {
            // Setup
            var rows = new List<object> { new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 1, ["Name"] = "AB" }, new Dictionary<string, object>(StringComparer.Ordinal) { ["Id"] = 2, ["Name"] = null } };

            // Act
            var actual = CopySize.OfRows(rows);

            // Assert
            Assert.AreEqual(12L,
                actual);
        }

        #endregion
    }
}

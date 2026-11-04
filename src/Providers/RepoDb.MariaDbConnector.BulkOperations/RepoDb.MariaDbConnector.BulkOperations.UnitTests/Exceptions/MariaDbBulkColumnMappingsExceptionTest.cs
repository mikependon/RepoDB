using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Enumerations.MariaDb;
using System;

namespace RepoDb.MariaDbConnector.BulkOperations.UnitTests.Exceptions
{
    /// <summary>
    /// Covers column mapping diagnostics.
    /// </summary>
    [TestClass]
    public class MariaDbBulkColumnMappingsExceptionTest
    {
        #region Public Methods

        /// <summary>
        /// Describes a column mismatch by default.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorExplainsMismatch()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException();

            // Assert
            StringAssert.Contains(exception.Message,
                "not aligned",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Preserves a custom diagnostic message.
        /// </summary>
        [TestMethod]
        public void MessageConstructorPreservesMessage()
        {
            // Setup
            const string message = "Custom mismatch";

            // Act
            var exception = new MariaDbBulkColumnMappingsException(message);

            // Assert
            Assert.AreEqual(message,
                exception.Message,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Preserves the original failure.
        /// </summary>
        [TestMethod]
        public void InnerExceptionConstructorPreservesCause()
        {
            // Setup
            var cause = new InvalidOperationException("Original failure");

            // Act
            var exception = new MariaDbBulkColumnMappingsException("Mismatch",
                cause);

            // Assert
            Assert.AreSame(cause,
                exception.InnerException);
        }

        /// <summary>
        /// Initializes missing columns to an empty collection.
        /// </summary>
        [TestMethod]
        public void MessageConstructorInitializesSourceColumnsNotInDestination()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Mismatch",
                new InvalidOperationException());

            // Assert
            Assert.HasCount(0,
                exception.SourceColumnsNotInDestination);
        }

        /// <summary>
        /// Treats absent column lists as empty.
        /// </summary>
        [TestMethod]
        public void NullColumnsInitializeSourceColumnsNotInDestination()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                null,
                null);

            // Assert
            Assert.HasCount(0,
                exception.SourceColumnsNotInDestination);
        }

        /// <summary>
        /// Initializes missing columns to an empty collection.
        /// </summary>
        [TestMethod]
        public void MessageConstructorInitializesDestinationColumnsNotInSource()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Mismatch",
                new InvalidOperationException());

            // Assert
            Assert.HasCount(0,
                exception.DestinationColumnsNotInSource);
        }

        /// <summary>
        /// Treats absent column lists as empty.
        /// </summary>
        [TestMethod]
        public void NullColumnsInitializeDestinationColumnsNotInSource()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                null,
                null);

            // Assert
            Assert.HasCount(0,
                exception.DestinationColumnsNotInSource);
        }

        /// <summary>
        /// Preserves the destination table.
        /// </summary>
        [TestMethod]
        public void DetailedConstructorPreservesTable()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                new[] { "Extra" },
                new[] { "Missing" });

            // Assert
            Assert.AreEqual("Orders",
                exception.TableName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Preserves the violated mapping behavior.
        /// </summary>
        [TestMethod]
        public void DetailedConstructorPreservesBehavior()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                new[] { "Extra" },
                new[] { "Missing" });

            // Assert
            Assert.AreEqual(MariaDbBulkColumnMappingsBehavior.Strict,
                exception.Behavior);
        }

        /// <summary>
        /// Identifies unmapped source columns.
        /// </summary>
        [TestMethod]
        public void DetailedConstructorDescribesExtraColumns()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                new[] { "Extra" },
                new[] { "Missing" });

            // Assert
            StringAssert.Contains(exception.Message,
                "'Extra'",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Identifies missing destination columns.
        /// </summary>
        [TestMethod]
        public void DetailedConstructorDescribesMissingColumns()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                new[] { "Extra" },
                new[] { "Missing" });

            // Assert
            StringAssert.Contains(exception.Message,
                "'Missing'",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Copies supplied columns independently of later changes.
        /// </summary>
        [TestMethod]
        public void DetailedConstructorCopiesSourceColumnsNotInDestination()
        {
            // Setup
            var columns = new[] { "Original" };

            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                columns,
                columns);
            columns[0] = "Changed";

            // Assert
            Assert.AreEqual("Original",
                exception.SourceColumnsNotInDestination[0],
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Copies supplied columns independently of later changes.
        /// </summary>
        [TestMethod]
        public void DetailedConstructorCopiesDestinationColumnsNotInSource()
        {
            // Setup
            var columns = new[] { "Original" };

            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                columns,
                columns);
            columns[0] = "Changed";

            // Assert
            Assert.AreEqual("Original",
                exception.DestinationColumnsNotInSource[0],
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Records an order mismatch without column lists.
        /// </summary>
        [TestMethod]
        public void OrderMismatchPreservesFlag()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                null,
                null,
                true);

            // Assert
            Assert.IsTrue(exception.IsOrderMismatch);
        }

        /// <summary>
        /// Describes the expected and actual column order.
        /// </summary>
        [TestMethod]
        public void OrderMismatchDescribesExpectedOrder()
        {
            // Act
            var exception = new MariaDbBulkColumnMappingsException("Orders",
                MariaDbBulkColumnMappingsBehavior.Strict,
                Array.Empty<string>(),
                Array.Empty<string>(),
                true,
                new[] { "Name", "Id" },
                new[] { "Id", "Name" });

            // Assert
            StringAssert.Contains(exception.Message,
                "Expected: 'Id', 'Name'. Actual: 'Name', 'Id'.",
                StringComparison.Ordinal);
        }

        #endregion
    }
}
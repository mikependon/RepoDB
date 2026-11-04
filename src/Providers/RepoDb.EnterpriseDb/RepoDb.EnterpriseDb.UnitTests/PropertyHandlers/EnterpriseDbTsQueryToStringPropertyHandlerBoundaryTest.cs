using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.EnterpriseDb;
using System;

namespace RepoDb.EnterpriseDb.UnitTests.PropertyHandlers
{
    /// <summary>
    /// Covers property conversion boundaries.
    /// </summary>
    [TestClass]
    public class EnterpriseDbTsQueryToStringPropertyHandlerBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves an absent database value.
        /// </summary>
        [TestMethod]
        public void GetNullReturnsNull()
        {
            // Setup
            var handler = new EnterpriseDbTsQueryToStringPropertyHandler();


            // Act
            var actual = handler.Get(null,
                null);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Treats a database null as absent.
        /// </summary>
        [TestMethod]
        public void GetDbNullReturnsNull()
        {
            // Setup
            var handler = new EnterpriseDbTsQueryToStringPropertyHandler();


            // Act
            var actual = handler.Get(DBNull.Value,
                null);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Passes through a text representation.
        /// </summary>
        [TestMethod]
        public void GetTextReturnsText()
        {
            // Setup
            var handler = new EnterpriseDbTsQueryToStringPropertyHandler();


            // Act
            var actual = handler.Get("'term'",
                null);


            // Assert
            Assert.AreEqual("'term'",
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects an unsupported database representation.
        /// </summary>
        [TestMethod]
        public void GetUnsupportedValueReportsArgument()
        {
            // Setup
            var handler = new EnterpriseDbTsQueryToStringPropertyHandler();


            // Act
            var exception = Assert.ThrowsExactly<ArgumentException>(() => handler.Get(42,
                null));


            // Assert
            Assert.AreEqual("input",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Preserves absent text when writing.
        /// </summary>
        [TestMethod]
        public void SetNullReturnsNull()
        {
            // Setup
            var handler = new EnterpriseDbTsQueryToStringPropertyHandler();


            // Act
            var actual = handler.Set(null,
                null);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Round-trips a simple full-text search term.
        /// </summary>
        [TestMethod]
        public void SetThenGetPreservesSimpleTerm()
        {
            // Setup
            var handler = new EnterpriseDbTsQueryToStringPropertyHandler();
            var value = handler.Set("'term'",
                null);


            // Act
            var actual = handler.Get(value,
                null);


            // Assert
            Assert.AreEqual("'term'",
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}
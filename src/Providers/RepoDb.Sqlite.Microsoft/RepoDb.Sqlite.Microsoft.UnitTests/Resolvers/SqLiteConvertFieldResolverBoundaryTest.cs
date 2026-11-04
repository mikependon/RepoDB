using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Sqlite.Microsoft.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class SqLiteConvertFieldResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Allows an absent field.
        /// </summary>
        [TestMethod]
        public void ResolveNullFieldReturnsNull()
        {
            // Setup
            var resolver = new SqLiteConvertFieldResolver();
            var setting = new SqLiteDbSetting();


            // Act
            var actual = resolver.Resolve(null,
                setting);


            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Quotes an untyped field without a conversion.
        /// </summary>
        [TestMethod]
        public void ResolveUntypedFieldQuotesName()
        {
            // Setup
            var resolver = new SqLiteConvertFieldResolver();
            var setting = new SqLiteDbSetting();
            var field = new Field("Value");


            // Act
            var actual = resolver.Resolve(field,
                setting);


            // Assert
            Assert.AreEqual(setting.OpeningQuote + "Value" + setting.ClosingQuote,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Quotes a field when its CLR type has no database mapping.
        /// </summary>
        [TestMethod]
        public void ResolveUnmappedTypeQuotesName()
        {
            // Setup
            var resolver = new SqLiteConvertFieldResolver();
            var setting = new SqLiteDbSetting();
            var field = new Field("Value",
                typeof(Uri));


            // Act
            var actual = resolver.Resolve(field,
                setting);


            // Assert
            Assert.AreEqual(setting.OpeningQuote + "Value" + setting.ClosingQuote,
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}
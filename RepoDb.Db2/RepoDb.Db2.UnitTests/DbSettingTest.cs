using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.DbSettings;

namespace RepoDb.Db2.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDb2();
        }

        [TestMethod]
        public void TestDb2DbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestDb2DbSettingAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.AverageableType);
        }

        [TestMethod]
        public void TestDb2DbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestDb2DbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestDb2DbSettingIsDirectionSupportedSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestDb2DbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestDb2DbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert - Db2's IBM Data Server .NET Provider executes multi-statement command text
            // in a single round trip (confirmed live - see ExecuteQueryMultipleTest.cs and
            // Db2StatementBuilder.cs's CreateInsertAll/CreateMergeAll/CreateUpdateAll).
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestDb2DbSettingQueryMultipleSeparatorProperty()
        {
            // Setup
            var setting = (BaseDbSetting)DbSettingMapper.Get<DB2Connection>();

            // Assert - RepoDb.Core's QueryMultiple/QueryMultipleAsync join each type's
            // independently-built CreateQuery() text using this separator. Db2's CreateQuery
            // deliberately never self-terminates its own output with a trailing " ;" (unlike
            // every other provider's default CreateQuery), so the base class's default " "
            // separator would join two Db2 queries with no delimiter between them at all -
            // confirmed live, this fails with SQL0104N. "; " matches the interior-separator,
            // no-trailing-terminator pattern already confirmed working for Db2 multi-statement
            // command text elsewhere in this provider (see ExecuteQueryMultipleTest.cs and
            // Db2StatementBuilder.WrapMergeWithReturningResult).
            Assert.AreEqual("; ", setting.MultiStatementSeparator);
        }

        [TestMethod]
        public void TestDb2DbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestDb2DbSettingIsPreparableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsTrue(setting.IsPreparable);
        }

        [TestMethod]
        public void TestDb2DbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestDb2DbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual(":", setting.ParameterPrefix);
        }
    }
}

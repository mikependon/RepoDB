using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Options;
using System;
using System.Diagnostics;

namespace RepoDb.UnitTests.Trace
{
    [TestClass]
    [DoNotParallelize]
    public class TraceWarningThresholdTest
    {
        [TestMethod]
        public void ThresholdDefaultsToOneSecondAndCanBeConfigured()
        {
            var options = new GlobalConfigurationOptions();
            Assert.AreEqual(TimeSpan.FromSeconds(1), options.TraceWarningThreshold);
            options.TraceWarningThreshold = TimeSpan.FromSeconds(2);
            Assert.AreEqual(TimeSpan.FromSeconds(2), options.TraceWarningThreshold);
            options.TraceWarningThreshold = null;
            Assert.IsNull(options.TraceWarningThreshold);
        }

        [DataTestMethod]
        [DataRow(null, 3000, false, TraceLevel.Info)]
        [DataRow(2000, 1999, false, TraceLevel.Info)]
        [DataRow(2000, 2000, false, TraceLevel.Warning)]
        [DataRow(2000, 3000, false, TraceLevel.Warning)]
        [DataRow(2000, null, false, TraceLevel.Info)]
        [DataRow(2000, 3000, true, TraceLevel.Error)]
        [DataRow(2000, 100, true, TraceLevel.Error)]
        public void CompletionLevelReflectsDurationAndError(int? thresholdMilliseconds,
            int? elapsedMilliseconds, bool failed, TraceLevel expected)
        {
            var options = GlobalConfiguration.Options;
            var previous = options.TraceWarningThreshold;
            try
            {
                options.TraceWarningThreshold = thresholdMilliseconds.HasValue
                    ? TimeSpan.FromMilliseconds(thresholdMilliseconds.Value) : null;
                var error = failed ? new InvalidOperationException("Execution failed") : null;
                var log = new ResultTraceLog<int>(Guid.NewGuid(), "ExecuteNonQuery",
                    elapsedMilliseconds.HasValue ? TimeSpan.FromMilliseconds(elapsedMilliseconds.Value) : null,
                    0, null, error);

                Assert.AreEqual(expected, log.Level);
                Assert.AreSame(error, log.Error);
            }
            finally
            {
                options.TraceWarningThreshold = previous;
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb;
using RepoDb.Interfaces;
using RepoDb.Telemetry.Core;
using RepoDb.Telemetry.Default;

namespace RepoDb.Telemetry.Default.UnitTests.Traces
{
    [TestClass]
    public class DefaultTelemetryTraceTest
    {
        // DefaultTelemetryTrace's constructor and 'Create' factory method are both internal; the only
        // supported way to create/reach the singleton from outside the assembly is via the public
        // 'UseDefaultTelemetry' extension method, followed by the public 'Instance' property. Each test
        // triggers creation itself (Create/UseDefaultTelemetry is idempotent) so the assertions below
        // do not depend on other tests having run first.

        [TestMethod]
        public void TestDefaultTelemetryTraceInstanceIsATelemetryTrace()
        {
            // Act
            GlobalConfiguration.Setup().UseDefaultTelemetry(host: "http://127.0.0.1:1", apiKey: null, applicationName: "MyApplication");
            var actual = DefaultTelemetryTrace.Instance is TelemetryTrace;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryTraceInstanceImplementsITrace()
        {
            // Act
            GlobalConfiguration.Setup().UseDefaultTelemetry(host: "http://127.0.0.1:1", apiKey: null, applicationName: "MyApplication");
            var actual = DefaultTelemetryTrace.Instance is ITrace;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryTraceInstanceReturnsTheSameInstanceOnSubsequentCalls()
        {
            // Act
            GlobalConfiguration.Setup().UseDefaultTelemetry(host: "http://127.0.0.1:1", apiKey: null, applicationName: "MyApplication");
            var first = DefaultTelemetryTrace.Instance;
            var second = DefaultTelemetryTrace.Instance;
            var actual = ReferenceEquals(first, second);
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb;
using RepoDb.Telemetry.Default;

namespace RepoDb.Telemetry.Default.UnitTests
{
    [TestClass]
    public class DefaultTelemetryGlobalConfigurationTest
    {
        // The port below is intentionally unassigned on the loopback address, so the background
        // publisher never actually reaches out over the network during these tests.
        private const string UnreachableHost = "http://127.0.0.1:1";

        [TestMethod]
        public void TestUseDefaultTelemetryWithDiscreteArgumentsReturnsTheSameGlobalConfigurationInstance()
        {
            // Act
            var globalConfiguration = GlobalConfiguration.Setup();
            var actual = globalConfiguration.UseDefaultTelemetry(host: UnreachableHost,
                apiKey: "12345",
                applicationName: "MyApplication");
            var expected = globalConfiguration;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestUseDefaultTelemetryWithOptionReturnsTheSameGlobalConfigurationInstance()
        {
            // Act
            var globalConfiguration = GlobalConfiguration.Setup();
            var option = new DefaultTelemetryOption("MyApplication")
            {
                Host = UnreachableHost
            };
            var actual = globalConfiguration.UseDefaultTelemetry(option);
            var expected = globalConfiguration;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestUseDefaultTelemetryWithDiscreteArgumentsRegistersAGlobalTrace()
        {
            // Act
            GlobalConfiguration.Setup().UseDefaultTelemetry(host: UnreachableHost,
                apiKey: "12345",
                applicationName: "MyApplication");
            var actual = GlobalTraceRegistration.GetTracers().Count > 0;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestUseDefaultTelemetryWithOptionRegistersAGlobalTrace()
        {
            // Act
            GlobalConfiguration.Setup().UseDefaultTelemetry(new DefaultTelemetryOption("MyApplication")
            {
                Host = UnreachableHost
            });
            var actual = GlobalTraceRegistration.GetTracers().Count > 0;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestUseDefaultTelemetryRegistersADefaultTelemetryTraceInstance()
        {
            // Act
            GlobalConfiguration.Setup().UseDefaultTelemetry(host: UnreachableHost,
                apiKey: "12345",
                applicationName: "MyApplication");
            var actual = GlobalTraceRegistration.GetTracers().Last() is DefaultTelemetryTrace;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

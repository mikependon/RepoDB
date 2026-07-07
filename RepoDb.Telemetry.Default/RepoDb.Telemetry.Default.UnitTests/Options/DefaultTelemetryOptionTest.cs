using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;
using RepoDb.Telemetry.Default;

namespace RepoDb.Telemetry.Default.UnitTests.Options
{
    [TestClass]
    public class DefaultTelemetryOptionTest
    {
        [TestMethod]
        public void TestDefaultTelemetryOptionIsATelemetryOption()
        {
            // Act
            var option = new DefaultTelemetryOption("MyApplication");
            var actual = option is TelemetryOption;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryOptionApplicationProperty()
        {
            // Act
            var option = new DefaultTelemetryOption("MyApplication");
            var actual = option.Application;
            var expected = "MyApplication";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryOptionGroupPropertyDefaultValue()
        {
            // Act
            var option = new DefaultTelemetryOption("MyApplication");
            var actual = option.Group;
            var expected = "Default";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryOptionHostPropertyDefaultValue()
        {
            // Act
            var option = new DefaultTelemetryOption("MyApplication");
            var actual = option.Host;
            var expected = "http://localhost:5000";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryOptionFrequencyPropertyDefaultValue()
        {
            // Act
            var option = new DefaultTelemetryOption("MyApplication");
            var actual = option.Frequency;
            var expected = TimeSpan.FromSeconds(5);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryOptionApiKeyPropertyIsSettable()
        {
            // Act
            var option = new DefaultTelemetryOption("MyApplication")
            {
                ApiKey = "12345"
            };
            var actual = option.ApiKey;
            var expected = "12345";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

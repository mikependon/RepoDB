using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;

namespace RepoDb.Telemetry.Core.UnitTests.Options
{
    [TestClass]
    public class TelemetryOptionTest
    {
        [TestMethod]
        public void TestTelemetryOptionApplicationProperty()
        {
            // Act
            var option = new TelemetryOption("MyApplication");
            var actual = option.Application;
            var expected = "MyApplication";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionGroupPropertyDefaultValue()
        {
            // Act
            var option = new TelemetryOption("MyApplication");
            var actual = option.Group;
            var expected = "Default";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionHostPropertyDefaultValue()
        {
            // Act
            var option = new TelemetryOption("MyApplication");
            var actual = option.Host;
            var expected = "http://localhost:5000";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionApiKeyPropertyDefaultValue()
        {
            // Act
            var option = new TelemetryOption("MyApplication");
            var actual = option.ApiKey;
            string expected = null;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionFrequencyPropertyDefaultValue()
        {
            // Act
            var option = new TelemetryOption("MyApplication");
            var actual = option.Frequency;
            var expected = TimeSpan.FromSeconds(5);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionGroupPropertyIsSettable()
        {
            // Act
            var option = new TelemetryOption("MyApplication")
            {
                Group = "Reporting"
            };
            var actual = option.Group;
            var expected = "Reporting";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionHostPropertyIsSettable()
        {
            // Act
            var option = new TelemetryOption("MyApplication")
            {
                Host = "https://collector.example.com"
            };
            var actual = option.Host;
            var expected = "https://collector.example.com";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionApiKeyPropertyIsSettable()
        {
            // Act
            var option = new TelemetryOption("MyApplication")
            {
                ApiKey = "12345"
            };
            var actual = option.ApiKey;
            var expected = "12345";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryOptionFrequencyPropertyIsSettable()
        {
            // Act
            var option = new TelemetryOption("MyApplication")
            {
                Frequency = TimeSpan.FromSeconds(30)
            };
            var actual = option.Frequency;
            var expected = TimeSpan.FromSeconds(30);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

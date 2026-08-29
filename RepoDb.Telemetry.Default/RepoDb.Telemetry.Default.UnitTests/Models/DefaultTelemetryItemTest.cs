using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;
using RepoDb.Telemetry.Default;

namespace RepoDb.Telemetry.Default.UnitTests.Models
{
    [TestClass]
    public class DefaultTelemetryItemTest
    {
        [TestMethod]
        public void TestDefaultTelemetryItemIsATelemetryItem()
        {
            // Act
            var item = new DefaultTelemetryItem();
            var actual = item is TelemetryItem;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryItemGroupPropertyDefaultValue()
        {
            // Act
            var item = new DefaultTelemetryItem();
            var actual = item.Group;
            var expected = "Default";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryItemApplicationProperty()
        {
            // Act
            var item = new DefaultTelemetryItem
            {
                Application = "MyApplication"
            };
            var actual = item.Application;
            var expected = "MyApplication";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

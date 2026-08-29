using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;

namespace RepoDb.Telemetry.Core.UnitTests.Models
{
    [TestClass]
    public class TelemetryItemTest
    {
        [TestMethod]
        public void TestTelemetryItemGroupPropertyDefaultValue()
        {
            // Act
            var item = new TelemetryItem();
            var actual = item.Group;
            var expected = "Default";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemGroupProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Group = "Reporting"
            };
            var actual = item.Group;
            var expected = "Reporting";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemApplicationProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Application = "MyApplication"
            };
            var actual = item.Application;
            var expected = "MyApplication";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemSessionIdProperty()
        {
            // Act
            var sessionId = Guid.NewGuid();
            var item = new TelemetryItem
            {
                SessionId = sessionId
            };
            var actual = item.SessionId;
            var expected = sessionId;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemOperationProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Operation = "Query"
            };
            var actual = item.Operation;
            var expected = "Query";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemStartTimeProperty()
        {
            // Act
            var startTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var item = new TelemetryItem
            {
                StartTime = startTime
            };
            var actual = item.StartTime;
            var expected = startTime;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemStatementProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Statement = "SELECT * FROM [Table];"
            };
            var actual = item.Statement;
            var expected = "SELECT * FROM [Table];";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemElapsedProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Elapsed = 123.45
            };
            var actual = item.Elapsed;
            var expected = 123.45;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemIsCancelledProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                IsCancelled = true
            };
            var actual = item.IsCancelled;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemClientProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Client = "MyMachine"
            };
            var actual = item.Client;
            var expected = "MyMachine";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemSourceProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Source = "RepoDb.Telemetry.Core"
            };
            var actual = item.Source;
            var expected = "RepoDb.Telemetry.Core";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryItemVersionProperty()
        {
            // Act
            var item = new TelemetryItem
            {
                Version = "1.0.0"
            };
            var actual = item.Version;
            var expected = "1.0.0";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

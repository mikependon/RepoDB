using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace RepoDb.Telemetry.Core.UnitTests.Repositories
{
    [TestClass]
    public class TelemetryPublisherRepositoryTest
    {
        #region Classes

        // TelemetryPublisherRepository is abstract, so it cannot be instantiated directly. This
        // minimal test double exists solely so the base class' behavior (Publish/PublishMany/etc.)
        // can be exercised directly.
        private sealed class InspectableTelemetryPublisherRepository : TelemetryPublisherRepository
        {
            public InspectableTelemetryPublisherRepository(
                string host = "http://localhost:5000",
                string apiKey = null,
                Action<Exception> errorCallback = null,
                ILogger logger = null)
                : base(host, apiKey, errorCallback, logger)
            { }

            public override string GetRequestUri() => "v1/telemetry/test";
        }

        #endregion

        // The port below is intentionally unassigned on the loopback address, so any connection
        // attempt against it fails immediately (connection refused) without needing real network
        // access. This lets us deterministically exercise the repository's failure-handling path.
        private const string UnreachableHost = "http://127.0.0.1:1";

        [TestMethod]
        public void TestTelemetryPublisherRepositoryImplementsIPublisherRepository()
        {
            // Act
            var repository = new InspectableTelemetryPublisherRepository();
            var actual = repository is IPublisherRepository;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryPublisherRepositoryConstructorDoesNotThrowWithDefaults()
        {
            // Act
            var repository = new InspectableTelemetryPublisherRepository();
            var actual = repository != null;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryPublisherRepositoryPublishRoutesFailureToErrorCallback()
        {
            // Act
            var invoked = false;
            var repository = new InspectableTelemetryPublisherRepository(host: UnreachableHost,
                errorCallback: (ex) => invoked = true);
            repository.Publish(new TelemetryItem
            {
                Application = "MyApplication"
            });
            var actual = invoked;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryPublisherRepositoryPublishManyRoutesFailureToErrorCallback()
        {
            // Act
            var invoked = false;
            var repository = new InspectableTelemetryPublisherRepository(host: UnreachableHost,
                errorCallback: (ex) => invoked = true);
            repository.PublishMany(new[]
            {
                new TelemetryItem { Application = "MyApplication" },
                new TelemetryItem { Application = "MyOtherApplication" }
            });
            var actual = invoked;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryPublisherRepositoryPublishDoesNotThrowWhenNoErrorCallbackIsProvided()
        {
            // Act
            var repository = new InspectableTelemetryPublisherRepository(host: UnreachableHost);
            repository.Publish(new TelemetryItem
            {
                Application = "MyApplication"
            });
            var actual = true;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

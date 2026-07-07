using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;

namespace RepoDb.Telemetry.Core.UnitTests.Repositories
{
    [TestClass]
    public class TelemetryPublisherRepositoryTest
    {
        // The port below is intentionally unassigned on the loopback address, so any connection
        // attempt against it fails immediately (connection refused) without needing real network
        // access. This lets us deterministically exercise the repository's failure-handling path.
        private const string UnreachableHost = "http://127.0.0.1:1";

        [TestMethod]
        public void TestTelemetryPublisherRepositoryImplementsIPublisherRepository()
        {
            // Act
            var repository = new TelemetryPublisherRepository();
            var actual = repository is IPublisherRepository;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryPublisherRepositoryConstructorDoesNotThrowWithDefaults()
        {
            // Act
            var repository = new TelemetryPublisherRepository();
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
            var repository = new TelemetryPublisherRepository(host: UnreachableHost,
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
            var repository = new TelemetryPublisherRepository(host: UnreachableHost,
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
            var repository = new TelemetryPublisherRepository(host: UnreachableHost);
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

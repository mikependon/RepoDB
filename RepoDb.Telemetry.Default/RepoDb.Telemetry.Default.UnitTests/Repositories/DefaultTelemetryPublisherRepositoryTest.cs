using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;
using RepoDb.Telemetry.Default;

namespace RepoDb.Telemetry.Default.UnitTests.Repositories
{
    [TestClass]
    public class DefaultTelemetryPublisherRepositoryTest
    {
        // Intentionally unassigned loopback port, so any connection attempt is refused immediately
        // without needing real network access.
        private const string UnreachableHost = "http://127.0.0.1:1";

        [TestMethod]
        public void TestDefaultTelemetryPublisherRepositoryIsATelemetryPublisherRepository()
        {
            // Act
            var repository = new DefaultTelemetryPublisherRepository();
            var actual = repository is TelemetryPublisherRepository;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryPublisherRepositoryImplementsIPublisherRepository()
        {
            // Act
            var repository = new DefaultTelemetryPublisherRepository();
            var actual = repository is IPublisherRepository;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDefaultTelemetryPublisherRepositoryPublishRoutesFailureToErrorCallback()
        {
            // Act
            var invoked = false;
            var repository = new DefaultTelemetryPublisherRepository(host: UnreachableHost,
                errorCallback: (ex) => invoked = true);
            repository.Publish(new DefaultTelemetryItem
            {
                Application = "MyApplication"
            });
            var actual = invoked;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

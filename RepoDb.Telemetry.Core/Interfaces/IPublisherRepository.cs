using System.Collections.Generic;
using System.Threading;
using RepoDb.Telemetry.Core.Models;

namespace RepoDb.Telemetry.Core.Repositories
{
    /// <summary>
    /// An interface that is used to publish the telemetry items to the insights solution.
    /// </summary>
    public interface IPublisherRepository
    {
        /// <summary>
        /// A method that is used to publish a telemetry item to the insights solution.
        /// </summary>
        /// <param name="telemetryItem">The telemetry data to publish.</param>
        public void Publish(
            TelemetryItem telemetryItem);

        /// <summary>
        /// A method that is used to publish a telemetry item to the insights solution in an asynchronous way.
        /// </summary>
        /// <param name="telemetryItem">The telemetry data to publish.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public void PublishAsync(TelemetryItem telemetryItem,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// A method that is used to publish multiple telemetry items to the insights solution.
        /// </summary>
        /// <param name="telemetryItems">The telemetry items to publish.</param>
        public void PublishMany(
            IEnumerable<TelemetryItem> telemetryItems);

        /// <summary>
        /// A method that is used to publish multiple telemetry items to the insights solution in an asynchronous way.
        /// </summary>
        /// <param name="telemetryItems">The telemetry items to publish.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public void PublishManyAsync(
            IEnumerable<TelemetryItem> telemetryItems,
            CancellationToken cancellationToken = default);
    }
}

using System.Collections.Generic;
using RepoDb.Telemetry.Default.Models;

namespace RepoDb.Telemetry.Default.Repositories
{
    internal class PublisherRepository
    {
        #region Privates

        private readonly string _host;

        #endregion

        #region Constructors

        public PublisherRepository(
            string host)
        {
            _host = host;
        }

        #endregion

        #region Methods

        public void Publish(DefaultTelemetryItem telemetryItem)
        {
        }

        public void PublishMany(IEnumerable<DefaultTelemetryItem> telemetryItem)
        {

        }

        #endregion
    }
}

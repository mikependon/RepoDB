#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Telemetry.Core.UnitTests.Helpers
{
    /// <summary>
    /// A repository that is only needed so a trace can be created. It is never used to publish.
    /// </summary>
    internal sealed class ObservableTelemetryPublisherRepository : TelemetryPublisherRepository
    {
        #region Public Methods

        /// <summary>
        /// Gets the request URI of the repository.
        /// </summary>
        /// <returns>The request URI.</returns>
        public override string GetRequestUri() => "v1/telemetry/test";

        #endregion
    }
}

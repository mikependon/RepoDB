#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySqlConnector;
using RepoDb.Attributes;
using RepoDb.PropertyHandlers.MariaDbConnector;

namespace RepoDb.MariaDb.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="GeometryToMySqlGeometryPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class MariaDbConnectorGeometryEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(GeometryToMySqlGeometryPropertyHandler))]
        public MySqlGeometry ColumnGeometry { get; set; }
    }
}

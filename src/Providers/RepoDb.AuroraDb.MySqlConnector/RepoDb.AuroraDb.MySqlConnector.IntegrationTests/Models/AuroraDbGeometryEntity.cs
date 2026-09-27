#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySqlConnector;
using RepoDb.Attributes;
using RepoDb.PropertyHandlers.AuroraDb;

namespace RepoDb.AuroraDb.MySqlConnector.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="AuroraDbGeometryToMySqlGeometryPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class AuroraDbGeometryEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(AuroraDbGeometryToMySqlGeometryPropertyHandler))]
        public MySqlGeometry ColumnGeometry { get; set; }
    }
}

#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.Types;
using RepoDb.Attributes;
using RepoDb.PropertyHandlers.MySql;

namespace RepoDb.MySql.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="MySqlObjectToGeometryPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class MySqlGeometryEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(MySqlObjectToGeometryPropertyHandler))]
        public MySqlGeometry ColumnGeometry { get; set; }
    }
}

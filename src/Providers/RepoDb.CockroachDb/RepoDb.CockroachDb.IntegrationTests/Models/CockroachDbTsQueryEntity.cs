#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes.Parameter.CockroachDb;
using RepoDb.Connector.CockroachDb;
using RepoDb.Attributes;
using RepoDb.PropertyHandlers.CockroachDb;

namespace RepoDb.CockroachDb.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="TsQueryToStringPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class CockroachDbTsQueryEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(TsQueryToStringPropertyHandler))]
        [CockroachDbType(CockroachDbType.TsQuery)]
        public System.String ColumnTsQuery { get; set; }
    }
}

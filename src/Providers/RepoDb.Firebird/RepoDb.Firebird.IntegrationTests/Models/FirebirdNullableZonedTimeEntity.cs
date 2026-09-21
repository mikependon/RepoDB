#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Firebird;
using RepoDb.PropertyHandlers.Firebird;

namespace RepoDb.Firebird.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class FirebirdNullableZonedTimeEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler))]
        [TypeMap(System.Data.DbType.Time)]
        [FbDbType(FbDbType.TimeTZ)]
        public System.DateTimeOffset? ColumnTimeTz { get; set; }
    }
}

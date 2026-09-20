#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using RepoDb.PropertyHandlers.Firebird;

namespace RepoDb.Firebird.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="FirebirdDecFloatToStringPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class FirebirdDecFloatToStringEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(FirebirdDecFloatToStringPropertyHandler))]
        public System.String ColumnDecFloat { get; set; }
    }
}

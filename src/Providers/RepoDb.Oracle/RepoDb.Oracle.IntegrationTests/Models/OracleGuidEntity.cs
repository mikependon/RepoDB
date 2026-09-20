#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using RepoDb.Oracle.PropertyHandlers;

namespace RepoDb.Oracle.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="OracleGuidToByteArrayPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class OracleGuidEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(OracleGuidToByteArrayPropertyHandler))]
        public System.Guid ColumnGuid { get; set; }
    }
}

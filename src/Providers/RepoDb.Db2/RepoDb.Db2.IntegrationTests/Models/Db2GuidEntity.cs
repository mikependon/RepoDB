#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using RepoDb.Db2.PropertyHandlers;

namespace RepoDb.Db2.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="Db2GuidToByteArrayPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class Db2GuidEntity
    {
        public System.Int32 Id { get; set; }

        [PropertyHandler(typeof(Db2GuidToByteArrayPropertyHandler))]
        public System.Guid ColumnGuid { get; set; }
    }
}

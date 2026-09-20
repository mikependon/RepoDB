#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using RepoDb.SapHana.PropertyHandlers;

namespace RepoDb.SapHana.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="SapHanaGuidToStringPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class SapHanaGuidEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(SapHanaGuidToStringPropertyHandler))]
        public System.Guid ColumnGuid { get; set; }
    }
}

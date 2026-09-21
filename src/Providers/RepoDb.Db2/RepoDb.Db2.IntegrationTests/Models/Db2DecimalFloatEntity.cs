#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
using IBM.Data.DB2Types;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.PropertyHandlers.Db2;

namespace RepoDb.Db2.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="Db2DecimalFloatPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class Db2DecimalFloatEntity
    {
        public System.Int32 Id { get; set; }

        [PropertyHandler(typeof(Db2DecimalFloatPropertyHandler))]
        [Db2Type(DB2Type.DecimalFloat)]
        public DB2DecimalFloat ColumnDecFloat { get; set; }
    }
}

#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.PropertyHandlers.Oracle;

namespace RepoDb.Oracle.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="OracleIntervalYMPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class OracleIntervalYMEntity
    {
        public System.Int64 Id { get; set; }

        [OracleDbType(OracleDbType.IntervalYM)]
        [PropertyHandler(typeof(OracleIntervalYMPropertyHandler))]
        public OracleIntervalYM ColumnIntervalYm { get; set; }
    }
}

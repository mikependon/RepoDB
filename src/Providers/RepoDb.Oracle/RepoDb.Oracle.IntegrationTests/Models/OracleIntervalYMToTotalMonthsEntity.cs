#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Types;
using RepoDb.Attributes;
using RepoDb.PropertyHandlers.Oracle;

namespace RepoDb.Oracle.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="OracleIntervalYMToTotalMonthsPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class OracleIntervalYMToTotalMonthsEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(OracleIntervalYMToTotalMonthsPropertyHandler))]
        public System.Int64? ColumnIntervalYm { get; set; }
    }
}

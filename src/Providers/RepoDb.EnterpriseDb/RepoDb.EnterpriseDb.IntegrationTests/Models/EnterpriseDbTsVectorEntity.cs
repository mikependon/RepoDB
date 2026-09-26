#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes.Parameter.EnterpriseDb;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Attributes;
using RepoDb.PropertyHandlers.EnterpriseDb;

namespace RepoDb.EnterpriseDb.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="EnterpriseDbTsVectorToStringPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class EnterpriseDbTsVectorEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(EnterpriseDbTsVectorToStringPropertyHandler))]
        [EnterpriseDbType(EDBType.TsVector)]
        public System.String ColumnTsVector { get; set; }
    }
}

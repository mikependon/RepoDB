#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using RepoDb.PropertyHandlers.SqlServer;

namespace RepoDb.SqlServer.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the [dbo].[PropertyHandler] table, used to test the property handlers.
    /// </summary>
    [Map("PropertyHandler")]
    public class VectorEntity
    {
        public System.Int32 Id { get; set; }

        [PropertyHandler(typeof(SqlServerVectorToFloatArrayPropertyHandler))]
        public System.Single[] Embedding { get; set; }
    }
}

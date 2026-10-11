#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.SqliteTurso.IntegrationTests.Entities
{
    /// <summary>
    /// The entity of the <c>[dbo].[Currency]</c> table.
    /// </summary>
    public class Currency
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}

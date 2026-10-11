#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.MariaDb.IntegrationTests.Entities
{
    /// <summary>
    /// The entity of the <c>[dbo].[Number]</c> table.
    /// </summary>
    public class Number
    {
        public int Value { get; set; }

        public string Label { get; set; }
    }
}

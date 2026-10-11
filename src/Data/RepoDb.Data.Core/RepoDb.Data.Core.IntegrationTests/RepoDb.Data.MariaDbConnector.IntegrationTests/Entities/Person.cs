#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Data.MariaDbConnector.IntegrationTests.Entities
{
    /// <summary>
    /// The entity of the <c>[dbo].[Person]</c> table.
    /// </summary>
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public int? CountryId { get; set; }

        public decimal? Salary { get; set; }

        public DateTime? Birthday { get; set; }

        public bool IsActive { get; set; }
    }
}

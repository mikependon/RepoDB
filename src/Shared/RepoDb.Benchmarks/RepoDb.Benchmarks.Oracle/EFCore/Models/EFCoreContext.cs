#region Copyright Attributions

// Copyright (c) 2026 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.Oracle.Models;

namespace RepoDb.Benchmarks.Oracle.EFCore.Models
{
    public class EFCoreContext : DbContext
    {
        private readonly string connectionString;

        public EFCoreContext(string connectionString) => this.connectionString = connectionString;

        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseOracle(connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .ToTable("Person");
        }
    }
}

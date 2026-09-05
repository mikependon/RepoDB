#region Copyright Attributions

// Copyright (c) 2026 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.Db2.Models;

namespace RepoDb.Benchmarks.Db2.EFCore.Models
{
    public class EFCoreContext : DbContext
    {
        private readonly string connectionString;

        public EFCoreContext(string connectionString) => this.connectionString = connectionString;

        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseDb2(connectionString, options => options.SetServerInfo(IBMDBServerType.LUW));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .ToTable("PERSON");

            modelBuilder.Entity<Person>().Property(x => x.Id).HasColumnName("ID");
            modelBuilder.Entity<Person>().Property(x => x.Name).HasColumnName("NAME");
            modelBuilder.Entity<Person>().Property(x => x.Age).HasColumnName("AGE");
            modelBuilder.Entity<Person>().Property(x => x.CreatedDateUtc).HasColumnName("CREATEDDATEUTC");
        }
    }
}

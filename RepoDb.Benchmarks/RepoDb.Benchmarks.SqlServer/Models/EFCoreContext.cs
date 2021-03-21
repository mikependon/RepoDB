using Microsoft.EntityFrameworkCore;

namespace RepoDb.Benchmarks.SqlServer.Models
{
    public class EFCoreContext : DbContext
    {
        private readonly string connectionString;

        public EFCoreContext(string connectionString) => this.connectionString = connectionString;

        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .ToTable("Person");
        }
    }
}
# The Development Experience

If you are to use RepoDb, the development experience is identical to both both Entity Framework and Dapper. However, since RepoDb is a micro-ORM library, it gives you the most attributes of micro-ORM (i.e: performant, efficient).

### :heavy_check_mark: Entity Framework

In Entity Framework, you tend to create a DbContext so you will inherit the entity-based operations.

```csharp
public class DatabaseContext : DbContext
{
	public DatabaseContext()
	{
		ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		ChangeTracker.AutoDetectChangesEnabled = false;
		ChangeTracker.LazyLoadingEnabled = true;
	}

	public DbSet<Person> People { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder.UseSqlServer("Server=.;Database=TestDB;Integrated Security=SSPI;");
	}
}
```

And you call the operations like below.

```csharp
using (var context = new DbContext())
{
	context.Add(new Person
	{
		Name = "John Doe",
		DateInsertedUtc = DateTime.UtcNow
	});
	context.SaveChanges();
}
```

### :heavy_check_mark: Dapper

In Dapper, you tend to simply open a connection and then execute an operation.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var people = connection.Query<Person>("SELECT * FROM [dbo].[Person]");
}
```

### :heavy_check_mark: RepoDb

In RepoDb, you just simply open a connection like Dapper, and then, call the operations like Entity Framework.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	connection.Insert(new Person
	{
		Name = "John Doe",
		DateInsertedUtc = DateTime.UtcNow
	});
}
```

Though you can as well force the complete Dapper-Like experience like below.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var people = connection.ExecuteQuery<Person>("SELECT * FROM [dbo].[Person]");
}
```
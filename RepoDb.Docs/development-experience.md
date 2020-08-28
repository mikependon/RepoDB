# The Development Experience

If you are to use RepoDB, your development experience will be identical to both Entity Framework and Dapper ORMs. But, it gives you the most attributes of micro-ORM (i.e: Direct, Performant, Efficient and etc).

### Entity Framework

If you are using Entity Framework ORM, you first create a DbContext object to inherit the entity-based operations for a specific data model.

See the code snippets below.

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

And then you call the operations like below.

```csharp
using (var context = new DbContext())
{
	context.People.Add(new Person
	{
		Name = "John Doe",
		DateInsertedUtc = DateTime.UtcNow
	});
	context.SaveChanges();
}
```

### Dapper

If you are using Dapper micro-ORM, you are simply opening a connection object and then execute an operation right away. The way you execute an operation is through SQL.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var people = connection.Query<Person>("SELECT * FROM [dbo].[Person]");
}
```

### RepoDB

If you are to use RepoDB, you just simply open a connection like Dapper, and then, call the operations like Entity Framework.

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

Though, you can as well force the complete Dapper-Like experience like below.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var people = connection.ExecuteQuery<Person>("SELECT * FROM [dbo].[Person]");
}
```

And force the EF-Like design with the use of repository.

```csharp
public class PeopleRepository : BaseRepository<Person, SqlConnection>
{
	public PeopleRepository() :
		base("Server=.;Database=TestDB;Integrated Security=SSPI;")
	{ }
}
```

And you call the operations like below.

```csharp
using (var repository = new PeopleRepository())
{
	repository.Insert(new Person
	{
		Name = "John Doe",
		DateInsertedUtc = DateTime.UtcNow
	});
}
```

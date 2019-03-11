## Index
* Asynchronous Operations
* Caching
* Connection Persistency
* Expression Trees
* Field Mapping
* Inline Hints
* Multi-Resultset Query
* Operations (native ORM)
* Statement Building
* Tracing
* Transaction
* Type Mapping

## Asynchronous Operations

DbConnection:	
	
	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Call the methods
		var result = connection.QueryAsync<Customer>(c => c.Id == 10045);
		
		// Do more logics here
		...
		
		// Extract the result here
		var customers = result.Result;
		
		// Process each customer
		customers.ToList().ForEach(customer =>
		{
			// Process each customer here
			connection.Update<Customer>(customer);
		});
	}

DbRepository:

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		// Call the methods
		var result = repository.QueryAsync<Customer>(c => c.Id == 10045);
		
		// Do more logics here
		...
		
		// Extract the result here via 'Extract' method
		var customers = result.Result.Extract();
		
		// Process each customer
		customers.ToList().ForEach(customer =>
		{
			// Process each customer here
			repository.Update<Customer>(customer);
		});
	}
	
BaseRepository:
	
	public class CustomerRepository : BaseRepository<Customer, SqlConnection>
	{
		public CustomerRepository() : base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
		{ }
	}

	using (var repository = new CustomerRepository())
	{
		// Call the methods
		var result = repository.QueryAsync(c => c.Id == 10045);
		
		// Do more logics here
		...
		
		// Extract the result here via 'Extract' method
		var customers = result.Result.Extract();
		
		// Process each customer
		customers.ToList().ForEach(customer =>
		{
			// Process each customer here
			repository.Update(customer);
		});
	}

## Caching
	
A class named `MemoryCache` is available for use. However, you can create a custom cache object like below.

	public class MyFileCache : ICache
	{
		public void Add(string key, object value, bool throwException = true)
		{
			// Create a file where the name is equals to key
			// Get the bytes of the object
			// Write the bytes to the file via file serialization
		}
		
		public CacheItem Get(string key, bool throwException = true)
		{
			// Locate the file from the directory by key
			// Deserialize the file and convert it to object
			// Returns a new instance of CacheItem
		}
	}

Let us say you have a class named `Global` with `DefaultCache` property.
	
	Global.DefaultCache = new MyFileCache(); // or new MemoryCache();
	
Inject it in anywhere of your code.

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Call the methods passing the cache key
		var orderTypes = connection.Query<OrderTypes>(c => c.Id == 10045, cacheKey: "KeyToOrderTypes", cache: Global.DefaultCache);
	}
	
Or, inject it a part of your `DbRepository`.

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", Global.DefaultCache))
	{
		// Call the methods passing the cache key
		var orderTypes = connection.Query<OrderTypes>(c => c.Id == 10045, cacheKey: "KeyToOrderTypes");
	}
	
Or, when creating a custom base repository via `BaseRepository`.

	public class CustomerRepository : BaseRepository<Customer, SqlConnection>
	{
		public CustomerRepository() : base(@"Server=.;Database=Northwind;Integrated Security=SSPI;", Global.DefaultCache)
		{ }
	}

	using (var repository = new CustomerRepository())
	{
		// Call the methods passing the cache key
		var orderTypes = connection.Query<OrderTypes>(c => c.Id == 10045, cacheKey: "KeyToOrderTypes");
	}


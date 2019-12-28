## Introduction

In this page, we will explain to you why it is important to use the *ConnectionPersistency* when working with *Repository*.

## What is Connection Persistency?

In practical terms, it refers to the persistency of the database connection which would enable the developers to reuse the connection object until its lifespan is over (or the *Dispose* method has been called).

Usually, we tend to dispose the connection object after we used it. See sample code below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = connection.QueryAll<Customer>();
}
```

By using the `using` keyword, the connection object is automatically disposed right after the execution.

In some cases, we tend not to dispose this connection object in order for us to maximize the performance and *behind-the-scene* helping the engine managing the *Connection Pools*.

In short, we declare the connection object as follows.

```csharp
var connection = new SqlConnection(ConnectionString))
```

And reusing it on any database related activity. Like for *Insert* operation.

```csharp
var customers = connection.QueryAll<Customer>();
```

Or *Delete* operation.

```csharp
var deletedRows = connection.Delete<Customer>(11911);
```

And any other calls.

## Repository Connection Persistency

In *RepoDb*, the *ConnectionPersistency* is provided in order for the developer to enable the reusability of the connection object within the *Repository* object.

**Below are the values:**
- *PerCall* - a new connection is being created on every call of the repository operation.
- *Instance* - a single connection is being used until the lifetime of the repository.

By default, the *PerCall* value is used.

### Setting the Connection Persistency

To set the connection persistency, simply pass the value in the constructor of the *Repository*.

For *DbRepository*.

```csharp
public class InventoryRepository : DbRepository<SqlConnection>
{
	public InventoryRepository(string connectionString)
		: base(connectionString, ConnectionPersistency.Instance)
	{ }
}
```

For *BaseRepository*.

```csharp
public class CustomerRepository : BaseRepository<Customer, SqlConnection>
{
	public CustomerRepository(string connectionString)
		: base(connectionString, ConnectionPersistency.Instance)
	{ }
}
```

By doing this, all calls to the operations of the mentioned *Repositories* would only use a single connection.

**Note**: There is a method named *CreateConnection* that will allow the developer to create a new connection object. If the *ConnectionPersistency* has been set the *Instance*, then the same instance of connection object will be returned. Unless, the *force* argument will bet set to *True*.

## Why Singleton Repository

To ensure that you are using single connection object all through the lifetime of the *Repository*, make sure to set the instance of that *Repository* to *Singleton*.

For .NET Core service registration, you can do it like below.

```csharp
services.AddSingleton<CustomerRepository, ICustomerRepository>();
```

For manual management, we prefer you customize your own *DbFactory* to handle it. See sample code snippets below.

```csharp
public static class DbFactory
{
	public static object m_syncObject = new object();
	public static CustomerRepository m_customeRepository;
	
	public static CustomerRepository CustomerRepository
	{
		get { return GetCustomerRepository(); }
	}
	
	private static void GetCustomerRepository()
	{
		if (m_customeRepository == null)
		{
			lock(m_syncObject)
			{
				m_customeRepository = new CustomerRepository(ConnectionString);
			}
		}
		return m_customeRepository;
	}
}
```

And use it like below.

```csharp
var repository = DbFactory.CustomerRepository;
var customer = new Customer
{
	Name = "John Doe",
	Address = "New York"
};
repository.Insert<Customer>(customer);
```

--------

**Voila! You have completed this tutorial!**
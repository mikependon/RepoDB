This page is still in-progress.

## Introduction

In this page, we will share you the differences and what sets [*RepoDb*](https://github.com/mikependon/RepoDb) apart from [*Dapper*](https://github.com/StackExchange/Dapper). We tried our best to make a *1-to-1* operation-calls comparisson. This page will hopefully help you decide as a developer to choose *RepoDb* as your micro-ORM (*with compelling reason*).

> All the contents of this page is written by the author itself. Our knowledge to *Dapper* is not that deep enough when compared to our knowledge with *RepoDb*. So, please allow yourselves to *check* or *comments* right away if you think we made this page bias for *RepoDb*. Please do a pull-requests for any change!

**Note**: The programming language and database provider we are using on our samples below are *C#* and *SQL Server*.

## Topics

- [Basic CRUD Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#basic-crud-differences)
- [Advance Calls Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#advance-calls-differences)
- [Features](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#features)
- [Performance and Efficiency](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#performance-and-efficiency)
- [Library Support](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#library-support)

## Before we begin

Both library is an *ORM* framework for *.NET*. They are both lightweight, fast and efficient. The *Dapper* is a full-fledge *micro-ORM* whereas *RepoDb* is a *hybrid-ORM*.

### Tables

Let us assumed we have the following database tables.

```
CREATE TABLE [dbo].[Customer]
(
	[Id] BIGINT IDENTITY(1,1) 
	, [Name] NVARCHAR(128) NOT NULL
	, [Address] NVARCHAR(MAX)
	, CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([Id] ASC )
)
ON [PRIMARY];
GO

CREATE TABLE [dbo].[Product]
(
	[Id] BIGINT IDENTITY(1,1) 
	, [Name] NVARCHAR(128) NOT NULL
	, [Price] Decimal(18,2)
	, CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC )
)
ON [PRIMARY];

CREATE TABLE [dbo].[Order]
(
	[Id] BIGINT IDENTITY(1,1) 
	, [ProductId] BIGINT NOT NULL
	, [CustomerId] BIGINT
	, [OrderDateUtc] DATETIME(5)
	, [Quantity] INT
	, CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC )
)
ON [PRIMARY];
```

### Models

Let us assumed we have the following class models.

```csharp
public class Customer
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Address { get; set; }
}

public class Product
{
	public long Id { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
}

public class Order
{
	public long Id { get; set; }
	public long ProductId { get; set; }
	public long CustomerId { get; set; }
	public int Quantity { get; set; }
	public DateTime OrderDateUtc{ get; set; }
}
```

--------

## Basic CRUD Differences

### Querying multiple rows

**Dapper**:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = connection.Query<Customer>("SELECT * FROM [dbo].[Customer];");
}
```

**RepoDb**:

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer];");
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = connection.QueryAll<Customer>();
}
```

### Querying a single record

**Dapper**:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

**RepoDb**:

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(e => e.Id == 10045).FirstOrDefault();
}
```

### Inserting a record

**Dapper**:

By default, it returns the number of affected rows.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York"
	};
	var affectedRows = connection.Execute("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address);", customer);
}
```

Returning the identity value.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York"
	};
	var id = connection.Query<long>("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());", customer).Single();
}
```

**RepoDb**:

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York"
	};
	var id = connection.ExecuteScalar<long>("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());");
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York"
	};
	var id = connection.Insert<Customer>(customer);
}
```

### Updating a record

**Dapper**:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var affectedRows = connection.Execute("UPDATE [dbo].[Customer] SET Name = @Name, Address = @Address WHERE Id = @Id;",
	new
	{
		Id = 10045,
		Name = "John Doe",
		Address = "New York"
	});
}
```

**RepoDb**:

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteScalar<int>("UPDATE [dbo].[Customer] SET Name = @Name, Address = @Address WHERE Id = @Id;",
	new
	{
		Id = 10045,
		Name = "John Doe",
		Address = "New York"
	});
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new Customer
	{
		Id = 10045,
		Name = "John Doe",
		Address = "New York"
	};
	var id = connection.Update<Customer>(customer);
}
```

### Deleting a record

**Dapper**:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var affectedRows = connection.Execute("DELETE FROM [dbo].[Customer] WHERE Id = @Id;", new { Id = 10045 });
}
```

**RepoDb**:

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteScalar<int>("DELETE FROM [dbo].[Customer] WHERE Id = @Id;", new { Id = 10045 });
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var affectedRows = connection.Delete<Customer>(10045);
}
```

--------

## Advance Calls Differences

### Querying a parent and its children

Let us assumed we have this structure of *Customer* class.

```csharp
public class Customer
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Address { get; set; }
	public IEnumerable<Order> Orders { get; set; }
}
```

**Dapper**:

Honestly, this is ***very impressive***!

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var sql = "SELECT C.Id, C.Name, C.Address, O.ProductId, O.Quantity, O.OrderDateUtc FROM [dbo].[Customer] C INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id WHERE C.Id = @Id;";
	var customers = connection.Query<Customer, Order, Customer>(sql,
	(customer, orders) =>
	{
		customer.Orders = orders.ToList();
		return customer;
	},
	new { Id = 10045 });
}
```

**RepoDb**:

***Join is purposely not being supported yet. We explained it [here](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple#querying-multiple-resultsets). Also, here is our [answer](https://github.com/mikependon/RepoDb/wiki#will-you-support-join-operations).***

I do not want to hack this on memory processing. The most optimal way is to do the *INNER JOIN* in the actual database itself like what *Dapper* is doing.

However, there is an alternative way to do this. It can be done via *Multi-Query* that executes *packed SELECT-statements* in a single-call.

For *join-support*, we are doing a poll-survey for this one (see [here](https://github.com/mikependon/RepoDb/issues/355)). We would like to hear yours!

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var sql = "SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId; SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;";
	var extractor = connection.ExecuteQueryMultiple(sql, new { CustomerId = 10045 });
	var customer = extractor.Extract<Customer>().FirstOrDefault();
	var orders = extractor.Extract<Order>().AsList();
	customer.Orders = orders;
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customerId = 10045;
	var tuple = connection.QueryMultiple<Customer, Order>(customer => customer.Id == customerId, order => order.CustomerId == customerId);
	var customer = tuple.Item1.FirstOrDefault();
	var orders = tuple.Item2.AsList();
	customer.Orders = orders;
}
```

### Querying multiple parent and their children

Almost the same as previous section.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var sql = "SELECT C.Id, C.Name, C.Address, O.ProductId, O.Quantity, O.OrderDateUtc FROM [dbo].[Customer] C INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id;";
	var customers = connection.Query<Customer, Order, Customer>(sql,
	(customer, orders) =>
	{
		customer.Orders = orders.ToList();
		return customer;
	});
}
```

**RepoDb**:

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var extractor = connection.ExecuteQueryMultiple("SELECT * FROM [dbo].[Customer]; SELECT * FROM [dbo].[Order];");
	var customers = extractor.Extract<Customer>().AsList();
	var orders = extractor.Extract<Order>().AsList();
	customers.ForEach(customer => customer.Orders = orders.Where(o => o.CustomerId == customer.Id).AsList()); // Client memory processing
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customerId = 10045;
	var tuple = connection.QueryMultiple<Customer, Order>(customer => customer.Id == customerId, order => order.CustomerId == customerId);
	var customers = tuple.Item1.FirstOrDefault();
	var orders = tuple.Item2.AsList();
	customers.ForEach(customer => customer.Orders = orders.Where(o => o.CustomerId == customer.Id).AsList()); // Client memory processing
}
```

### Inserting multiple rows

**Dapper**:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GenerateCustomers(1000);
	var affectedRows = connection.ExecuteQuery("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address);", customers);
}
```

**Actually, this is not clear to me**:
- Is it doing a magic to consolidate the *affected rows*?
- Is it creating an implicit transaction here? What if one row fails?
- Is it iterating the list and call the *DbCommand.Execute<Method>* multiple times?
- How should I get the *identity* of the entities?

Please correct me here so I can update this page right away.

**RepoDb**:

Batch operation:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GenerateCustomers(1000);
	var id = connection.InsertAll<Customer>(customers);
}
```

**Note**: The *identity* values are automatically set back to the entities.

Bulk operation:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GenerateCustomers(1000);
	var id = connection.BulkInsert<Customer>(customers);
}
```

**Note**: The operation is using the *SqlBulkCopy* of *ADO.Net*. This should not be compared to *Dapper* performance due to the fact that this is a real *bulk-operation* and is extremely fast.

### Merging multiple rows

### Updating multiple rows

### Bulk-inserting multiple rows

### Querying the rows by batch

--------

## Features

--------

## Performance

--------

## Library Support

--------

Thank you for reading this page. If you have any comments or changes-prososal, kindly submit a pull-requests to us.
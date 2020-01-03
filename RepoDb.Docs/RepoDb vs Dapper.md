## Introduction

In this page, we will share you the differences and what sets [*RepoDb*](https://github.com/mikependon/RepoDb) apart from [*Dapper*](https://github.com/StackExchange/Dapper). We tried our best to make a *1-to-1* comparisson for most area. This page will hopefully help you decide as a developer to choose *RepoDb* as your micro-ORM (*with compelling reason*).

> All the contents of this page is written by the author itself. Our knowledge to *Dapper* is not that deep enough when compared to our knowledge with *RepoDb*. So, please allow yourselves to *check* or *comments* right away if you think we made this page bias for *RepoDb*. Please do a pull-requests for any change!

**Note:** The programming language and database provider we are using on our samples below are *C#* and *SQL Server*.

## Topics

- [Basic CRUD Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#basic-crud-differences)
- [Advance Calls Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#advance-calls-differences)
- [Passing of Parameters](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#passing-of-parameters)
- [Array of Parameters](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#array-of-parameters)
- [Expression Trees](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#expression-trees)
- [Supported Databases](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#supported-databases)
- [Performance and Efficiency](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#performance-and-efficiency)
- [Quality](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#quality)
- [Library Support](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#library-support)
- [Licensing and Legality](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#licensing-and-legality)

## Before we begin

Both library is an *ORM* framework for *.NET*. They are both lightweight, fast and efficient. The *Dapper* is a full-fledge *micro-ORM* whereas *RepoDb* is a *hybrid-ORM*.

> To avoid the bias on the comparisson, we will not cover the features that is present in *RepoDb* but is absent in *Dapper* (ie: *Cache*, *Trace*, *QueryHints*, *Extensibility*, *StatementBuilder* and *Repositories*) (vice-versa). Also, the comparisson does not included any other extension library of *Dapper* (ie: *Dapper.Contrib*, *DapperExtensions*, *Dapper.SqlBuilder*, etc).

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

**Dapper:**

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = connection.Query<Customer>("SELECT * FROM [dbo].[Customer];");
	}
	```

**RepoDb:**

- Raw-SQL:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer];");
	}
	```

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = connection.QueryAll<Customer>();
	}
	```

### Querying a single record

**Dapper:**

- Query

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
	}
	```

**RepoDb:**

- Raw-SQL:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
	}
	```

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(e => e.Id == 10045).FirstOrDefault();
	}
	```

### Inserting a record

**Dapper:**

- Execute:

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

- Query:

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

**RepoDb:**

- Raw-SQL:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = new Customer
		{
			Name = "John Doe",
			Address = "New York"
		};
		var id = connection.ExecuteScalar<long>("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());", customer);
	}
	```

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = new Customer
		{
			Name = "John Doe",
			Address = "New York"
		};
		var id = connection.Insert<Customer>(customer); // or connection.Insert<Customer, long>(customer);
	}
	```

### Updating a record

**Dapper:**

- Execute:

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

**RepoDb:**

- Raw-SQL:

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

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = new Customer
		{
			Id = 10045,
			Name = "John Doe",
			Address = "New York"
		};
		var affectedRows = connection.Update<Customer>(customer);
	}
	```

### Deleting a record

**Dapper:**

- Execute:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var affectedRows = connection.Execute("DELETE FROM [dbo].[Customer] WHERE Id = @Id;", new { Id = 10045 });
	}
	```

**RepoDb:**

- Raw-SQL:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var affectedRows = connection.ExecuteScalar<int>("DELETE FROM [dbo].[Customer] WHERE Id = @Id;", new { Id = 10045 });
	}
	```

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var affectedRows = connection.Delete<Customer>(10045);
	}
	```

--------

## Advance Calls Differences

### Querying a parent and its children

Let us assumed we have added the *Orders (of type IEnumerable&lt;Order&gt;)* property on our *Customer* class.

- Customer

	```csharp
	public class Customer
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public IEnumerable<Order> Orders { get; set; }
	}
	```

- Order

	```csharp
	public class Order
	{
		public long Id { get; set; }
		public long ProductId { get; set; }
		public long CustomerId { get; set; }
		public int Quantity { get; set; }
		public DateTime OrderDateUtc{ get; set; }
	}
	```

**Dapper:**

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var sql = "SELECT C.Id, C.Name, C.Address, O.ProductId, O.Quantity, O.OrderDateUtc FROM [dbo].[Customer] C INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id WHERE C.Id = @Id;";
		var customers = connection.Query<Customer, Order, Customer>(sql,
		(customer, order) =>
		{
			customer.Orders = customer.Orders ?? new List<Order>();
			customer.Orders.Add(order);
			return customer;
		},
		new { Id = 10045 });
	}
	```
	
- QueryMultiple:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var sql = "SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId; SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;";
		using (var result = connection.QueryMultiple(sql, new { CustomerId = 10045 }))
		{
			var customer = result.Read<Customer>().First();
			var orders = result.Read<Order>().ToList();
		}
	}
	```

**RepoDb:**

The *JOIN* feature is purposely not being supported yet. We have explained it on our [Multiple Resultsets via QueryMultiple and ExecuteQueryMultiple](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple#querying-multiple-resultsets) page. Also, we have provided an answer already on our [FAQs](https://github.com/mikependon/RepoDb/wiki#will-you-support-join-operations).

However, the support to this feature will soon to be developed. We are now doing a poll-survey on how to implement this one based on the perusal of the community. The discussion can be seen [here](https://github.com/mikependon/RepoDb/issues/355) and we would like to hear yours!

> No question to this. The most optimal way is to do an actual *INNER JOIN* in the database like what *Dapper* is doing!

However, there is an alternative way to do this in *RepoDb*. It can be done via *Multi-Query* that executes *packed SELECT-statements* in a single-call.

- Raw-SQL:

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

- Fluent:

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

- Query:

	```csharp
	var customers = new List<Customer>();
	using (var connection = new SqlConnection(ConnectionString))
	{
		var sql = "SELECT C.Id, C.Name, C.Address, O.ProductId, O.Quantity, O.OrderDateUtc FROM [dbo].[Customer] C INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id;";
		var customers = connection.Query<Customer, Order, Customer>(sql,
		(customer, order) =>
		{
			customer = customers.Where(e => e.Id == customer.Id).FirstOrDefault() ?? customer;
			customer.Orders = customer.Orders ?? new List<Order>();
			customer.Orders.Add(order);
			return customer;
		});
	}
	```

	**Note:** The hacking technique happens on the developer side, not embedded inside the library.
	
- QueryMultiple:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var sql = "SELECT * FROM [dbo].[Customer]; SELECT * FROM [dbo].[Order];";
		using (var result = connection.QueryMultiple(sql, new { CustomerId = 10045 }))
		{
			var customers = result.Read<Customer>().ToList();
			var orders = result.Read<Order>().ToList();
			customers.ForEach(customer =>
				customer.Orders = orders.Where(o => o.CustomerId == customer.Id).ToList()); // Client memory processing
		}
	}
	```

**RepoDb:**

- Raw-SQL:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var extractor = connection.ExecuteQueryMultiple("SELECT * FROM [dbo].[Customer]; SELECT * FROM [dbo].[Order];");
		var customers = extractor.Extract<Customer>().AsList();
		var orders = extractor.Extract<Order>().AsList();
		customers.ForEach(customer =>
			customer.Orders = orders.Where(o => o.CustomerId == customer.Id).AsList()); // Client memory processing
	}
	```

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customerId = 10045;
		var tuple = connection.QueryMultiple<Customer, Order>(customer => customer.Id == customerId, order => order.CustomerId == customerId);
		var customers = tuple.Item1.FirstOrDefault();
		var orders = tuple.Item2.AsList();
		customers.ForEach(customer =>
			customer.Orders = orders.Where(o => o.CustomerId == customer.Id).AsList()); // Client memory processing
	}
	```

### Inserting multiple rows

**Dapper:**

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var identities = connection.Query<long>("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());", customers);
	}
	```

	**Actually, this is not clear to me:**
	- Is it creating an implicit transaction? What if one row fails?
	- Is it iterating the list and call the *DbCommand.Execute<Method>* multiple times?

	Please correct me here so I can update this page right away.

**RepoDb:**

- Batch operation:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.InsertAll<Customer>(customers);
	}
	```

	The above operation can be batched by passing a value on the *batchSize* argument.

	**Note:** You can target a specific column. In addition, the *identity* values are automatically set back to the entities.

- Bulk operation:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.BulkInsert<Customer>(customers);
	}
	```
	
	The above operation can be batched by passing a value on the *batchSize* argument.

	**Note:** This is just an FYI. The operation is using the *SqlBulkCopy* of *ADO.Net*. This should not be compared to *Dapper* performance due to the fact that this is a real *bulk-operation*. This is far (*extremely fast*) when compared to both *Dapper* (multi-inserts) and *RepoDb* (*InsertAll*) operations.

### Merging multiple rows

**Dapper:**

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var sql = @"MERGE [dbo].[Customer] AS T
			USING
				(SELECT @Name, @Address) AS S
			ON
				S.Id = T.Id
			WHEN NOT MATCH THEN
				INSERT INTO
				(
					Name
					, Address
				)
				VALUES
				(
					S.Name
					, S.
				Address)
			WHEN MATCHED THEN
				UPDATE
				SET Name = S.Name
					, Address = S.Address
			OUTPUT INSERTED.Id AS Result;";
		var customers = GenerateCustomers(1000);
		var identities = connection.Query<long>(sql, customers);
	}
	```

	Here, I have the same question as the previous section.

**RepoDb:**

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.MergeAll<Customer>(customers);
	}
	```
	
	The above operation can be batched by passing a value on the *batchSize* argument.

	**Note:** You can set the *qualifier fields*. In addition, the *identity* values are automatically set back to the entities for the newly inserted records.

### Updating multiple rows

**Dapper:**

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.Execute("UPDATE [dbo].[Customer] SET Name = @Name, Address = @Address WHERE Id = @Id;", customers);
	}
	```

**RepoDb:**

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.UpdateAll<Customer>(customers);
	}
	```
	
	The above operation can be batched by passing a value on the *batchSize* argument.

	**Note:** You can set the *qualifier fields*.

### Bulk-inserting multiple rows

**Dapper:**

- ADO.NET:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var table = ConvertToTable(customers);
		using (var sqlBulkCopy = new SqlBulkCopy(connection, options, transaction))
        	{
			sqlBulkCopy.DestinationTableName = "Customer";
			sqlBulkCopy.WriteToServer(table);
		}
	}
	```
	
	**Note:** You can as well pass an instance of *DbDataReader* (instead of *DataTable*).

**RepoDb:**

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.BulkInsert<Customer>(customers);
	}
	```

	**Note:** You can as well pass an instance of *DbDataReader*.
	
- Fluent (Targetted):

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.BulkInsert("[dbo].[Customer]", customers);
	}
	```

### Querying the rows by batch

**Dapper:**

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var sql = @"WITH CTE AS
			(
				SELECT TOP (@Rows) ROW_NUMBER() OVER(ORDER BY Name ASC) AS RowNumber
				FROM [dbo].[Customer]
				WHERE (Address = @Address)
			)
			SELECT Id
				, Name
				, Address
			FROM
				CTE
			WHERE
				RowNumber BETWEEN @From AND (@From + @Rows);";
		using (var connection = new SqlConnection(ConnectionString))
		{
			var customers = connection.Query<Customer>(sql, new { From = 0, Rows = 100, Address = "New York" });
		}
	}
	```
	
	**Note:** You can as well execute it via (*LIMIT*) keyword. It is on your preference.

**RepoDb:**

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = connection.BatchQuery<Customer>(e => e.Address == "New York",
			page: 0,
			rowsPerBatch: 100,
			orderBy: OrderField.Parse(new { Name = Order.Ascending }));
	}
	```

### Replicate records from different database

**Dapper:**

- Query:

	```csharp
	using (var sourceConnection = new SqlConnection(SourceConnectionString))
	{
		var customers = sourceConnection.Query<Customer>("SELECT * FROM [dbo].[Customer];");
		using (var destinationConnection = new SqlConnection(DestinationConnectionString))
		{
			var identities = destinationConnection.Query<long>("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());", customers);
		}
	}
	```
	

**RepoDb:**

- Fluent (InsertAll):

	```csharp
	using (var sourceConnection = new SqlConnection(SourceConnectionString))
	{
		var customers = sourceConnection.QueryAll<Customer>();
		using (var destinationConnection = new SqlConnection(DestinationConnectionString))
		{
			var affectedRows = destinationConnection.InsertAll<Customer>(customers);
		}
	}
	```

- Fluent (BulkInsert):

	```csharp
	using (var sourceConnection = new SqlConnection(SourceConnectionString))
	{
		var customers = sourceConnection.QueryAll<Customer>();
		using (var destinationConnection = new SqlConnection(DestinationConnectionString))
		{
			var affectedRows = destinationConnection.BulkInsert<Customer>(customers);
		}
	}
	```

- Fluent (Streaming):

	This is the most optimal and recommended calls for large datasets. We do not bring the data as class objects in the client application.

	```csharp
	using (var sourceConnection = new SqlConnection(SourceConnectionString))
	{
		using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
		{
			using (var destinationConnection = new SqlConnection(DestinationConnectionString))
			{
				var affectedRows = destinationConnection.BulkInsert<Customer>(reader);
			}
		}
	}
	```

	**Note:** Check for collation constraints. It is an *ADO.NET* thing.

--------

## Passing of Parameters

**Dapper:**

- Dynamic:

	```csharp
	Query<T>(sql, new { Id = 10045 });
	```

	It is always an *Equal* operation. You control the query through *SQL Statement*.

- Dynamic Parameters:

	```csharp
	var parameters = new DynamicParameters();
	parameters.Add("Name", "John Doe");
	parameters.Add("Address", "New York");
	Query<T>(sql, parameters);
	```

**RepoDb:**

- Dynamic:

	```csharp
	Query<T>(new { Id = 10045 });
	```

	Same as *Dapper*, it is always referring to an *Equal* operation. You control the query through *SQL Statement*.

- Linq Expression:

	```csharp
	Query<T>(e => e.Id == 10045);
	```
	
- QueryField:

	```csharp
	Query<T>(new QueryField("Id", 10045));
	```
	
- QueryField(s) or QueryGroup:

	```csharp
	var queryFields = new[]
	{
		new QueryField("Name", "John Doe")
		new QueryField("Address", "New York")
	};
	Query<T>(queryFields); // or Query<T>(new QueryGroup(queryFields));
	```

--------

## Array of Parameters

**Dapper:**

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var addresses = new [] { "New York", "Washington" };
		var customers = connection.Query<Customer>("SELECT * FROM [dbo].[Customer] WHERE Address IN (@Addresses);", new { Addresses = addresses });
	}
	```

**RepoDb:**

- ExecuteQuery:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var addresses = new [] { "New York", "Washington" };
		var customers = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE Address IN (@Addresses);", new { Addresses = addresses });
	}
	```

	For further explanation, you can visit our [documentation](https://repodb.readthedocs.io/en/latest/pages/rawsql.html#array-values).

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var addresses = new [] { "New York", "Washington" };
		var customers = connection.Query<Customer>(e => addresses.Contains(e => e.Address));
	}
	```

--------

## Expression Trees

- Dapper do not support *Linq Expressions*, only *dynamics* and *DynamicParameters*.
- RepoDb supports *Linq Expressions*, *dynamics* and *QueryObjects*.

**Note:** The *Dapper.DynamicParameters* is just a subset of *RepoDb.QueryObjects*. The *QueryObjects* has much more capability that can further support the *Linq Expressions*.

Please visit both documentation.

- [Dapper](https://dapper-tutorial.net/parameter-dynamic)
- [RepoDb](https://github.com/mikependon/RepoDb/wiki/Expression-Trees)

--------

## Supported Databases

**Dapper:**

Supports all RDBMS data providers.

**RepoDb:**

1. Raw-SQLs support all RDBMS data providers.
2. Fluent calls only supports *SQL Server*, *SqLite*, *MySql* and *PostgreSql*.

--------

## Performance and Efficiency

We only refer to one of the the community-approved ORM bencher, the [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher).

**Net Core:**

Here is our observation from the official execution results. The official result can be found [here](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20190520_netcore.txt).

Performance:

- RepoDb is the fastest ORM when fetching set-records. Both *raw-SQL* and *Fluent* calls.
- Dapper and RepoDb speed is identical when fetching single-record.
- Dapper is faster than RepoDb's *Fluent* calls when fetching single-record.

Efficiency:

- RepoDb is the most-efficient ORM when fetching set-records. Both *raw-SQL* and *Fluent* calls.
- Dapper is must more efficient than RepoDb when fetching single-record.

**NetFramework:**

RepoDb is the *fastest* and the *most-efficient* ORM for both *set* and *single* record(s) fetching. Official results can been found [here](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20190520_netfx.txt).

--------

## Quality

**Dapper:**

Dapper is already running since 2012 and is being used by *StackOverflow.com*. It has a huge consumers and is hugely backed by the community.

**RepoDb:**

We did our best to write *one-test per scenario* and we have delivered *thousand of items (approximately 7K)* for both *Unit* and *IntegrationTests*. We would like your help to review it as well.

Below are the links to our test suites.

- [Core Unit Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.UnitTests)
- [Core Integration Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.IntegrationTests)
- [SqLite Unit Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite/RepoDb.SqLite.UnitTests)
- [SqLite Integration Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite/RepoDb.SqLite.IntegrationTests)
- [MySql Unit Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.MySql.UnitTests)
- [MySql Integration Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.MySql.IntegrationTests)
- We are about to write more on *PostgreSql*.

> We (or I as an author) have been challenged that the quality of the software does not depends on the number of tests. However, we strongly believe that *spending* much efforts on writing a test will give confidence to the library consumers (ie: *.NET community*). Practially, it helps us to avoid manual revisits on the *already-working* features if somebody is doing a *PR* to us; it prevents the library from any surprising bugs.

**Conclusion:**

*Dapper* is far matured and with *high-quality* over *RepoDb*. We will not contest this!

--------

## Library Support

**Dapper:**

Proven and is backed hugely by the .NET Community; funded by *StackOverflow.com*.

**RepoDb:**

Backed by *one person* and is *not funded nor sponsored* by any entity. Just starting to expand and gather more supports from the .NET Community.

--------

## Licensing and Legality

Both is under the [Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) license.

**Disclaimer:**

We are not expert in legal, but we are consulting. If any conflict arises on the copyright or trademark in-front of *RepoDb*, then that is not yet addressed.

--------

Thank you for reading this page. If you have any comments or changes-prososal, kindly submit a pull-requests to us.

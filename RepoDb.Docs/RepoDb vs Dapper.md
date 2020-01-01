This page is still in-progress.

## Introduction

In this page, we will share you the differences and what sets [*RepoDb*](https://github.com/mikependon/RepoDb) apart from [*Dapper*](https://github.com/StackExchange/Dapper). We tried our best to make a *1-to-1* operation-calls comparisson. This page will hopefully help you decide as a developer to choose *RepoDb* as your micro-ORM (*with compelling reason*).

> All the contents of this page is written by the author itself. Our knowledge to *Dapper* is not that deep enough when compared to our knowledge with *RepoDb*. So, please allow yourselves to *check* or *comments* right away if you think we made this page bias for *RepoDb*. Please do a pull-requests for any change!

**Note**: The programming language and database provider we are using on our samples below are *C#* and *SQL Server*.

## Topics

- [Basic CRUD Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#basic-crud-differences)
- [Advance Calls Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#advance-calls-differences)
- [Passing of Parameters](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#passing-of-parameters)
- [Expression Trees](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#expression-trees)
- [Performance and Efficiency](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#performance-and-efficiency)
- [Quality](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#quality)
- [Library Support](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#library-support)
- [Licensing and Legality](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#licensing-and-legality)

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

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = connection.Query<Customer>("SELECT * FROM [dbo].[Customer];");
	}
	```

**RepoDb**:

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

**Dapper**:

- Query

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
	}
	```

**RepoDb**:

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

**Dapper**:

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

**RepoDb**:

- Raw-SQL:

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

**Dapper**:

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

**RepoDb**:

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

**Dapper**:

- Execute:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var affectedRows = connection.Execute("DELETE FROM [dbo].[Customer] WHERE Id = @Id;", new { Id = 10045 });
	}
	```

**RepoDb**:

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

- Query:

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

The *JOIN* feature is purposely not being supported yet. We have explained it on our [Multiple Resultsets via QueryMultiple and ExecuteQueryMultiple](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple#querying-multiple-resultsets) page. Also, we have provided an answer already on our [FAQs](https://github.com/mikependon/RepoDb/wiki#will-you-support-join-operations).

However, the support to this feature will soon to be developed. We are now doing a poll-survey on how to implement this one based on the perusal of the community. The discussion can be seen [here](https://github.com/mikependon/RepoDb/issues/355). We would like to hear yours!

> No question to this. The most optimal way is to do the *INNER JOIN* in the actual database itself like what *Dapper* is doing!

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

- Raw-SQL:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var extractor = connection.ExecuteQueryMultiple("SELECT * FROM [dbo].[Customer]; SELECT * FROM [dbo].[Order];");
		var customers = extractor.Extract<Customer>().AsList();
		var orders = extractor.Extract<Order>().AsList();
		customers.ForEach(customer => customer.Orders = orders.Where(o => o.CustomerId == customer.Id).AsList()); // Client memory processing
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
		customers.ForEach(customer => customer.Orders = orders.Where(o => o.CustomerId == customer.Id).AsList()); // Client memory processing
	}
	```

### Inserting multiple rows

**Dapper**:

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var identities = connection.Query<long>("INSERT INTO [dbo].[Customer] (Name, Address) VALUES (@Name, @Address); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());", customers);
	}
	```

	**Actually, this is not clear to me**:
	- Is it creating an implicit transaction here? What if one row fails?
	- Is it iterating the list and call the *DbCommand.Execute<Method>* multiple times?

	Please correct me here so I can update this page right away.

**RepoDb**:

- Batch operation:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.InsertAll<Customer>(customers);
	}
	```

	**Note**: You can target specific columns. In addition, the *identity* values are automatically set back to the entities.

- Bulk operation:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.BulkInsert<Customer>(customers);
	}
	```

	**Note**: The operation is using the *SqlBulkCopy* of *ADO.Net*. This should not be compared to *Dapper* performance due to the fact that this is a real *bulk-operation* and is extremely fast.

### Merging multiple rows

**Dapper**:

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

**RepoDb**:

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.MergeAll<Customer>(customers);
	}
	```

	**Note**: You can set the *qualifier fields*. In addition, the *identity* values are automatically set back to the entities for the newly inserted records.

### Updating multiple rows

**Dapper**:

- Query:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.Execute("UPDATE [dbo].[Customer] SET Name = @Name, Address = @Address WHERE Id = @Id;", customers);
	}
	```

**RepoDb**:

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.UpdateAll<Customer>(customers);
	}
	```

	**Note**: You can set the *qualifier fields*.

### Bulk-inserting multiple rows

**Dapper**:

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
	
	**Note**: You can as well pass an instance of *DbDataReader* (instead of *DataTable*).

**RepoDb**:

- Fluent:

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.BulkInsert<Customer>(customers);
	}
	```

	**Note**: You can as well pass an instance of *DbDataReader*.
	
- Fluent (Targetted):

	```csharp
	using (var connection = new SqlConnection(ConnectionString))
	{
		var customers = GenerateCustomers(1000);
		var affectedRows = connection.BulkInsert("[dbo].[Customer]", customers);
	}
	```

### Querying the rows by batch

**Dapper**:

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
			FROM CTE
			WHERE RowNumber BETWEEN @From AND (@From + @Rows)";
		using (var connection = new SqlConnection(ConnectionString))
		{
			var customers = connection.Query<Customer>(sql, new { From = 0, Rows = 100, Address = "New York" });
		}
	}
	```
	
	**Note**: You can as well execute it via (*LIMIT*) keyword. It is on your preference.

**RepoDb**:

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

--------

## Passing of Parameters

--------

## Expression Trees

--------

## Performance and Efficiency

--------

## Quality

--------

## Library Support

--------

## Licensing and Legality

--------

Thank you for reading this page. If you have any comments or changes-prososal, kindly submit a pull-requests to us.
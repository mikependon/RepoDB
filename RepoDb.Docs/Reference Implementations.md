## Introduction

In this page, we will only show you the reference implementation, not the usual actual implementation. The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*.

### Indexes

- [Installations](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#installations)
- [Querying a Data](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#querying-a-data)
- [Querying via TableName](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#querying-via-tablename)
- [Inserting a Data](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#inserting-a-data)
- [Inserting via TableName](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#inserting-via-tablename)
- [Updating a Data](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#updating-a-data)
- [Updating via TableName](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#updating-via-tablename)
- [Deleting a Data](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#deleting-a-data)
- [Deleting via TableName](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#deleting-via-tablename)
- [Merging a Data](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#merging-a-data)
- [Merging via TableName](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#merging-via-tablename)
- [Executing a Customized Query](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#executing-a-customized-query)
- [Calling a Stored Procedure](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md#calling-a-storedprocedure)

## Installations

RepoDb and its extension is available via Nuget as a NetStandard library. Type any of the command below at the *Package Manager Console* window.

### For SqlServer

```
Install-Package RepoDb
```

### For SqLite

```
Install-Package RepoDb.SqLite
```

### For MySql

```
Install-Package RepoDb.MySql
```

### For PostgreSql

```
Install-Package RepoDb.PostgreSql
```

## Snippets

Let us say you have a customer class named *Customer* that has an equivalent table in the database named `[dbo].[Customer]`.

```csharp
public class Customer
{
	public int Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public bool IsActive { get; set; }
	public DateTime LastUpdatedUtc { get; set; }
	public DateTime CreatedDateUtc { get; set; }
}
```

### Querying a Data

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(new { Id = 10045 });
}
```

Via Expression:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(c => c.Id == 10045);
}
```

Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(new QueryField("Id", 10045));
}
```

### Querying via TableName

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", new { Id = 10045 });
}
```

Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", new QueryField("Id", 10045));
}
```
	
Via QueryObject (targetting few fields):

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", new QueryField("Id", 10045),
		Field.From("Id", "FirstName", "LastName"));
}
```

### Inserting a Data

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer, int>(customer);
}
```

### Inserting via TableName

```csharp
var customer = new
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true,
	LastUpdatedUtc = DateTime.UtcNow,
	CreatedDateUtc = DateTime.UtcNow
};
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<int>("Customer", customer);
}
```

### Updating a Data

Via DataEntity:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, new { Id = 10045 });
}
```

Via Expression:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, e => e.Id == 10045);
}
```
	
Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, new QueryField("Id", 10045));
}
```

### Updating via TableName

Via Dynamic Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		Id = 10045,
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer);
}
```

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer, 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer, new { Id = 10045 });
}
```
	
Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer, new QueryField("Id", 10045));
}
```

### Deleting a Data

Via DataEntity:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(new { Id = 10045 });
}
```

Via Expression:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(c => c.Id == 10045);
}
```

Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(new QueryField("Id", 10045));
}
```

### Deleting via TableName

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete("Customer", 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete("Customer", { Id = 10045 });
}
```

Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete("Customer", new QueryField("Id", 10045));
}
```

### Merging a Data

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true,
	LastUpdatedUtc = DateTime.Utc,
	CreatedDateUtc = DateTime.Utc
};
using (var connection = new SqlConnection(ConnectionString))
{
	var qualifiers = new []
	{
		new Field("FirstName"),
		new Field("LastName"),
	};
	var mergeCount = connection.Merge<Customer>(customer, qualifiers);
}
```

### Merging via TableName

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new SqlConnection(ConnectionString))
{
	var qualifiers = new []
	{
		new Field("FirstName"),
		new Field("LastName"),
	};
	var mergeCount = connection.Merge("Customer", customer, qualifiers);
}
```

### Executing a Customized-Query

You can create a class with combined properties of different tables or with stored procedures. It does not need to be 100% identical to the schema, as long the property of the class is part of the result set.

```csharp
public class ComplexClass
{
	public int CustomerId { get; set; }
	public int OrderId { get; set; }
	public int ProductId { get; set; }
	public string CustomerName { get; set; }
	public string ProductName { get; set; }
	public DateTime ProductDescription { get; set; } // This is not in the CommandText, will be ignored
	public DateTime OrderDate { get; set; }
	public int Quantity { get; set; }
	public double Price { get; set; }
}
```

Then you can create this command text.

	var commandText = @"SELECT C.Id AS CustomerId
		, O.Id AS OrderId
		, P.Id AS ProductId
		, CONCAT(C.FirstName, ' ', C.LastName) AS CustomerName
		, P.Name AS ProductName
		, O.OrderDate
		, O.Quantity
		, P.Price
		, (O.Quatity * P.Price) AS Total /* Note: This is not in the class, but still it is valid */
	FROM [dbo].[Customer] C
	INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id
	INNER JOIN [dbo].[OrderItem] OI ON OI.OrderId = O.Id
	INNER JOIN [dbo].[Product] P ON P.Id = OI.ProductId
	WHERE (C.Id = @CustomerId)
		AND (O.OrderDate BETWEEN @OrderDate AND DATEADD(DAY, 1, @OrderDate));";

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<ComplexClass>(commandText, new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date });
}
```

Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var queryGroup = new QueryGroup(new []
	{
		new QueryField("CustomerId", 10045),
		new QueryField("OrderDate", DateTime.UtcNow.Date)
	});
	var customer = connection.ExecuteQuery<ComplexClass>(commandText, queryGroup);
}
```

The `ExecuteQuery` method is purposely not being supported by `Expression` based query as we are avoiding the user to bind the complex-class to its target query text.

**Note**: The most optimal when it comes to performance is to used the `QueryObjects`.

### Calling a StoredProcedure

Using the complex type above. If you have a stored procedure like below.

	DROP PROCEDURE IF EXISTS [dbo].[sp_get_customer_orders_by_date];
	GO
	CREATE PROCEDURE [dbo].[sp_get_customer_orders_by_date]
	(
		@CustomerId INT
		, @OrderDate DATETIME2(7)
	)
	AS
	BEGIN
		SELECT C.Id AS CustomerId
			, O.Id AS OrderId
			, P.Id AS ProductId
			, CONCAT(C.FirstName, ' ', C.LastName) AS CustomerName
			, P.Name AS ProductName
			, O.OrderDate
			, O.Quantity
			, P.Price
			, (O.Quatity * P.Price) AS Total /* Note: This is not in the class, but still it is valid */
		FROM [dbo].[Customer] C
		INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id
		INNER JOIN [dbo].[OrderItem] OI ON OI.OrderId = O.Id
		INNER JOIN [dbo].[Product] P ON P.Id = OI.ProductId
		WHERE (C.Id = @CustomerId)
			AND (O.OrderDate BETWEEN @OrderDate AND DATEADD(DAY, 1, @OrderDate));
	END

Then it can be called as below.

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<ComplexClass>("[dbo].[sp_get_customer_orders_by_date]",
		param: new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date },
		commandType: CommandType.StoredProcedure);
}
```

Via QueryObject:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var queryGroup = new QueryGroup(new []
	{
		new QueryField("CustomerId", 10045),
		new QueryField("OrderDate", DateTime.UtcNow.Date)
	});
	var customer = connection.ExecuteQuery<ComplexClass>("[dbo].[sp_get_customer_orders_by_date]",
		param: queryGroup,
		commandType: CommandType.StoredProcedure);
}
```

Please visit the [documentation](https://repodb.readthedocs.io/en/latest/) for further details about the library.

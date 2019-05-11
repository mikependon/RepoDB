
## RepoDb

A dynamic, lightweight, efficient and very fast Hybrid ORM library for .NET.

<img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Icons/RepoDb-128x128.png" height="128px" />

Package: [https://www.nuget.org/packages/RepoDb](https://www.nuget.org/packages/RepoDb)  
Documentation: [https://repodb.readthedocs.io/en/latest/](https://repodb.readthedocs.io/en/latest/)

Follow [@mike_pendon](https://twitter.com/mike_pendon) at Twitter.

## Highlight

RepoDb is the fastest and the most efficient ORM library in .NET as per the result of [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher).

Click [here](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20190307_netcore.txt) to see the official run result.

## Notes

RepoDb is currently **not** accepting any pull-requests until the target core features has all been implemented.

## Build Result

Type of Build		 | Net (Framework)																																	 | Net (Standard)
---------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
Project/Solution	 | [![Build status](https://ci.appveyor.com/api/projects/status/c563cikul4c2a5vc?svg=true)](https://ci.appveyor.com/project/mikependon/repodb)		 | [![Build status](https://ci.appveyor.com/api/projects/status/0ix2khcfrv1ub6ba?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw) |
Unit Test			 | [![Build status](https://ci.appveyor.com/api/projects/status/xtfp2urkxa29s7b9?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-g4ml5) | [![Build status](https://ci.appveyor.com/api/projects/status/78nch60yyoj6wiok?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx) |
Integration Test	 | [![Build status](https://ci.appveyor.com/api/projects/status/qnvun61tyyy1vwlb?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-neg8t) | [![Build status](https://ci.appveyor.com/api/projects/status/3fsp38vaqchmec7y?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-qksas) |

## Why RepoDb?

 - It is very fast in fetching data.
 - It is very efficient in memory usage.
 - It is extensible.
 - It is easy to switch between lightweight and massive operations.
 - It is fluent and clean.
 - It supports multi-resultset query.
 - It has massive ORM operations.
 - It support Async operations.
 - It is easy to write RawSql statements.
 - It has Linq Expressions.
 - It supports dynamic variable passing.
 - It has database helpers.
 - It has a tracing capability.
 - It is easy to cache the data.
 - It has a dynamic type mapping.
 - It support query hints.
 - It has abstracted the ADO.Net transaction.
 - It is always free!

## Features
 
 - Asynchronous Operations
 - Caching
 - Connection Persistency
 - Database Helpers
 - Expression Trees
 - Field Mapping
 - Inline Hints
 - Multi-Resultset Query
 - Operations (Generics/Explicits/MethodCalls)
 - Query Builder
 - Statement Builder
 - Tracing
 - Transaction
 - Type Mapping

## Code Samples

Let us say you have a customer class named `Customer` that has an equivalent table in the database named `[dbo].[Customer]`.

	public class Customer
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool IsActive { get; set; }
		public DateTime LastUpdatedUtc { get; set; }
		public DateTime CreatedDateUtc { get; set; }
	}

### Query

Below are the codes on how to query a record from the database.

Via PrimaryKey:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(10045);
	}

Via Dynamic:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(new { Id = 10045 });
	}

Via Expression:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(c => c.Id == 10045);
	}

Via Object:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(new QueryField(nameof(Customer.Id), 10045));
	}

### Insert

Below is the code on how to insert a record into the database.

	// Create a new instance
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
		// Call the insert method by passing the data entity object
		var id = Convert.ToInt32(connection.Insert<Customer>(customer));
	}

### Update

Below are the codes on how to update an existing record from the database.

Querying and updating an existing instance.

    using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(10045);
		// Set the properties
		customer.FirstName = "John";
		customer.LastUpdatedUtc = DateTime.UtcNow;
		// Call the method
		var updatedCount = connection.Update<Customer>(customer);
	}

Certain columns only.

    using (var connection = new SqlConnection(ConnectionString))
	{
		// Create a dynamic object
		var customer = new
		{
			FirstName = "John",
			LastUpdatedUtc = DateTime.UtcNow
		};
		// Call the method
		var updatedCount = connection.Update(nameof(Customer), customer, new { Id = 10045 });
	}

### Delete

Below are the codes on how to delete a record from the database.

Via PrimaryKey:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var deletedCount = connection.Delete<Customer>(10045);
	}

Via Dynamic:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var deletedCount = connection.Delete<Customer>(new { Id = 10045 });
	}

Via Expression:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var deletedCount = connection.Delete<Customer>(c => c.Id == 10045);
	}

Via Object:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var deletedCount = connection.Delete<Customer>(new QueryField(nameof(Customer.Id), 10045));
	}

Via DataEntity:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(new { Id = 10045 });
		var deletedCount = connection.Delete<Customer>(customer);
	}

### Merge

Inserts a new record in the database, otherwise update it.

	// Create a new instance
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
		// Create a qualifier (use FirstName and LastName as an example)
		var qualifiers = new []
		{
			new Field(nameof(Customer.FirstName)),
			new Field(nameof(Customer.LastName)),
		};
		// Merge the records (Upsert)
		var mergeCount = connection.Merge<Customer>(customer, qualifiers);
	}

### ExecuteQuery

You can create a class with combined properties of different tables or with stored procedures. It does not need to be 100% identical to the schema, as long the property of the class is part of the result set.

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

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.ExecuteQuery<ComplexClass>(commandText, new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date });
	}

Via Object:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var queryGroup = new QueryGroup(new []
		{
			new QueryField("CustomerId", 10045),
			new QueryField("OrderDate", DateTime.UtcNow.Date)
		});
		var customer = connection.ExecuteQuery<Customer>(commandText, queryGroup);
	}

The `ExecuteQuery` method is purposely not being supported by `Expression` based query as we are avoiding the user to bind the complex-class to its target query text.

Note: The most optimal when it comes to performance is to used the `Object-Based`.

### StoredProcedure

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

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.ExecuteQuery<ComplexClass>("[dbo].[sp_get_customer_orders_by_date]",
			param: new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date },
			commandType: CommandType.StoredProcedure);
	}

Via Object:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var queryGroup = new QueryGroup(new []
		{
			new QueryField("CustomerId", 10045),
			new QueryField("OrderDate", DateTime.UtcNow.Date)
		});
		var customer = connection.ExecuteQuery<Customer>(commandText, queryGroup,
			commandType: CommandType.StoredProcedure);
	}

Please visit our [documentation](https://repodb.readthedocs.io/en/latest/) for further details about the codes.

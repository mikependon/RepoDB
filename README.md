## RepoDb

A dynamic, lightweight, and fast repo-based ORM .NET Library.

![](https://ci.appveyor.com/api/projects/status/8sms05vy0ad9aao2?svg=true)
[![RepoDb](https://img.shields.io/nuget/v/RepoDb.svg)](https://www.nuget.org/packages/RepoDb/)
[![RepoDb](https://img.shields.io/nuget/dt/RepoDb.svg)](https://www.nuget.org/packages/RepoDb/)

Package: [https://www.nuget.org/packages/RepoDb](https://www.nuget.org/packages/RepoDb)  
Documentation: [https://repodb.readthedocs.io/en/latest/](https://repodb.readthedocs.io/en/latest/)

### Goal

To be the fastest and easiest-to-use lightweight ORM.

### Vision

To provide more flexibility and fast-switching development approach, whether to use the massive or lightweight ORM operations.

### Principles

 - Keep it as simple as possible (KISS principle)
 - Help developers be more focused on the SOLID principle
 - Make it as fast as possible
 - Make it more flexible
 - Never use try-catch inside the library
 - Never create complex implementations (especially for complex Join Queries)

### Features

 - Caching
 - Expression Trees (Dynamic-Based, Expression-Based, Object-Based)
 - Field Mapping
 - Operations (Asynchronous)
 - Recursive Query
 - SQL Statement Builder
 - Tracing
 - Transactions
 - Type Mapping

## Code Samples

Let us say you have a customer class named `Customer` that has an equivalent table in the database.

	public class Customer
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool IsActive { get; set; }
		public DateTime LastUpdatedUtc { get; set; }
		public DateTime CreatedDateUtc { get; set; }
	}

**Query**

There are 3 ways of doing this (dynamics, expression and object-based approach).

Dynamics:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(new { Id = 1005 });
	}

Expression:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(c => c.Id = 1005);
	}

Object-Based:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(new QueryField(nameof(Customer.Id), 1005));
	}

**ExecuteQuery**

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

Dynamics:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<ComplexClass>(commandText, new { CustomerId = 1005, OrderDate = DateTime.UtcNow.Date });
	}

Object-Based:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var queryGroup = new QueryGroup(new []
		{
			new QueryField("CustomerId", 1005),
			new QueryField("OrderDate", DateTime.UtcNow.Date)
		});
		var customer = connection.Query<Customer>(commandText, queryGroup);
	}

The `ExecuteQuery` method is purposely not being supported by `Expression` based query as we are avoiding the user to bind the complex-class to its target query text.

Note: Expression-based query is a typical ORM approach for a `DataSet` and `ClassObject`. The most optimal when it comes to performance is to used the `Object-Based` (followed by `Dynamics` and then `Expression`). However, writing an `Object-Based` is a bit informative.

**StoredProcedure**

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

	
Dynamics:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<ComplexClass>("[dbo].[sp_get_customer_orders_by_date]",
			new { CustomerId = 1005, OrderDate = DateTime.UtcNow.Date },
			commandType: CommandType.StoredProcedure);
	}

Object-Based:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var queryGroup = new QueryGroup(new []
		{
			new QueryField("CustomerId", 1005),
			new QueryField("OrderDate", DateTime.UtcNow.Date)
		});
		var customer = connection.Query<Customer>(commandText, queryGroup,
			commandType: CommandType.StoredProcedure);
	}

Please visit our [documentation](https://repodb.readthedocs.io/en/latest/) for further details about the codes.

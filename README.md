## RepoDb

A dynamic, lightweight, and very fast ORM .NET Library.

### Build Result

Target               | Status
---------------------|--------------------
Net (Framework)      | [![Build status](https://ci.appveyor.com/api/projects/status/c563cikul4c2a5vc?svg=true)](https://ci.appveyor.com/project/mikependon/repodb)
Net (Standard)       | [![Build status](https://ci.appveyor.com/api/projects/status/0ix2khcfrv1ub6ba?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)
Net (Framework Test) | [![Build status](https://ci.appveyor.com/api/projects/status/xtfp2urkxa29s7b9?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-g4ml5)
Net (Standard Test)  | [![Build status](https://ci.appveyor.com/api/projects/status/78nch60yyoj6wiok?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx)

Package: [https://www.nuget.org/packages/RepoDb](https://www.nuget.org/packages/RepoDb)  
Documentation: [https://repodb.readthedocs.io/en/latest/](https://repodb.readthedocs.io/en/latest/)

### Goal

To be the fastest and easiest-to-use lightweight ORM .NET Library.

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
 - Expression Trees (Expression-Based, Object-Based)
 - Field Mapping
 - Inline Hints
 - Multiple Query
 - Operations (Asynchronous)
 - SQL Statement Builder
 - Tracing
 - Transactions
 - Type Mapping

### Performance Result

Bencher: [https://github.com/FransBouma/RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher)

**Benchmarks run on:** Monday, February 25, 2019 8:44:04 PM

#### Non-change tracking individual fetches (100 elements, 25 runs), no caching

Library                                                              | In Milliseconds
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                            | 0.28ms (0.04ms) per individual fetch
ServiceStack OrmLite v5.0.0.0 (v5.1.0.0)                             | 0.35ms (0.05ms) per individual fetch
Tortuga Chain, Compiled v1.2.6553.39558                              | 0.36ms (0.04ms) per individual fetch
RepoDb (Poco) v1.8.0.5                                               | 0.37ms (0.06ms) per individual fetch
Dapper v1.50.5.0                                                     | 0.41ms (0.05ms) per individual fetch
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 0.43ms (0.02ms) per individual fetch
RepoDb (RawSql) v1.8.0.5                                             | 0.43ms (0.03ms) per individual fetch
Massive using dynamic class                                          | 0.44ms (0.07ms) per individual fetch
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 0.47ms (0.02ms) per individual fetch
Raw DbDataReader materializer using object arrays                    | 0.48ms (0.10ms) per individual fetch
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco typed view with QuerySpec        | 0.48ms (0.07ms) per individual fetch

#### Memory usage, per iteration

Library                                                              | In KB
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                            | 15,163 KB (15,527,336 bytes)
RepoDb (Poco) v1.8.0.5                                               | 15,163 KB (15,527,432 bytes)
RepoDb (RawSql) v1.8.0.5                                             | 15,169 KB (15,533,736 bytes)
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 15,182 KB (15,546,784 bytes)
PetaPoco Fast v4.0.3                                                 | 15,205 KB (15,570,808 bytes)
LINQ to DB v2.0.0.0 (v2.0.0) (normal)                                | 15,972 KB (16,356,248 bytes)
LINQ to DB v2.0.0.0 (v2.0.0) (compiled)                              | 15,976 KB (16,359,872 bytes)
Tortuga Chain, Compiled v1.2.6553.39558                              | 16,228 KB (16,618,464 bytes)
PetaPoco v4.0.3                                                      | 21,114 KB (21,620,992 bytes)
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 30,794 KB (31,533,392 bytes)
Dapper v1.50.5.0                                                     | 30,800 KB (31,539,712 bytes)
Raw DbDataReader materializer using object arrays                    | 31,015 KB (31,759,432 bytes)

#### Non-change tracking individual fetches (100 elements, 25 runs), no caching

Library                                                              | In Milliseconds
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                            | 0.28ms (0.04ms) per individual fetch
ServiceStack OrmLite v5.0.0.0 (v5.1.0.0)                             | 0.35ms (0.05ms) per individual fetch
Tortuga Chain, Compiled v1.2.6553.39558                              | 0.36ms (0.04ms) per individual fetch
RepoDb (Poco) v1.8.0.5                                               | 0.37ms (0.06ms) per individual fetch
Dapper v1.50.5.0                                                     | 0.41ms (0.05ms) per individual fetch
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 0.43ms (0.02ms) per individual fetch
RepoDb (RawSql) v1.8.0.5                                             | 0.43ms (0.03ms) per individual fetch
Massive using dynamic class                                          | 0.44ms (0.07ms) per individual fetch
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 0.47ms (0.02ms) per individual fetch
Raw DbDataReader materializer using object arrays                    | 0.48ms (0.10ms) per individual fetch
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco typed view with QuerySpec        | 0.48ms (0.07ms) per individual fetch
LINQ to DB v2.0.0.0 (v2.0.0) (normal)                                | 0.55ms (0.10ms) per individual fetch

#### Memory usage, per individual element

Library                                                              | In Bytes
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                            | 08 KB (8,192 bytes)
RepoDb (RawSql) v1.8.0.5                                             | 11 KB (12,016 bytes)
ServiceStack OrmLite v5.0.0.0 (v5.1.0.0)                             | 16 KB (16,384 bytes)
Dapper v1.50.5.0                                                     | 16 KB (16,384 bytes)
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 16 KB (16,384 bytes)
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 16 KB (16,384 bytes)
Tortuga Chain, Compiled v1.2.6553.39558                              | 24 KB (24,576 bytes)
RepoDb (Poco) v1.8.0.5                                               | 24 KB (24,576 bytes)
Massive using dynamic class                                          | 24 KB (24,576 bytes)
Tortuga Chain v1.2.6553.39558                                        | 32 KB (32,768 bytes)
Entity Framework Core v2.0.1.0 (v2.0.1.17303)                        | 48 KB (49,152 bytes)
PetaPoco Fast v4.0.3                                                 | 49 KB (50,816 bytes)

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

There are 2 ways of doing this (dynamics and expression-based approach).

Via PrimaryKey:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(1005);
	}

Via Expression:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(c => c.Id == 1005);
	}

Via Object:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.Query<Customer>(new QueryField(nameof(Customer.Id), 1005));
	}

The expressions can also be used on the following operations:

 - BatchQuery
 - Count
 - Delete
 - InlineMerge
 - InlineUpdate
 - Update

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

Via Dynamic:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.ExecuteQuery<ComplexClass>(commandText, new { CustomerId = 1005, OrderDate = DateTime.UtcNow.Date });
	}

Via Object:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var queryGroup = new QueryGroup(new []
		{
			new QueryField("CustomerId", 1005),
			new QueryField("OrderDate", DateTime.UtcNow.Date)
		});
		var customer = connection.ExecuteQuery<Customer>(commandText, queryGroup);
	}

The `ExecuteQuery` method is purposely not being supported by `Expression` based query as we are avoiding the user to bind the complex-class to its target query text.

Note: The most optimal when it comes to performance is to used the `Object-Based`.

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

Via Dynamic:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var customer = connection.ExecuteQuery<ComplexClass>("[dbo].[sp_get_customer_orders_by_date]",
			new { CustomerId = 1005, OrderDate = DateTime.UtcNow.Date },
			commandType: CommandType.StoredProcedure);
	}

Via Object:

	using (var connection = new SqlConnection(ConnectionString))
	{
		var queryGroup = new QueryGroup(new []
		{
			new QueryField("CustomerId", 1005),
			new QueryField("OrderDate", DateTime.UtcNow.Date)
		});
		var customer = connection.ExecuteQuery<Customer>(commandText, queryGroup,
			commandType: CommandType.StoredProcedure);
	}

Please visit our [documentation](https://repodb.readthedocs.io/en/latest/) for further details about the codes.

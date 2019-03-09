## RepoDb

A dynamic, lightweight, and very fast ORM .NET Library.

Package: [https://www.nuget.org/packages/RepoDb](https://www.nuget.org/packages/RepoDb)  
Documentation: [https://repodb.readthedocs.io/en/latest/](https://repodb.readthedocs.io/en/latest/)

## Build Result

Target               | Status
---------------------|--------------------
Net (Framework)      | [![Build status](https://ci.appveyor.com/api/projects/status/c563cikul4c2a5vc?svg=true)](https://ci.appveyor.com/project/mikependon/repodb)
Net (Standard)       | [![Build status](https://ci.appveyor.com/api/projects/status/0ix2khcfrv1ub6ba?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)
Net (Framework Test) | [![Build status](https://ci.appveyor.com/api/projects/status/xtfp2urkxa29s7b9?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-g4ml5)
Net (Standard Test)  | [![Build status](https://ci.appveyor.com/api/projects/status/78nch60yyoj6wiok?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx)

## Goal

To be the fastest and easiest-to-use lightweight ORM .NET Library.

## Vision

To provide more flexibility and fast-switching development approach, whether to use the massive or lightweight ORM operations.

## Principles

 - Keep it as simple as possible (KISS principle)
 - Help developers be more focused on the SOLID principle
 - Make it as fast as possible
 - Make it more flexible
 - Never create complex implementations (especially for complex Join Queries)

## Features

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

## Performance Result

**Bencher:** [https://github.com/FransBouma/RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) | **Benchmarks run on:** Saturday, March 9, 2019 6:01:00 PM

#### Non-change tracking fetches, set fetches (25 runs), no caching

Library                                                              | In Milliseconds
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 179.59ms (7.52ms)        Enum: 1.34ms (0.13ms)
RepoDb (RawSql) v1.8.1.2                                             | 184.61ms (12.26ms)       Enum: 1.22ms (0.10ms)
LINQ to DB v2.0.0.0 (v2.0.0) (normal)                                | 187.74ms (9.21ms)        Enum: 1.17ms (0.22ms)
Handcoded materializer using DbDataReader                            | 188.57ms (13.32ms)       Enum: 1.34ms (0.24ms)
Raw DbDataReader materializer using object arrays                    | 191.08ms (3.85ms)        Enum: 2.32ms (0.13ms)
PetaPoco Fast v4.0.3                                                 | 195.51ms (10.64ms)       Enum: 1.25ms (0.14ms)
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco typed view with QuerySpec        | 198.60ms (10.69ms)       Enum: 1.20ms (0.06ms)
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 199.15ms (10.49ms)       Enum: 1.13ms (0.09ms)
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco typed view with Linq             | 200.52ms (5.29ms)        Enum: 1.23ms (0.16ms)
ServiceStack OrmLite v5.0.0.0 (v5.1.0.0)                             | 202.27ms (7.53ms)        Enum: 1.25ms (0.10ms)
Tortuga Chain, Compiled v1.2.6553.39558                              | 202.56ms (6.85ms)        Enum: 1.30ms (0.16ms)
LINQ to DB v2.0.0.0 (v2.0.0) (compiled)                              | 206.28ms (15.13ms)       Enum: 1.15ms (0.14ms)

#### Memory usage, per iteration

Library                                                              | In KB
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                            | 15,163 KB (15,527,336 bytes)
RepoDb (Poco) v1.8.1.2                                               | 15,163 KB (15,527,344 bytes)
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 15,169 KB (15,533,344 bytes)
RepoDb (RawSql) v1.8.1.2                                             | 15,169 KB (15,533,656 bytes)
PetaPoco Fast v4.0.3                                                 | 15,206 KB (15,571,920 bytes)
LINQ to DB v2.0.0.0 (v2.0.0) (compiled)                              | 15,976 KB (16,360,032 bytes)
LINQ to DB v2.0.0.0 (v2.0.0) (normal)                                | 15,980 KB (16,363,576 bytes)
Tortuga Chain, Compiled v1.2.6553.39558                              | 16,164 KB (16,552,936 bytes)
PetaPoco v4.0.3                                                      | 21,109 KB (21,615,880 bytes)
Dapper v1.50.5.0                                                     | 30,800 KB (31,539,712 bytes)
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 30,801 KB (31,541,024 bytes)
Raw DbDataReader materializer using object arrays                    | 31,015 KB (31,759,432 bytes)

#### Non-change tracking individual fetches (100 elements, 25 runs), no caching

Library                                                              | In Milliseconds
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                            | 0.28ms (0.03ms) per individual fetch
ServiceStack OrmLite v5.0.0.0 (v5.1.0.0)                             | 0.34ms (0.04ms) per individual fetch
Dapper v1.50.5.0                                                     | 0.34ms (0.05ms) per individual fetch
RepoDb (RawSql) v1.8.1.2                                             | 0.35ms (0.06ms) per individual fetch
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 0.35ms (0.05ms) per individual fetch
Tortuga Chain, Compiled v1.2.6553.39558                              | 0.39ms (0.04ms) per individual fetch
Massive using dynamic class                                          | 0.39ms (0.06ms) per individual fetch
Tortuga Chain v1.2.6553.39558                                        | 0.42ms (0.04ms) per individual fetch
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 0.42ms (0.06ms) per individual fetch
RepoDb (Poco) v1.8.1.2                                               | 0.43ms (0.06ms) per individual fetch
Raw DbDataReader materializer using object arrays                    | 0.45ms (0.04ms) per individual fetch
PetaPoco Fast v4.0.3                                                 | 0.50ms (0.07ms) per individual fetch

#### Memory usage, per individual element

Library                                                              | In Bytes
---------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                            | 08 KB (8,192 bytes)
RepoDb (RawSql) v1.8.1.2                                             | 11 KB (11,936 bytes)
ServiceStack OrmLite v5.0.0.0 (v5.1.0.0)                             | 16 KB (16,384 bytes)
Dapper v1.50.5.0                                                     | 16 KB (16,384 bytes)
Handcoded materializer using DbDataReader (GetValues(array), boxing) | 16 KB (16,384 bytes)
LLBLGen Pro v5.4.0.0 (v5.4.1), Poco with Raw SQL                     | 16 KB (16,384 bytes)
Tortuga Chain, Compiled v1.2.6553.39558                              | 24 KB (24,576 bytes)
Massive using dynamic class                                          | 24 KB (24,576 bytes)
RepoDb (Poco) v1.8.1.2                                               | 24 KB (24,576 bytes)
Tortuga Chain v1.2.6553.39558                                        | 32 KB (32,768 bytes)
Entity Framework Core v2.0.1.0 (v2.0.1.17303)                        | 48 KB (49,152 bytes)
PetaPoco Fast v4.0.3                                                 | 50 KB (51,928 bytes)

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

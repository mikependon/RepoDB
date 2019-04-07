## RepoDb

A dynamic, lightweight, efficient and very fast Hybrid ORM library for .NET.

Package: [https://www.nuget.org/packages/RepoDb](https://www.nuget.org/packages/RepoDb)  
Documentation: [https://repodb.readthedocs.io/en/latest/](https://repodb.readthedocs.io/en/latest/)

## Highlight

RepoDb is the fastest and the most efficient .NET ORM Library (in set-fetches) as per the result of [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher).

## Build Result

Target               | Build																																			 | Unit Test																																		 | Integration Test
---------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
Net (Framework)      | [![Build status](https://ci.appveyor.com/api/projects/status/c563cikul4c2a5vc?svg=true)](https://ci.appveyor.com/project/mikependon/repodb)		 | [![Build status](https://ci.appveyor.com/api/projects/status/xtfp2urkxa29s7b9?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-g4ml5) | [![Build status](https://ci.appveyor.com/api/projects/status/qnvun61tyyy1vwlb?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-neg8t) |
Net (Standard)       | [![Build status](https://ci.appveyor.com/api/projects/status/0ix2khcfrv1ub6ba?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw) | [![Build status](https://ci.appveyor.com/api/projects/status/78nch60yyoj6wiok?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx) | [![Build status](https://ci.appveyor.com/api/projects/status/3fsp38vaqchmec7y?svg=true)](https://ci.appveyor.com/project/mikependon/repodb-qksas) |

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
 - Expression Trees
 - Field Mapping
 - Inline Hints
 - Multi-Resultset Query
 - Operations (native ORM)
 - Statement Building
 - Tracing
 - Transaction
 - Type Mapping

## Performance Result

**Bencher:** [https://github.com/FransBouma/RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) | **Benchmarks run on:** Thursday, 7 March 2019 15:54:14

#### Non-change tracking fetches, set fetches (25 runs), no caching

Library                                                               | In Milliseconds
----------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                             | 115,02ms (2,81ms)	Enum| 1,07ms (0,03ms)
RepoDb (RawSql) v1.8.0.6                                              | 125,95ms (1,31ms)	Enum| 1,05ms (0,01ms)
LINQ to DB v2.6.4.0 (v2.6.4) (compiled)                               | 126,02ms (4,21ms)	Enum| 0,93ms (0,10ms)
LINQ to DB v2.6.4.0 (v2.6.4) (normal)                                 | 126,09ms (3,62ms)	Enum| 0,93ms (0,12ms)
Entity Framework Core v2.2.2.0 (v2.2.2.19024)                         | 130,29ms (3,31ms)	Enum| 0,96ms (0,14ms)
Handcoded materializer using DbDataReader (GetValues(array), boxing)  | 131,68ms (1,53ms)	Enum| 1,71ms (0,10ms)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco with Raw SQL                      | 132,02ms (2,02ms)	Enum| 0,92ms (0,02ms)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with QuerySpec         | 143,30ms (4,81ms)	Enum| 1,60ms (0,00ms)
Raw DbDataReader materializer using object arrays                     | 144,11ms (0,74ms)	Enum| 4,20ms (0,05ms)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with Linq              | 146,38ms (1,00ms)	Enum| 1,40ms (0,02ms)
Handcoded materializer using DbDataReader (GetValue(Ordinal), boxing) | 147,32ms (1,24ms)	Enum| 1,79ms (0,15ms)
Dapper v1.60.0.0                                                      | 153,23ms (0,99ms)	Enum| 1,64ms (0,08ms)
ServiceStack OrmLite v5.0.0.0 (v5.4.0.0)                              | 170,57ms (2,60ms)	Enum| 1,63ms (0,08ms)
NPoco v3.9.4.0 (v3.9.4.0)                                             | 181,33ms (5,91ms)	Enum| 1,70ms (0,10ms)
LLBLGen Pro v5.5.0.0 (v5.5.2), DataTable based TypedView              | 234,47ms (7,27ms)	Enum| 4,06ms (0,14ms)
Tortuga Chain v2.1.0.0                                                | 287,25ms (4,86ms)	Enum| 1,66ms (0,07ms)

#### Memory usage, per iteration

Library                                                               | In KB
----------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                             | 15.202 KB (15.567.648 bytes)
RepoDb (RawSql) v1.8.0.6                                              | 15.205 KB (15.570.280 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco with Raw SQL                      | 15.206 KB (15.571.696 bytes)
LINQ to DB v2.6.4.0 (v2.6.4) (normal)                                 | 15.986 KB (16.370.632 bytes)
LINQ to DB v2.6.4.0 (v2.6.4) (compiled)                               | 15.993 KB (16.377.728 bytes)
Entity Framework Core v2.2.2.0 (v2.2.2.19024)                         | 20.153 KB (20.637.528 bytes)
Handcoded materializer using DbDataReader (GetValues(array), boxing)  | 30.834 KB (31.574.144 bytes)
Handcoded materializer using DbDataReader (GetValue(Ordinal), boxing) | 30.834 KB (31.574.144 bytes)
Dapper v1.60.0.0                                                      | 30.834 KB (31.574.216 bytes)
Raw DbDataReader materializer using object arrays                     | 31.048 KB (31.793.456 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with QuerySpec         | 31.861 KB (32.626.336 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with Linq              | 32.469 KB (33.248.608 bytes)
ServiceStack OrmLite v5.0.0.0 (v5.4.0.0)                              | 33.784 KB (34.595.352 bytes)
NPoco v3.9.4.0 (v3.9.4.0)                                             | 41.031 KB (42.016.352 bytes)
Tortuga Chain v2.1.0.0                                                | 43.685 KB (44.734.112 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), DataTable based TypedView              | 55.353 KB (56.681.880 bytes)

#### Non-change tracking individual fetches (100 elements, 25 runs), no caching

Library                                                               | In Milliseconds
----------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader (GetValues(array), boxing)  | 0,10ms (0,00ms) per individual fetch
Handcoded materializer using DbDataReader (GetValue(Ordinal), boxing) | 0,10ms (0,00ms) per individual fetch
Handcoded materializer using DbDataReader                             | 0,10ms (0,00ms) per individual fetch
RepoDb (RawSql) v1.8.0.6                                              | 0,12ms (0,00ms) per individual fetch
Dapper v1.60.0.0                                                      | 0,12ms (0,03ms) per individual fetch
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco with Raw SQL                      | 0,13ms (0,00ms) per individual fetch
ServiceStack OrmLite v5.0.0.0 (v5.4.0.0)                              | 0,13ms (0,00ms) per individual fetch
Raw DbDataReader materializer using object arrays                     | 0,18ms (0,00ms) per individual fetch
Tortuga Chain v2.1.0.0                                                | 0,18ms (0,00ms) per individual fetch
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with QuerySpec         | 0,22ms (0,00ms) per individual fetch
Entity Framework Core v2.2.2.0 (v2.2.2.19024)                         | 0,29ms (0,00ms) per individual fetch
LINQ to DB v2.6.4.0 (v2.6.4) (compiled)                               | 0,36ms (0,03ms) per individual fetch
LLBLGen Pro v5.5.0.0 (v5.5.2), DataTable based TypedView              | 0,37ms (0,03ms) per individual fetch
LINQ to DB v2.6.4.0 (v2.6.4) (normal)                                 | 0,39ms (0,00ms) per individual fetch
NPoco v3.9.4.0 (v3.9.4.0)                                             | 0,49ms (0,03ms) per individual fetch
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with Linq              | 0,79ms (0,00ms) per individual fetch

#### Memory usage, per individual element

Library                                                               | In Bytes
----------------------------------------------------------------------|--------------------------------
Handcoded materializer using DbDataReader                             | 14 KB (15.296 bytes)
Dapper v1.60.0.0                                                      | 15 KB (15.896 bytes)
Handcoded materializer using DbDataReader (GetValues(array), boxing)  | 15 KB (16.048 bytes)
Handcoded materializer using DbDataReader (GetValue(Ordinal), boxing) | 15 KB (16.048 bytes)
ServiceStack OrmLite v5.0.0.0 (v5.4.0.0)                              | 16 KB (17.336 bytes)
RepoDb (RawSql) v1.8.0.6                                              | 17 KB (18.368 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco with Raw SQL                      | 20 KB (20.544 bytes)
Tortuga Chain v2.1.0.0                                                | 30 KB (31.416 bytes)
Entity Framework Core v2.2.2.0 (v2.2.2.19024)                         | 57 KB (59.048 bytes)
LINQ to DB v2.6.4.0 (v2.6.4) (compiled)                               | 61 KB (62.664 bytes)
LINQ to DB v2.6.4.0 (v2.6.4) (normal)                                 | 64 KB (66.176 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with QuerySpec         | 65 KB (67.008 bytes)
NPoco v3.9.4.0 (v3.9.4.0)                                             | 138 KB (141.336 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), DataTable based TypedView              | 142 KB (146.408 bytes)
LLBLGen Pro v5.5.0.0 (v5.5.2), Poco typed view with Linq              | 248 KB (254.528 bytes)
Raw DbDataReader materializer using object arrays                     | 257 KB (264.016 bytes)

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

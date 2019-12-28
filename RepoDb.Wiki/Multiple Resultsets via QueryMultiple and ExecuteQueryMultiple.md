## Introduction

In this page, you will learn the following.

- [Querying Multiple Resultsets](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple#querying-multiple-resultsets)
- [Executing Multiple SQL Statements](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple#executing-multiple-sql-statements)

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*. Please have at least *Visual Studio 2017* and *SQL Server 2016* installed in your machine.

Please follow the steps at [Creating an Inventory Database and Project](https://github.com/mikependon/RepoDb/wiki/Creating-an-Inventory-Database-and-Project) before proceeding to the next sections.

## What is Multiple Resultsets?

A *Multiple Resultsets* is a terminology behind the *results* of multiple-queries execution from the database. It help maximizes the performance of the query as the *multiple SELECT statements* are being issued to the database engine once and being executed in *one-go*.

In *RepoDb*, there are 2 ways of doing this.

- [QueryMultiple<T1, ..., T7>](https://repodb.readthedocs.io/en/latest/pages/connection.html#querymultiple)
- [ExecuteQueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple)

You will further learn the practicality in the next sections.

## Querying Multiple Resultsets

In this section, you will learn on how to query *multiple data entities* via [QueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#querymultiple) operation.

The mentioned operation is implemented in a way to support querying the database using *multiple-SELECT statements* in *one-go*.

**It has the following implementations and supports:**

- Max to 7 *Tuples*
- *Linq* expressions
- With *query-hints*
- *Filtering* the number of rows
- Can be *ordered*

**Disclaimer**:

> This feature is not an alternative for *JOIN* query. It is completely addressing different scenarios and issues.

### QueryMultiple

This feature is mostly used to query the *parent-child* relationships.

Please see sample code snippets below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	// Query
	var result = connection.QueryMultiple<Customer, Order>(
		customer => customer.Id == 10045,
		order => order.CustomerId == 10045);
		
	// Extract
	var customer = result.Item1.FirstOrDefault();
	var orders = result.Item2.AsList();
	
	// Process
	ProcessCustomer(customer);
	orders.ForEach(order => ProcessOrder(order));
}
```

In the sample codes above, the `result` variable will contain the *Tuple* object of type *IEnumerable&lt;Customer&gt;* and *IEnumerable&lt;Order&gt;* (*Tuple&lt;IEnumerable&lt;Customer&gt;, IEnumerable&lt;Order&gt;&gt;*). The mappings will happen based on the *order* of the generic types you passed during the calls to the `QueryMultiple()` operation. In this case, the *Item1* will be of type *IEnumerable&lt;Customer&gt;* and *Item2* will be of type *IEnumerable&lt;Order&gt;*.

Below are the actual SQL Statements that will be executed in the database.

```
SELECT [Id], [Name], [Address], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] = @Id);
SELECT [Id], [ProductId], [CustomerId], [Quantity], ..., [ModifiedBy] FROM [Order] WHERE ([CustomerId] = @CustomerId);
```

Also, let us say you have an *Orders* property in the *Customer* class of type *IEnumerable&lt;Order&gt;*. The most *practical* way to do multiple-relationship queries is through the codes below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	// Target
	var ids = new [] { 10045, ..., 10112 };
	
	// Query
	var result = connection.QueryMultiple<Customer, Order>(
		customer => ids.Contains(customer.Id),
		order => ids.Contains(order.CustomerId));
		
	// Extract
	var customers = result.Item1.AsList();
	var orders = result.Item2.AsList();
	
	// Mappings
	customers.ForEach(customer => 
		customer.Orders = orders.Where(
			order => order.CustomerId == customer.Id).AsList());
}
```

In addition, the *QueryMultiple* operation also supports [QueryHints](https://repodb.readthedocs.io/en/latest/pages/hints.html), [Top](https://repodb.readthedocs.io/en/latest/pages/connection.html#querymultiple) and [Ordering](https://repodb.readthedocs.io/en/latest/pages/orderfield.html).

In which the order of the arguments varies on the number of generic-types you passed. Let us say you passed 3 generic types, then there will be 3 *hints<N>*, *top<N>* and *orderBy<N>* arguments (max 7).

### QueryHints

Below is a sample code make a calls on the *QueryMultiple* with *QueryHints*.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	// Query
	var result = connection.QueryMultiple<Customer, Order>(
		customer => customer.Id == 10045,
		order => order.CustomerId == 10045,
		hints1: SqlServerTableHints.NoLock,
		hints2: SqlServerTableHints.NoLock);
		
	// More codes here
	...
}
```

### Top

Below is a sample code make a calls on the *QueryMultiple* with *Top*.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	// Query
	var result = connection.QueryMultiple<Customer, Customer>(
		customer => customer.Address == "New York",
		customer => customer.Address == "Chicago",
		top1: 5,
		top2: 100);
		
	// More codes here
	...
}
```

### OrderField

Below is a sample code make a calls on the *QueryMultiple* with *OrderField*.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	// Query
	var result = connection.QueryMultiple<Customer, Order>(
		customer => customer.Id == 10045,
		order => order.CustomerId = 10045,
		orderBy1: OrderField.Parse(new { CustomerId = Order.Ascending },
		orderBy2: OrderField.Parse(new { CustomerId = Order.Ascending, OrderDateUtc = Order.Descending });
		
	// More codes here
	...
}
```

## Executing Multiple SQL Statements

On the other hand, the other way of querying *Multiple Resultsets* is to use the [ExecuteQueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple) operation.

This operation is implemented to support a much more dynamic query execution of the multiple-raw-SQL statements in one-go.

You can do the following:

- *Unlimited* number of raw-SQLs
- Combination of *Normal* and *Stored Procedures* scripts execution
- Mappings to *C# CLR Types*
- Accepts different type of *Parameters* (ie: *Dynamic*, *ExpandoObject*, *QueryObjects* and *Dictionaries*)

### Extract

Below is the sample codes on how to query the *Customer* with its corresponding *Orders*. Almost identical to the queries we made at *QueryMultiple* operation above.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;";

	using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 10045 }))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var orders = result.Extract<Order>();
	}
}
```

Notice in the code above, the result of the operation is of type *QueryMultipleExtractor* object. It contains 2 important methods.

- Extract - is used to *extract* the query result on any *C# CLR Type*.
- Scalar - is used to *retrieve* the first-column of the first-row of the query result.

### Scalar

Below is the sample codes on how to use the *Scalar* method.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT COUNT(*) FROM [dbo].[Order] WHERE CustomerId = @CustomerId;
		SELECT GETUTCDATE() AS CurrentDateTimeUtc;";

	using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 10045 }))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var ordersCount = result.Scalar<int>();
		
		// Extract the third  result
		var currentDateTimeUtc = result.Scalar<DateTime>();
	}
}
```

### Execution with StoredProcedures

Below is the sample codes on how to use execute multiple-SQL statements with *Stored Procedures*.

Let us say, you have a stored procedure named *sp_GetCustomerOrderedProducts* that returns the list of *Product* the *Customer* has ordered that varies on the *OrderDates*.

```
CREATE PROCEDURE [dbo].[sp_GetCustomerOrderedProducts]
(
	@CustomerId BIGINT
	, @OrderDateUtc NVARCHAR(64)
)
AS
BEGIN
	
	SELECT P.*
	FROM [dbo].[Product] P
	INNER JOIN [dbo].[Order] O ON O.ProductId = P.Id
	INNER JOIN [dbo].[Customer] C ON C.Id = O.CustomerId
	WHERE (Id = @CustomerId)
		AND (O.OrderDateUtc >= FromDate);
	
END
```

Then you can do call is via the scripts below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId AND OrderDateUtc >= @OrderDateUtc;
		CALL [dbo].[sp_GetCustomerOrderedProducts](@CustomerId, @OrderDateUtc);
		SELECT GETUTCDATE() AS CurrentDateTimeUtc;";

	using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 10045, OrderDateUtc = DateTime.UtcNow.AddDays(-7).Date }))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var orders = result.Extract<Order>();
		
		// Extract the third result
		var products = result.Extract<Product>();
		
		// Extract the fourth result
		var currentDateTimeUtc = result.Scalar<DateTime>();
	}
}
```

> You do *NOT* need to set the *CommandType* to *StoredProcedure*.

**Note**: You can also execute a stored procedure that does not return any values, or returning a single value that can be extracted through *Scalar* method.

### Different Types of Parameters

This operation also supports different types of parameters:
- Dynamics
- ExpandoObject (as Dynamic)
- ExpandoObject (as Dictionary&lt;string, object&gt;)
- Dictionary&lt;string, object&gt;
- QueryObjects (QueryGroup or QueryField)

### Dynamic Parameters

Below is a sample code to use the *Dynamics* as the parameters.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId";
	var param = new { CustomerId = 10045 };

	using (var result = connection.ExecuteQueryMultiple(commandText, param))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var orders = result.Extract<Order>();
	}
}
```

### ExpandoObject Parameters

Below is a sample code to use the *ExpandoObject* as the parameters.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId";
	var param = (dynamic)new ExpandoObject();

        // Set each parameter
        param.CustomerId = 10045;

	using (var result = connection.ExecuteQueryMultiple(commandText, param))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var orders = result.Extract<Order>();
	}
}
```

### ExpandoObject (as Dictionary) Parameters

Below is a sample code to use the *ExpandoObject (as Dictionary)* as the parameters.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId";
	var param = new ExpandoObject() as IDictionary<string, object>;

        // Set each parameter
        param.Add("CustomerId", 10045);

	using (var result = connection.ExecuteQueryMultiple(commandText, param))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var orders = result.Extract<Order>();
	}
}
```

### Dictionary Parameters

Below is a sample code to use the *Dictionary* as the parameters.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId";
	var param = new Dictionary<string, object>
        {
                { "CustomerId", 10045 },
        };

        // Set each parameter
        param.Add("CustomerId", 10045);

	using (var result = connection.ExecuteQueryMultiple(commandText, param))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var orders = result.Extract<Order>();
	}
}
```

### QueryGroup or QueryField Parameters

Below is a sample code to use the *QueryObjects* as the parameters.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var commandText = @"SELECT * FROM [dbo].[Customer] WHERE Id = @CustomerId;
		SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId";
	var param = new QueryGroup(new QueryField("CustomerId", 10045)),

        // Set each parameter
        param.Add("CustomerId", 10045);

	using (var result = connection.ExecuteQueryMultiple(commandText, param))
	{
		// Extract the first result
		var customer = result.Extract<Customer>().FirstOrDefault();

		// Extract the second result
		var orders = result.Extract<Order>();
	}
}
```

You can see more on our [documentation](https://repodb.readthedocs.io/en/latest/pages/rawsql.html#array-values).

--------

**Voila! You have completed this tutorial.**
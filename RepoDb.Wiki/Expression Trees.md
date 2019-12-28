## Introduction

In this page, you will learn the following.

- [Linq Expressions](https://github.com/mikependon/RepoDb/wiki/Expression-Trees#linq-expressions)
- [Dynamic Expressions](https://github.com/mikependon/RepoDb/wiki/Expression-Trees#dynamic-expressions)
- [QueryObject Expressions](https://github.com/mikependon/RepoDb/wiki/Expression-Trees#queryobject-expressions)

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*.

## List of Operations

- *Equal* - an equal operation.
- *NotEqual* - a not-equal operation.
- *LessThan* - a less-than operation.
- *GreaterThan* - a greater-than operation.
- *LessThanOrEqual* - a less-than-or-equal operation.
- *GreaterThanOrEqual* - a greater-than-or-equal operation.
- *Like* - a like operation. Defines the (LIKE) keyword in SQL Statement.
- *NotLike* - a not-like operation. Defines the (NOT LIKE) keyword in SQL Statement.
- *Between* - a between operation. Defines the (BETWEEN) keyword in SQL Statement.
- *NotBetween* - a not-between operation. Defines the (NOT BETWEEN) keyword in SQL Statement.
- *In* - an in operation. Defines the (IN) keyword in SQL Statement.
- *NotIn* - a not-in operation. Defines the (NOT IN) keyword in SQL Statement.

## Linq Expressions

By using the *Linq Expressions*, you can write a code through *Linq*. See sample code below.

### Equal Expression

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(e => e.Id == 10045);
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] (WHERE [Id] = 10045);
```

**You can do the same on the following operations.**

- *NotEqual* as (*!=*)
- *LessThan* as (*<*)
- *GreaterThan* as (*>*)
- *LessThanOrEqual* as (*<=*)
- *GreaterThanOrEqual* as (*>=*)

### Like and NotLike Expression

For *Like*, you can do it like this.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(e => e.Name.Contains("ohn"));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Name] LIKE '%ohn%');
```

Or, via *StartsWith* and *EndsWith*.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(e => e.Name.StartsWith("John"));
	var customer = connection.Query<Customer>(e => e.Name.EndsWith("Doe"));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Name] LIKE 'John%');
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Name] LIKE '%Doe');
```

### In and NotIn Expression

For *In*, you can do it like this.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var ids = new [] { 10045, 11211, ..., 14100 }
	var customer = connection.Query<Customer>(e => ids.Contains(e.Id));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] IN (10045, 11211, ..., 14100 ));
```

For *NoIn*, you can do it like this as an opposite to *In* operation.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var ids = new [] { 10045, 11211, ..., 14100 }
	var customer = connection.Query<Customer>(e => !ids.Contains(e.Id));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE NOT ([Id] IN (10045, 11211, ..., 14100 ));
```

Or, this:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var ids = new [] { 10045, 11211, ..., 14100 }
	var customer = connection.Query<Customer>(e => ids.Contains(e.Id) == false);
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] NOT IN (10045, 11211, ..., 14100 ));
```

**Note**: See the difference of (NOT IN) and (NOT (IN)). They are also affecting execution performance based on your own scenario.

### Between and NotBetween Expression

For *Between* and *NotBetween*, you can use the combinations of *LessThanOrEqual* and *GreaterThanOrEqual*.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var ids = new [] { 10045, 14100 }
	var customer = connection.Query<Customer>(e => e.Id >= 10045 && e.Id <= 14100);
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] >= 10045 && [Id] <= 14100);
```

## Dynamic Expressions

A *Dynamic Expression* is referring to using a *dynamic* object. With this regards, it is only targeting the [Equal](https://github.com/mikependon/RepoDb/wiki/Expression-Trees#equal-expression) operation.

A sample code snippets below is querying a *Customer* from the database.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(new { Id = 10045 });
}
```

Or when calling the *Execute* operations.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Order] WHERE (CustomerId = @CustomerId);", new { CustomerId = 10045 });
}
```

**Note**: Do not use this if you wish to use other operations (not just *Equal*). However, using *dynamic* is much more faster than using *Linq* expression due to the fact that it only uses *Reflection* and not doing any *Parsing* like what we are doing in the *Linq* expressions.

## QueryObject Expressions

This is the most *optimal* and most *performant* way of using an expression as it does not do any *Linq Expression Parsing* or *Object Properties Reflecting*. What it does is to skip both process and directly proceed with the actual SQL Statements transformation.

As mentioned at the [list of operations](https://github.com/mikependon/RepoDb/wiki/Expression-Trees#list-of-operations) section, all operations are supported.

A sample code below is querying a *Customer* record from the database.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(new QueryField("Id", 10045));
}
```

Or, with *Operation.Equal* in between.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(new QueryField("Id", Operation.Equal, 10045));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] = 10045);
```

**You can do the same on the following operations.**

- *NotEqual* as (*!=*)
- *LessThan* as (*<*)
- *GreaterThan* as (*>*)
- *LessThanOrEqual* as (*<=*)
- *GreaterThanOrEqual* as (*>=*)
- *Like* as (*LIKE*)
- *NotLike* as (*NOT LIKE*)

### In and NotIn using QueryObject

For *In*, you can do it like this.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var ids = new [] { 10045, 11211, ..., 14100 }
	var customer = connection.Query<Customer>(new QueryField("Id", Operation.In, ids));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] NOT IN (10045, 11211, ..., 14100 ));
```

And for *NotIn*, you can do it like this.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var ids = new [] { 10045, 11211, ..., 14100 }
	var customer = connection.Query<Customer>(new QueryField("Id", Operation.NotIn, ids));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] NOT IN (10045, 11211, ..., 14100 ));
```


### Between and NotBetween using QueryObject

For *Between*, you can do it like this.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var dates = new [] { DateTime.UtcNow.Date.AddDays(-7), DateTime.UtcNow }
	var customer = connection.Query<Customer>(new QueryField("DateInsertedUtc", Operation.Between, dates));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([DateInsertedUtc] BETWEEN '2020-01-01 00:00:00.00000' AND '2020-01-07 12:00:00.00000'));
```

And for *NotBetween*, you can do it like this.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var dates = new [] { DateTime.UtcNow.Date.AddDays(-7), DateTime.UtcNow }
	var customer = connection.Query<Customer>(new QueryField("DateInsertedUtc", Operation.NotBetween, dates));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([DateInsertedUtc] NOT BETWEEN '2020-01-01 00:00:00.00000' AND '2020-01-07 12:00:00.00000'));
```

You can also do it with *Numbers*, not just *Dates*.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var idRange = new [] { 10000, 20000 }
	var customer = connection.Query<Customer>(new QueryField("Id", Operation.Between, idRange));
}
```

In which it executes this SQL.

```
SELECT [Id], [Name], ..., [ModifiedBy] FROM [Customer] WHERE ([Id] BETWEEN 10000 AND 20000));
```

**Note**: The *maximum* number of *parameters* (or *Array*) is *2*. An exception will be thrown if the provided *parameters* (or *Array*) is less or more.

---------

**Voila! You have completed this tutorial!**

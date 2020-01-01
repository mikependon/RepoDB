This page is still in-progress.

## Introduction

In this page, we will share you the differences and what sets [*RepoDb*](https://github.com/mikependon/RepoDb) apart from [*Dapper*](https://github.com/StackExchange/Dapper). This page will also help you explain the compelling reason why you should choose *RepoDb* as your ORM.

> All the contents of this page is written by the author itself. Please allow yourselves to check or comments right away if you think I made this page bias for *RepoDb*. Please do a pull-requests for any change.

**Note**: The programming language and database provider we are using on our samples below are *C#* and *SQL Server*.

## Topics

- [Basic CRUD Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#basic-crud-differences)
- [Advance Query Differences](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#advance-query-differences)
- [Features](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#features)
- [Performance and Efficiency](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#performance-and-efficiency)
- [Library Support](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/RepoDb%20vs%20Dapper.md#library-support)

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
	, [DateInsertedUtc] DATETIME2(5) NOT NULL
	, [DateModifiedUtc] DATETIME2(5) NOT NULL
	, [ModifiedBy] NVARCHAR(64) NOT NULL
	, CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([Id] ASC )
)
ON [PRIMARY];
GO

CREATE TABLE [dbo].[Product]
(
	[Id] BIGINT IDENTITY(1,1) 
	, [Name] NVARCHAR(128) NOT NULL
	, [Price] Decimal(18,2)
	, [DateInsertedUtc] DATETIME2(5) NOT NULL
	, [DateModifiedUtc] DATETIME2(5) NOT NULL
	, [ModifiedBy] NVARCHAR(64) NOT NULL
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
	, [DateInsertedUtc] DATETIME2(5) NOT NULL
	, [DateModifiedUtc] DATETIME2(5) NOT NULL
	, [ModifiedBy] NVARCHAR(64) NOT NULL
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
	public DateTime DateInsertedUtc { get; set; }
	public DateTime DateModifiedUtc { get; set; }
	public string ModifiedBy { get; set; }
}

public class Product
{
	public long Id { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public DateTime DateInsertedUtc { get; set; }
	public DateTime DateModifiedUtc { get; set; }
	public string ModifiedBy { get; set; }
}

public class Order
{
	public long Id { get; set; }
	public long ProductId { get; set; }
	public long CustomerId { get; set; }
	public int Quantity { get; set; }
	public DateTime OrderDateUtc{ get; set; }
	public DateTime DateInsertedUtc { get; set; }
	public DateTime DateModifiedUtc { get; set; }
	public string ModifiedBy { get; set; }
}
```

## Basic CRUD Differences

### Querying a Record

**Dapper**:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

**RepoDb**:

Raw-SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

Fluent:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(e => e.Id == 10045).FirstOrDefault();
}
```

## Advance Query Differences

## Features

## Performance

## Library Support



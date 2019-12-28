## Introduction

The intention of this page is to help you manage the creation of the *Inventory* database and *C#* project.

The following objects will be created.

- An *Inventory* Database
- A *Customer* Table
- A *Product* Table
- An *Order* Table
- An *Inventory* C# Project
- A *Customer* Class
- A *Product* Class
- An *Order* Class

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*. Please have at least *Visual Studio 2017* and *SQL Server 2016* installed in your machine.

Please follow the steps below to create the database and C# project.

## Create the Inventory Database

- Open the Microsoft SQL Server Management Studio.
- Click `File` > `Connect Object Explorer...` menu.
- Enter the following values:
  - Server name = `.` 
  - Authentication = `Windows Authentication`
- Click the `Connect` button.
- In the `Object Explorer`, expand the top node entry.
- Right-click in the `Databases` and click the `New Database...` context-menu.
- Enter the following values:
  - Database name = `Inventory`.
  - Owner = `<default>`
- Click the `OK` button.

Or, please execute the script below.

```
CREATE DATABASE [Inventory];
GO
```

**Note**: Make sure that you have enough permission to create a database.

## Creating the Customer Table

In the query window, please execute the script below.

```
USE [Inventory];
GO

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
```

## Creating the Product Table

In the query window, please execute the script below.

```
USE [Inventory];
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
GO
```

## Creating the Order Table

In the query window, please execute the script below.

```
USE [Inventory];
GO

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
GO
```

## Create the C# InventoryProject

- Open the Microsoft Visual Studio.
- Click `File` > `New` > `Project...`.
- Enter the following values:
  - Project = `Console App (.NET Core)`
  - Name = `InventoryProject`
  - Location = `<Do not change the default>`
  - Create directory for solution = `checked`
- Click the `OK` button.

## Create the Customer Class

- In the `Solution Explorer`, right-click the `InventoryProject` and click the `Add` > `New Folder` and name it to `Models`.
- Right-click the `Models` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = *Customer*
- Click the `Add` button.
- The new file named `Customer.cs` will be created. Replace the class implementation with the script below.
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
```
- Press `Ctrl+S` keys to save the changes.

## Create the Product Class

- Right-click the `Models` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = *Product*
- Click the `Add` button.
- The new file named `Product.cs` will be created. Replace the class implementation with the script below.
```csharp
public class Product
{
	public long Id { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public DateTime DateInsertedUtc { get; set; }
	public DateTime DateModifiedUtc { get; set; }
	public string ModifiedBy { get; set; }
}
```
- Press `Ctrl+S` keys to save the changes.

## Create the Order Class

- Right-click the `Models` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = *Order*
- Click the `Add` button.
- The new file named `Order.cs` will be created. Replace the class implementation with the script below.
```csharp
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
- Press `Ctrl+S` keys to save the changes.

-------

**Voila! You have completed this tutorial.**
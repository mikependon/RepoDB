## Introduction

We will introduce you on how to work with *Transactions* in *RepoDb*. As *RepoDb* is just an extension of *ADO.NET*, then all *Transaction* based operation is very relative to *ADO.NET*.

In this page you will learn the following:

- [Wrapping the DbConnection object into Transaction](https://github.com/mikependon/RepoDb/wiki/Working-with-Transactions#Wrapping-the-DbConnection-object-into-Transaction)
- [Using the Transaction object in the Repository](https://github.com/mikependon/RepoDb/wiki/Working-with-Transactions#Using-the-Transaction-object-in-the-Repository)
- [Transaction for Multiple Operation Calls](https://github.com/mikependon/RepoDb/wiki/Working-with-Transactions#Transaction-for-Multiple-Operation-Calls)

In this tutorial, the programming language we will be using is C# and the database provider we will be using is SQL Server. Please have at least Visual Studio 2017 and SQL Server 2016 installed in your machine.

## What is Database Transaction

A *database transaction* is a *Unit of Work* that is being executed within RDBMS indepedently for manipulating the data (ie: *Insert*, *Delete* or *Update*). The transaction is [*ACID*](https://en.wikipedia.org/wiki/ACID) on its on way (*Atomic*, *Consistent*, *Isolated* and *Durable*)

The transaction is being started by initiating the *BEGIN TRANSACTION* command (ie: in *SQL Server*, could be different in other) signalling the database engine that the current context must be executed within the transaction. After this, any kind of operation and calculation can be done; each action is logged automatically within the database management system.

Any failure in the operation or calculation could lead to an invalid data, and therefore the state must be rolled-back. If this happens while within the transaction, you have an opportunity to *Rollback* all activities you have executed within the transaction by calling the *ROLLBACK* command.

If all activities has been completed without any error, then the *COMMIT* command must be called to *Fully-Commit* the activity within the database management system.

## Before we begin

Let us create the *Database*, *Table*, *C# Project* and the *Models* we will gonna use on this tutorial.

### Create a database

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

### Create a table

- In the `Object Explorer`, under `Databases` section, right-click the `Inventory` database and click the `New Query` context-menu.
- In the query window, copy the script below.
```
CREATE TABLE [dbo].[Order]
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
```
**Note**: The SQL script above creates a table named `[Customer]` under the schema of `[dbo]`. The field `[Id]` is being set as the **primary** field and is also an **identity**.
- Press the `F5` key.
- In the `Object Explorer`, the table named `Customer` is now available under `Databases` > `Inventory` > `Tables`.

### Create a C# Project

- Open the Microsoft Visual Studio.
- Click `File` > `New` > `Project...`.
- Enter the following values:
  - Project = `Console App (.NET Core)`
  - Name = `InventoryTransactionProject`
  - Location = `<Do not change the default>`
  - Create directory for solution = `checked`
- Click the `OK` button.

### Create a C# class object

- In the `Solution Explorer`, right-click the project `InventoryProject` and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = `Customer`
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

## Wrapping the DbConnection object into Transaction

To begin with, you have to initiate the *DbConnection* object and simply call the `BeginTransaction()` method.

```csharp
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var transaction = connection.BeginTransaction();
}
```

The script above returns an instance of *DbTransaction* object.

To *rollback*, simply call the `Rollback()` method of the *DbTransaction* object. See the code snippets below.

```csharp
transaction.Rollback();
```

To *commit*, simply call the `Commit()` method of the *DbTransaction* object. See the code snippets below.

```csharp
transaction.Commit();
```

To ensure that the activity is being rolled-back properly, we suggest that you **always** wrap any transactional activities within the *try-catch* block. See the codes below.

```csharp
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	using (var transaction = connection.BeginTransaction())
	{
		try
		{
			...
			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
		}
	}
}
```

**Note**: By using the *using* keyword without calling the `Rollback()` method would automatically *Rollback* all activities within the scope.

#### Testing the Transaction via Rollback

In your C# Project, double click the *Program.cs* file. Copy the code snippets below and place it just right after the `Main()` method.

```csharp
public static void DoTransaction()
{
	using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
	{
		using (var transaction = connection.BeginTransaction())
		{
			var customer = new
			{
				Name = "John Doe",
				Address = "New York",
				DateInsertedUtc = DateTime.UtcNow,
				DateModifiedUtc = DateTime.UtcNow,
				ModifiedBy = "ClientApplication"
			};
			connection.Insert<Customer>(customer);
			Console.WriteLine("A record has been inserted via Transaction.");
		}
	}
}
```

Replace the implementation of your `Main()` method with the codes provided below.

```csharp
public static void Main(string[] args)
{
	DoTransaction();
	Console.ReadLine();
}
```

Build the solution by pressing the `Alt + R + B` and then press `F5` to run.

A message `A record has been inserted via Transaction.` is shown in the `Console`.

Now, let us check the database.

In your Microsoft SQL Server Management Studio, write the script below in the query window and press the `F5` key.

```csharp
SELECT * FROM [dbo].[Customer];
```

You will notice that there is *NO* records returned from the query. This is because we have not really committed the *Transaction* object in our *C# Project*.

If you are working with multiple relationships (ie: *Order*, *OrderDetail* with *Products*), all activities will also be rolled-back automatically.

#### Committing the Transaction

Now, let us go back to your *C# Project*. Inside the `DoTransaction` method, add the `transaction.Commit()` calls just before the `Console.WriteLine()`. Please see the code snippets below.

```csharp
using (var transaction = connection.BeginTransaction())
{
	var customer = new
	{
		Name = "John Doe",
		Address = "New York",
		DateInsertedUtc = DateTime.UtcNow,
		DateModifiedUtc = DateTime.UtcNow,
		ModifiedBy = "ClientApplication"
	};
	connection.Insert<Customer>(customer);
	transaction.Commit();
	Console.WriteLine("A record has been inserted via Transaction.");
}
```

Build the solution by pressing the `Alt + R + B` and then press `F5` to run.

A message `A record has been inserted via Transaction.` is shown in the `Console`. This indicates that the record has been saved to the database.

Again, let us check the database.

In your Microsoft SQL Server Management Studio, write the script below in the query window and press the `F5` key.

```csharp
SELECT * FROM [dbo].[Customer];
```

You will see that a single `Customer` record has been inserted.

## Using the Transaction object in the Repository

All operations in *RepoDb* accepts the *DbTransaction* argument. This means that if you passed an instance of *DbTransaction* object in the specific operation, then the execution of that operation will be wrapped inside the *Transaction* object you had passed on. See example code below.

```csharp
using (var repository = new WhateverRepository())
{
	using (var transaction = repository.CreateConnection().BeginTransaction())
	{
		var entity = new Entity
		{
			...
		};
		repository.Insert<Entity>(entity, transaction);
	}
}
```

Notice in the code above, a *Transaction* object has been created via *repository.CreateConnection()* method. If the *Repository* is using the *ConnectionPersistency.Instance*, then the same instance of *DbConnection* is being reused. Otherwise, a new *DbConnection* object instance will be created.

Before we start, let us create the necessary objects needed.

### Create a Repository Class

- In the `Solution Explorer`, right-click the project `InventoryAPI` and click the `Add` > `New Solution Folder` and name it to `Repositories`.
- Right-click the `Repositories` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = `CustomerRepository`
- Click the `Add` button.
- The new file named `CustomerRepository.cs` will be created. Replace the class implementation with the script below.
```csharp
public class CustomerRepository : DbRepository<Customer, SqlConnection>
{
	public CustomerRepository()
		: base("Server=.;Database=Inventory;Integrated Security=SSPI", ConnectionPersistency.Instance)
	{ }
}
```
- Press `Ctrl+S` keys to save the changes.

The repository we had created above is very simple and is dedicated to only *Customer* entity in our database.

**Note**: We already have inheritted all the rich features of *RepoDb* as we have already inheritted the *BaseRepository* object. See all the operations [here](https://repodb.readthedocs.io/en/latest/pages/connection.html).

In your *Program.cs* file, add the following codes just right after the *DoTransaction()* method.

```csharp
public static void DoRepositoryTransaction()
{
	using (var repository = new CustomerRepository())
	{
		using (var transaction = repository.CreateConnection().BeginTransaction())
		{
			var customer = new
			{
				Name = "John Doe",
				Address = "New York",
				DateInsertedUtc = DateTime.UtcNow,
				DateModifiedUtc = DateTime.UtcNow,
				ModifiedBy = "ClientApplication"
			};
			repository.Insert(customer, transaction);
			Console.WriteLine("A record has been inserted via Transaction.");
		}
	}
}
```

Replace the implementation of your `Main()` method with the codes provided below.

```csharp
public static void Main(string[] args)
{
	DoRepositoryTransaction();
	Console.ReadLine();
}
```

Build the solution by pressing the `Alt + R + B` and then press `F5` to run.

A message `A record has been inserted via Transaction.` is shown in the `Console`. This indicates that the record has been saved to the database.

Again, let us check the database.

In your Microsoft SQL Server Management Studio, write the script below in the query window and press the `F5` key.

```csharp
SELECT * FROM [dbo].[Customer];
```

You will see that a single `Customer` record has been inserted.

## Transaction for Multiple Operation Calls

In this section, we will give you the recommended way on how to call *multiple operations* while using the *Transaction* object.

Let us say you have the following *Data Entities* below:

- *Customer*
- *Product* 
- *Order*
- *OrderItem*

And you have created the corresponding *Repositories* below:

- *CustomerRepository*
- *ProductRepository*
- *OrderRepository*
- *OrderItemRepository*

Then, your goal is to ensure that whatever *Order* the customer has been made is *ACID*. Below are the scenario.

- A *Customer* has opened the *Buy Product* section and has checked multiple *Product* for checkout.
- An *Order* is placed once the *Customer* has clicked the *Checkout* button.
- If any of the pre-calculation has failed, then the *Customer* orders must be rolled-back, otherwise it will be sent to *Ordering* section.

The solution to this is to use the *Transaction* in 4 activities. Code below will wrapped different repositories into a *Single* transaction.

```csharp
var customerRepository = new CustomerRepository();
var productRepository = new ProductRepository();
var orderRepository = new OrderRepository();
var orderItemRepository = new OrderItemRepository();

using (var transaction = customerRepository.CreateConnection().BeginTransaction())
{
	try
	{
		// Get the customer (Ex: 10045)
		var customerId = (long)customerRepository.Query<Customer>(10045, hints: SqlServerTableHints.NoLock, transaction: transaction);

		// Get the products (Ex: 122, 281)
		var products = productRepository.Query<Product>(e => e.Id = 122 || e.Id == 281);

		// Pre-calculation of the order
		var total = products.Sum(e => e.Price);

		// Add the new order
		var order = new Order
		{
			CustomerId = customerId,
			DateOrderdUtc = DateTime.UtcNow,
			Total = total
		};

		// Add the order
		var orderId = orderRepository.Insert<Order, long>(order, transaction);

		// Add all the items
		var orderItems = products.Select(e => new OrderItem
		{
			OrderId = orderId,
			ProductId = e.Id,
			DateInsertedUtc = DateTime.UtcNow
		});

		// Place all items
		orderItemRepository.InsertAll<OrderItem>(orderItems, transaction);
		
		// Validate the order after (throw exceptions if failing)
		EnsureCustomerOrder(customer, order);

		// Commit the transaction
		transaction.Commit();
	}
	cath
	{
		// Rollback the transaction
		transaction.Rollback();
	}
}

customerRepository.Dispose();
productRepository.Dispose();
orderRepository.Dispose();
```

Notice that all operations are wrapped with *Transaction* object. In the case of failure in the *EnsureCustomerOrder()* calls, then all operations above will be rolled back.

--------

**Voila! You have completed this tutorial.**

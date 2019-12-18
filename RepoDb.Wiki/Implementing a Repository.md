## Pre-requisites

Before you proceed with this tutorial, we suggest that you first visit our [Getting Started](https://github.com/mikependon/RepoDb/wiki/Getting-Started) page if you have not read it yet.

## Introduction

In this page, you will learn the following.

- [Creating a database-level Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository#creating-a-database-level-repository).
- [Inheritting the DbRepository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository#inheritting-the-dbrepository).
- [Inheritting the BaseRepository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository#inheritting-the-baserepository).

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*. Please have at least *Visual Studio 2017* and *SQL Server 2016* installed in your machine.

**Note**: The *database*, *table*, *project* and *model* we will be using is the same as what we have created at the [Getting Started](https://github.com/mikependon/RepoDb/wiki/Getting-Started) page. 

## What is Repository?

*Repository* is a software design pattern and practice in which it is being implemented as an additional layer between your application and your database. It is being represented as a class object within the application.Through repository, you are managing how the data is being manipulated from/to the database.

In this class (the *Repository*), we usually add the basic database operations/methods (ie: *Insert*, *Delete* and *Update*). But in most cases, we place the advance and reporting operations/methods here (ie: *GetTotalOrdersByMonth* or *RecalculateCustomerOrdersByDateRange*).

Then, the codes in your application is using the *Repository* object instead of directly accessing the database. Those allow the developers to follow the correct *chain-of calls* and *reusability* when it comes to data-accessibility.

## Creating a database-level Repository

A database-level repository is a *Repository* that can be used for a database specific purposes.

To implement a common *Repository*, please follow the steps below.

- In the `Solution Explorer`, right-click the `Solution` and click the `Add` > `New Solution Folder` and name it to `Repositories`.
- Right-click the `Repositories` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = `InventoryRepository`
- Click the `Add` button.
- The new file named `InventoryRepository.cs` will be created. Replace the class implementation with the script below.
```
public class InventoryRepository
{
	public InventoryRepository()
	{
		ConnectionString = "Server=.;Database=Inventory;Integrated Security=SSPI";
	}

	public string ConnectionString { get; }
}
```
- Press `Ctrl+S` keys to save the changes.

In this *Repository*, every implemented operation is using a new instance of *DbConnection* object.

### Adding a CRUD operation

Let us add the basic *CRUD* operations on our repository. Let us start by adding the *Insert* method.

Inside the *InventoryRepository* class, add the code snippets below just below the *ConnectionString* property.

```
public long Insert<T>(T instance)
	where T : class
{
	using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
	{
		return connection.Insert<T, long>(instance);
	}
}
```

Then, add the following code snippets below to support the *Query* operation.

```
public Customer Query<T>(object id)
	where T : class
{
	using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
	{
		return connection.Query<T>(id).FirstOrDefault();
	}
}
```

And then, add the following code snippets below for *Update* operation.

```
public int Update<T>(T instance)
	where T : class
{
	using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
	{
		return connection.Update<T>(instance);
	}
}
```

Finally, add the following code snippets below to support the *Delete* operation.

```
public int Delete<T>(long id)
	where T : class
{
	using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
	{
		return connection.Delete<T>(id);
	}
}
```

By this time, our *InventoryRepository* is now having the basic *CRUD* operations which could allow the callers to do the basic operations for the *Customer* entity.

### Refactoring the Program.cs

In your *Program.cs* file, there were methods that is already implemented during the [Getting Started](https://github.com/mikependon/RepoDb/wiki/Getting-Started) tutorial.

These methods were the following:
- [DoInsert](https://github.com/mikependon/RepoDb/wiki/Getting-Started#inserting-a-record)
- [DoQuery](https://github.com/mikependon/RepoDb/wiki/Getting-Started#querying-a-record)
- [DoUpdate](https://github.com/mikependon/RepoDb/wiki/Getting-Started#updating-a-record)
- [DoDelete](https://github.com/mikependon/RepoDb/wiki/Getting-Started#deleting-a-record)

We will refactor these methods to use our newly created *InventoryRepository* class.

But before doing that, let us implement a variable that will hold an instance of *InventoryRepository* class inside the *Program.cs* file. Please copy the code snippet below and paste it in your *Program.cs* file just right after the class declaration.

```
private static readonly InventoryRepository inventoryRepository = new InventoryRepository();
```

For the *DoInsert* method, replace the implementation with the code snippets below.

```
public void DoInsert()
{
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York",
		DateInsertedUtc = DateTime.UtcNow,
		DateModifiedUtc = DateTime.UtcNow,
		ModifiedBy = "Me"
	};
	inventoryRepository.Insert<Customer>(customer);
	Console.WriteLine("A customer record has been inserted.");
}
```

For the *DoQuery* method, replace the implementation with the code snippets below.

```
public void DoQuery()
{
	var customer = inventoryRepository.Query<Customer>(1);
	Console.WriteLine($"{customer.Name} ({customer.Address})");
}
```

For the *DoUpdate* method, replace the implementation with the code snippets below.

```
public void DoUpdate()
{
	var customer = inventoryRepository.Query<Customer>(1);
	customer.Name = "James Doe";
	customer.DateModifiedUtc = DateTime.UtcNow;
	inventoryRepository.Update<Customer>(customer);
	Console.WriteLine("A customer record has been updated.");
}
```

And for the *DoDelete* method, replace the implementation with the code snippets below.

```
public void DoDelete()
{
	inventoryRepository.Delete<Customer>(1);
	Console.WriteLine("A customer record has been deleted.");
}
```

**Note**: I put the value of *1* as the value of *[Id]* column of the *Customer* entity. You can change this value based on your local database record identity value.

We are almost settled on reusing our *InventoryRepository* class within our *Program.cs* file. To test the functionalities, please copy the code snippets below and replace the *Main()* method of your *Program.cs* file.

```
public static void Main(string[] args)
{
	DoInsert();
	DoQuery();
	DoUpdate();
	DoDelete();
	Console.ReadLine();
}
```

Press the `F5` key to stat the project.

**Note**: We suggest that you put the breakpoint at the *Main()* method and do press `F11` key on every step so you would see the actual code execution.

You will see that the following messages appeared at the *Console*.

- *A customer record has been inserted.*
- *John Doe (New York)*
- *A customer record has been updated.*
- *A customer record has been deleted.*

This signifies that all operations were succeeded.

## Inheritting the DbRepository

The *DbRepository* is an embedded class within *RepoDb* library that would allow the developers to inherit the implementation of the *Common-Repository* pattern. Practically, the way how we implemented the *InventoryRepository* is somewhat similar to *DbRepository*, however, it does not contain all the rich methods we have in the library.

In this section, we will refactor the *InventoryRepository* to utilize the *DbRepository* class.

Please follow the steps below to achieve this.

- Modify the *InventoryRepository* class declaration with the code snippets below.

```
public class InventoryRepository : DbRepository<SqlConnection>
{
	public InventoryRepository()
		: base("Server=.;Database=Inventory;Integrated Security=SSPI")
	{ }
}
```

- Remove the *ConnectionString* property in the *InventoryRepository* class as this property is already present in the *DbRepository* class.
- Remove all the implemented methods named *Insert*, *Update*, *Delete* and *Query*.

**Voila~** We are done refactoring!

We do not need to change anything in the *Program.cs* file. The same code-lines will still run as same as before.

**Note**: The *DbRepository* class is capable of managing the *ConnectionPersistency* which could enable the application to only use *Single* connection all throughout the lifetime of the repository. Furthermore, by inheritting this class, the derived class has all inheritted the rich operations of *RepoDb* library.

Press the `F5` key to run and test the solution.

We suggest that you place a breakpoint at the *Main()* method and press the `F11` key (same in the recent section) to see and follow the actual code executions.

## Inheritting the BaseRepository

The *BaseRepository* is an embedded class within *RepoDb* library that would allow the developers to implement a custom repository that is targetting a specific *DataEntity* object (or *Table* in the database).

The implementation of *BaseRepository* requires a *Model* and a *DbConnection* object. This must be contracted during the inherittance.

Since the *BaseRepository* is an entity-level repository, then we need to create a new *Repository* class that is entity-dedicated for our model *Customer*.

To implement an entity-dedicated *Repository*, please follow the steps below.

- Right-click the `Repositories` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = `CustomerRepository`
- Click the `Add` button.
- The new file named `CustomerRepository.cs` will be created. Replace the class implementation with the script below.
```
public class CustomerRepository : BaseRepository<Customer, SqlConnection>
{
	public CustomerRepository()
		: base("Server=.;Database=Inventory;Integrated Security=SSPI")
	{ }
}
```
- Press `Ctrl+S` keys to save the changes.

At to this point, the *Program.cs* is using the *InventoryRepository* class. We can optimize the *Program.cs* to utilize this newly created repository named *CustomerRepository*.

Let us target modifying the implementations of the following methods:
- [DoInsert](https://github.com/mikependon/RepoDb/wiki/Getting-Started#inserting-a-record)
- [DoQuery](https://github.com/mikependon/RepoDb/wiki/Getting-Started#querying-a-record)
- [DoUpdate](https://github.com/mikependon/RepoDb/wiki/Getting-Started#updating-a-record)
- [DoDelete](https://github.com/mikependon/RepoDb/wiki/Getting-Started#deleting-a-record)

But before doing that, let us implement a variable that will hold an instance of *CustomerRepository* class inside the *Program.cs* file. Please copy the code snippet below and paste it in your *Program.cs* file just right after the *inventoryRepository* declaration.

```
private static readonly CustomerRepository customerRepository = new CustomerRepository();
```

Then, let us modify the methods one-by-one.

For *DoInsert()* method, replace the implementation with the code snippets below.

```
public void DoInsert()
{
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York",
		DateInsertedUtc = DateTime.UtcNow,
		DateModifiedUtc = DateTime.UtcNow,
		ModifiedBy = "Me"
	};
	customerRepository.Insert(customer);
	Console.WriteLine("A customer record has been inserted.");
}
```

For *DoQuery()* method, replace the implementation with the code snippets below.

```
public void DoQuery()
{
	var customer = customerRepository.Query(1).FirstOrDefault();
	Console.WriteLine($"{customer.Name} ({customer.Address})");
}
```

For *DoUpdate()* method, replace the implementation with the code snippets below.

```
public void DoUpdate()
{
	var customer = customerRepository.Query(1).FirstOrDefault();
	customer.Name = "James Doe";
	customer.DateModifiedUtc = DateTime.UtcNow;
	customerRepository.Update(customer);
	Console.WriteLine("A customer record has been updated.");
}
```

For *DoDelete()* method, replace the implementation with the code snippets below.

```
public void DoDelete()
{
	customerRepository.Delete(1);
	Console.WriteLine("A customer record has been deleted.");
}
```

You will notice that we have removed all the *GenericType* passing when calling the operation. This is because the *CustomerRepository* is dedicated to only *Customer* entity. All calls are directed to the *[dbo].[Customer]* table.

Press the `F5` key to run and test the solution.

Same as previous section, we suggest that you place a breakpoint at the *Main()* method and press the `F11` key to see and follow the actual code executions.

**Voila! You have completed this tutorial.**

-------

You can now proceed in [Making the Repositories Dependency-Injectable](https://github.com/mikependon/RepoDb/wiki/Making-the-Repositories-Dependency-Injectable) tutorial.
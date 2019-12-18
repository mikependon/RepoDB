## Introduction

We are glad that you would like to learn RepoDb. In this page, you will learn the following.

- [Installation of *RepoDb*](https://github.com/mikependon/RepoDb/wiki/Getting-Started#installation).
- [Making a *CRUD* calls](https://github.com/mikependon/RepoDb/wiki/Getting-Started#making-a-crud-calls).
- [Executing a *Raw-SQL*](https://github.com/mikependon/RepoDb/wiki/Getting-Started#executing-a-raw-sql).
- [Calling a *Stored Procedure*](https://github.com/mikependon/RepoDb/wiki/Getting-Started#calling-a-stored-procedure).
- [*Table-Based* calls](https://github.com/mikependon/RepoDb/wiki/Getting-Started#table-based-calls).

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*. Please have at least *Visual Studio 2017* and *SQL Server 2016* installed in your machine.

Please follow the steps below before proceeding to the next section.

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
```
**Note**: The SQL script above creates a table named `[Customer]` under the schema of `[dbo]`. The field `[Id]` is being set as the **primary** field and is also an **identity**.
- Press the `F5` key.
- In the `Object Explorer`, the table named *Customer* is now available under `Databases` > `Inventory` > `Tables`.

### Create a C# Project

- Open the Microsoft Visual Studio.
- Click `File` > `New` > `Project...`.
- Enter the following values:
  - Project = `Console App (.NET Core)`
  - Name = `InventoryProject`
  - Location = `<Do not change the default>`
  - Create directory for solution = `checked`
- Click the `OK` button.

### Create a C# class object

- In the `Solution Explorer`, right-click the project `InventoryProject` and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = *Customer*
- Click the `Add` button.
- The new file named `Customer.cs` will be created. Replace the class implementation with the script below.
```
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

## Installation

To install *RepoDb*, write the script below in the `Package Manager Console` and press the `Enter` key.

```
Install-Package RepoDb
```

The installation will only take few seconds to complete.

## Making a CRUD calls

To make a *CRUD* calls via *RepoDb*, kindly follow the steps below.

### Inserting a record

To insert a record, please use the [Insert](https://repodb.readthedocs.io/en/latest/pages/connection.html#insert) operation. Please copy the provided sample script below and paste it in your *Program.cs*, just right after the `Main()` method.

```
public void DoInsert()
{
	using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
	{
		var customer = new Customer
		{
			Name = "John Doe",
			Address = "New York",
			DateInsertedUtc = DateTime.UtcNow,
			DateModifiedUtc = DateTime.UtcNow,
			ModifiedBy = "Me"
		};
		connection.Insert(customer);
		Console.WriteLine("A customer record has been inserted.");
	}
}
```

Inside the `Main()` method, call the `DoInsert()` method we have created above. Simply replace the `Main()` method of your *Program.cs* file with the codes below.

```
public static void Main(string[] args)
{
	DoInsert();
	Console.ReadLine();
}
```

Then press the `F5` key.

You will see a message *A customer record has been inserted*. Press any key to exit!

In your *Microsoft SQL Server Management Studio*, write the script below in the query window and press the `F5` key.

```
SELECT * FROM [dbo].[Customer];
```

You will see that a single *Customer* record has been inserted. The field values are corresponding the *Customer* class-properties values you specified at the `DoInsert()` method.

### Querying a record

To query a record, please use the [Query](https://repodb.readthedocs.io/en/latest/pages/connection.html#query) operation. Please copy the provided sample script below and paste it in your *Program.cs*, just right after the `DoInsert()` method.

```
public void DoQuery()
{
	using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
	{
		var customer = connection.QueryAll<Customer>().First();
		Console.WriteLine($"{customer.Name} ({customer.Address})");
	}
}
```

Inside the `Main()` method, call the `DoQuery()` method we have created above. Simply replace the `Main()` method of your *Program.cs* file with the codes below.

```
public static void Main(string[] args)
{
	DoQuery();
	Console.ReadLine();
}
```

Then press the `F5` key.

You will see a message *John Doe (New York)*. The message you seen here is the actual data we queried from the database.

You can as well query a record from the database by passing an `Expression`. See sample code below.

```
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var customer = connection.Query<Customer>(e => e.Name = "John Doe").First();
	Console.WriteLine($"{customer.Name} ({customer.Address})");
}
```

The output of the script above is the same as prior sample.

### Updating a record

To update a record, please use the [Update](https://repodb.readthedocs.io/en/latest/pages/connection.html#update) operation. Please copy the provided scripts below and paste it in your *Program.cs*, just right after the `DoQuery()` method.

```
public void DoUpdate()
{
	using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
	{
		var customer = connection.Query<Customer>(e => e.Name = "John Doe").First();
		customer.Name = "James Doe";
		customer.DateModifiedUtc = DateTime.UtcNow;
		connection.Update(customer);
		Console.WriteLine("A customer record has been updated.");
	}
}
```

Inside the `Main()` method, call the `DoUpdate()` method we have created above. Simply replace the `Main()` method of your *Program.cs* file with the codes below.

```
public static void Main(string[] args)
{
	DoUpdate();
	Console.ReadLine();
}
```

Then press the `F5` key.

A message *A customer record has been updated.* will appear in the console. This indicates that the existing record from the database has been updated with the new values we provided inside the *DoUpdate()* method.

In your Microsoft SQL Server Management Studio, write the script below in the query window and press the `F5` key.

```
SELECT * FROM [dbo].[Customer];
```

You will see that the existing *Customer* named `John Doe` has been renamed to `James Doe`.


### Deleting a record

To delete a record, please use the [Delete](https://repodb.readthedocs.io/en/latest/pages/connection.html#delete) operation. Please copy the provided scripts below and paste it in your *Program.cs*, just right after the `DoUpdate()` method.

```
public void DoDelete()
{
	using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
	{
		var customer = connection.QueryAll<Customer>().First();
		connection.Delete(customer);
		Console.WriteLine("A customer record has been deleted.");
	}
}
```

Inside the `Main()` method, call the `DoDelete()` method we have created above. Simply replace the `Main()` method of your *Program.cs* file with the codes below.

```
public static void Main(string[] args)
{
	DoDelete();
	Console.ReadLine();
}
```

Then press the `F5` key.

A message *A customer record has been deleted.* will appear in the console. This indicates that the existing record from the database has been deleted.

In your Microsoft SQL Server Management Studio, write the script below in the query window and press the `F5` key.

```
SELECT * FROM [dbo].[Customer];
```

You will notice that there will be no records shown.

## Executing a Raw-SQL

To execute a raw-SQL, you can use any of the operations below. Each operation has its own purpose, mainly inheritted from ADO.Net.

- [ExecuteQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequery)
- [ExecuteNonQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executenonquery)
- [ExecuteReader](https://repodb.readthedocs.io/en/latest/pages/connection.html#executereader)
- [ExecuteScalar](https://repodb.readthedocs.io/en/latest/pages/connection.html#executescalar)
- [ExecuteQueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple)

In this tutorial, we will only limit on `ExecuteQuery()` operation.

Assuming you have multiple records of *Customer* in the database. Please copy the provided scripts below and paste it in your *Program.cs*, just right after the `DoDelete()` method.

```
public void DoExecuteRawSql()
{
	using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
	{
		var customers = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer];");
		customers
			.AsList()
			.ForEach(e => Console.WriteLine($"{e.Name} ({e.Address})"));
	}
}
```

The query above will return all the *Customer* records from the database.

Inside the `Main()` method, call the `DoExecuteRawSql()` method we have created above. Simply replace the `Main()` method of your *Program.cs* file with the codes below.

```
public static void Main(string[] args)
{
	DoExecuteRawSql();
	Console.ReadLine();
}
```

Press the `F5` key.

Notice that the *Console* is filled with the list of *Customer* records from the database.

#### Passing of parameters

You can pass any value via *dynamic*, *IDictionary<string, object>*, *ExpandoObject* or object-based (ie: *QueryField* and *QueryGroup*) parameters.

```
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var customers = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE [Id] = @Id;", new { Id = 10045});
}
```

You can as well pass an array of values as your parameter. See below.

```
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var keys = new [] { 10045, 11910, 14500, ... };
	var customers = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE [Id] IN (@Keys);", new { Keys = keys });
}
```

See the [raw-SQL](https://repodb.readthedocs.io/en/latest/pages/rawsql.html) documentation to read more about passing the parameters.

## Calling a Stored Procedure

The logic of calling a stored procedure is somewhat similar on the codes sample mentioned at the [*Executing a Raw-SQL*](https://github.com/mikependon/RepoDb/wiki/Getting-Started#executing-a-raw-sql) section. The only difference is, you have to pass the name of the stored procedure and set the *commandType* argument to *CommandType.StoredProcedure*.

In your *Microsoft SQL Server Management Studio*, open a new query window and execute the scripts provided below.

```
DROP PROCEDURE IF EXISTS [dbo].[sp_get_customers];
GO

CREATE PROCEDURE [dbo].[sp_get_customers]
AS
BEGIN
	SELECT * FROM [dbo].[Customer] WITH (NOLOCK);
END
```

Once executed, the stored procedure named `sp_get_customers` will now be available under `Databases` > `Inventory` > `Programmability` > `Stored Procedures`.

Please copy the provided scripts below and paste it in your *Program.cs*, just right after the `DoExecuteRawSql()` method.

```
public void DoCallStoredProcedure()
{
	using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
	{
		var customers = connection.ExecuteQuery<Customer>("[dbo].[sp_get_customers]", commandType: CommandType.StoredProcedure);
		customers
			.AsList()
			.ForEach(e => Console.WriteLine($"{e.Name} ({e.Address})"));
	}
}
```

Inside the `Main()` method, call the `DoCallStoredProcedure()` method we have created above. Simply replace the `Main()` method of your *Program.cs* file with the codes below.

```
public static void Main(string[] args)
{
	DoCallStoredProcedure();
	Console.ReadLine();
}
```

Press the `F5` key.

Notice that the *Console* is filled with the list of *Customer* records from the database.

#### Passing a parameter in the Stored Procedure

Assuming that the mentioned stored procedure above is expecting a parameter named *CustomerId* of type *INT*. To pass a parameter in the stored procedure, you can use the script below.

```
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var customers = connection.ExecuteQuery<Customer>("[dbo].[sp_get_customers]", new { CustomerId = 10045 }, commandType: CommandType.StoredProcedure);
}
```

See the [raw-SQL](https://repodb.readthedocs.io/en/latest/pages/rawsql.html) documentation to read more about passing the parameters.

## Table-Based calls

Most operations of RepoDb has it own equivalent table-based calls. This allow the developers to make a dynamic calls directly in the database.

Below is a code the query all *Customer* data from the database.

```
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var customers = connection.Query("Customer");
}
```

Notice that there is no *model* being passed. The return type is *dynamic* (of *ExpandoObject*).

You can also filter the data by passing a dynamic parameter like below.

```
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var customers = connection.Query("Customer", new { Id = 10045 });
}
```

The code above will return the *Customer* record where the value of *Id* field is equals to *10045*.

In the event of inserting a record, you can actually make a targetted column insertion like below.

```
using (var connection = new SqlConnection("Server=.;Database=Inventory;Integrated Security=SSPI").EnsureOpen())
{
	var id = connection.Insert("Customer", new { Name = "James Doe" });
}
```

The same logic also applied if you are *merging* or *updating* a record.

**Note**: As generalized in C# language, by using *dynamics* is proven much slower than *explicit-types*.

To read more about the operations, kindly visit our library documentation at [ReadTheDocs](https://repodb.readthedocs.io/en/latest/pages/connection.html).

**Voila! You have completed the basics of RepoDb.**

-------

You can now proceed in [Implementing a Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository) tutorial.
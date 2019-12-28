## Introduction

In this page, you will learn the following.

- [Creating a custom trace object](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Trace#creating-a-custom-trace-object)
- [Understanding the trace members](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Trace#understanding-the-trace-members)
- [Injecting the customized trace object](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Trace#injecting-the-customized-trace-object)

## Before we begin

The programming language we will be using is *C#*. Please have at least *Visual Studio 2017* installed in your machine.

We suggest that you first visit our [Implementing a Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository) page if you have not read it yet.

This is an *advance* topic, we expect that you already had experienced using the *RepoDb* library.

## What is Trace?

A *Trace* is an object that is usually used to extract more information on the actual execution, those gives more detailed information when debugging the software.

In *RepoDb*, it is an object that does micro tracing-activity when you call any of the operations; provides more insights on the actual execution context.

If you as a developer would like to see the information during *debugging* or would like to override the *behaviour* and *values* of the execution, then consider using the *Trace* object.

## Creating a custom trace object

To create a customized-trace object, you must implement the *ITrace* interface. This is the contracted interface used by the library in all repositories and extended operations to mark your class as *Trace* object.

- In the `Solution Explorer`, right-click the `Solution` and click the `Add` > `New Solution Folder` and name it to `Traces`.
- Right-click the `Traces` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = `InventoryTrace`
- Click the `Add` button.
- The new file named `InventoryTrace.cs` will be created. Replace the class implementation with the script below.
- Press `Ctrl+S` keys to save the changes.

After implementing the *ITrace* interface, you have to implement the required methods for you to be able to do a tracing (or *debugging*) in the actual executions. Please see the code snippets below as your reference implementation.

```csharp
public class InventoryTrace : ITrace
{
	public void BeforeQuery(CancellableTraceLog log)
	{
		throw new NotImplementedExcetion();
	}
	
	
	public void AfterQuery(TraceLog log)
	{
		throw new NotImplementedExcetion();
	}
	
	...
}
```

**Note**: To auto-create all the necessary methods of the *ITrace* interface, simply click the *ITrace* interface in the code file and press the `Ctrl + Space` keys and the `Enter`.

## Understanding the trace members

The *Trace* methods are those methods that is being triggered by the library *Before* and *After* the actual operation executions. In each event, the object *TraceLog* is being passed containing all the information of the said execution context.

Let us say, you are targeting to make a tracing in the *Query* operation to see what is the actual *SQL* statements that were used during the fire-up of the execution. Then, in your *Trace* implementation, the following methods are available.

**BeforeQuery** - is executed *before* the actual execution.

```csharp
public void BeforeQuery(CancellableTraceLog log)
{
	...
}
```

**AfterQuery** - is executed *after* the actual execution.

```csharp
public void BeforeQuery(TraceLog log)
{
	...
}
```

You can set the debugger's *breakpoint* in any of these two methods to see the actual parameters in used during the execution.

**Note**: All *IDbConnection* extended operations within the library has its own corresponding *Before* and *After* operation.

### TraceLog

In *RepoDb*, there are 2 *trace-log* objects.

- *TraceLog* - a tracing log object used in the tracing operations.
- *CancellableTraceLog* - a cancellable tracing log object used in the tracing operations.

### Methods

A method `Cancel()` at *CancellableTraceLog* class is provided to allow the developers to cancel the execution on-the-fly. Below is the signature.

```csharp
public void Cancel(bool throwException)
```

By setting the *throwException* argument to *True* would throw an exception during the call to this method. Otherwise, the operation will be silently cancelled.

### Properties

On the other hand, the *Trace* properties are those objects that hold the information of the actual execution.

Below are the properties definitions:

- *Result* - gets the actual result of the actual operation execution.
- *Parameter* - gets or sets the parameter object used on the actual operation execution.
- *Statement* - gets or sets the SQL Statement used on the actual operation execution.
- *ExecutionTime* - gets the actual length of the operation execution.
- *IsCancelled* - gets a value whether the operation is cancelled.
- *IsThrowException* - gets a value whether an exception will be thrown after the `Cancel()` method was called.

## Injecting the customized trace object

In this section, you learn how to use your customized *Trace* object when calling the *IDbConnection* operations or when instantiating a *Repository* object.

At to this point, we would expect that you already have created the *InventoryTrace* custom trace class.

### Calling the IDbConnection operations

All "fluent" extended [operations](https://github.com/mikependon/RepoDb#operations) of the *IDbConnection* is accepting an *ITrace* argument.

For example, in the case of *Insert* operation, below is the way on how to inject the *ITrace* object.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var trace = new InventoryTrace();
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York"
	};
	connection.Insert<Customer>(customer, trace: trace);
}
```

If the kind of code snippets above is prevailing on your solution, every time the *Insert* operation is being called, then the *BeforeInsert* and *AfterInsert* of the *InventoryTrace* class object will be triggered.

> To *debug*, simply open the *InventoryTrace* class and add a breakpoint on the *BeforeInsert* and *AfterInsert* method. This breakpoint will be hit by the debugger when the *Insert* method is called. You can do the same thing with other methods.

During debugging, at *BeforeInsert* method you will see the following values on the important properties of the *CancellableTraceLog*.

#### Statement Property

Below would be the value.

```
INSERT INTO [Customer] ([Name], [Address]) VALUES (@Name, @Address);
```

The statement provided on this property can be changed before even the *actual* execution.

#### Value Property

Below would be the content.

- Name = *John Doe*
- Address = *New York*

The values above can also be changed before the *actual* execution.

#### Cancel Method

On the other hand, this method can be called if you wish to cancel the execution. By setting the *throwException* argument to *True*, then an exception will be thrown.

This method is very useful in *auditing*. See sample below.

```csharp
public void BeforeInsert(CancellableTraceLog log)
{
	if (log.Statement.Contains("Customer") && log.Statement.Contains("Id"))
	{
		// throw new InvalidOperationException("Cannot insert a value for the [Id] property.");
		log.Cancel(true);
	}
	...
}
```

### Injecting to a Repository

When you inherited the *BaseRepository* or instantiated the *DbRepository* object, the *ITrace* object can be injected right away. See sample code snippets below.

```csharp
using (var repository = new DbRepository(ConnectionString, new InventoryTrace()))
{
	var customer = new Customer
	{
		Name = "John Doe",
		Address = "New York"
	};
	repository.Insert<Customer>(customer);
}
```

Or, when inheriting the *BaseRepository*.

```csharp
public class CustomerRepository : BaseRepository<Customer, SqlConnection>
{
	public CustomerRepository()
		: base(ConnectionString, new InventoryTrace())
	{ }
}

using (var repository = new CustomerRepository())
{
	repository.Insert(new Customer
	{
		Name = "John Doe",
		Address = "New York"
	});
}
```

Once the *InventoryTrace* object has been injected the way it is being injected above in the *Repository* object, then the instance of that *InventoryTrace* object will be used by that *Repository* every time there is a call in any operation of that *Repository*.

-------

**Voila! You have completed this tutorial.**

-------

## Related Topics

- [Customizing a Cache](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache)
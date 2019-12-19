## Pre-requisites

Before you proceed with this tutorial we suggest that you first visit our [Implementing a Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository) page if you have not read it yet.

## Introduction

In this page, you will learn the following.

- [Implement a Repository with its own Interface Definition](https://github.com/mikependon/RepoDb/wiki/Making-the-Repositories-Dependency-Injectable#implement-a-repository-with-its-own-interface-definition).
- [Injecting a Repository in .NET Core REST API Project](https://github.com/mikependon/RepoDb/wiki/Making-the-Repositories-Dependency-Injectable#injecting-a-repository-in-net-core-rest-api-project).
- [Calling the Repository operations from the Controller](https://github.com/mikependon/RepoDb/wiki/Making-the-Repositories-Dependency-Injectable#calling-the-repository-operations-from-the-controller).

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*. Please have at least *Visual Studio 2017* and *SQL Server 2016* installed in your machine.

Also, we will only targetting creating a simple *.NET Core REST API* and have the *Repository* object get injected and being called at the *Controllers*.

Please follow the steps below before proceeding to the next section.

### Setup the database and table

- Create a database by following the [create a table](https://github.com/mikependon/RepoDb/wiki/Getting-Started#create-a-database) section of [Getting Started](https://github.com/mikependon/RepoDb/wiki/Getting-Started) page.
- Create a table by following the [create a table](https://github.com/mikependon/RepoDb/wiki/Getting-Started#create-a-table) section of [Getting Started](https://github.com/mikependon/RepoDb/wiki/Getting-Started) page.

### Create a C# .NET Core REST API Project

- Open the Microsoft Visual Studio.
- Click `File` > `New` > `Project...`.
- Enter the following values:
  - Project = `ASP.NET Core Web Application`
  - Name = `InventoryAPI`
  - Location = `<Do not change the default>`
  - Create directory for solution = `checked`
- Click the `OK` button.
- In the next page, select the following:
  - Type = `API`
  - Enable Docker Support = `unchecked`
  - Configure for HTTPS = `checked`
  - Authentication = `No Authentication`
- Click the `OK` button.

### Install the RepoDb Library

Copy and paste the script below in the `Package Manager Console` and press the `Enter` key.

```
Install-Package RepoDb
```

The installation will only take few seconds to complete.
  
### Create a Model Class

- In the `Solution Explorer`, right-click the project `InventoryAPI` and click the `Add` > `New Solution Folder` and name it to `Models`.
- Right-click the `Models` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = `Customer`
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

### Create a Repository Class

- In the `Solution Explorer`, right-click the project `InventoryAPI` and click the `Add` > `New Solution Folder` and name it to `Repositories`.
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

### Create a Repository Interface

- In the `Solution Explorer`, right-click the project `InventoryAPI` and click the `Add` > `New Solution Folder` and name it to `Interfaces`.
- Right-click the `Interfaces` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Interface`
  - Name = `ICustomerRepository`
- Click the `Add` button.
- The new file named `ICustomerRepository.cs` will be created. Replace the interface implementation with the script below.
```
public interface ICustomerRepository
{
	long Add(Customer customer);
	Customer Get(long id);
	int Modify(Customer customer);
	int Remove(long id);
}
```
- Press `Ctrl+S` keys to save the changes.

## Implement a Repository with its own Interface Definition

In this section, we will implement the *ICustomerRepository* we created above into the *CustomerRepository* class. To do this, simply change the *CustomerRepository* class declaration as follows.

```
public class CustomerRepository : BaseRepository<Customer, SqlConnection>, ICustomerRepository
{
	public CustomerRepository()
		: base("Server=.;Database=Inventory;Integrated Security=SSPI")
	{ }
}
```

The 4 methods we have created at *ICustomerRepository* interface must be implemented.

For `Add()` method, copy the code snippets below and paste it inside the *CustomerRepository* class just right after the constructor.

```
public long Add(Customer customer)
{
	return Insert<long>(customer);
}
```

For `Get()` method, copy the code snippets below and paste it inside the *CustomerRepository* class just right after the `Add()` method.

```
public Customer Get(long id)
{
	return Query(id).FirstOrDefault();
}
```

For `Modify()` method, copy the code snippets below and paste it inside the *CustomerRepository* class just right after the `Get()` method.

```
public int Modify(Customer customer)
{
	return Update(customer);
}
```

For `Remove()` method, copy the code snippets below and paste it inside the *CustomerRepository* class just right after the `Modify()` method.

```
public int Remove(long id)
{
	return Delete(id);
}
```

By this time, our *CustomerRepository* class has now implemented all necessary contracts of *ICustomerRepository* interface.

Though in .NET Core, we can inject the actual class itself, but we preferred to use an interface instead. We have created the *ICustomerRepository* interface as we are going to use it as the actual injectable object, not the class.

## Injecting a Repository in .NET Core REST API Project

In this section, we will register the *CustomerRepository* as part of the injectable component of our *C# REST API*. To do this, please follow the steps below.

- In your `Solution Explorer`, double-click the `Startup.cs` file.
- Navigate inside `ConfigureServices` method and paste the code below before the method end.
```
services.AddTransient<CustomerRepository, ICustomerRepository>();
```
- Resolve the missing namespaces by placing the mouse inside the *CustomerRepository* and press `Ctrl+Space` > `Enter`.
- Press `Ctrl+S` keys to save the changes.

The engine will register the `CustomerRepository` object (as implemented by `ICustomerRepository` interface) into the services collection. Once the registration is complete, it signifies that our *CustomerRepository* class is now ready to be used for injection.

**Note**: We can as well add it as `Singleton` via `AddSingleton` method of `IServiceCollection` if we wish to have our `Repository` in a singleton mood.

## Calling the Repository operations from the Controller

In this section, the ultimate goal is to call the *CustomerRepository* operation inside a *Controller* method.

To attain this, we need to do the following steps below.

### Create a Controller

- Right-click the `Controllers` folder and click the `Add` > `New Item...` context-menu.
- Enter the following values:
  - Type = `Class`
  - Name = `CustomerController`
- Click the `Add` button.
- The new file named `CustomerController.cs` will be created. Replace the class implementation with the script below.
```
[Route("api/[controller]")]
[ApiController]
public class CustomerController
{
}
```
- Press `Ctrl+S` keys to save the changes.

### Injecting the Repository

- Double-click the `CustomerController.cs` file.
- Add a variable that will hold the instance of `CustomerRepository`.
```
private CustomerRepository customerRepository;
```
- Add a constructor that accepts the *ICustomerRepository* interface.
```
public CustomerController(ICustomerRepository repository)
{
	customerRepository = repository;
}
```
- Press `Ctrl+S` keys to save the changes.

### Create a Get Method

The *Get()* method is the one who will call the *CustomerRepository* operation. Please copy the provided code snippets below and paste it inside *CustomerController* class just right after the *Constructor*.

```
[HttpGet("{id}")]
public ActionResult<Customer> Get(long id)
{
    return customerRepository.Get(id);
}
```

You will notice that we have called the `Get(long id)` operation of the *CustomerRepository* class inside the `Get()` method we have created.

**Note**: You will also notice that when you to *dot-notation* in the *customerRepository* variable, you get so many methods/operations. This is because the instance of *CustomerRepository* has inherritted all the rich features of *RepoDb* via *BaseRepository<TEntity, TDbConnection>* base class.

### Testing the Controller

At this point, our solution is now ready for testing. Build the solution by simply pressing the `Alt + B + R` keys.

Once the build is complete, press the `F5` key to start.

In the browser, type the URL `http://localhost:<port>/api/customer/10045`. Where the value *10045* is equals to the *Customer* id you have in the database.

You will notice that the result is being displayed in the browser as *JSON* file.

#### How to find a port?

- Right-click on the project `InventoryAPI` and click the `Properties`.
- Click the `Debug` tab.
- Under `Web Server Settings`, you will see the `App URL` field that contains the port.

--------

**Voila! You have completed this tutorial.**

--------

**Other topics you might be interested:**
- [Bulk-Operations vs Batch-Operations](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations)
- [Multiple Resultsets via QueryMultiple and ExecuteQueryMultiple](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple)
- [Working with Transactions](https://github.com/mikependon/RepoDb/wiki/Working-with-Transactions)
- [Expression Trees](https://github.com/mikependon/RepoDb/wiki/Expression-Trees)
- [Advance Field and Type Mapping Implementations](https://github.com/mikependon/RepoDb/wiki/Advance-Field-and-Type-Mapping-Implementation)
- [Customizing a Cache](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache)
- [Implementing a Trace](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Trace)
- [The importance of Connection Persistency](https://github.com/mikependon/RepoDb/wiki/The-importance-of-Connection-Persistency)
- [Working with Enumerations](https://github.com/mikependon/RepoDb/wiki/Working-with-Enumerations)
- [Extending the supports for specific DB Provider](https://github.com/mikependon/RepoDb/wiki/Extending-the-supports-for-specific-DB-Provider)

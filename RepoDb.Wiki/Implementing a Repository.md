## Pre-requisites

Before you proceed with this tutorial we suggest that you first visit our [Getting Started](https://github.com/mikependon/RepoDb/wiki/Getting-Started) page if you have not read it yet.

## Introduction

In this page, you will learn the following.

- [Creating a generalized Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository#creating-a-generalized-repository).
- [Inheritting the DbRepository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository#inhertting-the-dbrepository).
- [Inheritting the BaseRepository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository#inhertting-the-baserepository).
- [Calling the Repository anywhere from the application](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository#calling-the-repository-anywhere-from-the-application).

In this tutorial, the programming language we will be using is C# and the database provider we will be using is SQL Server. Please have at least Visual Studio 2017 and SQL Server 2016 installed in your machine.

**Note**: The *database*, *table*, *project* and *model* we will be using is the same as what we have created at the [Getting Started](https://github.com/mikependon/RepoDb/wiki/Getting-Started) page. 

## What is Repository?

*Repository* is a software design pattern and practice in which it is being implemented as an additional layer between your application and your database. Through repository, you are managing how the data is being manipulated from/to the database.

In this class, we usually add the basic database operations/methods like *Insert*, *Delete* and *Update*. In some cases, we also place the advance and reporting operations/methods like *GetTotalOrdersByMonth* or *RecalculateCustomerOrdersByDateRange* here.

Then, the codes in your application is using the *Repository* object instead of directly accessing the database. Those allow the developers to follow the correct *chain-of calls* and *reusability* when it comes to data-accessibility.

## Creating a generalized Repository

To implement a generalized *Repository*, please follow the steps below.

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
	public InventoryRepository(string connectionString)
	{
		ConnectionString = connectionString;
	}

	public string ConnectionString { get; set; }
}
```

- Press `Ctrl+S` keys to save the changes.

## Inheritting the DbRepository

## Inheritting the BaseRepository

## Calling the Repository anywhere from the application

-------

**Voila! You have completed this tutorial.**

You can now proceed in [Making the Repositories Dependency-Injectable](https://github.com/mikependon/RepoDb/wiki/making-the-repositories-dependeny-injectable) tutorial.
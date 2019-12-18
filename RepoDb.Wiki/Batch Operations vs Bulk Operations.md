## Introduction

In this page, you will learn the following.

- [Batch operation concepts]()
- [Bulk operation concepts]()
- [Behind the scene of batch operations]()

We will give you more insights about the core differences of the *Batch Operations* and *Bulk Operations*. By having this insights, it will help guide you as developer of when to use the proper operations based on your own scenario.

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*. Please have at least *Visual Studio 2017* and *SQL Server 2016* installed in your machine.

## Batch operation concepts

The batch operation is the process of making the multiple single-operations be executed against the database in one-go. The execution is *ACID*; implicit transaction is provided if not present.

In *RepoDb* library, the implementation of the batch operation is *flexible* due to the fact that it allow the developers to weigh the number of operations to be batched during the execution. The batch operations are only targeting certain operations (ie: *QueryMultiple*, *ExecuteQueryMultiple*, *InsertAll*, *UpdateAll* and *MergeAll*). The latter 3 operations are the only *ACID* in nature.

Below are some practical explanation to give you more insights about the batch operation.

### Normal executions

Let us say you have a model named *Customer* that corresponds to the *[dbo].[Customer]* table.

By calling the *Insert* operation below.

```
var customer = new Customer
{
	Name = "John Doe",
	Address = "New York"
};
using (var connection = new SqlConnection(ConnectionString))
{
	connection.Insert<Customer>(customer);
}
```

will execute the following *SQL* script in the database.

```
INSERT INTO [Customer] (Name, Address) VALUES (Name, Address);
```

**But what if you would like to insert multiple records?**

Most developers do it this way.

```
var customers = new List<Customer>();
... codes that add 1000 customers
using (var connection = new SqlConnection(ConnectionString))
{
	customers.ForEach(c => connection.Insert<Customer>(c));
}
```

To be more practical, we sometimes wrap them in a *Transaction* to make the executions more *ACID*.

**Well, that is not the most optimal way!**

Why? It creates multiple single-statement and execute each single-statement one by one against the database (even though you wrapped it all to a single connection).

### Batch executions

In batch executions, the way the *SQL* statements is being executed is as follows.

Let us say you have implemented the code snippets below.

```
var customers = new List<Customer>();
... codes that add 1000 customers
using (var connection = new SqlConnection(ConnectionString))
{
	connection.InsertAll<Customer>(customers);
}
```

The library will create a *packed-statements* that is executable in *one-go*.

In the case above, the library will create the following *SQL* statements.

```
INSERT INTO [Customer] (Name, Address) VALUES (Name, Address);
INSERT INTO [Customer] (Name, Address) VALUES (Name1, Address1);
...
INSERT INTO [Customer] (Name, Address) VALUES (Name999, Address999);
INSERT INTO [Customer] (Name, Address) VALUES (Name1000, Address1000);
```

All parameters will be passed along to its proper *indexes*, depends on the number of *batch-size*. This is way more optimal as it is executing the multiple *SQL* statements in *one-go*.

**Note**: The *ADO.NET* maximum parameters is 2100 only. The batch operation will fail if you reach that number (that is intentionally). You can set the batch number by passing the value in the *batchSize* argument.

You will learn more in the [what happened behind the scene]() section.

## Bulk operation concepts

## Behind the scene of batch operations

- Cache the statements
- Cache the execution context
- Cache the properties
- Add an implicit transaction
- Add a preparation
- Add a batches
- Execute in one big chunk

Voila! You have completed this tutorial.
## Introduction

In this page, you will learn the following.

- [Batch operation concepts](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#Batch-operation-concepts)
- [Bulk operation concepts](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#Bulk-operation-concepts)
- [Behind the scene of batch operations](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#Behind-the-scene-of-batch-operations)

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

The following *SQL* script will be executed in the database.

```
INSERT INTO [Customer] (Name, Address) VALUES (Name, Address);
```

**But, what if you would like to insert multiple records?**

Most developers do it this way.

```
var customers = new List<Customer>();
... codes that add 1000 customers
using (var connection = new SqlConnection(ConnectionString))
{
	customers.ForEach(c => connection.Insert<Customer>(c));
}
```

To be more practical on my sample above, we often wrap the calls within the *Transaction* object to make the executions more *ACID*.

**Well, that is not the most optimal way!**

Why? Even though we wrapped all executions in a single connection, it still creates multiple *Insert* statement and each statement is being executed against the database one-by-one. The traffic of execution between the *Client* application and the *Database* server is high and it varies on the number of records we are trying insert.

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

You will learn more in the [what happened behind the scene](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#Behind the scene of batch operations) section.

## Bulk operation concepts

## Behind the scene of batch operations

When you call any of the *Push* batch operations (ie: *InsertAll*, *UpdateAll* or *MergeAll*), then the following activities are happening behind the scene.

**Understanding your schema** - the first touch to your database will be done to extract the schema definitions. This includes getting the information of the *PrimaryKey*, *Identity* and *Nullable-Columns* information. The information will be cached in memory by using your class (or *Model*) actual name as the key.

**Caching the properties** - the properties of your class (or *Model*) is being extracted and is cached in the memory. Allowing the library to reuse it in any future calls that is using the same object.

**Caching the SQL statement** - the *SQL* statements are being generated and cached automatically by the library. The generated *SQL* statement is a multiple packed-statements that varies on the number of *batch-size* you passed in the *batchSize* argument. Let us say, you passed 30, then the number of packed-statements are 30.

**Cache the execution context** - this the feature that helps *RepoDb* reused the existing execution context that has already been executed in the database. The execution context contains the *SQL Statements*, *Parameters*, *Preparations* and even the *Compiled IL or Expressions*. By having this, *RepoDb* does not need to extract same operation every time there is an identity calls, those leads to become more high-performant and efficient.

**Add an implicit transaction** - a new *Transaction* object is being assigned to the *Execution* if the caller does not passed any explicit transaction.

**Add a preparation** - before executing the *DbCommand* object, the `Prepare()` method is being called to pre-define the execution against the database. In the case of *SQL Server*, create an *Execution-Plan* in advance.

**Execute in one big chunk** - the generated *packed statements* is being executed at once against database. Though in reality, RepoDb is also batching due to the fact that *ADO.NET* is limited only to *2100 parameters*. Also, through this batches, the caller is able to define the *best* batch number based on the situation and scenario (ie: *Number of Columns*, *Network*, etc).

--------

**Voila! You have completed this tutorial.**
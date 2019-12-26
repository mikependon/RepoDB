## Introduction

In this page, you will learn the following.

- [Batch operation concepts](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#batch-operation-concepts)
- [Bulk operation concepts](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#bulk-operation-concepts)
- [Behind the scene of the batch operations](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#behind-the-scene-of-the-batch-operations)

We will give you more insights about the core differences of the *Batch Operations* and *Bulk Operations*. It will help guide you as a developer of when to use the proper operations based on your own scenario.

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*. Please have at least *Visual Studio 2017* and *SQL Server 2016* installed in your machine.

## Batch operation concepts

The batch operation is the process of making the *multiple* single-operations be executed against the database in *one-go*. The execution is *ACID*; an implicit transaction is provided if not present.

In *RepoDb* library, the implementation of the batch operation is *flexible*. It allows you (as the developer) to control the number of operations to be batched during the execution. That flexibility helps you manage the performance based on your own situations (ie: *Network Latency*, *Number of Columns*, etc).

The batch operations are only targeting certain operations (ie: *QueryMultiple*, *ExecuteQueryMultiple*, *InsertAll*, *UpdateAll* and *MergeAll*). The latter 3 operations are the only *ACID* in nature.

Below are some practical explanation to give you more insights about the batch operation.

### Normal executions

Let us say you have a model named *Customer* that corresponds to the *[dbo].[Customer]* table.

By calling the *Insert* operation below.

```csharp
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

Then the following *SQL* script will be executed in the database.

```csharp
INSERT INTO [Customer] (Name, Address) VALUES (@Name, @Address);
```

**But, what if you would like to insert multiple records?**

Most developers do it this way.

```csharp
var customers = new List<Customer>();
... codes that add 1000 customers
using (var connection = new SqlConnection(ConnectionString))
{
	customers.ForEach(c => connection.Insert<Customer>(c));
}
```

To be more practical on my sample above, we often wrap the calls within the *Transaction* object to make the executions more *ACID*.

**Well, that is not the most optimal way!**

Why? Even though we wrapped all the executions into a single connection object, it is still creating multiple *Insert* statement and each statement is being executed against the database *one-by-one*. The traffic of the executions between the *Client* application and the *Database* server is high as it varies on the number of records we are inserting.

### Batch executions

In the *batch executions*, the way the *SQL* statement is being executed is as follows.

Let us say you have implemented the code snippets below.

```csharp
var customers = new List<Customer>();
... codes that add 1000 customers
using (var connection = new SqlConnection(ConnectionString))
{
	connection.InsertAll<Customer>(customers, batchSize: 100);
}
```

**Note**: The default value of the *batchSize* is *30*. The value can be seen at [Constant.DefaultBatchOperationSize](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Core/RepoDb/Constant.cs).

The library will then create the *packed-statements* that is executable in *one-go*.

In the case above, the library will create the following *SQL* statements that is batched by *100*.

```csharp
INSERT INTO [Customer] (Name, Address) VALUES (@Name, @Address);
INSERT INTO [Customer] (Name, Address) VALUES (@Name1, @Address1);
...
INSERT INTO [Customer] (Name, Address) VALUES (@Name99, @Address99);
```

The packed-statements above *cached* and is being executed *10* times. Without the batch, the executions will be *1000* times.

All parameters will be passed into its proper *indexes*, depending on the number of batches. This is way more optimal as it is executing the multiple *SQL* statements in *one-go*.

**Note**: The *ADO.NET* maximum parameters is 2100. The batch operation will fail if you reach that number and that is an expected behavior. You can set the batch number by passing the value in the *batchSize* argument.

You will learn more in the [what happened behind the scene](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations#behind-the-scene-of-the-batch-operations) section.

## Bulk operation concepts

The *bulk operations* on the other hand is the process of loading the data from your *Client* application into the destination database. This process ignores some *constraints*, *data-types* and even the *audits* when loading the data, those giving you the maximum performance.

*RepoDb* by default is using the *SqlBulkCopy* class of *ADO.NET* for *bulk-operation*. This means that it has inheritted all the functionalities of that class.

**Note**: The library allow you to *BulkInsert* the array of the *data entity* objects in which this feature is very *unique* to this library.

### Bulk executions

Below are some sample codes for your reference on how to do *bulk insert* via *RepoDb*.

Let us say you would like to copy all the data from one database to another database.

In our sample code below, we both have the *[dbo].[Customer]* table exists in both databases.

```csharp
using (var sourceConnection = new SqlConnection(SourceConnectionString))
{
	using (var reader = sourceConnection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		using (var destinationConnection = new SqlConnection(DestinationConnectionString))
		{
			destinationConnection.BulkInsert<Customer>(reader);
		}
	}
}
```

**Note**: You can also do the process in different RDBMS DB Providers (ie: *SqLite* to *SqlServer* or *MySql* to *SqlServer*).

You can also do *bulk insert* if you have the array of the *data entity* objects. This feature is *unique* to *RepoDb*.

```csharp
using (var sourceConnection = new SqlConnection(SourceConnectionString))
{
	var customers = sourceConnection.Query<Customer>(e => e.State == State.Valid);
	using (var destinationConnection = new SqlConnection(DestinationConnectionString))
	{
		destinationConnection.BulkInsert<Customer>(customers);
	}
}
```

To read more about this concepts, you can refer to Microsoft [documentation](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/bulk-copy-operations-in-sql-server).

## Behind the scene of the batch operations

When you call any of the *Push* batch operations (ie: *InsertAll*, *UpdateAll* or *MergeAll*), then the following activities are happening behind the scene.

#### Understanding your schema

The first touch to your database will be done to extract the schema definitions. This includes the retrieval of the *PrimaryKey*, *Identity* and *Nullable-Columns* information. The information will be cached in memory with the class (or *Model*) actual name as the key.

#### Caching the class properties

The properties of your class (or *Model*) is being extracted and is cached in the memory. This enables the library to reuse it in any future calls (that is using the same object).

#### Caching the SQL statement

The *SQL* statements are being generated and cached automatically by the library. The generated *SQL* statement is a multiple *packed-statements* that varies on the number of batches you passed in the *batchSize* argument. Let us say, you passed *30*, then the number of packed-statements are *30*.

#### Caching the execution context

The execution context and behavior is being cached. This enables the library to reused the existing execution context that has already been executed against the database. The execution context contains the *SQL Statements*, *Parameters*, *Preparations* and even the *Compiled ILs or Expressions*. By having this, *RepoDb* does not need to extract same operation every time there is an identity calls, those leads to become more high-performant and efficient.

#### Adding an implicit transaction

A new *Transaction* object is being assigned to the *Execution* if the caller does not passed any explicit transaction.

#### Preparation

Before executing the *DbCommand* object, the `Prepare()` method is being called to pre-define the execution against the database. In the case of *SQL Server*, it creates an *Execution-Plan* in advance.

#### Batch execution

The generated *packed statements* is being executed against the database only once. Though in reality, *RepoDb* is also batching due to the fact that *ADO.NET* is limited only to *2100 parameters*. Also, through these batches, the caller is able to define the *best* batch number based on the situations and scenarios (ie: *Number of Columns*, *Network Latency*, etc).

--------

**Voila! You have completed this tutorial.**
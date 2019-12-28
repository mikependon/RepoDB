# RepoDb

**RepoDb** is a *hybrid-ORM* library for .NET. It provides certain features of both micro-ORMs and macro-ORMs. It helps the developer to simplify the “switchover” of when to use the “basic” and “advance” operations during the development.

All [*operations*](https://github.com/mikependon/RepoDb#operations) were implemented as extended methods of the *IDbConnection* object. Once you hold the opened-state of your database connection object, you can then do all things you would like to do with your database through those extended methods.

For full library documentation, please click [here](https://repodb.readthedocs.io/en/latest/).

## How is it differed from Micro-ORM and Macro-ORM?

*RepoDb* library is differed from other *micro-ORMs* and *macro-ORMs* in many ways. Below are some high-level differences.

### Features

- The *second-layer cache* has been introduced; can be customized.
- The *tracing* has been introduced; can be customized.
- The library is *extensible* to other RDBMS DB Provider.
- The *dynamic* support is rich.
- The *batch-operations* were introduced, executed through *packed-statements* and is *ACID*.
- The *bulk-operations* were introduced reusing the existing ADO.NET implementation.
- The usage of *expression* is present in most operations (both *basic* and *advance* operations).
- The *statement builder* can be customized, developers can create its own and override the default implementation.
- The *repositories* were introduced; can be leveraged.
- The *hints* were introduced and can be used in fetched-operations.
- The *asynchronous* operations are present in all operations.
- The equivalent *table-based* operations are present for most operations.

### Usability
- *Easy* installation, only takes few seconds.
- No controlled layer like *DbContext*, those make the developers *speed-up the usage* right after the installation.
- Calls to *Query* and *ExecuteQuery* method is just a *dot-notation* away. *This refers to how easy it is to switch from basic and advance operations.*
- Repository implementation is *simple* when leveraging the built-in repositories.
- Can work *without* the models; everything can be *dynamic*.
- *Transmission* of the data between different RDBMS DB Providers will only take few lines of codes.

### Differences Diagram

Below is the high-level diagram that shows how *RepoDb* library differs from the other *ORMs*.

<p align="center">
	<img src="https://github.com/mikependon/RepoDb/blob/master/RepoDb.Raw/Images/Differences.PNG" height="360px" />
</p>

## High-Level Architecture

The diagram below shows the actual high-level architecture of RepoDb.

<p align="center">
	<img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/Images/HLA.PNG" height="480px" />
</p>

## Why it is fast and efficient?

*RepoDb* has its own *compiler* and helper *cache-objects* that helps itself to become more *performant* and *efficient*. Through its own compiler, it is able to transform the actual raw data into a C# class object (vice versa) in a very *fast* manner. Through its helper cache-objects, it is able to *reuse* all *already-executed* and *already-extracted* contexts and objects that helps itself manage the consumption of the computer memory.

<p align="center">
	<img src="https://github.com/mikependon/RepoDb/blob/master/RepoDb.Raw/Images/Layers.PNG" height="460px" />
</p>

### Preparation

All *push executions* in *RepoDb* are being prepared prior the actual execution. *Preparation* is the process in which your client application gives signal to your database engine to understand the pre-submitted query statements and its parameters, then cache it in the database layer itself. In the case of SQL Server, it is the actual *execution plan* we are aiming to be created. By having this, further execution on the same query statements with the same parameters (but with different parameter values) is very *optimal*.

**Note**: Push executions are when calling the *Insert*, *InsertAll*, *Merge*, *MergeAll*, *Update* and *UpdateAll* executions.

### Batch-operations

*RepoDb* has introduce the *batch-operations* which in many ways understood by most developers as normal single operations. The *batch-operation* is the process in between the layer of your application and your database where all operations are executed (or passed) in a batch. Meaning, multiple statements are *wrapped* and is executed at *once*. The execution is *ACID* as it is being implied by implicit transaction (if not present). Through this feature, the developers can control and optimize the way how the application transmit the data all over the network.

**Note**: RepoDb is also supporting the **bulk operations** in a separate implementation. You can see the difference of both operations [here](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations).

## Links and Resources

- [GitHub Repository](https://github.com/mikependon/RepoDb)

	- [RepoDb](https://github.com/mikependon/RepoDb/tree/master/RepoDb) - stopped!
	- [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core)
	- [RepoDb.SqLite](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite)
	- [RepoDb.MySql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql)
	- [RepoDb.PostgreSql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.PostgreSql)

- [Documentation](https://repodb.readthedocs.io/en/latest/) - mostly API usage.

- [Nuget Package](https://www.nuget.org/packages/RepoDb)

	- [RepoDb](https://www.nuget.org/packages/RepoDb)
	- [RepoDb.SqLite](https://www.nuget.org/packages/RepoDb.SqLite)
	- [RepoDb.MySql](https://www.nuget.org/packages/RepoDb.MySql)
	- [RepoDb.PostgreSql](https://www.nuget.org/packages/RepoDb.PostgreSql)

## Topics you might be interested

- [Getting started](https://github.com/mikependon/RepoDb/wiki/Getting-Started)
- [Implementing a Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository)
- [Making the Repositories Dependency-Injectable](https://www.nuget.org/packages/RepoDb)
- [Bulk-Operations vs Batch-Operations](https://www.nuget.org/packages/RepoDb)
- [Multiple Resultsets via QueryMultiple and ExecuteQueryMultiple](https://www.nuget.org/packages/RepoDb)
- [Working with Transactions](https://www.nuget.org/packages/RepoDb)
- [Expression Trees](https://www.nuget.org/packages/RepoDb)
- [Advance Field and Type Mapping Implementations](https://www.nuget.org/packages/RepoDb)
- [Customizing a Cache](https://www.nuget.org/packages/RepoDb)
- [Implementing a Trace](https://www.nuget.org/packages/RepoDb)
- [The importance of Connection Persistency](https://www.nuget.org/packages/RepoDb)
- [Working with Enumerations](https://www.nuget.org/packages/RepoDb)
- [Extending support for specific DB Provider](https://www.nuget.org/packages/RepoDb)
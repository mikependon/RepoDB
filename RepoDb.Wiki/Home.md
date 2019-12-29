# RepoDb

***RepoDb*** is a *hybrid-ORM* library for .NET. It provides certain features of both micro-ORMs and macro-ORMs. It helps the developer to simplify the “switchover” of when to use the “basic” and “advance” operations during the development.

All [*operations*](https://github.com/mikependon/RepoDb#operations) were implemented as extended methods of the *IDbConnection* object. Once you hold the opened-state of your database connection object, you can then do all things you would like to do with your database through those extended methods.

For full library documentation, please click [here](https://repodb.readthedocs.io/en/latest/).

## How is it differed from Micro-ORM and Macro-ORM?

*RepoDb* library is differed from other *micro-ORMs* and *macro-ORMs* in many ways.

Below are some high-level differences.

### For the Features

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

### For the Usability

- *Easy* installation, only takes few seconds.
- No controlled layer like *DbContext*, those make the developers *speed-up the usage* right after the installation.
- Calls to *Query* and *ExecuteQuery* method is just a *dot-notation* away. *This refers to how easy it is to switch from basic and advance operations.*
- Repository implementation is *simple* when leveraging the built-in repositories.
- Can work *without* the models; everything can be *dynamic*.
- *Transmission* of the data between different RDBMS DB Providers will only take few lines of codes.

## Learnings

- [Getting started](https://github.com/mikependon/RepoDb/wiki/Getting-Started)
- [Implementing a Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository)
- [Working with Transactions](https://github.com/mikependon/RepoDb/wiki/Working-with-Transactions)
- [Expression Trees](https://github.com/mikependon/RepoDb/wiki/Expression-Trees)
- [Customizing a Cache](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache)
- [Implementing a Trace](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Trace)

## Practical Topics

- [Making the Repositories Dependency-Injectable](https://github.com/mikependon/RepoDb/wiki/Making-the-Repositories-Dependency-Injectable)
- [Extending the supports for specific DB Provider](https://github.com/mikependon/RepoDb/wiki/Extending-the-supports-for-specific-DB-Provider)
- [Bulk-Operations vs Batch-Operations](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations)
- [Multiple Resultsets via QueryMultiple and ExecuteQueryMultiple](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple)
- [Advance Field and Type Mapping Implementations](https://github.com/mikependon/RepoDb/wiki/Advance-Field-and-Type-Mapping-Implementation)
- [The importance of Connection Persistency](https://github.com/mikependon/RepoDb/wiki/The-importance-of-Connection-Persistency)
- [Working with Enumerations](https://github.com/mikependon/RepoDb/wiki/Working-with-Enumerations)

## Frequently Asked Questions

- [How is it differed from Dapper?]() - in-progress
- [Will you support IQueryable?]() - in-progress
- [Will you support Join Operations?]() - in-progress
- [Which databases are you supporting?]() - in-progress
- [How do we generate the models?]() - in-progress

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

<p align="center">
	<img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Icons/RepoDb-64x64.png" height="64px" />
</p>

-----------------

[![SolutionBuilds](https://img.shields.io/appveyor/ci/mikependon/repodb-h87g9?label=sln%20builds)](https://ci.appveyor.com/project/mikependon/repodb-h87g9)
[![Wiki](https://img.shields.io/badge/wiki-information-yellow)](https://github.com/mikependon/RepoDb/wiki)
[![CodeSize](https://img.shields.io/github/languages/code-size/mikependon/repodb)](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core)
[![GitterChat](https://img.shields.io/gitter/room/mikependon/RepoDb?color=48B293)](https://gitter.im/RepoDb/community)
[![Documentation](https://img.shields.io/badge/docs-library-yellowgreen)](https://repodb.readthedocs.io/en/latest/?badge=latest)
[![License](https://img.shields.io/badge/license-apache-important)](http://apache.org/licenses/LICENSE-2.0.html)

## RepoDb - a hybrid ORM Library for .NET.

RepoDb is an open-source .NET ORM that bridge the gaps between micro-ORMs and macro-ORMs. It helps the developer to simplify the switch-over of when to use the “BASIC” and “ADVANCE” operations during the development.

It is the best alternative ORM to both Dapper and EntityFramework.

<details>
<summary><b>Why use RepoDb as your ORM?</b></summary>

- **Usabilities and Benefits**

	- The installation is easy and fast! It can then be used right-away.
	- The layer like *DbContext* is eliminated, those make the developers *speed-up* the usability.
	- The calls to *Fluent* and *Raw-SQL* method is just a *dot* away.
	- The implementation of *Repository* becoming more *simpler* by leveraging the built-in repositories.
	- The support to dynamics is rich; can work *without* the models.
	- The processes to handle the *large datasets* is very-simple with *Batch* and *Bulk* operations.
	- The *round-trips* to the database is being minimized with *2nd-Layer cache*.
	- The *transmission* of the data from *different* RDBMS DB Providers only take few lines of codes.

- **Features and Capabilities**

	- The *2nd-layer cache* has been introduced; can be customized.
	- The *tracing* has been introduced; can be customized.
	- The support to *dynamics* is rich.
	- The *batch-operations* were introduced; being executed through *packed-statements* and is *ACID*.
	- The *bulk-operations* were introduced and is leveraging the ADO.NET implementations.
	- The usage of *expression* is present in most operations.
	- The *statement builder* can be customized. The developers can override the default implementation.
	- The *repositories* were introduced; can be leveraged for actual implementation.
	- The *hints* were introduced to optimize the help query analyzers.
	- The *asynchronous* operations are present in all operations.
	- The equivalent *table-based* operations are present for most operations.

- **Important Stories to Read**

	- Please visit the official [RepoDb vs Dapper](https://dev.to/mikependon/c-what-will-make-you-choose-repodb-over-dapper-orm-3eb8) blog at [DEV.to](https://dev.to/mikependon).

</details>

### It is easy-to-use

RepoDb [operations](https://github.com/mikependon/RepoDb#operations) were implemented as extended methods of *IDbConnection* object. As long as the database connection is open, the developers can do all the activities towards the database.

### It is high-performant

RepoDb has its own compiler and pre-caches the already-generated compiled-ILs and compiled-Expressions for future reusabilities.

### It is efficient

RepoDb extracts and caches the object properties, mappings and SQL statements and reusing them throughout the process of transformations and executions. It helps eliminate the creation of unnecessary objects that leads to a low memory consumption.

## Community

RepoDb is rapidly expanding its capabilities and features to become the main-stream hybrid-ORM for .NET. Though it is *not* a macro-ORM, it really requires significant amount of time and effort to maintain.

It is always open for community contributions, so please help us build and realize the solution.

### Engagements

We would like to build a healthy and active community that would help fellow .NET developers build the knowledge-base when it comes to database accessibility. Please get in touch with us via:

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/questions/tagged/repodb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Contributions

To contribute, please open the [issues](https://github.com/mikependon/RepoDb/issues) tab and filter the list of items with [for-grabs](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) label. Otherwise, please create a [new issue](https://github.com/mikependon/RepoDb/issues/new) for us to look-at and discuss.

Your biggest contribution is to utilize and share this solution to other developers. You can:

- Blog it
- Discuss it
- Document it
- Share it
- Use it

We are really thanking you for that!

### Contribution hints

- [Building the Solutions](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/Building%20the%20Solutions.md) - let us build your copies.
- [Coding Standards](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/Coding%20Standards.md) - let us be uniformed.
- [Reporting an Issue](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/Reporting%20an%20Issue.md) - let us organize for easy tracking and fixing.

### Active code-lines

The pull-requests must be done on the following code-lines ([*RepoDb.Core*](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core), [*RepoDb.SqlServer*](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqlServer), [*RepoDb.SqLite*](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite), [*RepoDb.MySql*](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql) and [*RepoDb.PostgreSql*](https://github.com/mikependon/RepoDb/tree/master/RepoDb.PostgreSql)).

## Builds and Tests Result

Project/Solution                                                                                                       | Build                                                                                                                                                  | Version                                                                                                                                                             | Downloads                                                                                                                                                        | Unit Tests                                                                                                                                                    | IntegrationTests                                                                                                                                                                                           |
-----------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
[Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core)                                                   | [![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ek0nw)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)                    | [![CoreVersion](https://img.shields.io/nuget/v/RepoDb)](https://www.nuget.org/packages/RepoDb)                                                                      | [![CoreDL](https://img.shields.io/nuget/dt/repodb)](https://www.nuget.org/packages/RepoDb)                                                                       | [![CoreUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-yf1cx)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx/build/tests)        | [![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qksas)](https://ci.appveyor.com/project/mikependon/repodb-qksas/build/tests)                                              |
[SqlServer](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqlServer)                                         | [![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-paj1k)](https://ci.appveyor.com/project/mikependon/repodb-paj1k)                    | [![CoreVersion](https://img.shields.io/nuget/v/RepoDb.SqlServer)](https://www.nuget.org/packages/RepoDb.SqlServer)                                                  | [![CoreDL](https://img.shields.io/nuget/dt/repodb.sqlserver)](https://www.nuget.org/packages/RepoDb.SqlServer)                                                   | [![CoreUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-iqu81)](https://ci.appveyor.com/project/mikependon/repodb-iqu81/build/tests)        | [![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qja7a)](https://ci.appveyor.com/project/mikependon/repodb-qja7a/build/tests)                                              |
[SqLite](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite)                                               | [![SqLiteBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-o6787)](https://ci.appveyor.com/project/mikependon/repodb-o6787)                  | [![SqLiteVersion](https://img.shields.io/nuget/v/RepoDb.SqLite)](https://www.nuget.org/packages/RepoDb.SqLite)                                                      | [![SqLiteDL](https://img.shields.io/nuget/dt/repodb.sqlite)](https://www.nuget.org/packages/RepoDb.SqLite)                                                       | [![SqLiteUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-mhpo4)](https://ci.appveyor.com/project/mikependon/repodb-mhpo4/build/tests)      | [![SqLiteIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-eg27p)](https://ci.appveyor.com/project/mikependon/repodb-eg27p/build/tests)                                            |
[MySql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql)                                                 | [![MySqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)                   | [![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql)](https://www.nuget.org/packages/RepoDb.MySql)                                                         | [![MySqlDL](https://img.shields.io/nuget/dt/repodb.mysql)](https://www.nuget.org/packages/RepoDb.MySql)                                                          | [![MySqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)       | [![MySqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)                                             |
[PostgreSql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.PostgreSql)                                       | [![PostgreSqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-xb4rk)](https://ci.appveyor.com/project/mikependon/repodb-xb4rk)              | [![PostgreSqlVersion](https://img.shields.io/nuget/v/RepoDb.PostgreSql)](https://www.nuget.org/packages/RepoDb.PostgreSql)                                          | [![PostgreSqlDL](https://img.shields.io/nuget/dt/repodb.postgresql)](https://www.nuget.org/packages/RepoDb.PostgreSql)                                           | [![PostgreSqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-a63f5)](https://ci.appveyor.com/project/mikependon/repodb-a63f5/build/tests)  | [![PostgreSqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-uf6o7)](https://ci.appveyor.com/project/mikependon/repodb-uf6o7/build/tests)                                        |
[SqlServer (Bulk)](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations) | [![SqlServerBulkOperationsBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-uai8a)](https://ci.appveyor.com/project/mikependon/repodb-uai8a) | [![SqlServerBulkOperationsVersion](https://img.shields.io/nuget/v/repodb.sqlserver.bulkoperations)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations) | [![SqlServerBulkOperationsDL](https://img.shields.io/nuget/dt/repodb.sqlserver.bulkoperations)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)  | -                                                                                                                                                             | [![SqlServerBulkOperationsIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-oap1j?label=tests)](https://ci.appveyor.com/project/mikependon/repodb-oap1j/build/tests)               |

## Supported Databases

Practically, RepoDb has supported all RDBMS data-providers. Developers has the freedom to write their own SQL statements and execute it against the database through the *Execute()* methods mentioned below.

- [ExecuteQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequery)
- [ExecuteNonQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executenonquery)
- [ExecuteScalar](https://repodb.readthedocs.io/en/latest/pages/connection.html#executescalar)
- [ExecuteReader](https://repodb.readthedocs.io/en/latest/pages/connection.html#executereader)
- [ExecuteQueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple)

### Fully supported databases for fluent-methods

<img src="https://github.com/mikependon/RepoDb/blob/master/RepoDb.Raw/Images/SqlServer.png?raw=true" height="64px" title="SqlServer" /> <img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/Images/SqLite.png" height="64px" title="SqLite" /> <img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/Images/MySql.png" height="64px" title="MySql" /> <img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/Images/PostgreSql.png" height="64px" title="PostgreSql" />

RepoDb has “fluent” methods in which the SQL Statements are automatically being constructed as part of the execution context. These methods are the most common operations being used by most developers (*please see the [operations](https://github.com/mikependon/RepoDb#operations) section*). In this regards, RepoDb only fully supported the *SQL Server*, *SQLite*, *MySQL* and *PostgreSQL* RDBMS data providers.

### Extensibility

RepoDb is a highly extensible to further support customizations. You can:

- [Customize the Cache](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache)
- [Create a customized Property Handler](https://github.com/mikependon/RepoDb/wiki/Creating-a-Property-Handler)
- [Implement a Trace](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Trace)
- [Extending the supports for specific DB Provider](https://github.com/mikependon/RepoDb/wiki/Extending-the-supports-for-specific-DB-Provider)

And many more extensibility possibilities.

## Operations

Operation                                                                                                 | Normal<TEntity> | Normal<TEntity> (Async) | TableName | TableName (Async) | Packed Execution | Data Providers         |
----------------------------------------------------------------------------------------------------------|-----------------|-------------------------|-----------|-------------------|------------------|------------------------|
[Average](https://repodb.readthedocs.io/en/latest/pages/connection.html#average)                          | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[AverageAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#averageall)                    | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[BatchQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#batchquery)                    | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[BulkDelete](https://repodb.readthedocs.io/en/latest/pages/connection.html#bulkdelete)                    | YES             | YES                     | YES       | YES               | NO               | SQLSVR                 |
[BulkInsert](https://repodb.readthedocs.io/en/latest/pages/connection.html#bulkinsert)                    | YES             | YES                     | YES       | YES               | NO               | SQLSVR                 |
[BulkMerge](https://repodb.readthedocs.io/en/latest/pages/connection.html#bulkmerge)                      | YES             | YES                     | YES       | YES               | NO               | SQLSVR                 |
[BulkUpdate](https://repodb.readthedocs.io/en/latest/pages/connection.html#bulkupdate)                    | YES             | YES                     | YES       | YES               | NO               | SQLSVR                 |
[Count](https://repodb.readthedocs.io/en/latest/pages/connection.html#count)                              | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[CountAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#countall)                        | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Delete](https://repodb.readthedocs.io/en/latest/pages/connection.html#delete)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[DeleteAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#deleteall)                      | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[ExecuteNonQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executenonquery)          | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequery)                | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteQueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple)| YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteReader](https://repodb.readthedocs.io/en/latest/pages/connection.html#executereader)              | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteScalar](https://repodb.readthedocs.io/en/latest/pages/connection.html#executescalar)              | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[Exists](https://repodb.readthedocs.io/en/latest/pages/connection.html#exists)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Insert](https://repodb.readthedocs.io/en/latest/pages/connection.html#insert)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[InsertAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#insertall)                      | YES             | YES                     | YES       | YES               | YES         	 | SUPPORTED              |
[Max](https://repodb.readthedocs.io/en/latest/pages/connection.html#max)                                  | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[MaxAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#maxall)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Merge](https://repodb.readthedocs.io/en/latest/pages/connection.html#merge)                              | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[MergeAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#mergeall)                        | YES             | YES                     | YES       | YES               | YES              | SUPPORTED              |
[Min](https://repodb.readthedocs.io/en/latest/pages/connection.html#min)                                  | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[MinAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#minall)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Query](https://repodb.readthedocs.io/en/latest/pages/connection.html#query)                              | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[QueryAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#queryall)                        | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[QueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#querymultiple)              | YES             | YES                     | NO        | NO                | YES              | SUPPORTED              |
[Sum](https://repodb.readthedocs.io/en/latest/pages/connection.html#sum)                                  | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[SumAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#sumall)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Truncate](https://repodb.readthedocs.io/en/latest/pages/connection.html#truncate)                        | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Update](https://repodb.readthedocs.io/en/latest/pages/connection.html#update)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[UpdateAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#updateall)                      | YES             | YES                     | YES       | YES               | YES              | SUPPORTED              |

To learn more about these operations, please visit our [reference implementations](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/Reference%20Implementations.md) page.

## Benchmark

The benchmark result to be shown on this page will always be referring to the community-approved ORM bencher tool (the [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) tool).

Results below is the actual recent official execution [result](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20190520_netcore.txt).

<img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/RDAB/RDAB-Result.PNG" height="460px" />

This section will always be updated with the latest official result.

## Credits

- [AppVeyor](https://www.appveyor.com/) - for the builds and test-executions.
- [GitHub](https://github.com/) - for hosting this project.
- [Gitter](https://gitter.im/) - for the community engagements.
- [Moq](https://github.com/moq/moq4) - for being the tests mocking framework.
- [Nuget](https://www.nuget.org/) - for the package deliveries.
- [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) - for measuring the performance and efficiency.
- [ReadTheDocs](https://readthedocs.org/) - for the library documentations.
- [SharpLab](https://sharplab.io/) - for helping us on our IL coding.
- [Shields](https://shields.io/) - for the awesome badges.
- [StackEdit](https://stackedit.io) - for being the markdown file editor.
- [System.Data.SQLite](https://www.nuget.org/packages/System.Data.SQLite/), [MySql.Data](https://www.nuget.org/packages/MySql.Data/), [Npgsql](https://www.nuget.org/packages/Npgsql/) - for being the extended DB provider drivers.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

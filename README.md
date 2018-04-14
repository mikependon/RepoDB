## RepoDb

A dynamic ORM .Net Library used to create an entity-based repository classes when accessing data from the database.

### Package
Link: https://www.nuget.org/packages/RepoDb

### Documentation
Link: https://github.com/mikependon/RepoDb/blob/master/RepoDb.Documents/documentation_v1.0.9.md (in-progress)

### Goal

We aim to let .Net developers limit the implementation of SQL Statements within the application. We believe that as a .Net developer, one should only focus on .Net and Business scenario development and not on writing repetitive SQL Statements. Ofcourse, unless you are working closely with SQL Server Management Studio.

### Notes

 - We will keep it simple as possible (KISS principle)
 - We will make it fast as possible
 - We will never make complex implementations (specially for Queries and Methods)
 - We will avoid developing complex JOINs (until it is needed and requested by the community)
 - We will never ever do try-catch inside the library
 
## Next Version

 - 
 
### Features

 - Add the currently used IStatementBuilder on the IStatementBuilder methods - in progress (4/14/2018)
 - Add ExecutionTime (Timespan) property in the TraceLog class - in progress (4/14/2018)
 - Cache CommadType in Repository Level
 - Entity Property Caching
 - Support Column-Based Update using Dynamics
 - Support BatchQuery Operation
 - Support Field-Level Mapping
 - Cache SQL Statement Building
 - Reflection.Emit (SqlDataReader to Objects)
 - Support ObjectMapper
 - Support Multi-Mapping for Class-Level
 - MemoryCache Flush
 
### Todos

 - Documentation: Trace, StatementBuilder

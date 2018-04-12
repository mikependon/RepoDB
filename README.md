## RepoDb

A dynamic ORM .Net Library used to create an entity-based repository classes when accessing data from the database.

### Package
Link: https://www.nuget.org/packages/RepoDb

### Documentation
Link: https://github.com/mikependon/RepoDb/blob/master/RepoDb.Documents/documentation.md (in-progress)

### Goal

We aim to let .Net developers avoid writing more SQL Statements in the application. We believe that as a .Net developers, we should only focus on .Net development and not on writing repetitive SQL Statements, unless you are working closely in SQL Server Management Studio.

### Notes

 - We will always be FREE!
 - We will keep it simple as possible (KISS principle)
 - We will make it fast as possible
 - We will never make complex implementations (specially for Queries and Methods)
 - We will avoid developing complex JOINs (until it is needed and requested by the community)
 - We will never ever do try-catch inside the library
 
### Todos

 - Documentation: Cache, Trace, StatementBuilder
 - Add ExecutionTime (Timespan) property in the TraceLog class.
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

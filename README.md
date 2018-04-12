## RepoDb

A dynamic ORM .Net Library used to create an entity-based repository classes when accessing data from the database.

### Package
Link: https://www.nuget.org/packages/RepoDb

### Documentation
Link: https://github.com/mikependon/RepoDb/blob/master/RepoDb.Documents/documentation.md

### Goal

We will simplify and make your queries simple. We hope to let you avoid writing more SQL Statements in .Net applications as you go along the way. As a .Net developers, one should focus .Net development, unless you are closely working in SQL Server Management Studio.

### Notes

 - We will keep RepoDb as simple as possible
 - We will never make complex queries and methods (we will avoid developing JOINs until it is needed by the community)
 - We will never do try-catch inside the library
 
### Todos

 - Cache CommadType in Repository Level
 - Entity Property Caching
 - Reflection.Emit (SqlDataReader to Objects)
 - Cache SQL Statement Building
 - Support ObjectMapper
 - Support Multi-Mapping for Class-Level
 - Support Field-Level Mapping
 - MemoryCache Flush

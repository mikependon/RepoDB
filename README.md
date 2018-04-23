## RepoDb

A dynamic ORM .Net Library used to create an entity-based repository classes when accessing data from the database.

### Package
Link: https://www.nuget.org/packages/RepoDb

### Documentation
Link: https://github.com/mikependon/RepoDb/blob/master/RepoDb.Documents/documentation_v1.0.10.md (in-progress)

### Goal

We aim to let .Net developers limit the implementation of SQL Statements within the application. We believe that as a .Net developer, one should only focus on .Net and Business scenario development and not on writing repetitive SQL Statements. Ofcourse, unless you are working closely with SQL Server Management Studio.

### Community Address

As of to date, **RepoDb** is a running project. We will soon announce the stable release once we completed the 3 remaining features stated in the Next-Features and the Unit and Regression Tests. In preparation for the future usage, please refer to the documentation stated above. All developers can play around the library, but we advised not to used it for major projects for now.

### Notes

 - We will keep it simple as possible (KISS principle)
 - We will make it fast as possible
 - We will never make complex implementations (specially for Queries and Methods)
 - We will avoid developing complex JOINs (until it is needed and requested by the community)
 - We will never ever do try-catch inside the library

### Features

 - BatchQuery/BatchQueryAsync
 - Count/CountAsync
 - CountBig/CountBigAsync
 - Query/QueryAsync
 - Insert/InsertAsync
 - Delete/DeleteAsync
 - Update/UpdateAsync
 - InlineUpdate/InlineUpdateAsync
 - Merge/MergeAsync
 - BulkInsert/BulkInsertAsync
 - ExecuteReader/ExecuteReaderAsync
 - ExecuteNonQuery/ExecuteNonQueryAsync
 - ExecuteScalar/ExecuteScalarAsync
 - Transaction
 - Asynchronous Operations
 - Type Mapping
 - Field Mapping
 - Multiple Mapping (Person = SELECT FROM Person, DELETE FROM sp_delete_person, UPDATE FROM sp_update_person, etc)
 - Expression Tree
 - Caching
 - Tracing
 - SQL Builder

### Next Release

 - Support the DbConnection.ExecuteQuery<TEntity> method, to return the IEnumerable list of RepoDb.Interfaces.IDataEntity.
 - IL Emit the conversion of RepoDb.Interfaces.IDataEntity to System.Data.DataTable
 - IL Emit the conversion of System.Data.DbDataReader to System.Object when calling the DbConnection.ExecuteQuery method.
 
### Todos

 - Documentation: Count, DbConnection.ExecuteQuery, InlineUpdate, BatchQuery, Trace, StatementBuilder, Field Mapping, Object Mapping, Multi-Mapping

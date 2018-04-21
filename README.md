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
 
### Next Version (1.0.15)

 - Added RepoDb.Reflection.Delegates.DataReaderToEntityMapperDelegate
 - Added RepoDb.Reflection.ConstructorInfoCache
 - Added RepoDb.Reflection.DataReaderMapper
 - Added RepoDb.Reflection.DelegateCache
 - Added RepoDb.Reflection.DelegateFactory
 - Added RepoDb.Reflection.MethodInfoCache
 - Added RepoDb.Reflection.MethodInfoTypes
 - Added RepoDb.Reflection.ReflectionFactory
 - Added RepoDb.Reflection.TypeArrayCache
 - Added RepoDb.Reflection.TypeCache
 - Added RepoDb.Reflection.TypeTypes
 - Added RepoDb.Attributes.CreateMethodInfoAttribute
 - Returned IDataReader object when using the IDbConnection.ExecuteReader
 - Fixed the IL Emitter when mapping DataReader to Null properties
 - Added ExecuteQuery to IDbConnection extension (returns an IEnumerable<object>)
 - Added BeforeExecuteQuery and AfterExecuteQuery trace methods
 - Supported DataEntityMapper, DataEntityMap
 - Supported the DbRepository.Count and DbRepository.CountBig

### Features

 - BatchQuery/BatchQueryAsync
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
 - Expression Tree
 - Caching
 - Tracing
 - SQL Builder

### Next Release

 - IL Emit the conversion of IDataEntity to System.Data.DataTable
 - Support Multi-Mapping for Class-Level
 - MemoryCache Flush
 
### Todos

 - Documentation: Count, DbConnection.ExecuteQuery, InlineUpdate, BatchQuery, Trace, StatementBuilder, Field Mapping, Object Mapping, Multi-Mapping

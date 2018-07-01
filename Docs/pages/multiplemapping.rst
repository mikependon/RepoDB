Multiple Entity Mapping
=======================

This feature is a unique built-in feature of the library. It allows the developer to map a single `DataEntity` object into a multiple database objects. This is very useful on a scenario that requires the combination of the `Table`, `View` and `StoredProcedure` to be mapped into single `DataEntity` object.

The class named `RepoDb.DataEntityMapper` is used when implementing a multiple mapping. Below are the methods.

- **For**: used to create a mapping between the `DataEntity` and the database object. Returns an `RepoDb.DataEntityMapItem` object.
 
The class named `RepoDb.DataEntityMapItem` is used to map the operation level of the repository into the database object. Below are the methods.

- **On**: used to map on which repository command operation and database object the mapping is implemented.
- **Set**: used to set the repository command operation data entity mapping definition. This is the underlying method being called by `On` method.
- **Get**: get the entity mapping definition defined on the repository command operation.

Multi-mapping is bound in an operation-level of the repository. This means that the developer can map the `Query` operation of a `Customer` object into `[dbo].[Customer]` table of the database, whereas the `Delete` operation is mapped into `[dbo].[sp_DeleteCustomer]` database object.

Let say we have the following class and database objects below.

- A class named `Customer`.
- A table named `[dbo].[Customer]`.
- A stored procedure named `[dbo].[sp_DeleteCustomer]`.
- A stored procedure named `[dbo].[sp_InsertCustomer]`.
- A view named `[dbo].[vw_Customer]`.
 
Then, below are the codes for multiple mapping.

::

	DataEntityMapper.For<Stock>()
		.On(Command.Insert, "[dbo].[sp_QueryCustomer]", CommandType.StoredProcedure)
		.On(Command.Delete, "[dbo].[sp_DeleteCustomer]", CommandType.StoredProcedure)
		.On(Command.Query, "[dbo].[vw_Customer]")
		.On(Command.Update, "[dbo].[Customer]")
		.On(Command.BulkInsert, "[dbo].[Customer]", CommandType.TableDirect);

Below are the supported mapping for each command.

- **InlineInsert**: only for `CommandType.<Text | TableDirect>`.
- **InlineMerge**: only for `CommandType.<Text | TableDirect>`.
- **InlineUpdate**: only for `CommandType.<Text | TableDirect>`.
- **BatchQuery**: full support.
- **BulkInsert**: full support.
- **Count**: full support.
- **Delete**: full support.
- **DeleteAll**: full support.
- **Insert**: full support.
- **Merge**: full support.
- **Query**: full support.
- **Truncate**: full support.
- **Update**: full support.

Attempt to map to a wrong command would throw an `System.InvalidOperationException` back to the caller.
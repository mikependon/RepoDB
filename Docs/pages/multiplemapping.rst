Multiple Entity Mapping
=======================

This feature is a unique built-in feature of the library that enables the developer to do multiple mapping on a `DataEntity` object into multiple object in the database. This is very usable for some complex requirements that includes like the combined implementations of `Table`, `Views` and `StoredProcedures` must be mapped into one `DataEntity` object.

The class named `RepoDb.DataEntityMapper` is used when doing a multiple mapping. Below are the methods.

- **For**: used to create a mapping between the data entity and database object. Returns an `RepoDb.DataEntityMapItem` object.
 
The class named `RepoDb.DataEntityMapItem` is used to map the operation level of the repository into the database object. Below are the methods.

- **On**: used to map on which repository command operation and database object the mapping is implemented.
- **Set**: used to set the repository command operation data entity mapping definition. This is the underlying method being called by `On` method.
- **Get**: get the entity mapping definition defined on the repository command operation.

Multi-mapping is bound in an operation-level of the repository. This means that the developer can map the `Query` operation of a `Customer` object into `[dbo].[Customer]` table of the database, whereas the `Delete` operation is mapped into `[dbo].[sp_DeleteCustomer]` database object.

In scenario below where a `Customer` entity object was created in the solution, and the following objects exist in the database.

 - A table named `Customer`.
 - A stored procedure named `sp_DeleteCustomer`.
 - A stored procedure named `sp_InsertCustomer`, where the logic inside of is joining from different database tables.
 - A view named `vw_Customer`.
 
Then, the developers can call the mapper methods when mapping a `Customer` object into these database objects.

Below are the codes for multiple mapping.

::

	DataEntityMapper.For<Stock>()
		.On(Command.Insert, "[dbo].[sp_QueryCustomer]", CommandType.StoredProcedure)
		.On(Command.Delete, "[dbo].[sp_DeleteCustomer]", CommandType.StoredProcedure)
		.On(Command.Query, "[dbo].[vw_Customer]")
		.On(Command.Update, "[dbo].[Customer]")
		.On(Command.BulkInsert, "[dbo].[Customer]", CommandType.TableDirect);

These feature has its own limitations. In reality, all mappings could not be done on every command like `Merge`. The `Merge` can only be done at the `Table` and not in `StoredProcedure`.

Below are the supported mapping for each command.

- **BatchQuery**: only for a database Table and `CommandType.Text`.
- **Count**: only for a database Table and `CommandType.Text`.
- **CountBig**: only for a database Table and `CommandType.Text`.
- **InlineUpdate**: only for a database Table and `CommandType.Text`.
- **Merge**: only for a database Table and `CommandType.Text`.
- **BulkInsert**: only for a database Table and `CommandType.<Text | TableDirect>`.
- **Insert**: full support.
- **Delete**: full support.
- **Query**: full support.
- **Update**: full support.

Attempt to map to a wrong command would throw an `System.InvalidOperationException` back to the caller.
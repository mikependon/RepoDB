<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="Firebird.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.Firebird</strong></a></span>
    <br/>
    <span style="font-size:16px;">The Firebird schema reader and composer of RepoDB library.</span>
</div>

-----

- `FirebirdSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, identity, computed columns, primary key, indexes, foreign keys, unique and check constraints) from the `RDB$` system tables of the connection it is created with. Firebird has no schema, so a table is identified by its name only, and a name or a schema argument that has a schema (i.e.: `GetTables("sales")`) throws a `NotSupportedException`. A unique index is read as an index, while a unique constraint is read as a constraint.
- `FirebirdSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. Firebird supports the `CASCADE`, `SET NULL`, `SET DEFAULT` and `NO ACTION` rules on both `DELETE` and `UPDATE` of a foreign key (`RESTRICT` is the same as `NO ACTION`). The direction of an index is for the whole index, so an index is descending only if all of its columns are descending, and Firebird has no included columns, no partial indexes and no column-level collation. Firebird has no `DROP TABLE IF EXISTS`, so the statement is an `EXECUTE BLOCK` that checks the table first. A DDL statement is part of the transaction, so it can be rolled back.
- `GlobalConfiguration.Setup().UseFirebirdSchema()` (`FirebirdSchemaGlobalConfiguration`) registers the composer for `FbConnection`.

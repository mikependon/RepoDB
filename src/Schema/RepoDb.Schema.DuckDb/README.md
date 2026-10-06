<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="DuckDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.DuckDb</strong></a></span>
    <br/>
    <span style="font-size:16px;">The DuckDb schema reader and composer of RepoDB library.</span>
</div>

-----

- `DuckDbSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, identity, generated columns, primary key, indexes, foreign keys, unique and check constraints) from the `SYSCAT` catalog views of the connection it is created with. A schema is a DuckDb schema; a table without a schema belongs to the current schema of the connection. A unique index is read as an index, while a unique constraint is read as a constraint.
- `DuckDbSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. DuckDb supports the `CASCADE`, `SET NULL`, `NO ACTION` and `RESTRICT` rules on `DELETE` of a foreign key, and only the `NO ACTION` and `RESTRICT` rules on `UPDATE`. The included columns of an index are supported only by the unique indexes, and DuckDb has no partial indexes and no column-level collation. A DDL statement is part of the transaction, so it can be rolled back.
- `GlobalConfiguration.Setup().UseDuckDbSchema()` (`DuckDbSchemaGlobalConfiguration`) registers the composer for `DuckDBConnection`.

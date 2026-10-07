<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="DuckDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.DuckDb</strong></a></span>
    <br/>
    <span style="font-size:16px;">The DuckDB schema reader and composer of RepoDB library.</span>
</div>

-----

- `DuckDbSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, indexes, foreign keys, unique and check constraints) from the `duckdb_*()` catalog functions of the connection it is created with. A schema is a schema of the database of the connection (`main` by default). DuckDB does not keep the names of the constraints, so it generates them. It has no identity column (a column whose default is `nextval()` of a sequence is read as an identity) and no flag for a generated column (a default that refers to another column is read as the expression of a generated column). The direction of an index key is not kept, and DuckDB has no foreign key rules.
- `DuckDbSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. DuckDB can not add a foreign key to an existing table, so the foreign keys are part of `CREATE TABLE` (`ComposeAddForeignKey` is empty), the tables are created after the tables that they reference (circular references and references across schemas can not be created), and an identity column creates its sequence first (the statement is a script separated by a semicolon). A not nullable column is added in 2 statements, and a generated column can not be added to an existing table. DDL is part of the transaction.
- `GlobalConfiguration.Setup().UseDuckDbSchema()` (`DuckDbSchemaGlobalConfiguration`) registers the composer for `DuckDBConnection`.

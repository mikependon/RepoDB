<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="Oracle.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.Oracle</strong></a></span>
    <br/>
    <span style="font-size:16px;">The Oracle schema reader and composer of RepoDB library.</span>
</div>

-----

- `OracleSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, identity, virtual columns, primary key, indexes, foreign keys, unique and check constraints) from the `ALL_*` data dictionary views of the connection it is created with. A schema is the owner (user) of the table; a table without a schema belongs to the current schema of the session. A unique index is read as an index, while a unique constraint is read as a constraint.
- `OracleSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. Oracle supports only the `CASCADE`, `SET NULL` and `NO ACTION` rules on `DELETE` of a foreign key and has no `UPDATE` rule. A DDL statement is committed by the server, so it can not be rolled back.
- `GlobalConfiguration.Setup().UseOracleSchema()` (`OracleSchemaGlobalConfiguration`) registers the composer for `OracleConnection`.

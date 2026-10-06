<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="Vertica.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.Vertica</strong></a></span>
    <br/>
    <span style="font-size:16px;">The Vertica schema reader and composer of RepoDB library.</span>
</div>

-----

- `VerticaSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, identity, primary key, foreign keys, unique and check constraints) from the `v_catalog` system views of the connection it is created with. A schema is a Vertica schema; a table without a schema belongs to the current schema of the connection. Vertica has no index (it has projections), so a table has no indexes, and a foreign key has no rules.
- `VerticaSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. `ComposeCreateIndex` throws a `NotSupportedException`, and the indexes of a schema from another database engine are not created. A computed column is created as a column with a default expression, as Vertica has no generated column. The constraints of Vertica are not enforced unless they are enabled. A DDL statement is committed by the server, so it can not be rolled back.
- `GlobalConfiguration.Setup().UseVerticaSchema()` (`VerticaSchemaGlobalConfiguration`) registers the composer for `VerticaConnection`.

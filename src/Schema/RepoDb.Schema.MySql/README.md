<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="MySQL.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.MySql</strong></a></span>
    <br/>
    <span style="font-size:16px;">The MySQL schema reader and composer of RepoDB library.</span>
</div>

-----

- `MySqlSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, indexes, foreign keys, unique and check constraints) from the `information_schema` of the connection it is created with. A schema is a database: the tables of the current database have no schema, and the tables of the other databases are qualified with the name of their database. A unique index is read as a unique constraint, as MySQL does not tell them apart.
- `MySqlSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. A DDL statement is committed by the server, so it can not be rolled back.
- `GlobalConfiguration.Setup().UseMySqlSchema()` (`MySqlSchemaGlobalConfiguration`) registers the composer for `MySqlConnection`.

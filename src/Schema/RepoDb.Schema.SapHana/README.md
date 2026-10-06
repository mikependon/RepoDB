<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="SAPHANA.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.SapHana</strong></a></span>
    <br/>
    <span style="font-size:16px;">The SAP HANA schema reader and composer of RepoDB library.</span>
</div>

-----

- `SapHanaSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, identity, generated columns, primary key, indexes, foreign keys, unique and check constraints) from the `SYS` system views of the connection it is created with. A schema is a SAP HANA schema; a table without a schema belongs to the current schema of the connection. A unique index is read as a unique constraint, as SAP HANA does not tell them apart.
- `SapHanaSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. SAP HANA supports the `CASCADE`, `SET NULL`, `SET DEFAULT` and `RESTRICT` rules on `DELETE` and `UPDATE` of a foreign key, but it has no included columns, no partial indexes and no column-level collation. SAP HANA has no `DROP TABLE IF EXISTS`, so the statement is an anonymous block that checks the table first. A DDL statement is committed by the server, so it can not be rolled back.
- `GlobalConfiguration.Setup().UseSapHanaSchema()` (`SapHanaSchemaGlobalConfiguration`) registers the composer for `HanaConnection`.

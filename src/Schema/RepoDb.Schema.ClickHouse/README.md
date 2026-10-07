<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="ClickHouse.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.ClickHouse</strong></a></span>
    <br/>
    <span style="font-size:16px;">The ClickHouse schema reader and composer of RepoDB library.</span>
</div>

-----

- `ClickHouseSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, data-skipping indexes and check constraints) from the `system` tables of the connection it is created with. A schema is a database: the tables of the current database have no schema, and the tables of the other databases are qualified with the name of their database. The primary key is the sorting key of the table. ClickHouse has no foreign keys, unique constraints or identity columns, so none is read.
- `ClickHouseSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects: a `MergeTree` table ordered by its primary key, its check constraints, and the indexes as `minmax` data-skipping indexes. A foreign key is composed as a blank statement, which is skipped. A computed column is composed as `MATERIALIZED`. ClickHouse has no transactions, so a statement can not be rolled back.
- `GlobalConfiguration.Setup().UseClickHouseSchema()` (`ClickHouseSchemaGlobalConfiguration`) registers the composer for `ClickHouseConnection`.

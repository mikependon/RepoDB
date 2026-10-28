<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="RepoDbCore.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.Core</strong></a></span>
    <br/>
    <span style="font-size:16px;">The schema productivity core package of RepoDB library.</span>
</div>

## Saving table definitions

Register a schema reader for the source connection, then export a definition:

```csharp
SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(connection), force: true);

connection.SaveDefinitionAsYaml("sales.Invoice", "invoice.yaml",
    relationshipBehavior: RelatedTableBehavior.Parents,
    commandTimeout: 30);

connection.SaveDefinitionAsJson("sales.Invoice", "invoice.json");
connection.SaveDefinitionAsXml("sales.Invoice", "invoice.xml");
```

The three methods accept `connection`, `tableName`, `filename`, `relationshipBehavior`, `commandTimeout`, `traceKey`, and `trace`, in that order. The relationship behavior defaults to `TableOnly`. Each method writes UTF-8 without a byte-order mark and returns after the file is saved. The parent directory must exist. An existing file is replaced after extraction and serialization succeed, using a temporary file in the same directory. Exports contain definitions, not table data or connection strings.

`SaveDefinitionAsJsonAsync`, `SaveDefinitionAsXmlAsync`, and `SaveDefinitionAsYamlAsync` accept the same arguments plus an optional final `CancellationToken`, and return `Task`:

```csharp
await connection.SaveDefinitionAsJsonAsync("sales.Invoice", "invoice.json",
    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren,
    commandTimeout: 30,
    cancellationToken: cancellationToken);
```

The async methods use `ISchemaReader`'s asynchronous catalog operations sequentially, forwarding cancellation and the scoped query settings. Provider readers use asynchronous trace callbacks. Definition construction and serialization run in memory; file writes and flushing use asynchronous IO. Cancellation is checked before committing the complete temporary file. Query, serialization, write, or cancellation failures leave an existing destination intact and remove the temporary file. Replacement itself is a synchronous filesystem operation; cancellation after the commit is not reported as a failed save.

The exporters use explicit JSON nodes and XML construction, without reflection-based object serialization, runtime code generation, or `Task.Run`. The NativeAOT smoke project at `src/Providers/RepoDb.Core/RepoDb.NativeAotTests` roots the entire schema core assembly for compilation and exercises synchronous/asynchronous saves, reads, and generic and multiple-table copies against SQLite, for every relationship behavior. It disables reflection-based JSON serialization and compares async saves with synchronous output. Schema creation binds dictionary parameters instead of reflecting over anonymous objects. Publish the smoke project with `dotnet publish -c Release -r win-x64` from that project directory, then run the published executable. Native publishing requires the platform's native toolchain (on Windows, the Visual C++ build tools and Windows SDK). Application-level NativeAOT compatibility also depends on the selected database provider and any custom schema reader or trace implementation.

The provider's reader determines which catalog objects are available. The format captures columns, primary keys, indexes, unique constraints, checks, defaults, identity settings, computed expressions, column comments, collations, and foreign keys. Objects not exposed by `ISchemaReader`, such as triggers and sequence definitions, are outside version 1.

## RepoDB.Schema definition format, version 1

YAML, JSON and XML represent the same definition tree. The root identifies the format as `RepoDB.Schema` and `formatVersion` as integer `1`. A future loader must check both before interpreting a document. Identifiers retain their original case; schema, table, database and provider are separate fields. No SQL is executed by loading or parsing a definition document alone.

### Document structure

| Field | Meaning |
| --- | --- |
| `format`, `formatVersion` | Format identity and integer version. |
| `source` | Source provider and database, when reported by the reader. |
| `extraction` | Resolved `rootTable` identity and requested `relationshipBehavior`. |
| `tables` | Table definitions, sorted by schema and name using ordinal comparison. |
| `externalDependencies` | Identities of referenced tables not included in this document. |

Each table has `table`, `columns`, `primaryKey`, `indexes`, `uniqueConstraints`, `checkConstraints`, and `foreignKeys`. Each column has `name`, `ordinal`, `type`, `nullable`, `identity`, `default`, `computed`, `collation`, and `comment`.

Null means unknown or not applicable; it does not imply false, zero, or an empty string. Empty collections are emitted as arrays. A null `primaryKey` means the reader returned no primary key. An empty object collection means no objects were returned or, for `TableOnly` foreign keys, they were intentionally excluded. This is not a guarantee that the provider reader supports every database feature.

Column order follows catalog ordinal order. Key column order, index column order, and foreign-key column pair order are significant and must be preserved. Named constraints and indexes are sorted ordinally by name. No extraction timestamp is included, so unchanged catalog metadata produces stable output.

### Types and text

`type.logical` uses `string`, `binary`, `boolean`, `uuid`, `int8`, `uint8`, `int16`, `uint16`, `int32`, `uint32`, `int64`, `uint64`, `decimal`, `float32`, `float64`, `datetime`, `datetimeOffset`, `time`, or `unknown`. The logical type reflects the reader's client type; `native` retains the full provider declaration when available, otherwise the reader's database type name. `provider` identifies its dialect. Provider-specific types with no client mapping use `unknown` rather than a CLR assembly-qualified name.

`precision` and `scale` preserve numeric metadata. Extended column metadata can preserve precision above 255 and negative scales, which `DbField` cannot represent. PostgreSQL-family readers retain their full type declarations for this purpose. `temporalPrecision` describes fractional-second digits separately where available.

`length` retains the reported size; `-1` indicates an unbounded size when reported that way by the reader. `unbounded` is true for `-1`, false for positive sizes, and null otherwise. `lengthUnit` is `bytes`, `characters`, `utf16CodeUnits`, or null. Do not assume characters when the unit is unknown. `fixedLength` distinguishes recognized fixed and variable text types, or is null when not known.

`unicode` is nullable: true means a Unicode encoding, false means a known non-Unicode encoding, and null means unknown. **False does not mean ASCII.** `characterSet` preserves the actual reported encoding, such as `ascii`, `latin1`, `utf8mb4`, or `UTF-16`. `collation` retains source sorting and comparison rules. File encoding is always UTF-8 and is independent of column character encoding.

MySQL-family readers project character sets and temporal precision. PostgreSQL-family readers project database encoding, collation, full native types, and temporal precision. SQL Server readers distinguish national text and UTF-8 collations and preserve legacy code pages as names such as `CP1252`. When no code page is reported, the source collation is retained. SQLite readers preserve declared types and Unicode text semantics. Other metadata remains null unless the reader reports it. Custom readers can populate the additional `ColumnInfo` fields directly within supported provider assemblies. Exports must not guess unknown source metadata.

### Defaults, expressions and constraints

`identity` contains `seed` and `increment`, or is null. `default.kind` retains the `ColumnDefaultValue` enum name. Its `expression` preserves native SQL. `computed`, index `filter`, and check-constraint `expression` contain `{dialect, sql}` objects, or null. Native expressions are preserved, not automatically translated between providers. A future loader must resolve incompatibilities before creating objects.

Primary keys include `name`, ordered `columns`, and `clustered`. Unique constraints include `name` and ordered `columns`. Indexes include `name`, `unique`, `clustered`, ordered `{name, direction}` column entries, `includedColumns`, and `filter`. Directions are `ascending` or `descending`.

### Relationships

| Behavior | Selection |
| --- | --- |
| `TableOnly` | Only the root table; its foreign keys are omitted. |
| `Parents` | Root and recursive referenced tables. |
| `Children` | Root and recursive referencing tables. |
| `ParentsAndChildren` | The connected group in both directions. |

Traversal uses the provider's existing `GetRelatedTables` implementation. Each resolved table is emitted once, including in cyclic graphs. Foreign keys include `name`, ordered `columns`, `references`, `onDelete`, and `onUpdate`. Rules retain the `ForeignKeyRule` enum names. `references` contains the target `table`, ordered target `columns`, and `external` flag. In particular, `Children` may select a table that references another parent outside the selected graph. That foreign key is retained with `external: true`, and its target appears in `externalDependencies`. Extraction does not silently expand the requested traversal to include it.

A future loader should validate external dependencies, create all tables and local objects, and then create foreign keys. The array order is deterministic, not a DDL execution plan; cycles are supported through this staged creation approach.

### Serialization rules

JSON uses objects, arrays, strings, booleans, integers, and null. YAML uses the same types with double-quoted string scalars and JSON escape sequences (YAML 1.2). Quoting prevents defaults, comments, names and SQL from being interpreted as dates, booleans, aliases, or YAML syntax. Enum values are case-sensitive strings.

XML uses a `repoDbSchema` root. Each property becomes a named child element; arrays contain `item` elements. Every element has a `type` attribute (`object`, `array`, `string`, `integer`, `boolean`, or `null`) so empty arrays, null, and empty strings remain distinguishable. Strings are escaped by the XML writer. Invalid XML characters cause export to fail before replacing an existing file.

For example, the same small tree in the three syntaxes is:

```json
{"format":"RepoDB.Schema","formatVersion":1,"externalDependencies":[]}
```

```yaml
format: "RepoDB.Schema"
formatVersion: 1
externalDependencies: []
```

```xml
<repoDbSchema type="object">
  <format type="string">RepoDB.Schema</format>
  <formatVersion type="integer">1</formatVersion>
  <externalDependencies type="array" />
</repoDbSchema>
```

These fragments illustrate syntax; complete exports also contain `source`, `extraction`, and `tables`. A decimal column's type entry looks like:

```yaml
type:
  logical: "decimal"
  native: "decimal(18,4)"
  provider: "SqlServer"
  length: null
  lengthUnit: null
  unbounded: null
  precision: 18
  scale: 4
  temporalPrecision: null
  fixedLength: null
  unicode: null
  characterSet: null
```

### Reader query settings

All bundled readers consult `SchemaReadContext.Current` for the duration of an export. This supplies catalog queries with the export's timeout, trace key and `ITrace` instance. The enclosing context is restored on success or failure and is isolated by execution context. Existing `ISchemaReader` signatures are unchanged. Third-party readers must explicitly honor this context to support these settings; the exporter cannot impose a timeout on an arbitrary reader implementation.

### Reading saved definitions

Use `ReadDefinitionFromJson`, `ReadDefinitionFromXml`, or `ReadDefinitionFromYaml` to reconstruct a saved definition. Each also has an `Async` equivalent with an optional cancellation token.

```csharp
var definition = connection.ReadDefinitionFromJson("invoice.json");
var asynchronous = await connection.ReadDefinitionFromYamlAsync("invoice.yaml",
    cancellationToken: cancellationToken);

foreach (var table in definition.Tables)
{
    Console.WriteLine(table.Table.Name);
}
```

These methods read files only. They do not open the connection, require a schema reader registration, query catalogs, create tables, or execute saved SQL. Provider and database identities come from the file. `SchemaDefinitionResult` contains all included `TableSchema` objects, the root identity, source metadata, extraction relationship behavior, external dependency identities, and format version. No relationship behavior argument is needed: the file determines which tables and foreign keys are available. `TableOnly` exports cannot restore omitted foreign keys.

`Definition` retains the complete parsed document, including expression dialects and portable metadata without equivalent model properties. Models reconstruct CLR types from the documented logical type names; `unknown` produces a null CLR type, and `string` produces `System.String`, including when the original client type was `System.Char`. No CLR type names from the file are loaded. Native declarations remain available through `ColumnInfo.NativeType` and `DbField.DatabaseType`. Numeric precision or scale outside the byte range is retained on `ColumnInfo`, with the corresponding `DbField` value null. The file format does not preserve every distinction between the original reader's CLR objects.

JSON and typed XML use the structures described above. The YAML reader accepts the representation emitted by RepoDB's writer: two-space block indentation and JSON-quoted scalar strings. General YAML features such as tags, aliases, and block scalar strings are outside this representation. XML document type declarations and external entities are rejected.

Invalid or unsupported definitions throw `InvalidDataException`; file access errors retain their normal IO exceptions. Validation checks the format/version, table identities, key and index column references, foreign-key mappings, and included versus external targets. It does not check whether an external table actually exists in a database.

`traceKey` and `trace` apply to the file read. Sync methods use synchronous trace callbacks; async methods use asynchronous callbacks and forward cancellation. The before log's `Statement` is the filename, which a trace may change. Trace cancellation follows the existing contract: throw `CancelledExecutionException`, or return null when `Cancel(false)` is requested. A successful read invokes the after callback with the resulting definition.

### Loading definitions into a database

`LoadDefinitionFromJson`, `LoadDefinitionFromXml`, and `LoadDefinitionFromYaml` read a definition through the corresponding `ReadDefinitionFrom*` method and create its tables on the supplied destination connection. Each has an `Async` equivalent with an optional final cancellation token. Register a schema composer for the destination provider; no source schema reader or source database connection is needed.

```csharp
var result = await connection.LoadDefinitionFromJsonAsync("invoice.json",
    tableExistenceBehavior: TableExistsBehavior.Skip,
    createdCallback: item => Console.WriteLine($"{item.Target.Table.Table.Name}: {item.Outcome}"),
    commandTimeout: 30,
    cancellationToken: cancellationToken);

var loadedTables = result.Definition.Tables;
var creationOutcomes = result.CopyResult.Items;
var warnings = result.CopyResult.Warnings;
```

The methods accept `connection`, `filename`, `tableExistenceBehavior`, `createdCallback`, `commandTimeout`, `traceKey`, `trace`, and `transaction`, in that order. Async methods add `cancellationToken`. The return type is `LoadDefinitionResult`: `Definition` preserves the complete file definition and `CopyResult` contains the destination outcomes, scripts, warnings, and source/target table metadata. The source endpoint uses the file's database/provider metadata and has no server address. If the file-read trace calls `Cancel(false)`, the loader returns null and performs no database operations.

Loads and `CopyTableSchemaTo` share the same destination checks, SQL planning, execution, and table completion reporting. `createdCallback` receives the same item instances returned in `CopyResult.Items`, including skipped and aligned tables, at the same completion points as a copy. The saved table/schema identities are preserved; the corresponding destination namespaces must be available as required by its composer. The cycle-aware orderer creates included parents before children, and each composer handles foreign-key staging as it does for copies. No additional relationship traversal is performed: the file's saved selection controls which tables exist in the input. `TableOnly` files omit foreign keys. External tables are not loaded; foreign keys or features the copy pipeline cannot create are reported through the existing warnings and destination table metadata. The original file definition remains available even when destination features differ.

`Skip` is the default. `Align` adds missing objects supported by the copy pipeline; `Throw` fails if a destination table already exists; `Drop` permanently removes existing tables and their data before recreating them. `commandTimeout`, `traceKey`, `trace`, and the optional destination `transaction` are forwarded to existence probes and DDL commands. Async methods use asynchronous database operations and trace callbacks. Callbacks remain synchronous, matching the copy API.

Transactions must belong to the destination connection. Loaders neither commit nor roll them back. Without a suitable transaction, command failures, callback failures, or cancellation can leave earlier statements applied. DDL transaction support and implicit commits depend on the database provider. Use the returned definition for the input metadata and the copy result for what actually happened in the destination.

The NativeAOT smoke test also exercises all six loader methods and completion callbacks for each saved relationship behavior, with reflection-based JSON serialization disabled. It roots the full schema core assembly for native compilation.

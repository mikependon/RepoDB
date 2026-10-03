## Target Operations

| Operation | Purpose |
|---|---|
| `CopyTo` | Copies the rows of a table (optionally filtered) from the source connection into a table on a destination connection, in batches. The source rows are left untouched. |
| `Deduplicate` | Removes duplicate rows from a table, where duplicates are rows that share the same values in the chosen key columns. One row is kept per group of duplicates. |
| `MoveTo` | Transfers the rows of a table (optionally filtered) from the source connection to a destination connection, in batches. Intended to remove the rows from the source once they are copied. *The current implementation copies only and does not delete from the source yet.* |

## Proposed Operations

Suggested additions, sorted alphabetically. The ones to build first are `SyncTo` / `Upsert`, `Compare` / `Diff` and `CreateTableFrom`.

| Operation | Category | Purpose |
|---|---|---|
| `Archive` | Sync | Copies old rows (for example by a date filter) to another database and then deletes them from the source. |
| `Checkpoint` / `Resume` | Workflow | Restarts a failed copy from the last completed batch. |
| `Compare` / `Diff` | Verification | Reports the rows that are missing, extra or different between two tables. |
| `CreateTableFrom` | Schema | Creates the destination table from the source table's schema before copying, so the table no longer has to exist first. |
| `DetectSchemaDrift` | Schema | Compares column definitions between two databases or environments. |
| `Mask` / `Anonymize` | Data quality | Hides sensitive columns while copying. Can be built as an `ICopyDataInterceptor`. |
| `Partition` | Workflow | Copies in parallel slices, for example by key range. |
| `Profile` | Data quality | Reports per-column null counts, distinct counts, minimum and maximum values, and the most frequent values. |
| `Reconcile` | Verification | Compares row counts and checksums as a cheap alternative to a full `Compare`. |
| `SyncTo` / `Upsert` | Sync | Copies rows to the destination and updates the ones that already exist, matched by key, so the copy is safe to re-run. |
| `ToCsv` / `ToJson` | Export | Exports table rows to flat files, alongside `ToDataTable` and `ToParquet`. |
| `ToDataTable` | Export | Loads the rows of a table (optionally filtered) into a `System.Data.DataTable`, for use with APIs that expect one, such as `SqlBulkCopy` or reporting tools. |
| `ToParquet` | Export | Exports table rows to a Parquet file, a compact columnar format suited to analytics and data lakes. Likely needs a Parquet library such as Parquet.Net. |
| `Validate` | Data quality | Checks rows against rules (required columns, types, ranges) and separates valid rows from rejected ones. |


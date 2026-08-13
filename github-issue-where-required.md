## User Story

**As a** developer using RepoDb's fluent operations (`Max`, `Min`, `Sum`, `Average`, `Count`, `BatchQuery`, `SkipQuery`)
**I want** the `where` argument to be a required parameter instead of an optional one defaulting to `null`
**So that** callers are forced to be explicit about their filter criteria and can't accidentally omit it and operate against an entire table unintentionally.

## Background

Several aggregate/filter operations under `RepoDb.Core/RepoDb/Operations/` currently declare `where` as an optional parameter (`where = null`). This is inconsistent with operations like `Delete`, `Exists`, `Query`, and `Update`, which already require `where` to be supplied explicitly. Standardizing on a required `where` closes that gap and reduces the risk of unfiltered aggregate calls.

## Scope of Change

Removed the `= null` default from every `where` parameter overload in the following operations, across all three call surfaces (`BaseRepository`, `DbConnection` extension methods, and `DbRepository`):

- `Max`
- `Average`
- `BatchQuery`
- `Count`
- `Min`
- `SkipQuery`
- `Sum`

Files touched (`RepoDb.Core/RepoDb/Operations/`):

| Operation | BaseRepository | DbConnection | DbRepository |
|---|---|---|---|
| Max | ✅ | ✅ | ✅ |
| Average | ✅ | ✅ | ✅ |
| BatchQuery | ✅ | ✅ | ✅ |
| Count | ✅ | ✅ | ✅ |
| Min | ✅ | ✅ | ✅ |
| SkipQuery | ✅ | ✅ | ✅ |
| Sum | ✅ | ✅ | ✅ |

No methods were removed or renamed — only the default value on the `where` parameter was dropped, making it a required argument on every overload (including internal `*Internal`/`*AsyncInternal` helpers).

## Out of Scope

- `Delete`, `Exists`, `Query`, `Update` — already require `where`, untouched.
- `*All` variants (`AverageAll`, `CountAll`, `DeleteAll`, `MaxAll`, `MergeAll`, `MinAll`, `SumAll`, `UpdateAll`), `Insert`, `InsertAll`, `Truncate`, `QueryMultiple` — no `where` parameter, untouched.
- Existing test suites — intentionally left unmodified.

## Breaking Change / Follow-up

This is a **breaking API change**. Any caller (application code, unit tests, integration tests) invoking one of the affected methods without an explicit `where` argument will fail to compile. A follow-up task is needed to update those call sites (notably under `RepoDb.Tests/`) before this can be merged/released.

## Acceptance Criteria

- [ ] `where` has no default value on all `Max`, `Average`, `BatchQuery`, `Count`, `Min`, `SkipQuery`, `Sum` overloads in `BaseRepository`, `DbConnection`, and `DbRepository`.
- [ ] No method signatures were removed.
- [ ] Solution compiles once dependent call sites are updated to pass `where` explicitly.
- [ ] Existing test suites remain functionally unchanged (aside from required compile fixes for the new required argument).

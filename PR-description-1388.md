# Fix DBNull conversion for object and reference-type results

Fixes https://github.com/mikependon/RepoDB/issues/1388

## Summary

Fix a regression introduced in 1.16.0 where converting `DBNull.Value` to `object` could throw `InvalidCastException`. This affected untyped inserts into tables without an identity/key to return and aggregate queries over empty sets. For inserts, the exception could occur after the row had already been written.

Both generic and non-generic `Converter.ToType` overloads now return `null` for `null` or `DBNull.Value` when the target is a reference type (including `object`) or a nullable value type, even with default conversion enabled. Callers no longer need to enable automatic conversion or select a nullable result overload to handle these valid null results.

## Changes

- Share the null-handling decision through `CanReturnDefault` and `IsNullableOrReferenceType`.
- Preserve `InvalidCastException` for null inputs targeting non-nullable value types under default conversion.
- Preserve automatic conversion behavior and the generic overload's forced automatic conversion path.

## Regression coverage

- Converter unit tests cover generic and non-generic overloads, null and DBNull inputs, object/reference/nullable targets, non-nullable value types and enums, automatic conversion, forced automatic conversion, and ordinary non-null conversions.
- PostgreSQL integration tests cover synchronous and asynchronous operations:
  - Inserts without an identity return `null` and persist the row.
  - `Max`, `Min`, `Sum`, and `Average<object>` over empty sets return `null`.
  - `Count` and `Exists` over empty sets return zero and `false`, respectively.
  - `InsertAll` without an identity returns the inserted count and persists all rows.
  - Null `tsvector` and `tsquery` properties round-trip through property handlers.
- Add an isolated keyless-table fixture and restore global conversion settings after tests.

## Validation

Description verified against the issue and the #1388 commits on `v1.17.1-hotfixes` through `76658550`. Tests were inspected, not executed as part of preparing this description.

## Scope

This description covers #1388. The branch also contains the separate SQL Server string-key output length fix for #1385, which is outside this description's scope.

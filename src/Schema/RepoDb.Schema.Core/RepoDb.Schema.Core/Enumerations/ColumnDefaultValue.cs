#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define what the default value of a column is, independent of the database that it is read from. The schema reader sets it
    /// when the schema is extracted (see <see cref="SchemaDefaultValue.Classify"/>), so the composer of another database can create the equivalent default value:
    /// <c>getdate()</c> of SQL Server, <c>now()</c> of PostgreSQL and <c>SYSDATE</c> of Oracle are all <see cref="CurrentTimestamp"/>.
    /// </summary>
    public enum ColumnDefaultValue
    {
        /// <summary>
        /// The default value is not classified (the schema reader does not set it): the default expression is used as it is.
        /// </summary>
        Unclassified = 0,

        /// <summary>
        /// The column has no default value.
        /// </summary>
        None,

        /// <summary>
        /// A number or a text, i.e.: <c>0</c>, <c>'N/A'</c>. The text of the literal is the default expression (without the parentheses and the type casts of the source database).
        /// </summary>
        Literal,

        /// <summary>
        /// The <c>NULL</c> value.
        /// </summary>
        Null,

        /// <summary>
        /// The boolean <c>TRUE</c>.
        /// </summary>
        BooleanTrue,

        /// <summary>
        /// The boolean <c>FALSE</c>.
        /// </summary>
        BooleanFalse,

        /// <summary>
        /// The current date and time of the server, i.e.: <c>getdate()</c>, <c>now()</c>, <c>CURRENT_TIMESTAMP</c>, <c>SYSDATE</c>.
        /// </summary>
        CurrentTimestamp,

        /// <summary>
        /// The current date and time in UTC, i.e.: <c>getutcdate()</c>, <c>sysutcdatetime()</c>, <c>utc_timestamp()</c>.
        /// </summary>
        CurrentTimestampUtc,

        /// <summary>
        /// The current date and time with the offset from UTC, i.e.: <c>sysdatetimeoffset()</c>, <c>SYSTIMESTAMP</c>.
        /// </summary>
        CurrentTimestampWithOffset,

        /// <summary>
        /// The current date, i.e.: <c>CURRENT_DATE</c>, <c>curdate()</c>, <c>CONVERT(date, getdate())</c>.
        /// </summary>
        CurrentDate,

        /// <summary>
        /// The current time, i.e.: <c>CURRENT_TIME</c>, <c>curtime()</c>.
        /// </summary>
        CurrentTime,

        /// <summary>
        /// A new random GUID (UUID), i.e.: <c>newid()</c>, <c>uuid()</c>, <c>gen_random_uuid()</c>, <c>SYS_GUID()</c>.
        /// </summary>
        NewGuid,

        /// <summary>
        /// A new sequential GUID, i.e.: <c>newsequentialid()</c>.
        /// </summary>
        NewSequentialGuid,

        /// <summary>
        /// A random number, i.e.: <c>rand()</c>, <c>random()</c>.
        /// </summary>
        RandomNumber,

        /// <summary>
        /// The user of the session, i.e.: <c>suser_sname()</c>, <c>CURRENT_USER</c>, <c>USER</c>.
        /// </summary>
        CurrentUser,

        /// <summary>
        /// The name of the current database, i.e.: <c>db_name()</c>, <c>database()</c>, <c>current_database()</c>.
        /// </summary>
        CurrentDatabase,

        /// <summary>
        /// The next value of a sequence, i.e.: <c>nextval('seq')</c>, <c>NEXT VALUE FOR seq</c>, <c>seq.NEXTVAL</c>.
        /// </summary>
        SequenceNextValue,

        /// <summary>
        /// An expression that has no function call, i.e.: <c>1 + 2</c>. It is only created in a database of the same provider as the source.
        /// </summary>
        Expression,

        /// <summary>
        /// A default value that can not be translated (a function that is not known, i.e.: <c>LAST_INSERT_ID()</c>, or a function of the user). It is only created in a database of the same provider as the source.
        /// </summary>
        Unsupported
    }
}

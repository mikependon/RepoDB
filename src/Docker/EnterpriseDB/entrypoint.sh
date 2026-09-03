#!/bin/sh
#
# Minimal entrypoint for docker.enterprisedb.com/k8s/edb-postgres-advanced.
#
# That image is a bare payload built for EDB's Kubernetes operator (CloudNativePG) - it ships with
# no self-initializing entrypoint the way the official postgres/mariadb images do (its default Cmd
# is just `/bin/bash`, with cluster bootstrap normally handled by the operator instead). This script
# fills that gap for local/dev use: initialize PGDATA on first run (creating the EDB_SUPERUSER role
# with EDB_SUPERUSER_PASSWORD, UTF8/C.utf8 encoding, and password auth open to any host), then always
# exec the edb-postgres server in the foreground.

set -eu

PGBIN=/usr/edb/as17/bin
export PGDATA="${PGDATA:-/home/postgres/pgdata}"

EDB_SUPERUSER="${EDB_SUPERUSER:-enterprisedb}"
: "${EDB_SUPERUSER_PASSWORD:?EDB_SUPERUSER_PASSWORD must be set}"

if [ ! -s "$PGDATA/PG_VERSION" ]; then
    echo "Initializing EDB Postgres Advanced Server data directory at $PGDATA"

    mkdir -p "$PGDATA"

    PWFILE=$(mktemp)
    printf '%s' "$EDB_SUPERUSER_PASSWORD" > "$PWFILE"
    "$PGBIN/initdb" \
        -D "$PGDATA" \
        -U "$EDB_SUPERUSER" \
        --pwfile="$PWFILE" \
        --auth=scram-sha-256 \
        --encoding=UTF8 \
        --locale=C.utf8 \
        --no-instructions
    rm -f "$PWFILE"

    echo "listen_addresses = '*'" >> "$PGDATA/postgresql.conf"
    echo "host all all 0.0.0.0/0 scram-sha-256" >> "$PGDATA/pg_hba.conf"
    echo "host all all ::0/0 scram-sha-256" >> "$PGDATA/pg_hba.conf"
fi

exec "$PGBIN/edb-postgres" -D "$PGDATA"

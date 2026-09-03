# EDB Postgres Advanced Server (EPAS) for local/dev use via docker-compose.
#
# The base image lives in EDB's private registry and requires an authenticated pull:
#   docker login docker.enterprisedb.com --username k8s --password "<your EDB subscription token>"
#
# It ships as a bare payload for EDB's Kubernetes operator (CloudNativePG), with no self-initializing
# entrypoint of its own - entrypoint.sh (below) fills that gap for a plain `docker compose up`.
FROM docker.enterprisedb.com/k8s/edb-postgres-advanced:17

COPY --chmod=0755 entrypoint.sh /usr/local/bin/edb-entrypoint.sh

EXPOSE 5444

ENTRYPOINT ["/usr/local/bin/edb-entrypoint.sh"]

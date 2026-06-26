#!/bin/sh
set -eu

for db in ambertower_auth ambertower_profile
do
  if ! psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname postgres -tAc "SELECT 1 FROM pg_database WHERE datname='${db}'" | grep -q 1
  then
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname postgres -c "CREATE DATABASE ${db};"
  fi
done

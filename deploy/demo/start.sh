#!/bin/sh
# Starts the demo's apps on local ports behind nginx (8080, the port Render routes to). The storefront creates the
# SQLite database first; the admin panel starts once it is listening, so the two never create it at the same time.
set -eu

export DOTNET_gcServer=0 DOTNET_GCConserveMemory=7
export Database__Provider=Sqlite ConnectionStrings__SqlServer="Data Source=/tmp/marketly.db"
export FileApi__BaseAddress=http://127.0.0.1:8083/ FileApi__ApiKey="$FILE_API_KEY"
mkdir -p /tmp/uploads

cd /app/files
ASPNETCORE_HTTP_PORTS=8083 Authentication__ApiKey="$FILE_API_KEY" FileStorage__PublicBaseUrl=/files/api/files \
  FileStorage__RootPath=/tmp/uploads dotnet App.FileApi.dll &

cd /app/web
ASPNETCORE_HTTP_PORTS=8081 dotnet App.Eticaret.dll &
web=$!
until bash -c 'exec 3<>/dev/tcp/127.0.0.1/8081' 2>/dev/null; do sleep 1; done

cd /app/admin
ASPNETCORE_HTTP_PORTS=8082 PathBase=/admin dotnet App.Admin.dll &
admin=$!

nginx -e stderr -c /app/nginx.conf -g 'daemon off;' &
proxy=$!

trap 'kill 0' TERM INT
wait "$web" "$admin" "$proxy"

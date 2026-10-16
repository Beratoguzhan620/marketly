# One image definition for the three apps; pick one with --build-arg PROJECT=App.Eticaret|App.Admin|App.FileApi.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT
WORKDIR /src

COPY Directory.Build.props Marketly.slnx ./
COPY App/ App/
RUN dotnet publish "App/${PROJECT}/${PROJECT}.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
ARG PROJECT
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    APP_DLL=${PROJECT}.dll

EXPOSE 8080

COPY --from=build /app/publish .

# Upload folder for the File API; harmless for the other two apps.
RUN mkdir -p /app/App_Data/uploads && chown -R "$APP_UID":"$APP_UID" /app/App_Data

USER $APP_UID

ENTRYPOINT ["sh", "-c", "exec dotnet \"$APP_DLL\""]

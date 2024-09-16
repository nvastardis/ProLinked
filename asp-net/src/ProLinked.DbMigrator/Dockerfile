FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ProLinked.DbMigrator/ProLinked.DbMigrator.csproj", "src/ProLinked.DbMigrator/"]
RUN dotnet restore "src/ProLinked.DbMigrator/ProLinked.DbMigrator.csproj"
COPY . .
WORKDIR "/src/src/ProLinked.DbMigrator"
RUN dotnet build "ProLinked.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProLinked.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProLinked.DbMigrator.dll"]

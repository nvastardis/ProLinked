FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIRONMENT=Staging
ENV ASPNETCORE_URLS=https://+:8081;http://+:8080
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=fdd45d7b-f9f6-431b-be3b-9e6d90a28e7d
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certs/certificate.pfx

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["asp-net/src/ProLinked.API/ProLinked.API.csproj", "asp-net/src/ProLinked.API/"]
RUN dotnet restore "asp-net/src/ProLinked.API/ProLinked.API.csproj"

WORKDIR /src
COPY . .
WORKDIR /src/asp-net/src/ProLinked.API/
RUN dotnet build "ProLinked.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProLinked.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

VOLUME ["/app/certs"]
ENTRYPOINT ["dotnet", "ProLinked.API.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["asp-net/src/ProLinked.API/ProLinked.API.csproj", "asp-net/src/ProLinked.API/"]
RUN dotnet restore "asp-net/src/ProLinked.API/ProLinked.API.csproj"
RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https -ep /dev-cert/certificate.pfx -p fdd45d7b-f9f6-431b-be3b-9e6d90a28e7d --trust


WORKDIR /src
COPY . .
WORKDIR /src/asp-net/src/ProLinked.API/
RUN dotnet build "ProLinked.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProLinked.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
COPY --chmod=0755 --from=build /dev-cert/* /dev-cert/
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProLinked.API.dll"]

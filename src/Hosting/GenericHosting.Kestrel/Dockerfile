FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Hosting/GenericHosting.Kestrel/GenericHosting.Kestrel.csproj", "src/Hosting/GenericHosting.Kestrel/"]
RUN dotnet restore "src/Hosting/GenericHosting.Kestrel/GenericHosting.Kestrel.csproj"
COPY . .
WORKDIR "/src/src/Hosting/GenericHosting.Kestrel"
RUN dotnet build "GenericHosting.Kestrel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GenericHosting.Kestrel.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GenericHosting.Kestrel.dll"]

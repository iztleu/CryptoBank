FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Hosting/GenericHosting/GenericHosting.csproj", "src/Hosting/GenericHosting/"]
RUN dotnet restore "src/Hosting/GenericHosting/GenericHosting.csproj"
COPY . .
WORKDIR "/src/src/Hosting/GenericHosting"
RUN dotnet build "GenericHosting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GenericHosting.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GenericHosting.dll"]

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CriptoBank/CriptoBank.csproj", "CriptoBank/"]
RUN dotnet restore "CriptoBank/CriptoBank.csproj"
COPY . .
WORKDIR "/src/CriptoBank"
RUN dotnet build "CriptoBank.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CriptoBank.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CriptoBank.dll"]

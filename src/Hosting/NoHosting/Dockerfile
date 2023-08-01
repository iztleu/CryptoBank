FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/NoHosting/NoHosting.csproj", "src/NoHosting/"]
RUN dotnet restore "src/NoHosting/NoHosting.csproj"
COPY . .
WORKDIR "/src/src/NoHosting"
RUN dotnet build "NoHosting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NoHosting.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NoHosting.dll"]

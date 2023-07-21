FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CryptoBank.WebAPI/CryptoBank.WebAPI.csproj", "CryptoBank.WebAPI/"]
RUN dotnet restore "CryptoBank.WebAPI/CryptoBank.WebAPI.csproj"
COPY . .
WORKDIR "/src/CryptoBank.WebAPI"
RUN dotnet build "CryptoBank.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoBank.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoBank.WebAPI.dll"]

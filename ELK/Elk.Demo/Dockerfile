FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["Elk.Demo.csproj", "./"]
RUN dotnet restore "./Elk.Demo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Elk.Demo.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Elk.Demo.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Elk.Demo.dll"]

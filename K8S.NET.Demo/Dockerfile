FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["K8s.NET.Demo.csproj", ""]
RUN dotnet restore "./K8s.NET.Demo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "K8s.NET.Demo.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "K8s.NET.Demo.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "K8s.NET.Demo.dll"]
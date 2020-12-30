FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["K8S.NETCore.ConfigMap.csproj", ""]
RUN dotnet restore "./K8S.NETCore.ConfigMap.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "K8S.NETCore.ConfigMap.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "K8S.NETCore.ConfigMap.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "K8S.NETCore.ConfigMap.dll"]
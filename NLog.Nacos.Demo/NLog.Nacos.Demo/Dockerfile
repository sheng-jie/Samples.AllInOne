#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NLog.Nacos.Demo/NLog.Nacos.Demo.csproj", "NLog.Nacos.Demo/"]
RUN dotnet restore "NLog.Nacos.Demo/NLog.Nacos.Demo.csproj"
COPY . .
WORKDIR "/src/NLog.Nacos.Demo"
RUN dotnet build "NLog.Nacos.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NLog.Nacos.Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NLog.Nacos.Demo.dll"]

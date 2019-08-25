FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
RUN apt update && \
    apt install unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Remote.Debug.csproj", ""]
RUN dotnet restore "Remote.Debug.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Remote.Debug.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Remote.Debug.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Remote.Debug.dll"]
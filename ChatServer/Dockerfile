#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY ["ChatServer/ChatServer.csproj", "ChatServer/"]
RUN dotnet restore "ChatServer/ChatServer.csproj" -r linux-musl-arm64

COPY . .
WORKDIR "/src/ChatServer"
RUN dotnet build "ChatServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatServer.csproj" -c Release -r linux-arm64 --self-contained false -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatServer.dll"]
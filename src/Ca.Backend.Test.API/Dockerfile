#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ca.Backend.Test.API/Ca.Backend.Test.API.csproj", "src/Ca.Backend.Test.API/"]
COPY ["src/Ca.Backend.Test.Infra.Data/Ca.Backend.Test.Infra.Data.csproj", "src/Ca.Backend.Test.Infra.Data/"]
COPY ["src/Ca.Backend.Test.Domain/Ca.Backend.Test.Domain.csproj", "src/Ca.Backend.Test.Domain/"]
COPY ["src/Ca.Backend.Test.Infra.IoC/Ca.Backend.Test.Infra.IoC.csproj", "src/Ca.Backend.Test.Infra.IoC/"]
COPY ["src/Ca.Backend.Test.Application/Ca.Backend.Test.Application.csproj", "src/Ca.Backend.Test.Application/"]
RUN dotnet restore "./src/Ca.Backend.Test.API/Ca.Backend.Test.API.csproj"
COPY . .
WORKDIR "/src/src/Ca.Backend.Test.API"
RUN dotnet build "./Ca.Backend.Test.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ca.Backend.Test.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ca.Backend.Test.API.dll"]
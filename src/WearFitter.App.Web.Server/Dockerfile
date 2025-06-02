# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
COPY ["src/WearFitter.App.Web.Server/WearFitter.App.Web.Server.csproj", "src/WearFitter.App.Web.Server/"]
COPY ["WearFitter.App.Repository.EFCore/WearFitter.App.Repository.EFCore.csproj", "WearFitter.App.Repository.EFCore/"]
COPY ["WearFitter.App.Domain/WearFitter.App.Domain.csproj", "WearFitter.App.Domain/"]
COPY ["WearFitter.Domain.Common/WearFitter.Domain.Common.csproj", "WearFitter.Domain.Common/"]
COPY ["WearFitter.EFCore.Common/WearFitter.EFCore.Common.csproj", "WearFitter.EFCore.Common/"]
COPY ["WearFitter.Services.Server/WearFitter.Services.Server.csproj", "WearFitter.Services.Server/"]
COPY ["WearFitter.Contracts/WearFitter.Contracts.csproj", "WearFitter.Contracts/"]
COPY ["src/WearFitter.App.Web.Client/WearFitter.App.Web.Client.csproj", "src/WearFitter.App.Web.Client/"]
COPY ["WearFitter.Services.Client/WearFitter.Services.Client.csproj", "WearFitter.Services.Client/"]
COPY ["WearFitter.Web.Infrastructure/WearFitter.Web.Infrastructure.csproj", "WearFitter.Web.Infrastructure/"]
RUN dotnet restore "./src/WearFitter.App.Web.Server/WearFitter.App.Web.Server.csproj"
COPY . .
WORKDIR "/src/src/WearFitter.App.Web.Server"
RUN dotnet build "./WearFitter.App.Web.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WearFitter.App.Web.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WearFitter.App.Web.Server.dll"]
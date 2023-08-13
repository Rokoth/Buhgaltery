#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Buhgaltery/Buhgaltery.csproj", "Buhgaltery/"]
COPY ["Buhgaltery.Contract/Buhgaltery.Contract.csproj", "Buhgaltery.Contract/"]
COPY ["Buhgaltery.Db/Buhgaltery.Db.csproj", "Buhgaltery.Db/"]
COPY ["Buhgaltery.Services/Buhgaltery.Services.csproj", "Buhgaltery.Services/"]
COPY ["Common/Buhgaltery.Common.csproj", "Common/"]
COPY ["Deployer/Deployer.csproj", "Deployer/"]
RUN dotnet restore "Buhgaltery/Buhgaltery.csproj"
COPY . .
WORKDIR "/src/Buhgaltery"
RUN dotnet build "Buhgaltery.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Buhgaltery.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Buhgaltery.Host.dll"]
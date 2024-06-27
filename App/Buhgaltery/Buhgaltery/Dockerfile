#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5770
EXPOSE 5771
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

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

ENV NODE_OPTIONS=--openssl-legacy-provider
RUN dotnet publish "Buhgaltery.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Buhgaltery.Host.dll"]
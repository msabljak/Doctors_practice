#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["EmailService/EmailService.csproj", "EmailService_build/"]
COPY ["EventStoreImplementation/EventStoreImplementation.csproj", "EventStoreImplementation_build/"]
RUN dotnet restore "EmailService_build/EmailService.csproj"
RUN dotnet restore "EventStoreImplementation_build/EventStoreImplementation.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "EmailService/EmailService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmailService/EmailService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailService.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Doctors_practice/Doctors_practice.csproj", "Doctors_practice_build/"]
COPY ["EventStoreImplementation/EventStoreImplementation.csproj", "EventStoreImplementation_build/"]
COPY ["AuthorisationLibrary/AuthorisationLibrary.csproj", "AuthorisationLibrary_build/"]
RUN dotnet restore "Doctors_practice_build/Doctors_practice.csproj"
RUN dotnet restore "EventStoreImplementation_build/EventStoreImplementation.csproj"
RUN dotnet restore "AuthorisationLibrary_build/AuthorisationLibrary.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "Doctors_practice/Doctors_practice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Doctors_practice/Doctors_practice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Doctors_practice.dll"]
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
COPY *.cs ./
RUN dotnet restore

RUN dotnet run replace.cs
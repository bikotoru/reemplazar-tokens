FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

ENV json=__json__
ENV prefijo=__prefijo__
ENV sufijo=__sufijo__
ENV path=__path__

# Copy csproj and restore as distinct layers
COPY *.csproj ./
COPY *.cs ./
RUN dotnet restore

RUN dotnet run replace.cs
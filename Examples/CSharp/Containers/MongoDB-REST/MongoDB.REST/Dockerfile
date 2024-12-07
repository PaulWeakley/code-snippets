# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MongoDB.REST.csproj", "./"]
RUN dotnet restore "./MongoDB.REST.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MongoDB.REST.csproj" -c Release -o /app/build
RUN dotnet publish "MongoDB.REST.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ARG OTEL_VERSION=1.4.0
ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v${OTEL_VERSION}/otel-dotnet-auto-install.sh otel-dotnet-auto-install.sh
RUN apt-get update && apt-get install -y curl unzip && \
    OTEL_DOTNET_AUTO_HOME="/otel-dotnet-auto" sh otel-dotnet-auto-install.sh && \
    chmod +x /otel-dotnet-auto/instrument.sh
ENV OTEL_DOTNET_AUTO_HOME="/otel-dotnet-auto"
ENTRYPOINT ["/otel-dotnet-auto/instrument.sh", "dotnet", "MongoDB.REST.dll"]

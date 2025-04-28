# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /opt/build

# Copy csproj file only and run dotnet restore on it. This improves build times as if the .csproj file doesn't change, all its packages do not have to be rebuilt each time
COPY *.csproj .
RUN dotnet restore --no-cache

# Copy everything else and build
COPY . .
RUN dotnet restore --no-cache && dotnet build && dotnet publish -c Release -o out

# Build runtime image. We're using aspnet rather than the SDK since it's smaller and we no longer need to compile source code
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /opt/koin/konnect-tpp

# Copy the compiled code from above into this container
COPY --from=build-env /opt/build/out .

# This allows the port 80 to be exposed to other services. Also used as a hint to other build processes as what ports this service listens on.
EXPOSE 80

# Set default environment variables. These need to be properly set at runtime to the proper database. Other environment variables that are required to be set
# should be added here as well for clarity of future people working on the project.
ENV DB_HOST=127.0.0.1 \
    DB_PORT=3306 \
    DB_NAME=konnect-tpp \
    DB_USER=root \
    DB_PASS= \
    KAFKA_ENABLE=N

# This is the command used to start the service
ENTRYPOINT ["dotnet", "mtdg-tpp.dll"] 


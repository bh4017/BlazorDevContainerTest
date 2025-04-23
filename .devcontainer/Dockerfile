FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

RUN apt-get update \
    && apt-get upgrade -y  \
    && apt-get -y install build-essential mariadb-client  \
    && apt-get autoremove -y \
    && apt-get clean -y
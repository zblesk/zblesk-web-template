# Build BE
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS dotnetbuild
WORKDIR /source

COPY *.sln .
COPY zblesk-web/*.csproj ./zblesk-web/
RUN dotnet restore -r linux-musl-x64

COPY zblesk-web/. ./zblesk-web/

WORKDIR /source/zblesk-web
RUN dotnet publish -c ReleaseNoNode -o /app --no-restore -r linux-musl-x64 --self-contained false

# Build FEs	
FROM node:16 AS nodebuild
WORKDIR /frontend
COPY zblesk-web/ClientApp/package*.json ./
RUN npm install
COPY zblesk-web/ClientApp/ .
RUN npm run build -- --prod

# final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine-amd64
WORKDIR /zblesk-web
COPY --from=dotnetbuild /app ./
COPY --from=nodebuild /frontend/dist ./ClientApp/dist

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8
RUN apk add --no-cache icu-libs
ENTRYPOINT ["./zblesk-web"]
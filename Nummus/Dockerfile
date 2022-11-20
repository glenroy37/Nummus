#----------------- Build Image
FROM mcr.microsoft.com/dotnet/sdk:7.0.100-alpine3.16-amd64 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "Nummus.csproj" -c Release -o /app/publish

#----------------- App Image
FROM mcr.microsoft.com/dotnet/aspnet:7.0.0-alpine3.16-amd64 AS base
ENV ASPNETCORE_ENVIRONMENT Docker
RUN apk add gettext
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
COPY ./docker-entrypoint.sh /app/docker-entrypoint.sh
ENTRYPOINT ["sh", "/app/docker-entrypoint.sh"]

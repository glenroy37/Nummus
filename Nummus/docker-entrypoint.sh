#!/bin/bash
envsubst </app/appsettings.Docker.template.json >/app/appsettings.Docker.json
ASPNETCORE_ENVIRONMENT=Docker dotnet /app/Nummus.dll
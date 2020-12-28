#!/bin/bash
envsubst </app/appsettings.Docker.template.json >/app/appsettings.Docker.json
dotnet /app/Nummus.dll
FROM --platform=arm64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .

RUN dotnet publish -c Release -o out

FROM --platform=arm64 mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "LoadTesterSample.dll"]
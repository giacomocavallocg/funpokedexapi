
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY FunPokedex.sln .
COPY FunPokedex.Api/FunPokedex.Api.csproj ./FunPokedex.Api/
COPY FunPokedex.Application/FunPokedex.Application.csproj ./FunPokedex.Application/
COPY FunPokedex.Test/*.csproj ./FunPokedex.Test/
RUN dotnet restore

COPY . .

RUN dotnet publish FunPokedex.Api/FunPokedex.Api.csproj -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
COPY --from=build /app .
ENTRYPOINT ["dotnet", "FunPokedex.Api.dll"]
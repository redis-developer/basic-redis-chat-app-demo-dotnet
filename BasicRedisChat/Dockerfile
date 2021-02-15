FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build

WORKDIR /app


COPY *.csproj ./

RUN dotnet restore


COPY . ./

WORKDIR /app


RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

WORKDIR /app

COPY --from=build /app/client/build ./client/build

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "BasicRedisChat.dll"]

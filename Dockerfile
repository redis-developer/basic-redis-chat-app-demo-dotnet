FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "BasicRedisChat/BasicRedisChat.csproj"
#COPY . .

WORKDIR "/src/BasicRedisChat"
RUN dotnet build "BasicRedisChat.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BasicRedisChat.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
#COPY --from=publish /app .
COPY --from=build /app/client/build ./client/build
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "BasicRedisChat.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ProductGraphQLService/ProductGraphQLService.csproj", "ProductGraphQLService/"]
RUN dotnet restore "ProductGraphQLService/ProductGraphQLService.csproj"
COPY . .
WORKDIR "/src/ProductGraphQLService"
RUN dotnet build "ProductGraphQLService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductGraphQLService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductGraphQLService.dll"]

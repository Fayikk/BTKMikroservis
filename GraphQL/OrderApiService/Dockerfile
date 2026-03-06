FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["OrderApiService/OrderApiService.csproj", "OrderApiService/"]
RUN dotnet restore "OrderApiService/OrderApiService.csproj"
COPY . .
WORKDIR "/src/OrderApiService"
RUN dotnet build "OrderApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderApiService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderApiService.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ApiGatewayService/ApiGatewayService.csproj", "ApiGatewayService/"]
RUN dotnet restore "ApiGatewayService/ApiGatewayService.csproj"
COPY . .
WORKDIR "/src/ApiGatewayService"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiGatewayService.dll"]
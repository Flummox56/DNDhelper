FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SheetService/SheetService.csproj", "SheetService/"]
RUN dotnet restore "SheetService/SheetService.csproj"
COPY . .
WORKDIR "/src/SheetService"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SheetService.dll"]
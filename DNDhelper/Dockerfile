FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DNDhelper/DNDhelper.csproj", "DNDhelper/"]
RUN dotnet restore "DNDhelper/DNDhelper.csproj"
COPY . .
WORKDIR "/src/DNDhelper"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DNDhelper.dll"]
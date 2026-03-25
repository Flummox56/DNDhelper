FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SceneService/SceneService.csproj", "SceneService/"]
RUN dotnet restore "SceneService/SceneService.csproj"
COPY . .
WORKDIR "/src/SceneService"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SceneService.dll"]
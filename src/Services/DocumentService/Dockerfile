FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/DocumentService/DocumentService.csproj", "src/Services/DocumentService/"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
RUN dotnet restore "src/Services/DocumentService/DocumentService.csproj"

COPY . .
WORKDIR "/src/src/Services/DocumentService"
RUN dotnet build "DocumentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocumentService.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/ProcessingService/ProcessingService.csproj", "src/Services/ProcessingService/"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
RUN dotnet restore "src/Services/ProcessingService/ProcessingService.csproj"

COPY . .
WORKDIR "/src/src/Services/ProcessingService"
RUN dotnet build "ProcessingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProcessingService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcessingService.dll"]

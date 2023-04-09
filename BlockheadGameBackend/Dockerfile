FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlockheadGameBackEnd/BlockheadGameBackEnd.fsproj", "BlockheadGameBackEnd/"]
RUN dotnet restore "BlockheadGameBackEnd/BlockheadGameBackEnd.fsproj"
COPY . .
WORKDIR "/src/BlockheadGameBackEnd"
RUN dotnet build "BlockheadGameBackEnd.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlockheadGameBackEnd.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlockheadGameBackEnd.dll"]

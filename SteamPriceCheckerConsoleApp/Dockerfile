FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Minden .csproj fájlt másoljon át a jelenlegi mappába
COPY *.csproj ./
RUN dotnet restore

# Másoljuk a többi fájlt is
COPY . .

# Itt a kimeneti nevet fixáljuk
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .

# FIGYELEM: Itt a .dll neve meg kell egyezzen a projekted nevével!
# Ha a projekted neve "Valami", akkor Valami.dll kell ide.
ENTRYPOINT ["dotnet", "SteamPriceCheckerConsoleApp.dll"]


# Giai đoạn 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["Odata_Demo/Odata_Demo.csproj", "Odata_Demo/"]
RUN dotnet restore "Odata_Demo/Odata_Demo.csproj"
COPY . .
WORKDIR "/src/Odata_Demo"
RUN dotnet build "Odata_Demo.csproj" -c Release -o /app/build

# Giai đoạn 2: Publish
FROM build AS publish
RUN dotnet publish "Odata_Demo.csproj" -c Release -o /app/publish

# Giai đoạn 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Odata_Demo.dll"]

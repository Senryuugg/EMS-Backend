FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EmsDispatch.Backend/EmsDispatch.Backend.csproj", "EmsDispatch.Backend/"]
RUN dotnet restore "EmsDispatch.Backend/EmsDispatch.Backend.csproj"

COPY . .
WORKDIR "/src/EmsDispatch.Backend"
RUN dotnet build "EmsDispatch.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmsDispatch.Backend.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "EmsDispatch.Backend.dll"]

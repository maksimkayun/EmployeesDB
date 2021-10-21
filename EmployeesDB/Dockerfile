FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EmployeesDB/EmployeesDB.csproj", "EmployeesDB/"]
RUN dotnet restore "EmployeesDB/EmployeesDB.csproj"
COPY . .
WORKDIR "/src/EmployeesDB"
RUN dotnet build "EmployeesDB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeesDB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeesDB.dll"]

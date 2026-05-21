# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Project files copy karo
COPY ["HospitalBedTracker/HospitalBedTracker.csproj", "HospitalBedTracker/"]
COPY ["HospitalBedTracker.Application/HospitalBedTracker.Application.csproj", "HospitalBedTracker.Application/"]
COPY ["HospitalBedTracker.Domain/HospitalBedTracker.Domain.csproj", "HospitalBedTracker.Domain/"]
COPY ["HospitalBedTracker.Infrastructure/HospitalBedTracker.Infrastructure.csproj", "HospitalBedTracker.Infrastructure/"]

# Restore
RUN dotnet restore "HospitalBedTracker/HospitalBedTracker.csproj"

# Code copy karo
COPY . .

# Build
WORKDIR "/src/HospitalBedTracker"
RUN dotnet publish "HospitalBedTracker.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HospitalBedTracker.dll"]
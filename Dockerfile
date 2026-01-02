# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["AI.OfferService.Api/AI.OfferService.Api.csproj", "AI.OfferService.Api/"]
COPY ["AI.OfferService.Domain/AI.OfferService.Domain.csproj", "AI.OfferService.Domain/"]
COPY ["AI.OfferService.Application/AI.OfferService.Application.csproj", "AI.OfferService.Application/"]
RUN dotnet restore "AI.OfferService.Api/AI.OfferService.Api.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/AI.OfferService.Api"

# Build and publish the application
RUN dotnet build "AI.OfferService.Api.csproj" -c Release -o /app/build
RUN dotnet publish "AI.OfferService.Api.csproj" -c Release -o /app/publish

# Use the official .NET runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "AI.OfferService.Api.dll"]
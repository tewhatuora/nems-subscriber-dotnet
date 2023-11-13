FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY source/ ./
# Restore as distinct layers
RUN dotnet restore GuaranteedSubscriber.csproj
# Build and publish a release
RUN dotnet publish GuaranteedSubscriber.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .

# Finally bring in properties
COPY properties.json ./
ENTRYPOINT ["dotnet", "GuaranteedSubscriber.dll"]
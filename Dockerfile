FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base 
WORKDIR /app 
EXPOSE 80 
 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
WORKDIR /src 
COPY . . 
RUN dotnet restore "SafeNet.Web/SafeNet.Web.csproj" 
RUN dotnet publish "SafeNet.Web/SafeNet.Web.csproj" -c Release -o /app/out 
 
FROM base AS final 
WORKDIR /app 
COPY --from=build /app/out . 
ENTRYPOINT ["dotnet", "SafeNet.Web.dll"]

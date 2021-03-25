#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["TLSpammer.WEB/TLSpammer.WEB.csproj", "TLSpammer.WEB/"]
RUN dotnet restore "TLSpammer.WEB/TLSpammer.WEB.csproj"
COPY . .
WORKDIR "/src/TLSpammer.WEB"
RUN dotnet build "TLSpammer.WEB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TLSpammer.WEB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TLSpammer.WEB.dll"]
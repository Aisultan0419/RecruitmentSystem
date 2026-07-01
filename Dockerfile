FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["RecruitmentSystemAPI/RecruitmentSystemAPI.csproj", "RecruitmentSystemAPI/"]
COPY ["RecruitmentSystemApplication/RecruitmentSystemApplication.csproj", "RecruitmentSystemApplication/"]
COPY ["RecruitmentSystemInfrastructure/RecruitmentSystemInfrastructure.csproj", "RecruitmentSystemInfrastructure/"]
COPY ["RecruitmentSystemDomain/RecruitmentSystemDomain.csproj", "RecruitmentSystemDomain/"]


RUN dotnet restore "RecruitmentSystemAPI/RecruitmentSystemAPI.csproj"

COPY . .

WORKDIR "/src/RecruitmentSystemAPI"
RUN dotnet build "RecruitmentSystemAPI.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "RecruitmentSystemAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecruitmentSystemAPI.dll"]
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["StudentAssistant.Domain/StudentAssistant.Domain.csproj", "StudentAssistant.Domain/"]
COPY ["StudentAssistant.Data/StudentAssistant.Data.csproj", "StudentAssistant.Data/"]
COPY ["StudentAssistant.Service/StudentAssistant.Service.csproj", "StudentAssistant.Service/"]
COPY ["StudentAssistant.Bot/StudentAssistant.Bot.csproj", "StudentAssistant.Bot/"]
COPY ["StudentAssistant.WebApi/StudentAssistant.WebApi.csproj", "StudentAssistant.WebApi/"]

RUN dotnet restore "StudentAssistant.Bot/StudentAssistant.Bot.csproj"

COPY . .
WORKDIR "/src/StudentAssistant.Bot"
RUN dotnet build "StudentAssistant.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudentAssistant.Bot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentAssistant.Bot.dll"]

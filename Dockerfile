FROM microsoft/dotnet:2.2-sdk AS build

# copy csproj and restore as distinct layers
WORKDIR /system
COPY . .
RUN dotnet restore
RUN dotnet build

FROM build AS publish
WORKDIR /system
RUN dotnet publish -c Release -o bin/publish /p:LinkDuringPublish=true

FROM microsoft/dotnet:2.2-runtime AS runtime

LABEL Author="Vadim Kosin <vkosin@outlook.com>"
WORKDIR /system
COPY --from=publish /system/ThinkingHome.Console/bin/publish .

# db connection string
ENV plugins:ThinkingHome.Plugins.Database.DatabasePlugin:connectionString host=postgres;port=5432;database=postgres;user name=postgres;password=123
     
# пробрасываем из контейнера порт 8080
EXPOSE 8080

# при старте контейнера поднимаем наше приложение
CMD ["./ThinkingHome.Console"]
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build

# copy csproj and restore as distinct layers
WORKDIR /system
COPY . .
RUN dotnet publish -c Release -o bin/publish /p:LinkDuringPublish=true

FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS runtime

LABEL Author="Vadim Kosin <vkosin@outlook.com>"
WORKDIR /system
COPY --from=build /system/bin/publish .

# db connection string
ENV plugins:ThinkingHome.Plugins.Database.DatabasePlugin:connectionString host=postgres;port=5432;database=postgres;user name=postgres;password=123
     
EXPOSE 8080

CMD ["./ThinkingHome.Console"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.



FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# let's assume now we are in "rootDir" of the  container which are running docker build 
# command:  docker build -f "Test.App\Dockerfile" --force-rm -t testapp:dev --target base .

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# docker file is in /rootDir/Test.App/DockerFile, however SRC context is set to /rootDir/
# copy file /rootDir/Test.App/Test.App.csproj to src/Test.App/ of the "build" image
COPY ["Test.App/Test.App.csproj", "Test.App/"]

# restore project dependencies (creates obj and bin directory with some files)  to src/Test.App/bin
RUN dotnet restore "Test.App/Test.App.csproj"

# copy all /rootDir directory to the build image /src directory
# as result we have src/Test.App/ with all intial context abd bin and obj folders
COPY . .

# now we inside directory where Test.App.csproj in the build image
WORKDIR "/src/Test.App"

# build applciation - create app/build fodler - actually command can be skipped
RUN dotnet build "Test.App.csproj" -c Release -o /app/build

FROM build AS publish
# pack application to the executable artifact
RUN dotnet publish "Test.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Prepare dotnet Test.App.dll command to run web server on the final image
ENTRYPOINT ["dotnet", "Test.App.dll"]

# run command: docker run -dt -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=https://+:443;http://+:80" -P --name Test.App --entrypoint tail testapp:dev -f /dev/null

# postgres env variables: connection__host   connection__database   connection__username  connection__password 
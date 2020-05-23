FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build
WORKDIR /app
COPY ./XUnitTestRW.TEST/*.csproj ./XUnitTestRW.TEST/
COPY ./XUnitTest.WEB/*.csproj ./XUnitTest.WEB/
COPY *.sln . 
RUN dotnet restore
COPY . .
RUN dotnet test ./XUnitTestRW.TEST/*.csproj
RUN dotnet publish ./XUnitTest.WEB/*.csproj -o /publish/
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT ["dotnet","XUnitTest.WEB.dll"]
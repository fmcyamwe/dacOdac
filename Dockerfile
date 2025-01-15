#Build Neo4j
## does not create two separate containers so pulled in yaml file

#FROM neo4j

#ENV NEOHOME "/Users/florentcyamweshi/Library/Application Support/Neo4j Desktop/Application/relate-data/dbmss/dbms-3b43d9ab-43ef-41b9-9ef4-d3b3ce0cce29"

#ENV NEO4J_AUTH=none
#EXPOSE 7474
#EXPOSE 7687

#WORKDIR /app

#CMD ["neo4j"]

#Build API

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .

#think restore only needed once? seems better to run twice
WORKDIR "/app/Dac.Neo"
RUN dotnet restore
RUN dotnet build "./Dac.Neo.csproj" -c Release -o /app/build


WORKDIR "/app/Dac.API"
RUN dotnet restore
RUN dotnet build "./Dac.API.csproj" -c Release -o /app/build

FROM build AS publish
#weirdly cant find Dac.Neo..cause of workdir? >>nope...umm  well seems good with just line below
# should copy those json here too? COPY --from=build /app/seedDoctor.json .
RUN dotnet publish -c Release -o /app/publish
#COPY *.json /app/publish # nope moot after RUN cmd smh

#RUN dotnet publish "./Dac.Neo.csproj" -c Release -o /app/publish
#RUN dotnet publish "./Dac.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#EXPOSE 5113
#EXPOSE 7687
#EXPOSE 7474
#CMD ["dotnet", "Dac.Neo.dll"]
ENTRYPOINT ["dotnet", "Dac.API.dll"]
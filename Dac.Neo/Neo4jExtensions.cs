using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Dac.Neo.Repositories;
using Dac.Neo.Data;

using Neo4j.Driver;

namespace Dac.Neo;


public static class Neo4jExtensions
{
    public static IServiceCollection ConfigureNeo4jService(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        //umm settings from calling site...
        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
        var settings = new ApplicationSettings();
        configuration.GetSection("ApplicationSettings").Bind(settings);

        //services.AddSingleton(GraphDatabase.Driver(
        //    settings.Neo4jConnection, 
        //    AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)
        //));
        

        var driver = GraphDatabase.Driver(//Driver(uri, AuthTokens.Basic(user, password));
            settings.Neo4jConnection, 
            AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword));
        
        services.AddSingleton(driver);

        //verify connection...could throw error so should surround with try/catch prolly... 
        //>>so far no issue(even in docker!) >>oh prolly have to await result LOL
        driver.VerifyConnectivityAsync();

        //Data Access Wrapper over Neo4j session, 
        //that is a helper class for executing parameterized Neo4j Cypher queries in Transactions
        services.AddScoped<INeo4jDataAccess, Neo4jDataAccess>();
        
        //registration for domain repository class
        services.AddTransient<IPatientRepository, PatientRepository>();

        //todo**n add for DoctorRepository too(although it will prolly be small)

        services.AddTransient<ISeeder, SeedGraphDB>();

        return services;
    }
}

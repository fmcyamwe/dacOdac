﻿using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

using Dac.Neo.Data.Configurations;
using Dac.Neo.Repositories;



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
        
        Console.WriteLine("ConfigureNeo4jService:: {0}-{1}-{2}",settings.Neo4jConnection,settings.Neo4jUser, settings.Neo4jPassword);

        var driver = GraphDatabase.Driver(
            settings.Neo4jConnection, 
            AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword));
        
        //verify connection...
        try
        {
            driver.VerifyConnectivityAsync();
        }catch(Exception e) {
            Console.WriteLine("ConfigureNeo4jService:: EXCEPTION VerifyConnectivityAsync >> {0}",e);
            //driver.CloseAsync(); //--todo** //driver.CloseAsync()
        }
       

        services.AddSingleton(driver); 

        //services.AddScoped(typeof(INeo4jDataAccess).GetTypeInfo().Assembly);
        //services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

        //Data Access Wrapper over Neo4j session, 
        //helper class for executing parameterized Neo4j Cypher queries in Transactions
        services.AddScoped<INeo4jDataAccess, Neo4jDataAccess>();

        services.AddScoped<ISeeder, Seeder>(); 
   
        
        services.AddTransient<IPatientRepository, PatientRepository>();
        services.AddTransient<IDoctorRepository, DoctorRepository>();
        
        return services;
    }

    public static async void CheckMigration(IServiceProvider sp) //IHostEnvironment hEnv
    {
       
        var sd = sp.GetService<ISeeder>();
        if (sd == null){
            Console.WriteLine("CheckMigration::Seeder == null>> ERROR!! Aborting :(...");
            return;
        }//else{Console.WriteLine("CheckMigration::Seeder GOOD {0}",sd);} 

        //Console.WriteLine("CheckMigration::Seeder GOOD {0} >> {1} >> {2}",sd,hEnv.ContentRootPath,hEnv.EnvironmentName);
        ////>>wrong >>  /Users/florentcyamweshi/dacOdac/Dac.API >> Development
        //string currentDir = Environment.CurrentDirectory;

        //Console.WriteLine("CheckMigration::Seeder GOOD {0} ",currentDir);
        //smdh also shows >>/Users/florentcyamweshi/dacOdac/Dac.API
        ///Users/florentcyamweshi/dacOdac/Dac.Neo/Data/Configurations/seedDoctor.json

        //var sourcePath = Path.Combine(contentRootPath, "Data", "Configurations","seedPatient.json");
        
        bool populated = await sd.AlreadyPopulated(); 
        
        if(populated){
           return;
        }

        await sd.CreatePatientNodeConstraints();

        await sd.CreateDoctorNodeConstraints();
                
        //seed graphDB with initial data...--toReview** use configuration Path instead of hardcoding
        var e = await sd.SeedPatientData("/Users/florentcyamweshi/dacOdac/Dac.Neo/Data/Configurations/seedPatient.json");
        var d = await sd.SeedDoctorData("/Users/florentcyamweshi/dacOdac/Dac.Neo/Data/Configurations/seedDoctor.json");
            
        ///also add some relationships?!? toReview**
       return;
    }
}

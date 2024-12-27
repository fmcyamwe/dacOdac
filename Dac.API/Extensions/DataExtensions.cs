using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Dac.Neo;
using Dac.API.Repositories;

namespace Dac.API.Extensions;

public static class DataExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder) //removed async
    {
        
        var services = builder.ConfigureNeo4jService();
        
        //builder.ConfigureNeo4jService();

        //auth middleware could go here prolly

        
        //huh see if below works! >>nope ...might be cause of multi project?!?
        /*services.Add(ServiceDescriptor.Describe(
        serviceType: typeof(INeo4jDataAccess),
        //implementationFactory: static _ =>  new Neo4jDataAccess(),//builder.GetRequiredService(typeof(ILogger<Neo4jDataAccess>)); IDriver,ILogger<Neo4jDataAccess>),
        implementationType: typeof(Neo4jDataAccess),
        lifetime: ServiceLifetime.Scoped)); //bon still scoped(dont make sense to make it a Singleton --and even borks as singleton)
        */
        
        //registration for domain repository classes
        services.AddTransient<IPatientRepository, PatientRepository>();
        services.AddTransient<IDoctorRepository, DoctorRepository>();

        /*services.Configure<ApiBehaviorOptions>(options =>{
            //suppress default validation that returns badRequest...toTest**
            options.SuppressModelStateInvalidFilter = true; // was true...
            //toSee if was cause of >> InvalidOperationException: Each parameter in the deserialization constructor on type 'Dac.Neo.Model.Patient' must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object.
            ///nope still error out!! remove hust in case for testing
        });*/

        /*services.Configure<JsonOptions>(options => //serieux...just to deserialize properly smh
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.IncludeFields = true; //false by default >>no change in both case
            
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; //overriden by Model's annotations

        });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //option.SerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.IncludeFields = true; //nope smh
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });*/

        //huh could use to handle Exceptions? //toDo**
        services.AddControllers().AddNewtonsoftJson();  //sheesh!! just to properly deserialize json 
        //Could add below for 200 globally? >>todo**
        //services.AddControllers(options => { //huh global for all endpoints...umm prolly overkill for some endpoints AND can be set on each endpoint!! >>bon removing!!
        //    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 400)); //add badRequest globally --see above--toTest**
        //    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 500)); //for exceptions handling with UseExceptionHandler() 
        //});
        
        
        var sp = services.BuildServiceProvider(); 

        //seed Graph DB
        Neo4jExtensions.CheckMigration(sp);
    }

}
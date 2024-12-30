using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Dac.Neo;
using Dac.API.Services;


namespace Dac.API.Extensions;


public static class DataExtensions
{
    
    public static void AddApplicationServices(this IHostApplicationBuilder builder) //removed async
    {
        
        var services = builder.ConfigureNeo4jService();

        //auth middleware could go here prolly
        services.AddProblemDetails(); //toSee** for Exceptions

        //services.AddAntiforgery(); //todo** Configure
        
        
        services.AddScoped<IApiManagerService, ApiManagerService>();

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
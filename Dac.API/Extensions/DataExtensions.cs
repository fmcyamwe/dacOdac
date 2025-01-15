using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Dac.Neo;
using Dac.API.Services;
using Dac.API.Constants;
using Dac.API.Model;

namespace Dac.API.Extensions;


public static class DataExtensions
{
    
    public static async void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        
        var services = builder.ConfigureNeo4jService();

        //auth middleware could go here prolly
        services.AddProblemDetails(); //toSee** for Exceptions
        

        //services.AddAntiforgery(); //todo** Configure
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
    
        // Add authentication services
        var key = Encoding.UTF8.GetBytes(AuthorizationConstants.JWT_SECRET_KEY); 
       
        services.AddAuthentication(options =>
        {
            //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                //ValidateIssuer = true,
                //ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "your-issuer",
                ValidAudience = "your-audience",
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };
        });
        
        services.AddScoped<IApiManagerService, ApiManagerService>();

        services.AddAuthorization();

        //huh could use to handle Exceptions? //toDo**
        services.AddControllers().AddNewtonsoftJson();  //sheesh!! just to properly deserialize json 

        //Could add below for 200 globally? >>todo**
        //services.AddControllers(options => { //huh global for all endpoints...umm prolly overkill for some endpoints AND can be set on each endpoint!! >>bon removing!!
        //    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 400)); //add badRequest globally --see above--toTest**
        //    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 500)); //for exceptions handling with UseExceptionHandler() 
        //});
        
        
        var sp = services.BuildServiceProvider(); 
        
        //seed Graph DB
        Neo4jExtensions.CheckMigrations(sp);
    }

    public static IEndpointRouteBuilder MapApiV1(this IEndpointRouteBuilder app)
    {
        Console.WriteLine("WOAH:: in MapApiV1!!"); //umm should use?

        /*app.MapGet("/products", () => new[] { "Product1", "Product2" });
        app.MapPost("/products", (PatientRequest product) =>
        {
            return Results.Created($"/products/{product.Action}", product);
        });*/

        return app;
    }

}
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
        //needed?!? nope dont seem so? >>umm for swagger? nope >> dont seem to help
        /*services.AddSwaggerGen(c => {

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //c.EnableAnnotations();
            //c.SchemaFilter<CustomSchemaFilters>();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                            Enter 'Bearer' [space] and then your token in the text input below.
                            \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
            });
        });*/
    
        // Add authentication services
        var key = Encoding.UTF8.GetBytes(AuthorizationConstants.JWT_SECRET_KEY); //Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY); //
        /*
        services.AddAuthentication(config =>
        {
            config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(config =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });*/
        
        // Add authentication services--chatty 
        ///>>prolly better defined than above with those flags for Valid?!? nope
        services.AddAuthentication(options =>
        {
            //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //or should use explicit .Equals ?
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
                IssuerSigningKey = new SymmetricSecurityKey(key),//new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthorizationConstants.JWT_SECRET_KEY)) //"your-secret-key"
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
        Neo4jExtensions.CheckMigration(sp); //todo** await
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
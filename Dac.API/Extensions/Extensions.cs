using Dac.Neo;

using Dac.Neo.Model;
using Dac.Neo.Repositories;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        //var services = builder.Services;

        builder.ConfigureNeo4jService();


        //services.AddControllers(); //yup needed >>done at call site

        //auth middleware could go here prolly
        
    }

    //just for testing getting instance of Repository but nope---toRemove**
    public static IApplicationBuilder AddControllers(this WebApplication app)
    {
        app.UseRouting(); //needed for routing

        var e = app.Services.GetService<IPatientRepository>(); //error System.InvalidOperationException: Cannot resolve 'Dac.Neo.Repositories.IPatientRepository' from root provider because it requires scoped service 'Dac.Neo.Repositories.INeo4jDataAccess'.

        //var p = new PatientListEndpoint(e);

        //todo** use MapGet
        //app.UseEndpoints(endpoints => { p.AddRoute(endpoints);}); //oldie that worked >> endpoints.MapControllers(); 

        //huh could do below here too!! + add more info 
        /*app.MapGet("/weatherforecast", () =>{
            //something
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi(); */
        
        //todo** test that using MapGet would work with IPatientRepository in class!!
        
        //app.MapOpenApi(); >>nope


        var api = app.MapGroup("api"); //prolly patients for group?
/*
        //routes
        api.MapGet("/patients", GetAllPatients)
            .WithName("ListPatients")
            .WithSummary("List Patients")
            .WithDescription("Get a list of all patients.")
            .WithTags("Patients");
        api.MapGet("/patients/{id}", GetPatientById) //umm need to specify type for id?!? i.e /{id:int}
            .WithName("GetPatient")
            .WithSummary("Patient info")
            .WithDescription("View Single Patient info")
            .WithTags("Patients"); //umm same tag?
            */
        
        return app;
    }


}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Dac.Neo.Model;
using Dac.Neo.Repositories;

namespace Dac.Api.Services;

public class EndpointConfigurator  //toRemove --failed test for setting endpoints
{
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;
    //private readonly IPatientRepository _patientRepository;

    public EndpointConfigurator(IEndpointRouteBuilder endpointRouteBuilder)//,IPatientRepository patientRepository)
    {
        _endpointRouteBuilder = endpointRouteBuilder;
        //_patientRepository = patientRepository;
    }

    public void ConfigureEndpoints()
    {
        // Use _endpointRouteBuilder to configure your endpoints
        Console.WriteLine("coliiis...ConfigureEndpoints");
        _endpointRouteBuilder.MapGet("/api/data", GetData); //, [ApiController] => async context =>
        //{
        //  endpoint logic here
        //});
    }
    
    // This method gets called by the runtime. Used to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      //if (env.IsDevelopment()){
      //  app.UseDeveloperExceptionPage();
      //}
      Console.WriteLine("coliiis...CONFIGURE");
        
      app.UseDefaultFiles();
      app.UseStaticFiles();

      //app.UseRouting(); //yup for routes
        
      

      app.UseHttpsRedirection(); //for swagger? toSee but doesnt seem like I can add it :(  >>also complains as no redirection found 

      //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

     ///
     var endpointConfigurator = app.ApplicationServices.GetService<EndpointConfigurator>();
     endpointConfigurator.ConfigureEndpoints();

     app.UseRouting();
     app.UseEndpoints(endpoints =>
    {
        // endpoint configurations will be applied here
    });
     
    }
    public static void  GetData(){//Task<long> 
        return; //_patientRepository.GetPatientCount();
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;


using Dac.API.Services;

namespace Dac.API.Controllers.Patients;

//[Route("patients")]
//[ApiController]
public class ListPatients : BaseController // ControllerBase
{
    public ListPatients(IApiManagerService apiService) : base(apiService)
    {}

    //GET patients/ 
    [Route("patients")] //  >> //[EndpointName("patients")] must be unique--huh
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Get patients")]
    [EndpointDescription("Get list of patients")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //[Authorize]
    [Authorize(Roles = "admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Accessible only to Admin role
    //--seem needed as default authorize above fails :( ..or should just set Roles = "" ? toTry**
    public async Task<Ok<List<Dictionary<string, object>>>> GetAllPatients() //List<Patient>  //IResult //Task<ActionResult<bool>>
    {
        
        //this actually is the minimum info for Patients (doesnt include Treatment,etc) <would need auth for extra details>
        
        var p = await _apiService.GetAllPatients(); //_patientRepository
        return TypedResults.Ok(p); 

        //Results.Ok();//return TypedResults.Ok();
        //return (Task<IResult>)Results.Ok(true); 
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }
    
}
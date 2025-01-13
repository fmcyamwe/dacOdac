using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Services;
using Dac.API.Model;

namespace Dac.API.Controllers;


[Route("doctors")]
public class DoctorController : BaseController //ControllerBase
{
    //private readonly IDoctorRepository _doctorRepository;
    //private readonly IApiManagerService _apiService; 
    public DoctorController(IApiManagerService apiService) : base(apiService) 
    {
        //_doctorRepository = doctorRepository;
        //_apiService = apiService;
    }

    // GET doctors/{id}/patients
    [Route("{id}/patients")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("Doctor's Patients")]
    [EndpointDescription("Get patients Doctor in charge of")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    [Authorize] //toTest* when enabled >> limit access to authenticated users for that controller or action.
    //[Authorize(Roles = "Admin")]  // Accessible only to Admin role
    public async Task<Ok<List<Dictionary<string, object>>>> GetDoctorPatients([FromRoute] string id)
    {
        //not to be confused with >> GET doctors/{id}/patients/   
        
        //TypedResults.Ok(); 
        //todo** use TypedResults<IResult> for ease of unit tests
        var res = await _apiService.GetDoctorPatients(id);

        return TypedResults.Ok(res); 
    }
    

    //Get doctors/{id}/requests
    [Route("{id}/requests")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("Doctor Pending Requests")]
    [EndpointDescription("List of Requests from patients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //[AllowAnonymous] //authorization
    public async Task<List<Dictionary<string, object>>> GetDoctorPendingRequests([FromRoute] string id)
    {
        return await _apiService.GetPatientRequests(id);
    }

    [Route("{id}/requests")]
    [HttpPost]
    [Tags("Doctors")]
    [EndpointSummary("Doctor response on Requests")]
    [EndpointDescription("Response to pending patient's Request")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //[AllowAnonymous] //authorization
    public async Task<IResult> UpdatePatientRequest([FromRoute] string id, [FromBody] DoctorActionResponse resp)
    { //need async for to return Task<IResult> estiiii

        //todo** check that resp.PatientId exist!! 
        if (resp == null || resp.Action.Length == 0 || resp.Status.Length == 0)
        {
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }
        //should pass in resp.PatientId ? toSee**
        var result = await _apiService.UpdatePatientRequest(id,resp.Action,resp.Status);

        //todo** check result
        _apiService.GetLogger().LogInformation("UpdatePatientRequest :: {patientID} > {result} ", resp.PatientId, result);
            
        return Results.NoContent(); //umm 204?

    }

    //GET doctors/count
    [Route("count")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("Doctors count")]
    [EndpointDescription("Get current Doctors count")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetCurrentDoctorsCount()
    {
        var c = await _apiService.GetDoctorsCount();
        return TypedResults.Ok(c);
    }
}
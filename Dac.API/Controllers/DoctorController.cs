using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Services;
using Dac.API.Model;

namespace Dac.API.Controllers;


[Route("doctors")]
public class DoctorController : BaseController
{
    //private readonly IDoctorRepository _doctorRepository;
    //private readonly IApiManagerService _apiService; 
    public DoctorController(IApiManagerService apiService) : base(apiService) 
    {}

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
    public async Task<IResult> GetDoctorPatients([FromRoute] string id)
    {//Task<Ok<List<Dictionary<string, object>>>>

        if(id == null || string.IsNullOrWhiteSpace(id)){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Id is not valid"
            });
        }

        return TypedResults.Ok(await _apiService.GetDoctorPatients(id));
    }
    

    //Get doctors/{id}/requests
    [Route("{id}/requests")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("Doctor's Pending Requests")]
    [EndpointDescription("List of Requests from patients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //[AllowAnonymous] //authorization
    public async Task<IResult> FetchPendingRequests([FromRoute] string id)
    {//Task<List<Dictionary<string, object>>> 
        return TypedResults.Ok(await _apiService.GetPendingRequests(id));
    }

    //Post doctors/{id}/requests
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

       
        if (string.IsNullOrWhiteSpace(id) || resp == null || resp.Action.Length == 0 || resp.Status.Length == 0)
        {
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }

        //todo** check that resp.PatientId exist!! 
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
        return TypedResults.Ok(await _apiService.GetDoctorsCount());
        //return TypedResults.Ok(c);
    }
}
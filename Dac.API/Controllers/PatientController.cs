using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Services;
using Dac.API.Model; 

namespace Dac.API.Controllers;


[Route("patients")]
public class PatientController : BaseController  //ControllerBase 
{
    //protected readonly IApiManagerService _apiService;  //through service 

    public PatientController(IApiManagerService apiService) : base(apiService) 
    {}

    //TypedResults.Ok(); 
    //todo** use TypedResults<IResult> for ease of unit testing
    //annotations for openApi

    //Get patients/search
    [Route("search")] 
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Search for a patient by Name")]
    [EndpointDescription("Search by firstName and LastName")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> SearchPatientByName([FromQuery(Name = "firstname")] string first,[FromQuery(Name = "lastname")] string last)
    {//Task<Results<Ok<List<Dictionary<string, object>>>, NotFound, BadRequest<ProblemDetails>>>

        //todo** check that not malformed?
        //also  >> NotFound();
        if (first == null || last == null || first.Length == 0 || last.Length == 0){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Name cannot be empty"
            });
        }
        return TypedResults.Ok(await _apiService.SearchPatientByFullName(first, last));
        //return TypedResults.Ok(p);
    }

    //GET patients/{id}/doctors
    [Route("{id}/doctors")] //OR? {id}/doctors/current
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Patient's doctor")]
    [EndpointDescription("Get current Patient's doctor")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> FetchPatientDoctors([FromRoute] string id)
    {
        if(id == null || string.IsNullOrWhiteSpace(id)){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Id is not valid"
            });
        }

        return TypedResults.Ok(await _apiService.FetchPatientAttendingDoctors(id)); 
    }

    //GET patients/count
    [Route("count")]
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Patients count")]
    [EndpointDescription("Get current Patients count")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetCurrentPatientsCount()
    {
        return TypedResults.Ok(await _apiService.GetPatientsCount());
    }

    //POST patients/{id}/requests
    [Route("{id}/requests")]
    [HttpPost]
    [Tags("Patients")]
    [EndpointSummary("Request Visit")]
    [EndpointDescription("Patient request visit from a doctor")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> CreateRequest([FromRoute] string id,[FromBody] PatientRequest request)
    {
        if(id == null || string.IsNullOrWhiteSpace(id) || request == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }

        //todo** handle errors!!>>check that doctor exists**
        return TypedResults.Ok(await _apiService.CreatePatientRequest(id, request));
        //return TypedResults.Ok(c);
    }
}
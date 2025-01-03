using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Model;
using Dac.API.Services;

namespace Dac.API.Controllers.Patients;

//[Route("patients")]
//[ApiController]
public class GetPatient : BaseController // ControllerBase
{
    public GetPatient(IApiManagerService apiService) : base(apiService)
    {}

    //GET patients/{id}
    //for viewing more info on Patient --todo** requieres Auth
    [Route("patients/{id}")]
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("View a patient info")]
    [EndpointDescription("View a patient's info")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]//StatusCodes.Status400BadRequest  >>prolly no need? --toReview**
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //oh doesnt show 200...could use to setup HATEOAS? toSee**
    public async Task<Results<Ok<Patient>, NotFound, BadRequest<ProblemDetails>>> FetchPatient([FromRoute] string id)
    {  //Task<Ok<Patient>>   //Task<IResult>
        if (id == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Id is not valid"
            }); //(Task<IResult>)Results.BadRequest();
        }

        var p = await _apiService.FetchPatientByID(id);
        return TypedResults.Ok(p); 
        //(Task<IResult>)Results.Ok();
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    // GET patients/count
    [Route("patients/count")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    //[Authorize] //same as above...toTest* when enabled >> limit access to authenticated users for that controller or action.
    public async Task<Ok<long>> GetPatientsCount() //oldie >>Task<long>
    {   
        var c = await _apiService.GetPatientCount();  //_patientRepository.GetPatientCount();

        return TypedResults.Ok(c); 
    }

}
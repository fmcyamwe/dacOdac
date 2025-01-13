using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;


using Dac.API.Model;
using Dac.API.Services;

namespace Dac.API.Controllers.Patients;

//[Route("patients")]
//[ApiController]
public class GetPatient : BaseController
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
    public async Task<IResult> FetchPatient([FromRoute] string id)
    {  //Task<Results<Ok<Patient>, NotFound, BadRequest<ProblemDetails>>>   
        if(id == null || string.IsNullOrWhiteSpace(id)){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Id is not valid"
            });
        }

        var p = await _apiService.FetchPatientByID(id, true); //todo** catch any exception here!
        return TypedResults.Ok(p); 
        //(Task<IResult>)Results.Ok();
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //GET patients/ 
    [Route("patients/{id}/more")]
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Get patients")]
    [EndpointDescription("Get list of patients")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //[Authorize]
    //[Authorize(Roles = "admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Accessible only to Admin role --todo**
    public async Task<IResult> GetPatientMedicalHistory([FromRoute] string id)
    { //Task<Ok<List<Dictionary<string, object>>>> 
        var c = await _apiService.PatientMedicalHistory(id); 

        return TypedResults.Ok(c); 
    }

}
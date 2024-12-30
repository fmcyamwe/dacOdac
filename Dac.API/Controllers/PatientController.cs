using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Services;

namespace Dac.API.Controllers;


[Route("patients")]
[ApiController]
public class PatientController : ControllerBase  //todo** use BaseController
{
    //private readonly IPatientRepository _patientRepository;
    protected readonly IApiManagerService _apiService;  //through service 

    public PatientController(IApiManagerService apiService) //IPatientRepository patientRepository, 
    {
        //_patientRepository = patientRepository;
        _apiService = apiService;
    }

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
    public async Task<Results<Ok<List<Dictionary<string, object>>>, NotFound, BadRequest<ProblemDetails>>> SearchPatientByName([FromQuery(Name = "firstname")] string first,[FromQuery(Name = "lastname")] string last)
    {//Task<List<Dictionary<string, object>>> 

        //todo** check that not malformed?
        //also  >> NotFound();
        if (first == null || last == null || first.Length == 0|| last.Length == 0){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Name cannot be empty"
            }); //(Task<IResult>)Results.BadRequest();
        }
        var p = await _apiService.SearchPatientByFullName(first, last); //_patientRepository
        return TypedResults.Ok(p); 
    }

    //GET patients/{id}/doctors
    [Route("{id}/doctors")] //OR {id}/doctors/current
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Patient's doctor")]
    [EndpointDescription("Get current Patient's doctor")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public Task<IResult> GetCurrentPatientDoctor([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo**
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }


    //GET patients/{id}/treatments
    [Route("{id}/treatments")]
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Patient treatments")]
    [EndpointDescription("Get a patient's treatments")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public Task<IResult> GetPatientTreatments([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo**
    }

    //POST patients/{id}/treatments
    [Route("{id}/treatments")]
    [HttpPost]
    [Tags("Patients")]
    [EndpointSummary("Add Patient treatment")]
    [EndpointDescription("Add a new treatment for a patient")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IResult> AddPatientTreatment([FromRoute] string id) //toAdd** [FromBody] Treatment treatment
    {
        return (Task<IResult>)Results.Ok(true); //todo**
    }

    
    /*// Post patients/new
    [Route("new")]  //same as Post patients/ but explicit endpoint--toReview**
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public Task<string> CreateNewPatient([FromBody] Patient patient)
    {

        return _apiService.AddPatient(patient); //_patientRepository
    }*/
    
}
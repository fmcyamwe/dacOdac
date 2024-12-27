using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.Neo.Model;
using Dac.API.Repositories;
//oldy >> Dac.Neo.Repositories;

namespace Dac.Api.Controllers;


[Route("patients")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly IPatientRepository _patientRepository;

    public PatientController(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    //TypedResults.Ok(); 
    //todo** use TypedResults<IResult> for ease of unit testing
    //annotations for openApi

    //GET patients/ 
    //[Route("/")]  >> removing shows the main route. //[Tags("Hello")] //removed as creates own subHeader //[EndpointName("patients")] must be unique--huh
    [HttpGet]
    [EndpointSummary("Get patients")]
    [EndpointDescription("Get list of patients")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<Ok<List<Dictionary<string, object>>>> GetAllPatients() //List<Patient>  //IResult //Task<ActionResult<bool>>
    {
        
        //this actually is the minimum info for Patients (doesnt include Treatment,etc) <would need auth for extra details>
        
        var p = await _patientRepository.GetAllPatients();
        return TypedResults.Ok(p); 

        //Results.Ok();//return TypedResults.Ok();
        //return (Task<IResult>)Results.Ok(true); 
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //POST patients
    //[Route("/")]
    [HttpPost]
    //[ValidateAntiForgeryToken] //toUse** (goes with POST only) >>ahhh error No service for type 'Microsoft.AspNetCore.Mvc.ViewFeatures.Filters.ValidateAntiforgeryTokenAuthorizationFilter' has been registered.
    [EndpointSummary("New patients")]
    [EndpointDescription("Add a new patient")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<Results<Ok<string>, NotFound, BadRequest<ProblemDetails>>> CreatePatient([FromBody] Patient patient) //[FromBody] string request
    { //oldie >>  Task<Ok<string>>
       
        //var patient = JsonExtensions.FromJson<Patient>(request); //>>nope still borks parsing string request
        Console.WriteLine("AddPatient {0} {1}", patient?.FirstName ?? "", patient?.LastName);
        if (patient == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }
        var p = await _patientRepository.AddPatient(patient);
        return TypedResults.Ok(p); 
        //todo** Return Created with url link to id >> TypedResults.Created($"/api/patients/{patient.Id}")
        //(Task<IResult>)Results.Ok(_patientRepository.AddPatient(patient));
    }

    //GET patients/{id}
    //for viewing more info on Patient --todo** requieres Auth
    [Route("{id}")]
    [HttpGet]
    [EndpointSummary("View a patient info")]
    [EndpointDescription("View a patient's info")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]//StatusCodes.Status400BadRequest
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<Results<Ok<Patient>, NotFound, BadRequest<ProblemDetails>>> GetPatient([FromRoute] string id)
    {  //Task<Ok<Patient>>   //Task<IResult>
        if (id == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Id is not valid"
            }); //(Task<IResult>)Results.BadRequest();
        }

        var p = await _patientRepository.FetchPatientByID(id);
        return TypedResults.Ok(p); 
        //(Task<IResult>)Results.Ok();
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //Get patients/search
    [Route("search")]
    [HttpGet]
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
        var p = await _patientRepository.SearchPatientByFullName(first, last);
        return TypedResults.Ok(p); 
    }

    //GET patients/{id}/doctors
    [Route("{id}/doctors")]
    [HttpGet]  
    public Task<IResult> GetPatientDoctors([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo**
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //GET patients/{id}/doctors/current
    [Route("{id}/doctors/current")]
    [HttpGet]  
    public Task<IResult> GetCurrentPatientDoctor([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo**
    }

    //GET patients/{id}/treatments
    [Route("{id}/treatments")]
    [HttpGet]  
    public Task<IResult> GetPatientTreatments([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo**
    }

    //POST patients/{id}/treatments
    [Route("{id}/treatments")]
    [HttpPost]  
    public Task<IResult> AddPatientTreatment([FromRoute] string id, [FromBody] Treatment treatment)
    {
        return (Task<IResult>)Results.Ok(true); //todo**
    }

    // GET patients/count
    [Route("count")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    //[Authorize] //same as above...toTest* when enabled >> limit access to authenticated users for that controller or action.
    public async Task<Ok<long>> GetPatientsCount() //oldie >>Task<long>
    {   
        var c = await _patientRepository.GetPatientCount();
        return TypedResults.Ok(c); //await _patientRepository.GetPatientCount()
    }

    // Post patients/new
    [Route("new")]  //same as Post patients/ but explicit endpoint--toReview**
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public Task<string> CreateNewPatient([FromBody] Patient patient)
    {

        return _patientRepository.AddPatient(patient);
    }
    
}
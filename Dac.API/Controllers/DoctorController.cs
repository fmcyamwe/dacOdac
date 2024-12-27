using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 

using Dac.Neo.Model;
using Dac.Neo.Data;
using Dac.API.Repositories;

namespace Dac.Api.Controllers;


[Route("doctors")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorController(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    //Get doctors/search
    [Route("search")]
    [HttpGet]
    [EndpointSummary("Search doctor by LastName")]
    [EndpointDescription("Search by LastName")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]//StatusCodes.Status400BadRequest
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<Results<Ok<List<Dictionary<string, object>>>, NotFound,BadRequest<ProblemDetails>>> SearchDoctorByName(
        [FromQuery(Name = "lastname")] string last)
        //[FromHeader(Name = "x-requestid")] Guid requestId) //huh toUse***
    {
        
        if (last == null || last.Length == 0){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Name cannot be empty"
            }); //(Task<IResult>)Results.BadRequest();
        }
        var p = await _doctorRepository.SearchDoctorByName(last);
        return TypedResults.Ok(p); 
    }

    // GET doctors/{id}/patients/count
    [Route("{id}/patients/count")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    //[Authorize] //same as above...toTest* when enabled >> limit access to authenticated users for that controller or action.
    public async Task<Ok<long>> GetDoctorPatientsCount([FromRoute] string id)
    {
        //not to be confused with >> GET doctors/{id}/patients/     (retrieves all patients)--todo** 
        
        //TypedResults.Ok(); 
        //todo** use TypedResults<IResult> for ease of unit tests
        
        return TypedResults.Ok(await _doctorRepository.GetPatientsCount(id));
    }

    //for clarity as no GET for doctors...should there be? toReview**
    // Post doctors/new
    [Route("new")]
    [HttpPost]
    //[ValidateAntiForgeryToken] //toTest** (goes with POST only) >>ahhh error No service for type 'Microsoft.AspNetCore.Mvc.ViewFeatures.Filters.ValidateAntiforgeryTokenAuthorizationFilter' has been registered.
    [EndpointSummary("New doctor")]
    [EndpointDescription("Add a new doctor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<Results<Ok<string>, NotFound, BadRequest<ProblemDetails>>> AddDoctor([FromBody] Doctor doctor)
    {
        Console.WriteLine("AddDoctor {0} {1}", doctor?.FirstName ?? "", doctor?.LastName);
        if (doctor == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){ //huh define new...
                Detail = "Malformed request :("
            });
        }
        var p = await _doctorRepository.AddDoctor(doctor);
        return TypedResults.Ok(p); 
        //todo** should return Created with url link to id >> TypedResults.Created($"/api/doctors/{doctor.Id}") 
    }
    
    //Get doctors/speciality
    [Route("speciality")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<List<Dictionary<string, object>>> FetchDoctorsBySpeciality([FromQuery(Name = "q")] string search)
    {
        //todo** check that not malformed?
        //also  >> NotFound();
        return _doctorRepository.ListDoctorsBySpeciality(search); //todo**
    }
}
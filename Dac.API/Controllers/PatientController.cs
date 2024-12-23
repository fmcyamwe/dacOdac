using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

using Dac.Neo.Model;
using Dac.Neo.Repositories;

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
    //todo** use TypedResults<IResult> for ease of unit tests

    //GET patients/  >>prob with '/' ? toSee** or should remove?
    //[Route("/")]
    [HttpGet]  ////[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public Task<IResult> GetAllPatients()
    {
        return (Task<IResult>)Results.Ok(true); //todo
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //POST patients/
    [Route("/")]
    [HttpPost]  ////[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ValidateAntiForgeryToken] //toSee** what this does (goes with POST)
    public Task<IResult> CreatePatient([FromBody] Patient patient) // umm async or not? but weird await or local var...
    {
         return (Task<IResult>)Results.Ok(true); //todo**
    }

    //GET patients/{id}
    [Route("{id}")]
    [HttpGet]  
    public Task<IResult> GetPatient([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //GET patients/{id}/doctors
    [Route("{id}/doctors")]
    [HttpGet]  
    public Task<IResult> GetPatientDoctors([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //GET patients/{id}/doctors/current
    [Route("{id}/doctors/current")]
    [HttpGet]  
    public Task<IResult> GetCurrentPatientDoctor([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //GET patients/{id}/treatments
    [Route("{id}/treatments")]
    [HttpGet]  
    public Task<IResult> GetPatientTreatments([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true); //todo
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //POST patients/{id}/treatments
    [Route("{id}/treatments")]
    [HttpPost]  
    public Task<IResult> AddPatientTreatment([FromRoute] string id, [FromBody] Patient patient) //body is something else todo**
    {
        return (Task<IResult>)Results.Ok(true); //todo
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    // GET patient/count
    [Route("count")] //"{name}"
    [HttpGet]  ////[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [AllowAnonymous] //authorization
    //[Authorize] //same as above...toTest* when enabled
    public Task<long> GetPatientsCount() // umm async or not? but weird await or local var...
    {   
        return _patientRepository.GetPatientCount();
    }

    // Post patient/new
    [Route("new")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public Task<bool> CreateNewPatient([FromBody] Patient patient)
    {

        return _patientRepository.AddPatient(patient);
    }
    
    //Get patient/search
    [Route("search")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<List<Dictionary<string, object>>> FetchPatientByName([FromQuery(Name = "q")] string search)
    {
        //todo** check that not malformed?
        //also  >> NotFound();
        return _patientRepository.SearchPatientByName(search);
    }
}
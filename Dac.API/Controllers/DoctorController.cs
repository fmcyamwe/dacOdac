using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Dac.Neo.Model;
using Dac.Neo.Repositories;

namespace Dac.Api.Controllers;


[Route("patient")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly IPatientRepository _patientRepository;

    public DoctorController(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    // GET patient/count
    [Route("count")] //"{name}"
    [HttpGet]  ////[ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public Task<long> GetPatientsCount() // umm async or not? but weird await or local var...
    {
        
        //TypedResults.Ok(); 
        //todo** use TypedResults<IResult> for ease of unit tests
        
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
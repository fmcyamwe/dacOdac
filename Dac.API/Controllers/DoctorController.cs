using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Services;

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


    // GET doctors/{id}/patients/count
    [Route("{id}/patients/count")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("Patient count")]
    [EndpointDescription("Get how many patients Doctor in charge of")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    [Authorize] //toTest* when enabled >> limit access to authenticated users for that controller or action.
    //[Authorize(Roles = "Admin")]  // Accessible only to Admin role
    public async Task<Ok<long>> GetDoctorPatientsCount([FromRoute] string id)
    {
        //not to be confused with >> GET doctors/{id}/patients/     (retrieves all patients)--todo** 
        
        //TypedResults.Ok(); 
        //todo** use TypedResults<IResult> for ease of unit tests
        
        return TypedResults.Ok(await _apiService.GetPatientsCount(id)); //_doctorRepository
    }
    
    //Get doctors/speciality
    [Route("speciality")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("Doctors by Speciality")]
    [EndpointDescription("List Doctors by Speciality")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    public Task<List<Dictionary<string, object>>> FetchDoctorsBySpeciality([FromQuery(Name = "q")] string search)
    {
        //todo** check that not malformed?
        //also  >> NotFound();
        return _apiService.ListDoctorsBySpeciality(search); //_doctorRepository
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
    public Task<List<Dictionary<string, object>>> FetchDoctorsB([FromRoute] string id)
    {
        return _apiService.GetPatientRequests(id); //async? 
    }
}
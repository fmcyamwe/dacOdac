using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Model;
using Dac.API.Services;

namespace Dac.API.Controllers.Doctors;

//[Route("doctors")]
//[ApiController]
public class CreateDoctor : BaseController
{
    public CreateDoctor(IApiManagerService apiService) : base(apiService)
    {}

    // Post doctors/new
    [Route("doctors/new")]
    [HttpPost]
    [Tags("Doctors")]
    //[ValidateAntiForgeryToken] //toTest** (goes with POST only) >>ahhh error No service for type 'Microsoft.AspNetCore.Mvc.ViewFeatures.Filters.ValidateAntiforgeryTokenAuthorizationFilter' has been registered.
    [EndpointSummary("New doctor")]
    [EndpointDescription("Add a new doctor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<Results<Ok<string>, NotFound, BadRequest<ProblemDetails>>> AddDoctor([FromBody] Doctor doctor)
    {
        Console.WriteLine("AddDoctor {0} {1}", doctor?.FirstName ?? "", doctor?.LastName);
        if (doctor == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){ //huh define new...
                Detail = "Malformed request :("
            });
        }
        var p = await _apiService.AddDoctor(doctor); //_doctorRepository
        return TypedResults.Ok(p); 
        //todo** should return Created with url link to id >> TypedResults.Created($"/api/doctors/{doctor.Id}") 
    }
    
}
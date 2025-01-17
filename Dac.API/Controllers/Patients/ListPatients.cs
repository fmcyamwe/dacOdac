using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;


using Dac.API.Services;
using Dac.API.Model;

namespace Dac.API.Controllers.Patients;

//[Route("patients")]
//[ApiController]
public class ListPatients : BaseController
{
    public ListPatients(IApiManagerService apiService) : base(apiService)
    {}

    //GET patients/ 
    [Route("patients")] //  >> //[EndpointName("patients")] must be unique--huh
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Get patients")]
    [EndpointDescription("Get list of patients")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //[Authorize]
    [Authorize(Roles = "admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Accessible only to Admin role
    //--seem needed as default authorize above fails :( ..or should just set Roles = "" ? toTry**
    public async Task<IResult> GetAllPatients([FromQuery] PaginationRequest paginationRequest)
    {//Task<Ok<List<Dictionary<string, object>>>>
    
        //this actually is the minimum info for Patients (doesnt include Treatment,etc) <would need auth for extra details>
        
        var p = await _apiService.GetAllPatients(paginationRequest.PageSize, paginationRequest.PageIndex);
        return TypedResults.Ok(p); 
    }
}
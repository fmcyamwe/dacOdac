using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Model;
using Dac.API.Services;

namespace Dac.API.Controllers.Doctors;

//[Route("doctors")]
[ApiController]
public class ListDoctors : BaseController
{
    public ListDoctors(IApiManagerService apiService) : base(apiService)
    {}


    //GET doctors/ 
    [Route("doctors")]
    [HttpGet] 
    [Tags("Doctors")]
    [EndpointSummary("Get doctors")]
    [EndpointDescription("Get list of doctors")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<Ok<List<Dictionary<string, object>>>> GetAllDoctors()
    {
        
        var p = await _apiService.GetAllDoctors();
        return TypedResults.Ok(p); 

        //Results.Ok();//return TypedResults.Ok();
        //return (Task<IResult>)Results.Ok(true); 
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }
    
}
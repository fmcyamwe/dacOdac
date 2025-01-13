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
    public async Task<IResult> GetAllDoctors([FromQuery] PaginationRequest paginationRequest)
    {    //Task<Ok<List<Dictionary<string, object>>>>
        //[AsParameters] PaginationRequest paginationRequest) //[AsParameters] dont work......fromQuery does >> //doctors?PageSize=10&PageIndex=0'
    
         _apiService.GetLogger().LogInformation("GetAllDoctors :: PaginationRequest {index} > {size} ", paginationRequest.PageIndex, paginationRequest.PageSize);
         
        var p = await _apiService.GetAllDoctors(paginationRequest.PageSize, paginationRequest.PageIndex);
        return TypedResults.Ok(p); 

        //Results.Ok();//return TypedResults.Ok();
        //return (Task<IResult>)Results.Ok(true); 
        //Results.Ok(List<Patient>>);//TypedResults.Ok(); //Task<Ok<List<Patient>>>
    }

    //Get doctors/speciality
    [Route("doctors/speciality")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("List Doctors by Speciality")]
    [EndpointDescription("List Doctors by Speciality")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    public async Task<IResult> FetchDoctorsBySpeciality([FromQuery(Name = "q")] string search)
    {//Task<List<Dictionary<string, object>>>
        
        //also  >> NotFound();
        return TypedResults.Ok(await _apiService.ListDoctorsBySpeciality(search)); 
    }
    
    //Get doctors/speciality //umm moot?
    [Route("doctors/speciality/count")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("Count Doctors by Speciality")]
    [EndpointDescription("Count of Doctors by Speciality")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous]
    public async Task<IResult> CountDoctorsBySpeciality() 
    {//Task<List<Dictionary<string, object>>>
        return TypedResults.Ok(await _apiService.DoctorsCountBySpeciality());
    }
}
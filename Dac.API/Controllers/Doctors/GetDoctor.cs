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
public class GetDoctor : BaseController // ControllerBase
{
    public GetDoctor(IApiManagerService apiService) : base(apiService)
    {}

    //GET doctors/{id}
    //for viewing more info of Doctor --todo** requieres Auth
    [Route("doctors/{id}")]
    [HttpGet]
    [Tags("Doctors")]
    [EndpointSummary("View a doctor's info")]
    [EndpointDescription("View a doctor's info")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]//StatusCodes.Status400BadRequest  >>prolly no need? --toReview**
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    //oh doesnt show 200...could use to setup HATEOAS? toSee**
    public async Task<Results<Ok<Doctor>, NotFound, BadRequest<ProblemDetails>>> FetchDoctor([FromRoute] string id)
    {  //Task<Ok<Doctor>>   //Task<IResult>
        if (id == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Id is not valid"
            }); //(Task<IResult>)Results.BadRequest();
        }

        var p = await _apiService.FetchDoctorByID(id); //_doctorRepository
        return TypedResults.Ok(p); 
        
    }

    //Get doctors/search
    [Route("doctors/search")]
    [HttpGet]
    [Tags("Doctors")]
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
        var p = await _apiService.SearchDoctorByName(last); //_doctorRepository
        return TypedResults.Ok(p); 
    }

}
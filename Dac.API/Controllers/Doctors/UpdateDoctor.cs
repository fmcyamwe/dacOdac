using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


using Dac.API.Model;
using Dac.API.Services;

namespace Dac.API.Controllers.Doctors;

//[Route("doctors")]
//[ApiController]
public class UpdateDoctor : BaseController
{
    public UpdateDoctor(IApiManagerService apiService) : base(apiService)
    {}

    //PUT doctors/{id} 
    [Route("doctors/{id}")]
    [HttpPut]
    [Tags("Doctors")]
    [EndpointSummary("Update Doctor")]
    [EndpointDescription("Update a doctor's information")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateADoctor([FromRoute] string id, [FromBody] Doctor doc)
    {
        if (doc == null)
        {
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }
        try
        {
            var d = await _apiService.FetchDoctorByID(id);

            d.FirstName = doc.FirstName;
            d.LastName = doc.LastName;
            d.Speciality = doc.Speciality;
            d.PractiseSince = doc.PractiseSince;
            
            await _apiService.AddDoctor(d); //kinda cheating--toFix**
            return Results.NoContent();
        }catch (Exception ex){
            Console.WriteLine("UpdateADoctor::ERROR {0}", ex);
            return TypedResults.NotFound();
        }
        //return await (Task<IResult>)Results.Ok(true);
    }
    
}
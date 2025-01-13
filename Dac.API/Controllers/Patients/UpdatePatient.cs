using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Model;
using Dac.API.Services;

namespace Dac.API.Controllers.Patients;

//[Route("patients")]
//[ApiController]
public class UpdatePatient : BaseController // ControllerBase
{
    public UpdatePatient(IApiManagerService apiService) : base(apiService)
    {}

    //PUT patients/{id} 
    [Route("patients/{id}")]
    [HttpPut]
    [Tags("Patients")]
    [EndpointSummary("Update a patient")]
    [EndpointDescription("Update a patient information")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateAPatient([FromRoute] string id, [FromBody] Patient patient)
    {
        
        if (patient == null || string.IsNullOrWhiteSpace(id))
        {
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }

        try
        {
            var p = await _apiService.FetchPatientByID(id, false);
            p.FirstName = patient.FirstName;
            p.LastName = patient.LastName;
            p.Born = patient.Born;
            p.Gender = patient.Gender;

            await _apiService.AddPatient(p); //kinda cheating--toFix**

            return Results.NoContent();

        }catch (Exception ex){
            Console.WriteLine("UpdateAPatient::ERROR {0}", ex);
            return TypedResults.NotFound();
        }
    }
}
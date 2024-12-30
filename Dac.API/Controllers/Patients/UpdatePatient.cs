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
public class UpdatePatient :  BaseController // ControllerBase
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
    public async Task<IResult> UpdateAPatient([FromRoute] string id) //[FromBody] Patient patient
    {
        return await (Task<IResult>)Results.Ok(true); //todo**
    }
    
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Model;
using Dac.API.Services;

namespace Dac.API.Controllers.Patients;

//[Route("patients")]
[ApiController]
public class DeletePatient : BaseController // ControllerBase
{
    public DeletePatient(IApiManagerService apiService) : base(apiService)
    {}

    //DELETE patients/{id} 
    [Route("patients/{id}")]
    [HttpDelete]
    [Tags("Patients")]
    [EndpointSummary("Remove patient")]
    [EndpointDescription("Delete a patient")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> DeleteAPatient([FromRoute] string id)
    {
        return await (Task<IResult>)Results.Ok(true); //todo**
    }
    
}
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
public class DeleteDoctor : BaseController
{
    public DeleteDoctor(IApiManagerService apiService) : base(apiService)
    {}

    //DELETE doctors/{id} 
    [Route("doctors/{id}")]
    [HttpDelete]
    [Tags("Doctors")]
    [EndpointSummary("Remove doctor")]
    [EndpointDescription("Delete a doctor")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> DeleteADoctor([FromRoute] string id)
    {
        return await (Task<IResult>)Results.Ok(true); //todo**
    }
    
    
}
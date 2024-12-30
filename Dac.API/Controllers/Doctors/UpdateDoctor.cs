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
    public async Task<IResult> UpdateADoctor([FromRoute] string id) //[FromBody] Doctor doc
    {
        return await (Task<IResult>)Results.Ok(true); //todo**
    }
    
}
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
public class CreatePatient : BaseController // ControllerBase
{
    public CreatePatient(IApiManagerService apiService) : base(apiService)
    {}

    [Route("patients")] //better here than above
    [HttpPost]
    [Tags("Patients")]
    //[ValidateAntiForgeryToken] //toUse** (goes with POST only) >>ahhh error No service for type 'Microsoft.AspNetCore.Mvc.ViewFeatures.Filters.ValidateAntiforgeryTokenAuthorizationFilter' has been registered.
    [EndpointSummary("New patients")]
    [EndpointDescription("Add a new patient")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    //[ProducesResponseType(typeof(string), 200)] //huh could actually set to a typeof(Patient) too?!? --todo**
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<Results<Ok<string>, NotFound, BadRequest<ProblemDetails>>> NewPatient([FromBody] Patient patient) //[FromBody] string request
    { //oldie >>  Task<Ok<string>>
       
        //var patient = JsonExtensions.FromJson<Patient>(request); //>>nope still borks parsing string request
        //Console.WriteLine("AddPatient {0} {1}", patient?.FirstName ?? "", patient?.LastName);
        if (patient == null){
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }
        //var p = await _patientRepository.AddPatient(patient);
        var p = await _apiService.AddPatient(patient);

        return TypedResults.Ok(p); 
        //todo** Return Created with url link to id >> TypedResults.Created($"/api/patients/{patient.Id}")
        //(Task<IResult>)Results.Ok(_patientRepository.AddPatient(patient));
    }
}
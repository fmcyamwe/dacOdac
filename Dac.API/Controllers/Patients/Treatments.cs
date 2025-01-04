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
public class Treatments : BaseController // ControllerBase
{
    public Treatments(IApiManagerService apiService) : base(apiService)
    {}

    //GET patients/{id}/treatments
    [Route("patients/{id}/treatments")]
    [HttpGet]
    [Tags("Patients")]
    [EndpointSummary("Patient treatments")]
    [EndpointDescription("Get a patient's treatments")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public Task<IResult> GetPatientTreatments([FromRoute] string id)
    {
        return (Task<IResult>)Results.Ok(true);  //todo** surface with .CurrentPatientTreatment(id)
    }

    //POST patients/{id}/treatments
    [Route("patients/{id}/treatments")]
    [HttpPost]
    [Tags("Patients")]
    [EndpointSummary("Add Patient treatment")]
    [EndpointDescription("Add a new treatment for a patient")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IResult> AddPatientTreatment([FromRoute] string id, [FromBody] TreatmentRequest treatment)
    {
         //(Task<IResult>)Results.Ok(true);
        if (treatment == null || id.Length == 0 || id != treatment.PatientId) // id == treatment.patientId >>should
        {
            return TypedResults.BadRequest<ProblemDetails>(new (){
                Detail = "Malformed request :("
            });
        }
       
        try
        {
            var p = await _apiService.AddUpdatePatientTreatment(treatment.DoctorId, treatment.PatientId, treatment.Name, treatment.Details);
        
            //todo** check return p that not empty
            Console.WriteLine("AddUpdatePatientTreatment:: >> {0}", p);
            return Results.NoContent(); //or created?
        }catch (Exception ex){
            Console.WriteLine("AddPatientTreatment::ERROR {0}", ex);
            return TypedResults.NotFound();
        } 

    }
}
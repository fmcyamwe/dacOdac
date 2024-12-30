using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Services;

namespace Dac.API.Controllers;

public class ConnectController : BaseController 
{
    //toReview** contains endpoints that dont belong in any Patient or Doctor controllers
    //used to check for connect by UI
    public ConnectController(IApiManagerService apiService) : base(apiService) 
    {}

    // GET connect >> for testing connection from web ui >>complains
    [Route("connect")]
    [HttpGet]
    [ExcludeFromDescription]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous] //authorization
    //[Authorize] //same as above...toTest* when enabled >> limit access to authenticated users for that controller or action.
    public async Task<Ok<long>> CanConnect() //[FromRoute] string id
    {
        //not to be confused with >> GET doctors/{id}/patients/     (retrieves all patients)--todo** 
        
        //TypedResults.Ok(); 
        //todo** use TypedResults<IResult> for ease of unit tests
        
        return TypedResults.Ok(await _apiService.GetPatientCount()); 
    }

    // GET specialities  //hardcoded specialities for uniformity 
    [Route("specialities")]
    [HttpGet] //huh () ? complains tho lol
    [ExcludeFromDescription]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous]
    public Ok<List<string>> FetchAllSpecialities() //
    {
        
        string[] hardcodedArray = new[] { 
            "Pediatrician", "Family physian", "Geriatry",
            "Allergist", "Dermatology", "Ophtamology",
            "OB/GYN", "Cardiology", "Endocrinology",
            "Gastroenterology", "Nephrology", "Hematology",
            "Geneticist", "Neurology", "Oncology",
            "Osteopathy", "Otolaryngology", "Pathology",
            "Plastic surgery", "Podiatry", "Psychiatry",
            "Pulmonology", "Radiology", "Rheumatology"
            };
        
        return TypedResults.Ok(hardcodedArray.ToList());
    }
}
/*
Pediatrician
Family physian
Geriatric  //older adults huh
Allergist
Dermatology
Ophtamology
OB/GYN (obestetrician/gynecologist)
cardiology
endocrinology
gastroenterologist (gastro)...umm should use short name?
nephrology (kidney..huh)
hematologist (blood)
geneticist
neurology
Oncology (cancer)
Osteopathy (whole body)
Otolaryngology (ears, nose, throat)
pathology
plastic
podiatry (ankles and feet)
psychiatry
Pulmonology
Radiology
Rheumatology (arthritis)
*/
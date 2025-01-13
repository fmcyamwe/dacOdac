using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;


using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Http.HttpResults; 

using Dac.API.Services;
using Dac.API.Constants;
using Dac.API.Model;

namespace Dac.API.Controllers;

public class ConnectController : BaseController 
{
    //toReview** contains endpoints that dont belong in any Patient or Doctor controllers
    //used to check for connection by UI
    public ConnectController(IApiManagerService apiService) : base(apiService) 
    {}

    // GET connect >> for testing connection from web ui >>complains
    [Route("connect")]
    [HttpGet]
    [ExcludeFromDescription]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous]
    public async Task<IResult> CanConnect()
    {//Ok<long>
        //TypedResults.Ok(); 
        //todo** use TypedResults<IResult> for ease of unit tests
        
        return TypedResults.Ok(await _apiService.FetchRandomAccts()); 
    }

    // GET specialities  //hardcoded specialities for data uniformity 
    [Route("specialities")]
    [HttpGet]
    [ExcludeFromDescription]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(typeof(ProblemDetails), 500)]
    [AllowAnonymous]
    public Ok<List<string>> FetchAllSpecialities()
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

    [Route("auth/login")] //api/[controller]
    [HttpPost]
    [ExcludeFromDescription]
    public IActionResult Login([FromBody] LoginRequest request)//IActionResult  //Results<Ok<string>, Not>
    {//Task<Results<NoContent, NotFound>>
        // Dummy validation for demonstration purposes
        if (request.Username == "admin" && request.Password == "password")
        {
            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(AuthorizationConstants.JWT_SECRET_KEY); //"your-secret-key"
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim(ClaimTypes.Name, request.Username), //umm needed?!? or gets those nulls in token<SecurityToken> ?
                    new Claim(ClaimTypes.Role, "admin") // Assign roles
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "your-issuer",
                Audience = "your-audience",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //_apiService.GetLogger().LogInformation("ConnectController :: {validTo} -> {toString} -> {toJson} ", token.ValidTo, token.ToString(), token.ToJson<SecurityToken>());
            
            return Ok(new { Token = tokenString });
            //return Results.Ok(tokenString);//TypedResults.Ok(tokenString); //
            //a JWT token that should be used in the  Authorization header >i.e Authorization: Bearer <token>
        }

        return Unauthorized();//TypedResults.NoContent;
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
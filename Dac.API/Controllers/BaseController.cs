using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults; 


using Dac.API.Model;
//using Dac.API.Repositories;
//oldy >> back to it!!
//using Dac.Neo.Repositories;
using Dac.API.Services;

namespace Dac.API.Controllers;

[ApiController] //prolly needed
public class BaseController : ControllerBase
{
    protected readonly IApiManagerService _apiService;  //through service 

    public BaseController(IApiManagerService apiService) //IPatientRepository patientRepository, 
    {
        //_patientRepository = patientRepository;
        _apiService = apiService;
    }


}
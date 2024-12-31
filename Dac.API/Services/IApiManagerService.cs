using Dac.Neo.Repositories;
using Dac.API.Model; 

namespace Dac.API.Services;
public interface IApiManagerService
{
    //string ExtractRedirectUriFromReturnUrl(string url);
    
    /////Patient/////
    Task<long> GetPatientCount(); //for testing---toRemove
    Task<List<Dictionary<string, object>>> GetAllPatients(); //should paginate--todo**
    Task<string> AddPatient(Patient person); //return string ID
    Task<Patient> FetchPatientByID(string id); //requiere auth!
    Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName);

    Task<string> CreatePatientRequest(string patientId, VisitRequest v);

    /////Doctor/////
    Task<List<Dictionary<string, object>>> GetAllDoctors();
    Task<string> AddDoctor(Doctor doc); 
    Task<Doctor> FetchDoctorByID(string id); //requiere auth!
    Task<List<Dictionary<string, object>>> SearchDoctorByName(string lastName);
    Task<List<Dictionary<string, object>>> ListDoctorsBySpeciality(string speciality);
    Task<List<Dictionary<string, object>>> DoctorsCountBySpeciality();
    
    Task<long> GetPatientsCount(string id); 
    Task<List<Dictionary<string, object>>> GetPatientRequests(string id);
}
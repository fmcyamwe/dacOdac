
using Dac.API.Model; 
using Dac.Neo.Data.Model;

namespace Dac.API.Services;
public interface IApiManagerService
{
    //string ExtractRedirectUriFromReturnUrl(string url);
    ILogger<ApiManagerService> GetLogger();//for testing---toRemove
    Task<string[]> FetchRandomAccts(); 

    /////Patient/////
    Task<List<Dictionary<string, object>>> GetAllPatients(int pageSize, int pageIndex);
    Task<string> AddPatient(Patient person); //return string ID
    Task<string> UpdatePatient(string id, Patient person);
    Task<long> GetPatientsCount();
    Task<bool> DeletePatient(string id);//requiere auth!
    Task<Patient> FetchPatientByID(string id, bool fetchAll); //requiere auth!
    Task<List<Dictionary<string, object>>> FetchPatientAttendingDoctors(string id);
    Task<Treatment> CurrentPatientTreatment(string id);
    Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName);
    Task<List<Dictionary<string, object>>> PatientMedicalHistory(string id);
    Task<string> CreatePatientRequest(string patientId, PatientRequest v);
    

    /////Doctor/////
    Task<List<Dictionary<string, object>>> GetAllDoctors(int pageSize, int pageIndex);
    Task<long> GetDoctorsCount();
    Task<string> AddDoctor(Doctor doc); //auth
    Task<Doctor> FetchDoctorByID(string id); //requiere auth!
    Task<string> UpdateDoctor(string id, Doctor doc);
    Task<List<Dictionary<string, object>>> SearchDoctorByName(string lastName);
    Task<List<Dictionary<string, object>>> ListDoctorsBySpeciality(string speciality);
    Task<List<Dictionary<string, object>>> DoctorsCountBySpeciality();
    Task<List<Dictionary<string, object>>> GetPendingRequests(string id);
    Task<List<Dictionary<string, object>>> GetDoctorPatients(string id);
    Task<string> UpdatePatientRequest(string doctorId, string action, string newStatus);
    Task<string> AddUpdatePatientTreatment(string docId,string patientId, string name, string details);
    Task<bool> DeleteDoctor(string id);
}
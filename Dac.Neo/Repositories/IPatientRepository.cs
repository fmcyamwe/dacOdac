//using Dac.Neo.Model; 
using Dac.Neo.Data.Model;


namespace Dac.Neo.Repositories;  
public interface IPatientRepository
{
    Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName);
    Task<PatientDB> FetchPatientByID(string id); //requiere auth!
    Task<List<Dictionary<string, object>>> PatientAttendingDoctors(string id); 
    Task<string> AddPatient(PatientDB person); //return string ID
    //todo** 
    //updatePatient?
    Task<long> GetPatientCount();
    Task<string> RandomPatient();
    Task<List<Dictionary<string, object>>> GetAllPatients(int skipPaginate);
    Task<Treatment> CurrentPatientTreatment(string id);
    Task<string> UpdatePatientTreatment(string docId,string patientId, string name, string details);

    Task<List<Dictionary<string, object>>> PatientMedicalHistory(string id);
    Task<string> CreatePatientRequest(string patientId, string doctorId, string action, string reason);
    Task<string> AddAttendingDoctor(string patientId, string docId, string triggerAction);
    
}
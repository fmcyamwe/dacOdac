//using Dac.Neo.Model; 
using Dac.Neo.Data.Model; 

namespace Dac.Neo.Repositories;  
public interface IPatientRepository
{
    Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName);
    Task<PatientDB> FetchPatientByID(string id); //requiere auth!
    Task<string> AddPatient(PatientDB person); //return string ID
    Task<long> GetPatientCount();
    Task<List<Dictionary<string, object>>> GetAllPatients(); //should paginate--todo**
        
    //todo** 
    //updatePatient?
    //addTreatment

    //addVisit (by Nurse to add patient's visit (date and seen_by<doctor>)) 
    Task<string> CreatePatientRequest(string patientId, string doctorId, string action, string reason); 
    
}
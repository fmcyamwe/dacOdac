using Dac.Neo.Model; 

namespace Dac.API.Repositories;  
public interface IPatientRepository
{
    Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName);
    Task<Patient> FetchPatientByID(string id); //requiere auth!
    Task<string> AddPatient(Patient person); //string ID 
    
    //todo** 
    //updatePatient?
    //addTreatment
    //addVisit (by Nurse to add patient's visit (date and seen_by<doctor>)) 

    Task<long> GetPatientCount();
    Task<List<Dictionary<string, object>>> GetAllPatients(); //should paginate--todo**
        
}
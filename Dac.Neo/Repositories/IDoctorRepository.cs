using Dac.Neo.Data.Model;

namespace Dac.Neo.Repositories;
    public interface IDoctorRepository
    {
        Task<List<Dictionary<string, object>>> GetAllDoctors(int skipPaginate);
        Task<List<Dictionary<string, object>>> SearchDoctorByName(string lastName);
        Task<List<Dictionary<string, object>>> ListDoctorsBySpeciality(string speciality);
        Task<List<Dictionary<string, object>>> DoctorsCountBySpeciality();
        Task<long> GetDoctorCount();
        Task<string> AddDoctor(DoctorDB doc); 
        Task<string> RandomDoctor();
        Task<DoctorDB> FetchDoctorByID(string id); //requiere auth!
        Task<string> AddPatientTreatment(string docId,string patientId, string name, string details);
        Task<List<Dictionary<string, object>>> GetPendingRequests(string id);
        Task<string> UpdatePatientRequest(string doctorId, string action, string newStatus);
        Task<List<Dictionary<string, object>>> GetOwnPatients(string id); //minimal info of current patients
    }
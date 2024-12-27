using Dac.Neo.Model;

namespace Dac.API.Repositories;
    public interface IDoctorRepository
    {
        Task<List<Dictionary<string, object>>> SearchDoctorByName(string lastName);
        Task<List<Dictionary<string, object>>> ListDoctorsBySpeciality(string speciality);
        Task<List<Dictionary<string, object>>> DoctorsCountBySpeciality(); //group
        Task<string> AddDoctor(Doctor doc); 

        Task<Doctor> FetchDoctorByID(string id); //requiere auth!
        //todo** 
        //addPatientTreatment
        //addMedicalRecord

        Task<long> GetPatientsCount(string id); //redundant as could use below--testing** for now
        Task<List<Dictionary<string, object>>> GetAllPatients(string id); //all patients of doctor
    }
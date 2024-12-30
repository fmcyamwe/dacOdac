
using Dac.Neo.Repositories;
using Dac.API.Model; 

namespace Dac.API.Services;
public class ApiManagerService : IApiManagerService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

     private ILogger<ApiManagerService> _logger;

    public ApiManagerService(IPatientRepository patientRepo, IDoctorRepository doctorRepo,  ILogger<ApiManagerService> logger)  
    {
        _patientRepository = patientRepo;
        _doctorRepository = doctorRepo;
        _logger = logger;
    }

    public async Task<long> GetPatientCount()
    {
        return  await _patientRepository.GetPatientCount();
    }
    
    public async Task<List<Dictionary<string, object>>> GetAllPatients()
    {
        return await _patientRepository.GetAllPatients();
    }
    public async Task<string> AddPatient(Patient patient)
    {
        return await _patientRepository.AddPatient(Mapper.MapToPatientDB(patient)); //todo** generate GUIID here
    }

    public async Task<Patient> FetchPatientByID(string id)
    {
        var p = await _patientRepository.FetchPatientByID(id) ?? throw new Exception("Patient not found");
        return Mapper.MapToPatient(p);
    }

    public async Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName)
    {
        return await _patientRepository.SearchPatientByFullName(firstName, lastName); //todo** check for null
    }

    public async Task<List<Dictionary<string, object>>> GetAllDoctors()
    {
        return await _doctorRepository.GetAllDoctors();
    }

    public async Task<string> AddDoctor(Doctor doctor)
    {
        return await _doctorRepository.AddDoctor(Mapper.MapToDoctorDB(doctor)); //todo** add GUIID here
    }

    public async Task<Doctor> FetchDoctorByID(string id)
    {
        var d = await _doctorRepository.FetchDoctorByID(id) ?? throw new Exception("Doctor not found");
        return Mapper.MapToDoctor(d);
    }

    public async Task<List<Dictionary<string, object>>> SearchDoctorByName(string lastName)
    {
        return await _doctorRepository.SearchDoctorByName(lastName); //todo** check for null

    }

    public async Task<List<Dictionary<string, object>>> ListDoctorsBySpeciality(string speciality)
    {
        return await _doctorRepository.ListDoctorsBySpeciality(speciality);
    }

    public async Task<List<Dictionary<string, object>>> DoctorsCountBySpeciality()
    {
        return await _doctorRepository.DoctorsCountBySpeciality();
    }

    public async Task<long> GetPatientsCount(string id)
    {
        return await _doctorRepository.GetPatientsCount(id);
    }
}
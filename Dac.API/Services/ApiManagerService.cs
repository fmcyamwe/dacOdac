
using Dac.Neo.Repositories;
using Dac.API.Model; 

namespace Dac.API.Services;
public class ApiManagerService : IApiManagerService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    private readonly ILogger<ApiManagerService> _logger;

    public ApiManagerService(IPatientRepository patientRepo, IDoctorRepository doctorRepo,  ILogger<ApiManagerService> logger)  
    {
        _patientRepository = patientRepo;
        _doctorRepository = doctorRepo;
        _logger = logger;
    }

    public ILogger<ApiManagerService> GetLogger()
    {
        return _logger;
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
        return await _patientRepository.AddPatient(Mapper.MapToPatientNode(patient));
    }

    public async Task<Patient> FetchPatientByID(string id)
    {
        try
        {
            var p = await _patientRepository.FetchPatientByID(id);

            var currentDocs = await _patientRepository.PatientAttendingDoctors(id);
            _logger.LogInformation("FetchPatientByID >> PatientAttendingDoctors {id} >> {docs} > {size}", id,currentDocs,currentDocs.Count);

            var currentTreatment = await _patientRepository.CurrentPatientTreatment(id);//todo** add to return

            _logger.LogInformation("FetchPatientByID >> CurrentPatientTreatment {id} > {currentTreatment}", id,currentTreatment);
        
            return Mapper.MapFromPatientNode(p); //todo** add currentDocs to return
        } catch (Exception ex){
            _logger.LogTrace(ex, "FetchPatientByID Exception!! {id}", id);
            throw new Exception("Patient not found");
        }
        //var p = await _patientRepository.FetchPatientByID(id) ?? throw new Exception("Patient not found");
        //return Mapper.MapFromPatientNode(p);
    }

    public async Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName)
    {
        return await _patientRepository.SearchPatientByFullName(firstName, lastName);
    }

    public async Task<string> CreatePatientRequest(string patientId, PatientRequest v)
    {
        return await _patientRepository.CreatePatientRequest(patientId, v.DoctorId, v.Action, v.Reason);

    }

    public async Task<List<Dictionary<string, object>>> GetAllDoctors()
    {
        return await _doctorRepository.GetAllDoctors();
    }

    public async Task<string> AddDoctor(Doctor doctor)
    {
        return await _doctorRepository.AddDoctor(Mapper.MapToDoctorNode(doctor)); //todo** add GUIID here
    }

    public async Task<Doctor> FetchDoctorByID(string id)
    {
        var d = await _doctorRepository.FetchDoctorByID(id) ?? throw new Exception("Doctor not found");
        return Mapper.MapFromDoctorNode(d);
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

    public async Task<List<Dictionary<string, object>>> GetDoctorPatients(string id)
    {
        return await _doctorRepository.GetAllPatients(id);
    }

    public async Task<List<Dictionary<string, object>>> GetPatientRequests(string id) //PendingRequests
    {
        return await _doctorRepository.GetPendingRequests(id);
    }

    public async Task<string> UpdatePatientRequest(string doctorId, string action, string newStatus)
    {//doctor aggrees to request >>creating a relationship between him and patient when status != 'rejected'
         //"approved" || "rejected"

        var patientId = await _doctorRepository.UpdatePatientRequest(doctorId, action,newStatus);

        _logger.LogInformation("UpdatePatientRequest {doctorId} >> {action} > {status} >> {done}", doctorId,action,newStatus,patientId);
        
        if (!string.IsNullOrWhiteSpace(patientId) && newStatus != "rejected")
        {
            //toReview** should allow only one attending doctor? >>makes sense to have many doctors tho.. 
            return await _patientRepository.AddAttendingDoctor(patientId,doctorId, action);
        }

        return ""; //todo** check return at call site
    }

    public async Task<string> AddUpdatePatientTreatment(string docId,string patientId, string name, string details)
    {
        var currentTreatment = await _patientRepository.CurrentPatientTreatment(patientId);

        _logger.LogInformation("AddUpdatePatientTreatment {doctorId} >> {patientId} > {currentTreatment}", docId,patientId,currentTreatment);
        
        //check r.to is not null for Treatment updates
        if (currentTreatment != null)
        {
            //todo** should add .to to  currentTreatment before creating new Treatment || check that same doctor?!?
            return await _doctorRepository.AddPatientTreatment(docId, patientId,name, details);
        }

        return ""; //todo** check return at call site

    }
}
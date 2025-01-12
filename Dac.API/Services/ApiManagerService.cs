
using Dac.Neo.Repositories;
using Dac.API.Model; 
using Dac.Neo.Data.Model;

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

    public async Task<string[]> GetPatientCount()
    {
        return await RandomLoginAccts();
        //return  await _patientRepository.GetPatientCount();
    }
    
    public async Task<List<Dictionary<string, object>>> GetAllPatients(int pageSize, int pageIndex)
    {
        return await _patientRepository.GetAllPatients(pageIndex >=1 ? pageSize*pageIndex : 0); //skip 0 at first--toReview**
    }
    public async Task<string> AddPatient(Patient patient)
    {
        return await _patientRepository.AddPatient(Mapper.MapToPatientNode(patient));
    }

    public async Task<string[]> RandomLoginAccts() //Task<string>
    {//Dictionary<string, string>
        var rDoc = await _doctorRepository.RandomDoctor();
        var rPatient = await _patientRepository.RandomPatient();

        _logger.LogInformation("RandomLoginAccts:: >> {rDoc} >> {rPatient}",rDoc, rPatient);

        return [rDoc,rPatient]; //(rDoc, rPatient);
    } 

    public async Task<List<Dictionary<string, object>>> PatientMedicalHistory(string id)
    {
         var currentDocs = await _patientRepository.PatientAttendingDoctors(id);
            _logger.LogInformation("PatientMedicalHistory >> PatientAttendingDoctors {id} >> {docs} > {size}", id,currentDocs,currentDocs.Count);
            
        var currentDocsList = currentDocs.Select(Mapper.MapDictionaryToModel).ToList();

        //totTtest
        //@"MATCH (p:Patient {id: "544e30f81841"})-[r:HAS_TREATMENT]->() with p match q=(p)-[r:REQUESTED]->() RETURN p,q"
        var ptMedicHist =  await _patientRepository.PatientMedicalHistory(id); 

        return ptMedicHist; //todo add attendingDocts
    }

    public async Task<Patient> FetchPatientByID(string id, bool fetchAll)
    {
        try
        {
            var p = await _patientRepository.FetchPatientByID(id);
            if(!fetchAll){ //for updating patient no need to fetch everything
                return Mapper.MapFromPatientNode(p);
            }

            var currentDocs = await _patientRepository.PatientAttendingDoctors(id);
            _logger.LogInformation("FetchPatientByID >> PatientAttendingDoctors {id} >> {docs} > {size}", id,currentDocs,currentDocs.Count);
            
            var currentDocsList = currentDocs.Select(Mapper.MapDictionaryToModel).ToList();

            _logger.LogInformation("PatientAttendingDoctors::MapperAttendingDoctors >> {list}",currentDocsList);

            var currentTreatment = await _patientRepository.CurrentPatientTreatment(id);

            _logger.LogInformation("FetchPatientByID >> CurrentPatientTreatment {id} > {currentTreatment}", id,currentTreatment);
        
            return Mapper.MapFromPatientNode(p, currentDocsList, currentTreatment);
        } catch (Exception ex){
            _logger.LogTrace(ex, "FetchPatientByID Exception!! {id}", id);
            throw new Exception("Patient not found");
        }
        //var p = await _patientRepository.FetchPatientByID(id) ?? throw new Exception("Patient not found");
        //return Mapper.MapFromPatientNode(p);
    }
    public async Task<List<Dictionary<string, object>>> FetchPatientAttendingDoctors(string id)
    {
        var currentDocs = await _patientRepository.PatientAttendingDoctors(id);
        _logger.LogInformation("FetchPatientAttendingDoctors >> PatientAttendingDoctors {id} >> {docs} > {size}", id,currentDocs,currentDocs.Count);
        return currentDocs;
    }

    public async Task<Treatment> CurrentPatientTreatment (string id)
    {
        var currentTreatment = await _patientRepository.CurrentPatientTreatment(id);
        return currentTreatment;
    }

    public async Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName)
    {
        return await _patientRepository.SearchPatientByFullName(firstName, lastName);
    }

    public async Task<string> CreatePatientRequest(string patientId, PatientRequest v)
    {
        return await _patientRepository.CreatePatientRequest(patientId, v.DoctorId, v.Action, v.Reason);

    }

    public async Task<List<Dictionary<string, object>>> GetAllDoctors(int pageSize, int pageIndex)
    {
        return await _doctorRepository.GetAllDoctors(pageIndex >=1 ? pageSize*pageIndex : 0); //umm skip be 0 at first..toReview&+**
    }

    public async Task<string> AddDoctor(Doctor doctor)
    {
        return await _doctorRepository.AddDoctor(Mapper.MapToDoctorNode(doctor));
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
            //todo** redundant operation if already patient_of....BUT useful to update r.fromAction?
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
            //yup to do >>add .to and create a new Treatment for versionning
            _logger.LogInformation("AddUpdatePatientTreatment :: HAS TREATMENT {doctorId} <-> {by}", docId, currentTreatment.By);

            return await _patientRepository.UpdatePatientTreatment(docId, patientId,name, details);
        }

        return await _doctorRepository.AddPatientTreatment(docId, patientId,name, details); //todo** check return at call site

    }
}
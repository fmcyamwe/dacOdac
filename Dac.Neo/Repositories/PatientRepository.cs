using Microsoft.Extensions.Logging;

using Dac.Neo.Data.Model;

//using Neo4j.Driver;

namespace Dac.Neo.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly INeo4jDataAccess _neo4jDataAccess;

    private readonly ILogger<PatientRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientRepository"/> class.
    /// </summary>
    public PatientRepository(INeo4jDataAccess neo4jDataAccess, ILogger<PatientRepository> logger)
    {
        _neo4jDataAccess = neo4jDataAccess;
        _logger = logger;
    }

    /// <summary>
    /// Search patient by First & Last Name--minimal info returned
    /// </summary>
    public async Task<List<Dictionary<string, object>>> SearchPatientByFullName(string firstName, string lastName)
    {
        //var query = @"MATCH (p:Patient) WHERE (toUpper(p.firstName) = toUpper($first) AND toUpper(p.lastName) = toUpper($last))
        //                   RETURN p{Id: p.id, firstName: p.firstName, lastName: p.lastName} ORDER BY p.firstName LIMIT 5";

        var query = @"MATCH (p:Patient) WHERE (p.upperFirstName CONTAINS toUpper($first) OR p.upperLastName CONTAINS toUpper($last))
                         RETURN p{Id: p.id, firstName: p.firstName, lastName: p.lastName} ORDER BY p.firstName LIMIT 5";

        IDictionary<string, object> parameters = new Dictionary<string, object> { { "first", firstName }, { "last", lastName } };

        var patients = await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "p", parameters);

        _logger.LogInformation("SearchPatientByFullName {count}", patients.Count); //should be one...toReview**

        return patients;

    }

    /// <summary>
    /// Add a new patient
    /// </summary>
    public async Task<string> AddPatient(PatientDB person)
    {
        if (person != null && !string.IsNullOrWhiteSpace(person.FirstName) && !string.IsNullOrWhiteSpace(person.LastName))
        {//todo** should remove this check with constraints change....

            Console.WriteLine("AddPatient {0} {1} >> {2}", person.FirstName, person.LastName, person.Id);

            var query = @"MERGE (p:Patient {upperFirstName: toUpper($firstName), upperLastName: toUpper($lastName)})
                            ON CREATE SET p.firstName = $firstName, p.lastName = $lastName, p.id = $id, p.born = $born, p.gender = $gender
                            ON MATCH SET p.born = $born, p.gender = $gender, p.updatedAt = timestamp()
                            RETURN p.id";

            //todo** match should update name too?
            //todo** Id should not be empty?

            IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "firstName", person.FirstName },
                    { "lastName", person.LastName },
                    { "born", person.Born ?? 0 },
                    { "id", person.Id ?? ""}, //Guid.NewGuid().ToString()[^12..] }, //more robust than Neo4J's ID(p) //only saving last part
                    { "gender", person.Gender ?? ""},
                    //todo** add other props and update in query
                };
            return await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);
        }
        else
        {
            throw new System.ArgumentNullException(nameof(person), "Person must not be null");
        }
    }

    /// <summary>
    /// Get count of patients
    /// </summary>
    public async Task<long> GetPatientsCount()
    {
        //MATCH (n:Patient)-[r:PATIENT_OF]-() RETURN n, count(DISTINCT r) AS num
        var query = @"MATCH (p:Patient) RETURN count(p) as patientCount";
        var count = await _neo4jDataAccess.ExecuteReadScalarAsync<long>(query);
        _logger.LogInformation("GetPatientsCount {count}", count);
        return count;
    }

    /// <summary>
    /// Random patient for login.
    /// </summary>
    public async Task<string> RandomPatient()
    {

        var query = @"MATCH (d:Patient)-[r:REQUESTED]-() with d, count(distinct r) as i 
                    RETURN d.id order by i desc limit 1";

        return await _neo4jDataAccess.ExecuteReadScalarAsync<string>(query);
        //return count;
    }

    /// <summary>
    /// Patient Request from a Doctor that can end up with a Doctor-Patient relation
    /// </summary>
    public async Task<string> CreatePatientRequest(string patientId, string docId, string action, string reason)
    {
        var query = @"MATCH (p:Patient) WHERE p.id = $patientId 
                    MATCH (d:Doctor) WHERE d.id = $docId
                    WITH p, d
                    MERGE (p)-[r:REQUESTED {action: $action, reason:$reason, status:'pending', date:timestamp()}]->(d)
                    RETURN r.status";
        
        IDictionary<string, object> parameters = new Dictionary<string, object> 
        {
            { "patientId",patientId },
            { "docId", docId},
            { "action",action},
            { "reason",reason}
        };

        _logger.LogInformation("CreatePatientRequest {patient}--{action}->{docId}", patientId, action, docId);

        return await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);
    }

    /// <summary>
    /// List Patients with minimal information
    /// </summary>
    public async Task<List<Dictionary<string, object>>> GetAllPatients(int skipPaginate) //todo** pass in pageRequest too...
    {
        var query = @"Match (p:Patient) RETURN p{ Id: p.id, firstName: p.firstName, lastName: p.lastName } 
                    ORDER BY p.lastName SKIP $skip 
                    LIMIT 3"; 
        //default 10..todo* readd after pagination tests
        //coalesce for missing info prolly --todo**

        IDictionary<string, object> parameters = new Dictionary<string, object> { { "skip", skipPaginate } };

        var patients = await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "p", parameters); //skip

        _logger.LogInformation("GetAllPatients {count}", patients.Count);

        return patients;
    }

    /// <summary>
    /// Update a Patient ID--only the main information.
    /// </summary>
    public async Task<string> UpdatePatient(string id, PatientDB person)
    {
        return await Task.Factory.StartNew(() => "true"); //todo**

    }

    /// <summary>
    /// Retrieve patient by ID--everything about the patient
    /// </summary>
    public async Task<PatientDB> FetchPatientByID(string id)
    {
        var query = @"Match (p:Patient {id: $id}) RETURN p";
        //RETURN p{ Id: p.id, FirstName: p.firstName, LastName: p.lastName, Born: COALESCE(p.born, 0), Gender: COALESCE(p.gender, '') }";
        //toSee...with above...think just had to upper case return?!? or cause of empty properties? >> nope bork as casting from INode smh
        //also cast born to int? meh no need toInteger()

        IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id", id },
            };

        var patient = await _neo4jDataAccess.ExecuteReadScalarToModelAsync<PatientDB>(query, "p", parameters); //<ILookupDataResponse<PatientDB>>
                                                                                                               //error out >>  Unable to cast object of type `Neo4j.Driver.Internal.Types.Node` to type `Dac.Neo.Model.Patient`.
                                                                                                               //prolly should be explicit in query with return values- >>still borks
                                                                                                               //or just use Neo4J Extension package? >>tryin...(aint looking good >>gotta add lower Driver version...coliiiis)

        //OR use <object> and cast/build into Patient here? >>first try was half successful at least--toTry** if Extension dont work
        //_logger.LogInformation("FetchPatientByID {patient}", patient); //>>type {type} //patient.GetType()

        //todo** catch exception when not found and return null
        /*try
        {
            var p = patient.As<Patient>(); //bon to see >>nope still borks BUT does bring object back!!
            return p;
        }catch (Exception ex){
            _logger.LogTrace(ex, "FetchPatientByID Exception!! {patient}", patient);
            throw;
        }*/
        //return patient.As<Patient>();
        //return patient;//.As<Patient>();
        return patient;
    }

    public async Task<List<Dictionary<string, object>>> PatientAttendingDoctors(string id)
    {
        var query = @"MATCH (p:Patient {id: $id})
                        OPTIONAL MATCH (p)-[r:PATIENT_OF]->(d:Doctor) WHERE r.to IS NULL
                        RETURN p{doctorId: d.id, doctorName: d.firstName+' '+d.lastName, speciality: d.speciality, since: r.since, fromAction: r.fromAction }";
        //could just return d, r  smh

        IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id", id },
            };

        return await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "p", parameters);
    }

    public async Task<string> AddAttendingDoctor(string patientId, string docId, string triggerAction)
    {
        var query = @"MATCH (p:Patient {id: $patientId})
                        MATCH (d:Doctor {id: $docId})
                        MERGE (p)-[r:PATIENT_OF]->(d)
                        ON CREATE SET r.since = timestamp(), r.fromAction = $action
                        ON MATCH SET r.fromAction = r.fromAction+'<->'+$action, r.updatedAt = timestamp()
                        RETURN r.fromAction";//umm match toTest** no overwrite of fromAction  

        IDictionary<string, object> parameters = new Dictionary<string, object> {
                { "docId", docId },
                { "patientId", patientId},
                { "action", triggerAction},
            };

        return await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);

    }

    public async Task<List<Dictionary<string, object>>> PatientMedicalHistory(string id)
    {
        // OPTIONAL MATCH (p)-[r:PATIENT_OF]->(d:Doctor) WHERE r.to IS NULL
        //umm better to do small query instead?!? and merge data... especially for order...

        //todo**
        //MATCH (p:Patient) WHERE p.id = $id
        //get all requests
        //MATCH p=(a:Patient {id: '544e30f81841'})-[r:REQUESTED]->() RETURN r order by r.date
        //get all Treatments
        //get all attending doctors >>done >>PatientAttendingDoctors()
        //order by date or something....
        //umm need doctor?
        //order by r.date first then h.from? or both at end? toTest
        var query = @"OPTIONAL MATCH (p:Patient {id: $id})-[r:REQUESTED]->(d) with p, r,d 
                        OPTIONAL MATCH (p)-[h:HAS_TREATMENT]->(t:Treatment)
                        RETURN {daP:p.id,daDoc:d.id,from:h.from,deetsN:t.name, deets:t.details, date:r.date} as w";
        //RETURN p,d,h,t, r order by r.date"; 
        //{daP:p,daDoc:d,from:h,deets:t, date:r.date} as w 
        //{daP:p.id,daDoc:d.id,from:h.from,deets:t.details, date:r.date} as w 

        //"OPTIONAL MATCH (p:Patient {id: $id})-[r:REQUESTED]->() RETURN r order by r.date
        //OPTIONAL MATCH (p:Patient {id: $id})-[r:HAS_TREATMENT]->() RETURN r order by r.date
        //RETURN p{doctorId: d.id, doctorName: d.firstName+' '+d.lastName, speciality: d.speciality, since: r.since, fromAction: r.fromAction }";
        //could just return d, r  smh
        IDictionary<string, object> parameters = new Dictionary<string, object> {
                { "id", id }
            };

        return await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "w", parameters);   //Task.Factory.StartNew(() => new List<Dictionary<string, object>>());

    }

    /// <summary>
    /// Fetches current Patient treatment
    /// </summary>
    public async Task<Treatment> CurrentPatientTreatment(string id)
    {
        var query = @"MATCH (a:Patient {id:$id})-[r:HAS_TREATMENT]->(t:Treatment)
                        WHERE r.to is null
                        RETURN t";
        //WITH t, r ORDER BY r.from LIMIT 1  >>toTest** if should add!!
        //RETURN t.name, t.by;

        IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

        //toTest or need manual return?
        return await _neo4jDataAccess.ExecuteReadScalarToModelAsync<Treatment>(query, "t", parameters);
    }

    /// <summary>
    /// Update a Patient's Treatment
    /// </summary>
    public async Task<string> UpdatePatientTreatment(string docId, string patientId, string name, string details)
    {
        var query = @"MATCH (p:Patient {id:$patientId})-[r:HAS_TREATMENT]->(:Treatment)
                        WHERE r.to is null
                        WITH p, r
                        SET r.to = timestamp(), r.updatedBy = $docId
                        CREATE (p)-[:HAS_TREATMENT {from: timestamp()}]->(t:Treatment {by: $docId, name:$name, details:$details, startDate:timestamp()})
                        RETURN t.by";
        //OPTIONAL MATCH? limit?!?  and updatedBy? //also create?
        // (d)<-[r:PATIENT_OF]-(p:Patient) WHERE r.to IS NULL
        IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "patientId",patientId},
                { "docId", docId},
                { "name", name},
                { "details", details}
            };

        return await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);
    }

    /// <summary>
    /// Deletes a Patient by ID
    /// </summary>
    public async Task<bool> DeletePatient(string id)
    {
        //var query = @"MATCH (a:Patient {id: $patientId})DETACH DELETE a";
        var query = @"OPTIONAL MATCH (a:Patient {id: $patientId}) DETACH DELETE a
                    RETURN CASE 
                     WHEN a IS NOT NULL THEN true
                     ELSE false
                    END AS result";

        IDictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "patientId",id}
        };

        return await _neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
    }
}
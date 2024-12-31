using Microsoft.Extensions.Logging;
using Dac.Neo.Model; 
using Dac.Neo.Data;
using Dac.Neo.Data.Model;
//using Neo4j.Driver;

namespace Dac.Neo.Repositories;
    
    public class PatientRepository : IPatientRepository
    {
        private INeo4jDataAccess _neo4jDataAccess;

        private ILogger<PatientRepository> _logger;

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
            var query = @"MATCH (p:Patient) WHERE (toUpper(p.firstName) = toUpper($first) AND toUpper(p.lastName) = toUpper($last))
                                RETURN p{Id: p.id, firstName: p.firstName, lastName: p.lastName} ORDER BY p.firstName LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "first", firstName },{ "last", lastName } };

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
            {
                Console.WriteLine("AddPatient {0} {1}", person.FirstName, person.LastName);

                var query = @"MERGE (p:Patient {firstName: $firstName, lastName: $lastName})
                            ON CREATE SET p.id = $id, p.born = $born, p.gender = $gender
                            ON MATCH SET p.born = $born, p.updatedAt = timestamp() RETURN p.id";
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "firstName", person.FirstName },
                    { "lastName", person.LastName },
                    { "born", person.Born ?? 0 },
                    { "id", Guid.NewGuid().ToString()[^12..] }, //more robust than Neo4J's ID(p) //only saving last part
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
        public async Task<long> GetPatientCount()
        {
            var query = @"Match (p:Patient) RETURN count(p) as patientCount";
            var count = await _neo4jDataAccess.ExecuteReadScalarAsync<long>(query);
            _logger.LogInformation("GetPatientCount {count}", count);
            return count;
        }

        public async Task<string> CreatePatientRequest(string patientId, string docId, string action, string reason)
        {
            var query = @"MATCH (p:Patient) WHERE p.id = $patientId 
                        MATCH (d:Doctor) WHERE d.id = $docId
                        WITH p, d
                        MERGE (p)-[r:REQUESTED {action: $action, reason:$reason, status:'pending', date:timestamp()}]->(d)
                        RETURN r.status";
                        //status could be needed--toReview


            IDictionary<string, object> parameters = new Dictionary<string, object> { 
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
        public async Task<List<Dictionary<string, object>>> GetAllPatients()
        {
            var query = @"Match (p:Patient) RETURN p{ Id: p.id, firstName: p.firstName, lastName: p.lastName } LIMIT 20"; 
            //order by?
            //coalesce for missing info prolly --todo**
            //should upper-case first letter --todo**
            var patients = await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "p");

            _logger.LogInformation("GetAllPatients {count}", patients.Count);

            return patients;
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
            _logger.LogInformation("FetchPatientByID {patient} >>type {type}", patient,patient.GetType());
            //Console.WriteLine("FetchPatientByID {0} >>type {1}", patient, patient.GetType()); //toSee
            //_logger.LogInformation("ExecuteReadScalarAsync {}", typeof(T));
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
    }
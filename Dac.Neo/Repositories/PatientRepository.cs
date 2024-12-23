using Dac.Neo.Model; 
using Neo4j.Driver;
using Microsoft.Extensions.Logging;

namespace Dac.Neo.Repositories;
    
    //should be in own file but for brevity
    public interface IPatientRepository
    {
        Task<List<Dictionary<string, object>>> SearchPatientByName(string searchString); //todo** also search by contactInfo? 

        Task<bool> AddPatient(Patient person); 
        //todo** 
        //updatePatient?
        //addMedicalRecord

        Task<long> GetPatientCount(); //todo** for Doctor returns how many Patients treating?
    }

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
        /// Searches the name of the patient.
        /// </summary>
        public async Task<List<Dictionary<string, object>>> SearchPatientByName(string searchString)
        {
            var query = @"MATCH (p:Patient) WHERE toUpper(p.name) CONTAINS toUpper($searchString) 
                                RETURN p{ name: p.name, born: p.born } ORDER BY p.Name LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", searchString } };

            var patients = await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "p", parameters);

            _logger.LogInformation("SearchPatientByName {count}", patients.Count());

            return patients;
        }

        /// <summary>
        /// Adds a new patient
        /// </summary>
        public async Task<bool> AddPatient(Patient person)
        {
            if (person != null && !string.IsNullOrWhiteSpace(person.FirstName) && !string.IsNullOrWhiteSpace(person.LastName))
            {
                var query = @"MERGE (p:Patient {name: $name}) ON CREATE SET p.born = $born 
                            ON MATCH SET p.born = $born, p.updatedAt = timestamp() RETURN true";
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "firstName", person.FirstName },
                    { "lastName", person.LastName },
                    { "born", person.Born ?? 0 },
                    { "id",Guid.NewGuid().ToString() }
                    //todo** add other props
                };
                return await _neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
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
            return count;
        }
    }
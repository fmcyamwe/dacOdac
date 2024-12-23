using Dac.Neo.Model; 
using Neo4j.Driver;
using Microsoft.Extensions.Logging;

namespace Dac.Neo.Repositories;
    
    //should be in own file but for brevity
    public interface IDoctorRepository
    {
        Task<List<Dictionary<string, object>>> SearchPatientByName(string searchString); //todo** also search by contactInfo? 

        Task<bool> AddDoctor(Doctor person); 
        //todo** 
        //updatePatient?
        //addMedicalRecord

        Task<long> GetPatientCount(); //todo** for Doctor returns how many Patients treating?
    }

    public class DoctorRepository : IDoctorRepository
    {
        private INeo4jDataAccess _neo4jDataAccess;

        private ILogger<DoctorRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorRepository"/> class.
        /// </summary>
        public DoctorRepository(INeo4jDataAccess neo4jDataAccess, ILogger<DoctorRepository> logger)
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

            return patients;
        }

        /// <summary>
        /// Adds a doctor
        /// </summary>
        public async Task<bool> AddDoctor(Doctor doctor)
        {
            if (doctor != null && !string.IsNullOrWhiteSpace(doctor.Name))
            {
                var query = @"MERGE (p:Doctor {name: $name}) ON CREATE SET p.born = $born 
                            ON MATCH SET p.born = $born, p.updatedAt = timestamp() RETURN true";
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "name", doctor.Name },
                    { "born", doctor.Born ?? 0 }
                    //todo** add other props
                };
                return await _neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
            }
            else
            {
                throw new System.ArgumentNullException(nameof(doctor), "Person must not be null");
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
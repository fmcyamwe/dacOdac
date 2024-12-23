using Dac.Neo.Model; 
using Dac.Neo.Repositories;
using Microsoft.Extensions.Logging;

namespace Dac.Neo.Data;
    
    //should be in own file but for brevity
    public interface ISeeder
    {
        Task<bool> CreateConstraints(); //creates an index underneath

        Task<bool> AddDummyData(); 
       
    }

    public class SeedGraphDB : ISeeder
    {
        private INeo4jDataAccess _neo4jDataAccess;

        private ILogger<SeedGraphDB> _logger;

        /// <summary>
        /// Initializes index and add some data on startup.
        /// </summary>
        public SeedGraphDB(INeo4jDataAccess neo4jDataAccess, ILogger<SeedGraphDB> logger)
        {
            _neo4jDataAccess = neo4jDataAccess;
            _logger = logger;
        }

        /// <summary>
        /// Creates Unique constraint on names of the patient.
        /// </summary>
        public async Task<bool> CreateConstraints()
        {
            var query = @"CREATE CONSTRAINT person_unique IF NOT EXISTS 
            FOR (n:Patient) REQUIRE (n.firstName, n.lastName) IS UNIQUE";

            Console.WriteLine("coliiis::CreateConstraints");
        
            //_logger.LogInformation("SearchPatientByName {count}", patients.Count());
            return await _neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query);
        }

        /// <summary>
        /// Adds a new patient
        /// </summary>
        public async Task<bool> AddDummyData()
        {
            var query = @"MERGE (p:Patient {name: $name}) ON CREATE SET p.born = $born 
                            ON MATCH SET p.born = $born, p.updatedAt = timestamp() RETURN true";
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "name", "person.Name" },
                    { "born", ""}, //person.Born ?? 0 
                    { "id",Guid.NewGuid().ToString() }
                    //todo** add other props
                };
            
            return await _neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
            
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

using Microsoft.Extensions.Logging;

namespace Dac.Neo.Data.Configurations;

//should be in own file but for brevity
public interface ISeeder
    {
        Task<bool> AlreadyPopulated();
        Task CreatePatientNodeConstraints(); //creates indexes underneath
        Task CreateDoctorNodeConstraints();
        Task<bool> AddDummyData(); 
       
    }

    public class Seeder : ISeeder, IDisposable
    {
        private INeo4jDataAccess _neo4jDataAccess;

        private ILogger<Seeder> _logger;

        /// <summary>
        /// Initializes indexes and seed with some data on startup.
        /// </summary>
        public Seeder(INeo4jDataAccess neo4jDataAccess, ILogger<Seeder> logger)
        {
            _neo4jDataAccess = neo4jDataAccess;
            _logger = logger;
            Console.WriteLine("in Seeder!! OYEEE!");
        }

        public Seeder(){
                    
            var e =  AlreadyPopulated();
            _logger.LogInformation("huh here? {e}",e); 
            Console.WriteLine("in Seeder::Seeder!! OYEEE!");
            //toSee if can initialize stuff here during registration?
            
            return;
        }

        /// <summary>
        /// Check if Graph database is already populated.
        /// </summary>
        public async Task<bool> AlreadyPopulated()
        {
            var query = @"MATCH (n) RETURN COUNT(n) > 0 AS hasData";
            
            var hasData = await _neo4jDataAccess.ExecuteReadScalarAsync<bool>(query);
            _logger.LogInformation("AlreadyPopulated > {hasData}", hasData);

            return hasData;
        }

        /// <summary>
        /// Create unique constraint on Names of the patient.
        /// </summary>
        public async Task CreatePatientNodeConstraints()
        {
            var query = @"CREATE CONSTRAINT patient_unique IF NOT EXISTS 
            FOR (n:Patient) REQUIRE (n.firstName, n.lastName) IS UNIQUE"; //could also make it as Node Key

            _logger.LogInformation("Creating NameConstraint");
            
            await _neo4jDataAccess.ExecuteWriteTransactionAsync(query);

            //also add constraint on id property
            var patient = @"CREATE CONSTRAINT patient_node_id IF NOT EXISTS
            FOR (n:Patient) REQUIRE (n.id) IS NODE KEY"; 

            await _neo4jDataAccess.ExecuteWriteTransactionAsync(patient);
            return;
        }

        /// <summary>
        /// Creates unique constraint on Doctor Node for lastName and id
        /// </summary>
        public async Task CreateDoctorNodeConstraints()
        {
            ////making it as Node Key >>could be multiple doctors with same lastName?
            ///--could make speciality part of key?!? >>yup better
            ///umm better as Unique actually?
            var query = @"CREATE CONSTRAINT doctor_unique IF NOT EXISTS
            FOR (n:Doctor) REQUIRE (n.lastName, n.id) IS NODE KEY"; //todo** add speciality**

            _logger.LogInformation("Creating DoctorNodeConstraints");
        
            await _neo4jDataAccess.ExecuteWriteTransactionAsync(query);
            
        }

        /// <summary>
        /// Add some initial Data
        /// </summary>
        public async Task<bool> AddDummyData()
        {
            //todo** either load csv or some else...
            return await Task.Factory.StartNew(() => true);
            
        }

        public void Dispose()
        {
            //_ = _neo4jDataAccess.DisposeAsync();
            _logger.LogInformation("SeedNeoDB Graph disposed!");
        }
    }
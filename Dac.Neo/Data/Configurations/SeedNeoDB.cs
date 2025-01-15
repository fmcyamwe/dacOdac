
using Dac.Neo.Data.Model;
using Microsoft.Extensions.Logging;

namespace Dac.Neo.Data.Configurations;

//should be in own file but for brevity
public interface ISeeder
    {
        Task<bool> AlreadyPopulated();
        Task CreatePatientNodeConstraints(); //creates indexes underneath
        Task CreateDoctorNodeConstraints();
        Task<List<string>> SeedPatientData(string path); 
        Task<List<string>> SeedDoctorData(string path); 
        Task <bool> SeedRelations(List<string> doc, List<string> pts);
    }

    public class Seeder : ISeeder, IDisposable
    {
        private readonly INeo4jDataAccess _neo4jDataAccess;

        private readonly ILogger<Seeder> _logger;

        /// <summary>
        /// Initializes indexes and seed with some data on startup.
        /// </summary>
        public Seeder(INeo4jDataAccess neo4jDataAccess, ILogger<Seeder> logger)
        {
            _neo4jDataAccess = neo4jDataAccess;
            _logger = logger;
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
            //no need for both to be present
            //using pre-normalized names as indexes instead of (n.firstName, n.lastName)
            //upperFirstName == firstName
            //upperLastName ==  lastName
            var query = @"CREATE CONSTRAINT patient_unique_names IF NOT EXISTS 
            FOR (n:Patient) REQUIRE (n.upperFirstName, n.upperLastName) IS UNIQUE"; 

            _logger.LogInformation("Creating NameConstraint");
            
            await _neo4jDataAccess.ExecuteWriteTransactionAsync(query);

            //also add constraint on id property
            var patient = @"CREATE CONSTRAINT patient_node_key IF NOT EXISTS
            FOR (n:Patient) REQUIRE (n.id) IS NODE KEY";

            await _neo4jDataAccess.ExecuteWriteTransactionAsync(patient);
        }

        /// <summary>
        /// Creates unique constraint on Doctor Node for lastName and id
        /// </summary>
        public async Task CreateDoctorNodeConstraints()
        {
            ////making it as Node Key >>could be multiple doctors with same lastName?
            ///--making it possible via different speciality
            ///--could make speciality part of key?!? >>yup better speciality
            ///upperLastName == lastName
            var query = @"CREATE CONSTRAINT doctor_node_key IF NOT EXISTS
            FOR (n:Doctor) REQUIRE (n.upperLastName, n.speciality, n.id) IS NODE KEY"; 

            _logger.LogInformation("Creating DoctorNodeConstraints");
        
            await _neo4jDataAccess.ExecuteWriteTransactionAsync(query); 
        }

        /// <summary>
        /// Add some initial Patient Data
        /// </summary>
        public async Task<List<string>> SeedPatientData(string sourcePath)
        {
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonExtensions.FromJson<PatientDB[]>(sourceJson); 

            var query = @"MERGE (p:Patient {upperFirstName: toUpper($firstName), upperLastName: toUpper($lastName)})
                        ON CREATE SET p.firstName = $firstName, p.lastName = $lastName, p.id = $id, p.born = $born, p.gender = $gender
                        ON MATCH SET p.born = $born, p.gender = $gender, p.updatedAt = timestamp()
                        RETURN p.id";

            List<string> ls = [];
  
            foreach (var person in sourceItems)
            {
                var id = Guid.NewGuid().ToString()[^12..];
                
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "firstName", person.FirstName },
                    { "lastName", person.LastName },
                    { "born", person.Born ?? 0 },
                    { "id", id}, 
                    { "gender", person.Gender ?? ""},
                    //todo** add other props and update in query
                };

                var ret = await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);
                ls.Add(ret);
            }
            
            return await Task.Factory.StartNew(() =>ls);
        }

        /// <summary>
        /// Add some initial Doctor Data
        /// </summary>
        public async Task<List<string>> SeedDoctorData(string sourcePath)
        {
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonExtensions.FromJson<DoctorDB[]>(sourceJson); 
            
            List<string> ls = [];

            var query = @"MERGE (d:Doctor {upperLastName: toUpper($lastName), speciality: $speciality})
                        ON CREATE SET d.id = $id, d.firstName = $firstName, d.lastName = $lastName, d.practiseSince = $practiseSince
                        ON MATCH SET d.firstName = $firstName, d.speciality = $speciality, d.updatedAt = timestamp() RETURN d.id";
                         
            foreach (var doctor in sourceItems)
            {
                var id = Guid.NewGuid().ToString()[^12..]; ////Guid.NewGuid().ToString()[^12..],  33d488c2-5bfd-4139-957d-4ac8e4ec9910 > 4ac8e4ec9910
            
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "lastName", doctor.LastName },
                    { "firstName", doctor.FirstName?.Trim() ?? "Dr"}, //default to 'Dr'
                    { "speciality", doctor.Speciality ?? ""},
                    { "id", id}, //doctor.Id ?? "" 
                    { "practiseSince", doctor.PractiseSince ?? DateTime.Now}
                };

                var ret = await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);
                ls.Add(ret);
            }
              
            return await Task.Factory.StartNew(() => ls);
        }

        public async Task <bool> SeedRelations(List<string> docs, List<string> pts)
        {
            Random rnd = new();

            int r = rnd.Next(docs.Count);
            int i = 0;

            var query = @"MATCH (p:Patient) WHERE p.id = $patientId 
                    MATCH (d:Doctor) WHERE d.id = $docId
                    WITH p, d
                    MERGE (p)-[r:REQUESTED {action: $action, reason:$reason, status:'pending', date:timestamp()}]->(d)
                    RETURN r.status";

            while (i < pts.Count)
            {
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "patientId",pts[i] },
                    { "docId", docs[r]},
                    { "action","checkup"},
                    { "reason","Be my doctor?"}
                };
                
                await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);
                i++;
                r = rnd.Next(docs.Count); //ummm
            }

            return true; //Task.Factory.StartNew(() => true);
        }

        public void Dispose()
        {
            _ = _neo4jDataAccess.DisposeAsync();
            _logger.LogInformation("SeedNeoDB Graph disposed!");
        }
    }
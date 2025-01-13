
using Dac.Neo.Data.Model;
using Microsoft.Extensions.Logging;

namespace Dac.Neo.Data.Configurations;

//should be in own file but for brevity
public interface ISeeder
    {
        Task<bool> AlreadyPopulated();
        Task CreatePatientNodeConstraints(); //creates indexes underneath
        Task CreateDoctorNodeConstraints();
        Task<bool> SeedPatientData(string path); 
        Task<bool> SeedDoctorData(string path); 
       
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

        /*public Seeder(){
                    
            var e =  AlreadyPopulated();
            _logger.LogInformation("Seeder::huh here in Seeder? {e}",e); 
            Console.WriteLine("in Seeder::Seeder!! OYEEE! OYEEE");
            //could initialize stuff here during registration?
            
            return;
        }*/

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
            FOR (n:Patient) 
            REQUIRE (n.upperFirstName, n.upperLastName) IS UNIQUE"; 

            _logger.LogInformation("Creating NameConstraint");
            
            await _neo4jDataAccess.ExecuteWriteTransactionAsync(query);

            //also add constraint on id property
            var patient = @"CREATE CONSTRAINT patient_node_key IF NOT EXISTS
            FOR (n:Patient) REQUIRE (n.id) IS NODE KEY"; //IS UNIQUE 

            await _neo4jDataAccess.ExecuteWriteTransactionAsync(patient);
            return;
        }

        /// <summary>
        /// Creates unique constraint on Doctor Node for lastName and id
        /// </summary>
        public async Task CreateDoctorNodeConstraints()
        {
            ////making it as Node Key >>could be multiple doctors with same lastName?
            ///--making it possible via different speciality
            ///--could make speciality part of key?!? >>yup better speciality
            ///umm better as Unique actually? meh better added as part of node key
            ///upperLastName == lastName
            var query = @"CREATE CONSTRAINT doctor_node_key IF NOT EXISTS
            FOR (n:Doctor) REQUIRE (n.upperLastName, n.speciality, n.id) IS NODE KEY"; 

            _logger.LogInformation("Creating DoctorNodeConstraints");
        
            await _neo4jDataAccess.ExecuteWriteTransactionAsync(query);
            
        }

        /// <summary>
        /// Add some initial Patient Data
        /// </summary>
        public async Task<bool> SeedPatientData(string sourcePath)
        {
            //todo** either load csv or some else...
            //var contentRootPath = "";
            
            //var sourcePath = Path.Combine(contentRootPath, "Data", "Configurations","seedPatient.json");
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonExtensions.FromJson<PatientDB[]>(sourceJson); 
            
            _logger.LogInformation("SeedPatientData:: {sourceItems}", sourceItems);
            Console.WriteLine("SeedPatientData:: {0}", sourceItems);

             var query = @"MERGE (p:Patient {upperFirstName: toUpper($firstName), upperLastName: toUpper($lastName)})
                            ON CREATE SET p.firstName = $firstName, p.lastName = $lastName, p.id = $id, p.born = $born, p.gender = $gender
                            ON MATCH SET p.born = $born, p.gender = $gender, p.updatedAt = timestamp()
                            RETURN p.id"; 
  
            // Insert nodes
            foreach (var person in sourceItems)
            {
                var id = Guid.NewGuid().ToString()[^12..];
                
                _logger.LogInformation("SeedPatientData:: {id}:{first}-{last} <>{gender}{born} :: {id}", person.Id, person.FirstName,  person.LastName, person.Gender, person.Born, id);
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "firstName", person.FirstName },
                    { "lastName", person.LastName },
                    { "born", person.Born ?? 0 },
                    { "id", id}, //oldie but just generating id here >> person.Id ?? "" 
                    { "gender", person.Gender ?? ""},
                    //todo** add other props and update in query
                };

                var ret = await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters); //should return?!? for and relationships ? toSee**
                _logger.LogInformation("SeedPatientData:: END for {id}:{id} >> {ret} \n", person.Id, id, ret);
            }
            
            return await Task.Factory.StartNew(() => sourceItems == null);
        }

        /// <summary>
        /// Add some initial Doctor Data
        /// </summary>
        public async Task<bool> SeedDoctorData(string sourcePath)
        {
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonExtensions.FromJson<DoctorDB[]>(sourceJson); 
            
            _logger.LogInformation("SeedDoctorData:: {sourceItems}", sourceItems);
            Console.WriteLine("SeedDoctorData:: {0}", sourceItems);

            var query = @"MERGE (d:Doctor {upperLastName: toUpper($lastName), speciality: $speciality})
                        ON CREATE SET d.id = $id, d.firstName = $firstName, d.lastName = $lastName, d.practiseSince = $practiseSince
                        ON MATCH SET d.firstName = $firstName, d.speciality = $speciality, d.updatedAt = timestamp() RETURN d.id";
                         
            // Insert nodes
            foreach (var doctor in sourceItems)
            {
                var id = Guid.NewGuid().ToString()[^12..]; ////Guid.NewGuid().ToString()[^12..],  33d488c2-5bfd-4139-957d-4ac8e4ec9910 > 4ac8e4ec9910
                _logger.LogInformation("SeedDoctorData:: {id}:{first}-{last} <>{speciality} >> {id}", doctor.Id, doctor.FirstName, doctor.LastName, doctor.Speciality,id);
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "lastName", doctor.LastName },
                    { "firstName", doctor.FirstName?.Trim() ?? "Dr"}, //default to 'Dr'
                    { "speciality", doctor.Speciality ?? ""},
                    { "id", id}, //doctor.Id ?? "" 
                    { "practiseSince", doctor.PractiseSince ?? DateTime.Now}
                };

                var ret = await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters); ////should return for and relationships? toSee**
                //_logger.LogInformation("SeedDoctorData:: END for {id}:{f} >> {ret} \n", doctor.Id, id, ret);
            }
              
            return await Task.Factory.StartNew(() => sourceItems == null);
        }

        public void Dispose()
        {
            //_ = _neo4jDataAccess.DisposeAsync();
            _logger.LogInformation("SeedNeoDB Graph disposed!");
        }
    }
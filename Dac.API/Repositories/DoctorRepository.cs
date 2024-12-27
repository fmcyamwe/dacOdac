using Dac.Neo.Model; 
using Dac.Neo.Data;
using Neo4j.Driver;

namespace Dac.API.Repositories;
    
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
        /// Searches Doctor by lastName
        /// </summary>
        public async Task<List<Dictionary<string, object>>> SearchDoctorByName(string lastName)
        {
            var query = @"MATCH (d:Doctor) WHERE toUpper(d.lastName) CONTAINS toUpper($searchString) 
                            RETURN d{Id: d.id, firstName: d.firstName, lastName: d.lastName, speciality: COALESCE(d.speciality, '') } ORDER BY d.lastName LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", lastName } };

            var doctors = await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "d", parameters);

            _logger.LogInformation("SearchDoctorByName {count}", doctors.Count);

            return doctors;
        }

        /// <summary>
        /// List Doctors by Speciality.
        /// </summary>
        public async Task<List<Dictionary<string, object>>> ListDoctorsBySpeciality(string speciality)
        {
            var query = @"MATCH (d:Doctor {speciality: $searchString})
            RETURN d{ Id: d.id, firstName: d.firstName, lastName: d.lastName, speciality: d.speciality} ORDER BY d.lastName"; //LIMIT 5
            //umm exact match..should use contain ? toReview** 
            //DEF toUpper(on entry maybe) as would help with grouping in other endopoints too ---todo**

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", speciality } };

            var doctors = await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "d", parameters);

            return doctors;
        }

        /// <summary>
        /// Count of Doctors by Speciality.
        /// </summary>
        public async Task<List<Dictionary<string, object>>> DoctorsCountBySpeciality()
        {
            //def gotta run this b4 hand!   
            var query = @"MATCH (d:Doctor) WITH d RETURN distinct d.speciality, count(d) "; //ORDER BY d.speciality 

            //IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", speciality } };

            var doctors = await _neo4jDataAccess.ExecuteReadDictionaryAsync(query, "d"); //parameters

            return doctors;

        }

        /// <summary>
        /// Add a doctor
        /// </summary>
        public async Task<string> AddDoctor(Doctor doctor)
        {
            if (doctor != null && !string.IsNullOrWhiteSpace(doctor.LastName))
            {
                var query = @"MERGE (d:Doctor {lastName: $lastName})
                            ON CREATE SET d.id = $id, d.firstName = $firstName, d.speciality = $speciality
                            ON MATCH SET d.firstName = $firstName, d.speciality = $speciality, d.updatedAt = timestamp() RETURN d.id"; //RETURN true
                IDictionary<string, object> parameters = new Dictionary<string, object> 
                { 
                    { "lastName", doctor.LastName },
                    { "firstName", doctor.FirstName?.Trim() ?? "Dr"}, //default to 'Dr'
                    { "speciality", doctor.Speciality ?? ""}, //toReview** if shouldnt be requiered
                    { "id", Guid.NewGuid().ToString()[^12..]}, //robust than Neo4J's ID(p) //saving last part >> NewGuid.ToString()[^12..]
                    { "practisingSince", doctor.PractisingSince ?? DateTime.Now}
                    
                };
                return await _neo4jDataAccess.ExecuteWriteTransactionAsync<string>(query, parameters);
            }
            else
            {
                throw new System.ArgumentNullException(nameof(doctor), "Doctor must not be null");
            }
        }

        /// <summary>
        /// Get count of patients in Doctor's care.
        /// </summary>
        public async Task<long> GetPatientsCount(string id)
        {
            //todo** use id and change to query Relationship
            var query = @"Match (d:Doctor) RETURN count(d) as patientCount";
            var count = await _neo4jDataAccess.ExecuteReadScalarAsync<long>(query);
            return count;
        }

        /// <summary>
        /// Get all patients treated by Doctor
        /// </summary>
        public Task<List<Dictionary<string, object>>> GetAllPatients(string id)
        {
            //todo** use id and change to query Relationship
            return Task.Factory.StartNew(() => new List<Dictionary<string, object>>());
        }

        public async Task<Doctor> FetchDoctorByID(string id)
        {
            var query = @"Match (d:Doctor {id: $id}) 
                        RETURN d{ Id: d.id, FirstName: d.firstName, LastName: d.lastName, Speciality: COALESCE(d.speciality,'') }";
                        
            IDictionary<string, object> parameters = new Dictionary<string, object> 
            { 
                { "id", id },
            };

            var doc = await _neo4jDataAccess.ExecuteReadScalarAsync<object>(query, parameters);
            //error out >>  Unable to cast object of type `Neo4j.Driver.Internal.Types.Node` to type `Dac.Neo.Model.Patient`.
            //prolly should be explicit in query with return values >>good but still borks
            //OR use <object> and cast/build into Doctor here? toSee**

            _logger.LogInformation("FetchDoctorByID {doc}", doc);
            Console.WriteLine("FetchDoctorByID {0} >>type {1}", doc,doc.GetType());

            try
            {
                var p = doc.As<Doctor>(); //bon to see
                return p;
            }catch (Exception ex){
                _logger.LogTrace(ex, "FetchDoctorByID Exception!! {doc}", doc);
                throw;
            }
            //return doc;
        }
        
    }
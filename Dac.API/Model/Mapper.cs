using Dac.Neo.Data.Model;
using Dac.Neo.Model;

namespace Dac.API.Model; 

public class Mapper   //mapping from Neo4J to API models
{
    public static Patient MapToPatient(PatientDB entity)
    {
        return new Patient
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Born = entity.Born,
            Gender = entity.Gender,
            //FormattedCreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public static PatientDB MapToPatientDB(Patient p)
    {
        return new PatientDB
        {
            Id =  p.Id ?? "", //umm should use Guid here? >> toReview**
            FirstName = p.FirstName,
            LastName = p.LastName,
            Born = p.Born,
            Gender = p.Gender,
            //FormattedCreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };

    }

    public static Doctor MapToDoctor(DoctorDB entity)
    {
        return new Doctor
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Speciality = entity.Speciality ?? "", //should not be null --toTest**
            //FormattedCreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public static DoctorDB MapToDoctorDB(Doctor d)
    {
        return new DoctorDB
        {
            Id = d.Id,
            FirstName = d.FirstName,
            LastName = d.LastName,
            Speciality = d.Speciality ?? "", //should not be null --toTest**
            //FormattedCreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
}
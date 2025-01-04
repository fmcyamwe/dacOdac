using Dac.Neo.Data.Model;

namespace Dac.API.Model; 

public class Mapper   //mapping from Neo4J to API models
{
    public static Patient MapFromPatientNode(PatientDB entity)
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

    public static PatientDB MapToPatientNode(Patient p)
    {
        return new PatientDB
        {
            Id =  p.Id ?? Guid.NewGuid().ToString()[^12..] , //p.Id ?? "", // generate GUIID here
            FirstName = p.FirstName,
            LastName = p.LastName,
            Born = p.Born,
            Gender = p.Gender,
            //FormattedCreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };

    }

    public static Doctor MapFromDoctorNode(DoctorDB entity)
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

    public static DoctorDB MapToDoctorNode(Doctor d)
    {
        return new DoctorDB
        {
            Id = d.Id ?? Guid.NewGuid().ToString()[^12..],
            FirstName = d.FirstName ?? "Dr.", //default to 'Dr.'
            LastName = d.LastName,
            Speciality = d.Speciality ?? "", //should not be null --toTest**
            //FormattedCreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
}
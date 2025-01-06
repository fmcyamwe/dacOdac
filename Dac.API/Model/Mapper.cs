using Dac.Neo.Data.Model;

namespace Dac.API.Model; 

public class Mapper   //mapping from Neo4J to API models
{
    public static Patient MapFromPatientNode(PatientDB entity, List<AttendingDoctor>? attendingDocs = null, Treatment? treatment = null)
    {
        return new Patient
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Born = entity.Born,
            Gender = entity.Gender,
            AttendingDoctors = attendingDocs, //attendingDocs?.Select(c => new Doctor{Id = c.doctorId, FirstName =c.doctorName, Speciality: c.speciality} ),
            CurrentTreatment = treatment ?? new Treatment(),
           //since: r.since, fromAction: r.fromAction
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

    ////p{doctorId: d.id, doctorName: d.firstName+' '+d.lastName, speciality: d.speciality, since: r.since, fromAction: r.fromAction }";
    public static AttendingDoctor MapDictionaryToModel(Dictionary<string, object> dict)
    {
        return new AttendingDoctor(
            DoctorId: dict["doctorId"]?.ToString() ?? "", //null,
            DoctorName: dict["doctorName"]?.ToString() ?? "", //dict.ContainsKey("doctorName") ?  //null,
           // dict.ContainsKey("doctorName") && int.TryParse(dict["Property2"]?.ToString(), out int property2) ? property2 : 0,
            Speciality: dict["speciality"]?.ToString() ?? "", //dict.ContainsKey("speciality") ? 
            // dict.ContainsKey("Property3") && bool.TryParse(dict["Property3"]?.ToString(), out bool property3) ? property3 : false,
            Since: null,//dict.ContainsKey("since") && long.TryParse(dict["since"].ToString(), out long property2) ? property2 : 0,
            FromAction: dict["fromAction"]?.ToString() ?? "" //dict.ContainsKey("fromAction") ? //null,
        );
    }
}
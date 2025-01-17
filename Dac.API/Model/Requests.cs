using System.ComponentModel;

namespace Dac.API.Model; 


public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public record PaginationRequest(
    [property: Description("Number of items to return in single request")]
    [property: DefaultValue(10)]
    int PageSize = 10,

    [property: Description("The index of the page of results to return")]
    [property: DefaultValue(0)] //umm should be one for ease?
    int PageIndex = 0
);

public record PatientRequest( // or ActionRequest (to handle multiple 'action' types)
    [property: Description("The doctor's Id")]
    //[property: Required()] //borks when this present...coliiis
    //[JsonPropertyName("id"), JsonIgnore]
    string DoctorId,

    [property: Description("Short Reason for Request")]
    [property: DefaultValue("")]
    string Reason, //no complain here as below

    [property: Description("The type of request made by Patient")]
    [property: DefaultValue("")]
    string Action
);

//response to a PatientRequest by doctor
public record DoctorActionResponse( 
    [property: Description("The patient's Id")]
    //[property: Required()] //borks when this present...coliiis
    //[JsonPropertyName("id"), JsonIgnore]
    string PatientId,

    [property: Description("Original Request action of patient")]
    [property: DefaultValue("")]
    string Action,

    [property: Description("The new status response from Doctor")]
    [property: DefaultValue("")]
    string Status
);

public record TreatmentRequest(
    [property: Description("The patient's Id")]
    //[property: Required()] //borks when this present...coliiis
    //[JsonPropertyName("id"), JsonIgnore]
    string PatientId, //redundant?

    [property: Description("Id of doctor adding Treatment")]
    //[property: Required()] //borks when this present...coliiis
    //[JsonPropertyName("id"), JsonIgnore]
    string DoctorId,

    [property: Description("Name of Treatment")]
    [property: DefaultValue("")]
    string Name,

    [property: Description("Some details")]
    [property: DefaultValue("")]
    string Details
);
/*{
  "patientId": "123",
  "doctorId": "456",
  "name": "More food",
  "details":"",
  //other stuf?
}*/

//p{doctorId: d.id, doctorName: d.firstName+' '+d.lastName, speciality: d.speciality, since: r.since, fromAction: r.fromAction }";
public record AttendingDoctor(
    [property: Description("The patient's Id")]
    string DoctorId, 

    [property: Description("Id of doctor adding Treatment")]
    //[property: Required()] //borks when this present...coliiis
    //[JsonPropertyName("id"), JsonIgnore]
    string DoctorName,

    [property: Description("Name of Treatment")]
    [property: DefaultValue("")]
    string Speciality,

    [property: Description("Name of Treatment")]
    [property: DefaultValue("")]
    string? FromAction,

    [property: Description("Some details")]
    [property: DefaultValue("")]
    long? Since
);
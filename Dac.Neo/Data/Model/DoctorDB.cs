using System.ComponentModel.DataAnnotations;
//using System.Text.Json.Serialization;
using Neo4j.Driver.Extensions;

namespace Dac.Neo.Data.Model; 

public class DoctorDB  //<T> where T : new() //: ILookupDataResponse<DoctorDB>
{
    public DoctorDB(){} //for INode to be mapped--need constructor without parameter
    //public DoctorDB<T> where T : new(){}

    //[JsonPropertyName("id"), JsonIgnore]
    [Neo4jProperty(Name = "id")]
    public string? Id { get; set;}

    //could be skipped and default to 'Dr.'
    //[JsonPropertyName("firstName")]
    [Neo4jProperty(Name = "firstName")]
    public string? FirstName { get; set;}

    //[Required]
    //[JsonPropertyName("lastName")]
    [Neo4jProperty(Name = "lastName")]
    public string? LastName { get; set;} //required

    //[JsonPropertyName("speciality")]
    [Neo4jProperty(Name = "speciality")]
    public string? Speciality { get; set;}  //requiered?

    //phone? or contact info...
    //[JsonPropertyName("practising")]
    [Neo4jProperty(Ignore = true)]
    public DateTime? PractiseSince { get; set;} //requiered?

}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Dac.API.Model; 

public class Doctor
{
    public Doctor(){} //for INode to be mapped--need constructor without parameter  >> bon using DB entities with Mapper (would help for encapsulation too)

    [JsonPropertyName("id"), JsonIgnore]
    public string? Id { get; set;} //should be requiered here--toReview**

    //could be skipped and default to 'Dr.'
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set;}

    [Required]
    [JsonPropertyName("lastName")]
    public string LastName { get; set;} //required

    [JsonPropertyName("speciality")]
    public string Speciality { get; set;}  //requiered definitely!

    //phone? or contact info...
    [JsonPropertyName("practiseSince")]
    public DateTime? PractiseSince { get; set;} 

}